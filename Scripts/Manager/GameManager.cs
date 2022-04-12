using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 게임 관리 클래스
/// </summary>
public partial class GameManager : Singleton<GameManager>, ISaveable
{
    #region === Serialized Fields ===

    [Header("Debug Option")]
    [SerializeField, ReadOnly(true)] private bool debugMode;
    [SerializeField] private int goldDebugMode = 0xFFFFFFF;
    [SerializeField] private DebugConsole debugConsolePrefab;

    [Header("Game Option")]
    [SerializeField, ReadOnly(true)] private int startSceneIndex;
    [SerializeField, ReadOnly(true)] private GameLevel level;

    [Header("Scene Load")]
    [SerializeField] private float waitTimeBeforeLoading = 1f;
    [SerializeField] private float waitTimeAfterLoading = 1f;
    [SerializeField] private string spawnTag = "Respawn";

    [Header("Record")]
    [SerializeField, ScenePopup] private int mainVillage;
    [SerializeField, ScenePopup] private int stage1;
    [SerializeField, ScenePopup] private int finalStage;
    [SerializeField, Min(0)] private int clearRecordCount = 10;

    //[SerializeField] private string finalStageBossName;

    #endregion

    #region === Events ===

    /// <summary>
    /// Scene이 로드 될 때 발생하는 이벤트. Scene 로드 시 총 4번 발동됨.
    /// </summary>
    public event SceneLoadingHandler OnSceneLoaded;

    #endregion

    #region === Public Static Methods ===

    /// <summary>
    /// 해당 난이도에 대해 저장되고 로드될 때 사용되는 게임 데이터 파일 이름을 반환한다. 실제 파일 존재 여부와는 관계 없다.
    /// </summary>
    /// <returns>게임 난이도에 따른 파일 이름</returns>
    /// <exception cref="ArgumentException">난이도에 대응하는 파일 이름이 존재하지 않을 때</exception>
    public static string GetGameDataFileName(GameLevel level) =>
        level switch
        {
            GameLevel.Easy => "gamedata_easy",
            GameLevel.Hard => "gamedata_hard",
            _ => throw new ArgumentException($"{level} 난이도에 대한 게임 데이터 파일 이름이 존재하지 않습니다.")
        };

    /// <summary>
    /// 게임 데이터 파일을 삭제한다.
    /// </summary>
    /// <param name="level">게임 난이도</param>
    public static void DeleteGameData(GameLevel level) => SaveManager.DeleteFile(GetGameDataFileName(level));


    #endregion

    #region === Public Properties ===

    /// <summary>
    /// 게임 난이도.
    /// </summary>
    public GameLevel Level
    {
        get => level;
        set
        {
            switch (value)
            {
                case GameLevel.Easy:
                    break;
                case GameLevel.Hard:
                    StartStageBuildIndex = stage1;
                    break;
                default:
                    throw new ArgumentException($"{value}는 규약된 게임 난이도가 아니므로 설정할 수 없습니다.");
            }
            level = value;
        }
    }

    /// <summary>
    /// 골드를 반환 및 설정하는 프로퍼티.
    /// </summary>
    public int Gold
    {
        get => gold;
        set
        {
            if (IsGameOver)
                return;

            if (value >= 0)
            {
                gold = value;
                UIManager.Instance.SetGoldUI(gold);
            }
        }
    }


    /// <summary>
    /// 사망 횟수
    /// </summary>
    public int DeathCount { get => this.deathCount; private set => this.deathCount = Mathf.Max(0, value); }

    /// <summary>
    /// 클리어 횟수
    /// </summary>
    public int ClearCount { get => this.clearCount; private set => this.clearCount = Mathf.Max(0, value); }


    /// <summary>
    /// 플레이어가 죽어 게임오버 됐는지를 반환.
    /// </summary>
    public bool IsGameOver { get; private set; }

    /// <summary>
    /// 마지막 클리어 기록
    /// </summary>
    public ClearRecord LastClearRecord { get; private set; }

    /// <summary>
    /// 클리어 기록 Top 10
    /// </summary>
    public IEnumerable<ClearRecord> ClearRecords
    {
        get
        {
            foreach (var record in this.clearRecords)
            {
                yield return record;
            }
        }
    }

    /// <summary>
    /// 패드 연결 상태를 반환
    /// </summary>
    public bool IsControllerConnection { get; private set; }

    /// <summary>
    /// 게임 시작 Scene 빌드인덱스
    /// </summary>
    public int InGameStartSceneBuildIndex { get; set; }

    public double StageRunningTime { get => this.stageRunningTime; private set => this.stageRunningTime = value < 0d ? 0d : value; }

    public bool IsInStage { get; private set; }

    [Obsolete]
    public GameData GameDataLoaded { get; private set; }

    /// <summary>
    /// 게임 난이도에 의한 시작 스테이지의 빌드 인덱스
    /// </summary>
    public int StartStageBuildIndex
    {
        get => startStageBuildIndex;
        set
        {
            startStageBuildIndex = value;
            Debug.Log($"<{nameof(GameManager)}> 시작 Stage Build Index: {value}", this);
        }
    }

    private int startStageBuildIndex;


    #endregion

    #region === Public Methods ===

    /// <summary>
    /// 현재 게임 난이도에 따른 데이터 저장.
    /// </summary>
    public void SaveGameData()
    {
        SaveManager.Save(GetGameDataFileName(this.Level), SaveKey.GameData);
        Debug.Log($"<{nameof(GameManager)}> {this.Level} 난이도의 게임 데이터를 저장함.", this);
    }

    /// <summary>
    /// 현재 게임 난이도에 따른 데이터 로드.
    /// </summary>
    public bool LoadGameData()
    {
        try
        {
            SaveManager.Load(GetGameDataFileName(this.Level), SaveKey.GameData);
            Debug.Log($"<{nameof(GameManager)}> {this.Level} 난이도의 게임 데이터 파일을 로드하는데 성공함.", this);
            return true;
        }
        catch (System.IO.FileNotFoundException)
        {
            Debug.Log($"<{nameof(GameManager)}> {this.Level} 난이도의 게임 데이터 파일이 존재하지 않음.", this);
            return false;
        }
    }

    /// <summary>
    /// Scene을 로드한다.
    /// </summary>
    /// <param name="sceneIndex">빌드 세팅에 있는 Scene 인덱스</param>
    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(LoadSceneAsync(sceneIndex, null));
    }

    /// <summary>
    /// Scene을 로드한다. Player 오브젝트를 Spawn Point에 배치시킨다.
    /// </summary>
    /// <param name="sceneIndex">빌드 세팅에 있는 Scene 인덱스</param>
    /// <param name="player">다른 Scene으로 이동할 Player Object</param>
    public void LoadScene(int sceneIndex, GameObject player)
    {
        if (player == null)
            throw new ArgumentNullException(nameof(player));

        StartCoroutine(LoadSceneAsync(sceneIndex, player));
    }

    /// <summary>
    /// Scene을 로드한다. Player 오브젝트를 Spawn Point에 배치시킨다.
    /// </summary>
    /// <param name="sceneIndex">빌드 세팅에 있는 Scene 인덱스</param>
    /// <param name="player">다른 Scene으로 이동할 Player Object</param>
    /// <param name="mode">씬 로드 방식</param>
    public void LoadScene(int sceneIndex, GameObject player, LoadSceneMode mode)
    {
        StartCoroutine(LoadSceneAsync(sceneIndex, player, mode));
    }


    /// <summary>
    /// 활성화되어있는 Scene들의 상태를 유지한 채 Scene간 전환하는 메서드
    /// </summary>
    /// <param name="sceneIndex">전환하려는 Scene</param>
    /// <param name="player">다른 Scene으로 이동할 Player Object</param>
    /// <param name="destoryPreviousScene">Scene 전환 전 현재 활성화된 Scene을 파괴할지 여부</param>
    public void SwitchScene(int sceneIndex, GameObject player, bool destoryPreviousScene)
    {
        var targetScene = SceneManager.GetSceneByBuildIndex(sceneIndex);
        if (targetScene.isLoaded)
        {
            StartCoroutine(SwitchSceneAsync(targetScene, player, destoryPreviousScene));
        }
        else
        {
            LoadScene(sceneIndex, player);
        }
    }

    public void MoveToSpawnPoint(GameObject player)
    {
        if (player != null)
        {
            try
            {
                var spawnPoint = GameObject.FindGameObjectsWithTag(spawnTag).First(sp => sp.scene == this.currentScene);

                player.transform.position = spawnPoint.transform.position;
                Debug.Log($"<{typeof(GameManager)}> Spawn 위치: {spawnPoint.transform.position}, Spawn Point Scene: {spawnPoint.scene}");

            }
            catch (Exception)
            {
                throw new InvalidOperationException("{spawnTag}\" 태그를 가지는 Game Object가 Scene에 존재하지 않습니다. 반드시 추가하세요.\n" +
                        $"Scene Name : {SceneManager.GetActiveScene().name}, Scene Index in Build Settings : {SceneManager.GetActiveScene().buildIndex}");
            }
        }
    }

    #endregion
}

public enum GameLevel
{
    Easy,
    Hard
}

#region === Save System ===

public partial class GameManager
{
    #region === ISaveable Interface Members ===

    public string ID => null;

    public object Save() => new GameManagerData(this.InGameStartSceneBuildIndex);

    public void Load(object loaded)
    {
        if (loaded is GameManagerData data)
        {
            this.Gold = this.debugMode ? this.goldDebugMode : data.gold;
            this.ClearCount = data.clearCount;
            this.DeathCount = data.deathCount;
            this.LastClearRecord = data.lastClearRecord;
            this.InGameStartSceneBuildIndex = data.sceneBuildIndex;
            this.StageRunningTime = data.stageRunningTime;
            this.StartStageBuildIndex = data.startStageBuildIndex;

            this.clearRecords.RemoveAll(r => true);
            for (int i = 0; i < data.clearRecords.Length; i++)
            {
                TryAddClearRecord(data.clearRecords[i]);
            }

            Debug.Log($"<{typeof(GameManager)}> {data}");

            string clearRecordsTxt = string.Empty;
            int k = 0;
            foreach (var record in this.ClearRecords)
            {
                clearRecordsTxt += $"[{k + 1}] - {record}\n";
                k++;
            }
            Debug.Log($"<{typeof(GameManager)}> 클리어 Top 10 기록 \n{clearRecordsTxt}");
        }
        else
        {
            Debug.Log($"<{nameof(GameManager)}> 잘못된 데이터가 로드되었습니다. 필요한 데이터 타입: {typeof(GameManagerData)}, 로드된 데이터 타입: {loaded?.GetType() ?? null}", this);
        }
    }

    #endregion
}

[Serializable]
public class GameManagerData
{
    public readonly int gold;
    public readonly int deathCount;
    public readonly int clearCount;
    public readonly ClearRecord lastClearRecord;
    public readonly ClearRecord[] clearRecords;
    public readonly int sceneBuildIndex;
    public readonly int startStageBuildIndex;
    public readonly double stageRunningTime;

    //public GameManagerData() : this(SceneManager.GetActiveScene().buildIndex) { }

    public GameManagerData(int sceneBuildIndex)
    {
        gold = GameManager.Instance.Gold;
        deathCount = GameManager.Instance.DeathCount;
        clearCount = GameManager.Instance.ClearCount;
        lastClearRecord = GameManager.Instance.LastClearRecord;
        clearRecords = GameManager.Instance.ClearRecords.ToArray();
        stageRunningTime = GameManager.Instance.StageRunningTime;
        this.sceneBuildIndex = sceneBuildIndex;
        this.startStageBuildIndex = GameManager.Instance.StartStageBuildIndex;
    }

    public override string ToString()
    {
        return $"GameManager Data {{ Gold = {gold}, Death Count = {deathCount}, Clear Count = {clearCount}, " +
            $"Scene Build Index = {sceneBuildIndex}, Stage Running Time = {stageRunningTime:0.00}, " +
            $"Last Clear Record = {lastClearRecord}, Clear Records Count = {clearRecords.Length} }}";
    }
}

#endregion


#region === SceneLoadingHandler ===

/// <summary>
/// Scene 로딩 타이밍. 정수 크기가 작은 순으로 실행됨.
/// </summary>
public enum SceneLoadingTiming
{
    BeforeFadeOut,
    BeforeLoading,
    AfterLoading,
    AfterFadeIn
}

/// <summary>
/// Scene 로드 대리자
/// </summary>
/// <param name="previous">로딩 되기 이전의 Scene</param>
/// <param name="loaded">로딩 된 Scene</param>
/// <param name="when">로딩 타이밍</param>
public delegate void SceneLoadingHandler(Scene? previous, Scene? loaded, SceneLoadingTiming when);

#endregion


#region === Private Memebers ===

public partial class GameManager
{
    #region === Private Fields ===

    private Animator sceneChanger;
    private Hero player;
    private Scene? currentScene;

    private int gold;
    private int deathCount;
    private int clearCount;
    private bool isLevelSet = false;
    private bool isGameStarted;
    private bool isGameDataSyncronized;
    //private float stageRunningTime;
    //private float minClearTime;
    private float startTime;
    private double stageRunningTime;

    private List<ClearRecord> clearRecords = new List<ClearRecord>();

    #endregion

    #region === Unity Event Methods ===

    private void Awake()
    {
        if (!CheckSingletonInstance(true))
            return;

        if (this.debugMode)
            Instantiate(debugConsolePrefab);

        //this.LoadGameDataDeprecated();

        sceneChanger = GetComponentInChildren<Animator>();
        this.InGameStartSceneBuildIndex = this.mainVillage;
        this.StartStageBuildIndex = this.stage1;

        ////UI Manager의 OnClickRestartButton 이벤트
        //UIManager.Instance.OnClickRestartButton += () =>
        //{
        //    LoadScene(0);
        //};

        //this.Gold = this.Gold;
        this.SetDebugMode();

        SettingDataController.LoadSettingData();
        this.Level = SettingDataController.Data.lastSelectedLevel;

        SceneManager.sceneLoaded += SceneLoaded;

        SaveManager.Add(this, SaveKey.GameData);
    }

    private void Start()
    {
        IsControllerConnection = false;
    }

    private void Update()
    {
        //if (Input.GetButtonDown("Fire1"))
        //{
        //    IsControllerConnection = false;
        //}
        //else if (Input.GetAxisRaw("right trigger") != 0)
        //{
        //    // 패드 지원시 true로 전환
        //    IsControllerConnection = false;
        //}

        if (!this.IsGameOver && this.IsInStage)
            this.StageRunningTime += Time.deltaTime;
    }

    #endregion

    #region === Private Methods ===

    protected GameManager() { }

    protected override void OnDestroyed()
    {
        SaveManager.Remove(this, SaveKey.GameData);
    }

    private IEnumerator SwitchSceneAsync(Scene scene, GameObject player, bool destoryPreviousScene)
    {
        var previous = this.currentScene;

        sceneChanger.SetTrigger("Fade Out");

        OnSceneLoaded?.Invoke(previous, null, SceneLoadingTiming.BeforeFadeOut);

        yield return new WaitForSeconds(waitTimeBeforeLoading);

        OnSceneLoaded?.Invoke(previous, null, SceneLoadingTiming.BeforeLoading);

        if (destoryPreviousScene)
            SceneManager.UnloadSceneAsync(previous.Value.buildIndex);

        this.currentScene = scene;
        SceneManager.SetActiveScene(this.currentScene.Value);
        MoveToSpawnPoint(player);

        OnSceneLoaded?.Invoke(previous, this.currentScene, SceneLoadingTiming.AfterLoading);

        sceneChanger.SetTrigger("Fade In");

        yield return new WaitForSeconds(waitTimeAfterLoading);

        OnSceneLoaded?.Invoke(previous, this.currentScene, SceneLoadingTiming.AfterFadeIn);
    }

    private void SetDebugMode()
    {
        if (!debugMode)
            return;

        this.Gold = this.goldDebugMode;
    }

    private bool isLoadingScene;

    // 비동기 Scene 로딩 처리
    private IEnumerator LoadSceneAsync(int sceneIndex, GameObject player, LoadSceneMode mode = LoadSceneMode.Single)
    {
        // 로딩 전 처리 코드(애니메이션, 플레이어 컴포넌트 비활성화 등)
        if (sceneIndex < 0 || sceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogError($"<{typeof(GameManager)}> Scene 인덱스가 빌드 세팅의 인덱스 범위를 벗어났습니다.");
            yield break;
        }

        if (isLoadingScene)
        {
            Debug.LogError($"<{nameof(GameManager)}> Scene이 이미 로딩중이기 떄문에 로딩할 수 없습니다.", this);
            yield break;
        }

        isLoadingScene = true;
        var previous = this.currentScene;

        sceneChanger.SetTrigger("Fade Out");

        this.TriggerOnSceneLoadedEvent(previous, null, SceneLoadingTiming.BeforeFadeOut);

        yield return new WaitForSeconds(waitTimeBeforeLoading);

        this.TriggerOnSceneLoadedEvent(previous, null, SceneLoadingTiming.BeforeLoading);

        // 로딩 동작
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex, mode);

        while (!operation.isDone)
        {
            float loadProgress = Mathf.Clamp01(operation.progress / 0.9f) * 100f;

            Debug.Log($"<{typeof(GameManager)}> 로딩 진행도 : {loadProgress}%");

            yield return null;
        }

        // 로딩 완료 직후 처리 코드(애니메이션 등)
        // Player 스폰 위치를 지정
        MoveToSpawnPoint(player);

        if (mode == LoadSceneMode.Additive && this.currentScene.HasValue)
        {
            SceneManager.SetActiveScene(this.currentScene.Value);
        }

        this.TriggerOnSceneLoadedEvent(previous, this.currentScene, SceneLoadingTiming.AfterLoading);

        sceneChanger.SetTrigger("Fade In");

        yield return new WaitForSeconds(waitTimeAfterLoading);

        this.TriggerOnSceneLoadedEvent(previous, this.currentScene, SceneLoadingTiming.AfterFadeIn);

        isLoadingScene = false;
    }

    /// <summary>
    /// Global Event인 OnSceneLoaded의 Exception 처리를 위한 메서드
    /// </summary>
    private void TriggerOnSceneLoadedEvent(Scene? previous, Scene? loaded, SceneLoadingTiming timing)
    {
        if (this.OnSceneLoaded == null)
            return;

        foreach (SceneLoadingHandler eventItem in this.OnSceneLoaded.GetInvocationList())
        {
            try
            {
                eventItem(previous, loaded, timing);
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
            }
        }
    }

    private bool TryAddClearRecord(ClearRecord record)
    {
        int idx = clearRecords.FindIndex(r => record.ClearTime < r.ClearTime);
        if (idx >= 0)
        {
            clearRecords.Insert(idx, record);
            if (clearRecords.Count > this.clearRecordCount)
                clearRecords.RemoveRange(this.clearRecordCount, clearRecords.Count - this.clearRecordCount);

            return true;
        }
        else if (clearRecords.Count < this.clearRecordCount)
        {
            clearRecords.Add(record);
            return true;
        }

        return false;
    }

    // Scene이 로드 될 때마다 실행될 메서드
    private void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"<{typeof(GameManager)}> Scene Name : {scene.name}, Load Scene Mode : {mode}");
        this.currentScene = scene;

        // Main Village인 경우
        if (scene.buildIndex == this.mainVillage)
            IsGameOver = false;

        // Stage1인 경우
        if (scene.buildIndex == this.stage1)
        {
            // Stage1으로 이동했을 때 StageRunningTime을 0으로 초기화
            // 게임 난이도에 관계 없이 Stage1 -> Boss 클리어까지 걸린 시간을 측정 가능
            this.StageRunningTime = 0d;
        }

        // 현재 Scene에 Enemy가 없을 경우 Stage로 간주하지 않음.
        var enemy = FindObjectOfType<Enemy>(true);
        this.IsInStage = enemy != null;
        Debug.Log($"<{nameof(GameManager)}> 스테이지 여부: {IsInStage}", this);

        // 마지막 Stage일 경우
        if (scene.buildIndex == finalStage)
        {
            try
            {
                FindObjectOfType<Boss>(true).OnDeath += ClearGame;
            }
            catch (NullReferenceException)
            {
                Debug.LogError($"<{typeof(GameManager)}> Final Stage에서 Boss를 찾을 수 없습니다. " +
                $"{nameof(finalStage)}의 값을 확인하거나 Final Stage에 보스가 있는지 확인해주세요.");

            }
        }

        // Scene 내의 Player 참조를 위한 시도
        // Player는 unique 오브젝트이기 때문에 null일 때만 참조 시도
        if (player == null)
        {
            player = FindObjectOfType<Hero>();

            // 참조에 성공했을 시 관련 처리
            if (player != null)
            {
                player.OnDeath += GameOver;

                //UI Manager의 OnClickRestartButton 이벤트
                UIManager.Instance.OnClickRestartButton += () =>
                {
                    LoadScene(mainVillage, this.player.gameObject);
                };
                //UIManager.Instance.OnClickRestartButton += () => this.IsGameOver = false;
            }
        }
    }

    private void ClearGame()
    {
        this.IsGameOver = true;
        this.ClearCount++;
        this.LastClearRecord = new ClearRecord(DateTime.Now, StageRunningTime);

        // 클리어 기록 보관
        this.TryAddClearRecord(this.LastClearRecord);
        Debug.Log($"<{nameof(GameManager)}> 보스 사망, 클리어 횟수: {ClearCount}, 클리어: {this.LastClearRecord}", this);

        this.InGameStartSceneBuildIndex = mainVillage;
        this.StartStageBuildIndex = stage1;

        //this.SaveData();
        SaveManager.Save(GetGameDataFileName(this.Level), SaveKey.GameData);
    }

    // 게임 오버 처리
    private void GameOver()
    {
        IsGameOver = true;
        DeathCount++;
        this.InGameStartSceneBuildIndex = mainVillage;
        this.StartStageBuildIndex = player.RemainedLife == 0 ? stage1 : (this.currentScene.HasValue ? this.currentScene.Value.buildIndex : stage1);

        //switch (Level)
        //{
        //    case GameLevel.Easy:
        //        this.StartStageBuildIndex = this.currentScene.HasValue ? this.currentScene.Value.buildIndex : stage1;
        //        break;
        //}
        //SaveData();
        SaveManager.Save(GetGameDataFileName(this.Level), SaveKey.GameData);
    }

    [Obsolete]
    private void SaveData()
    {
        this.GameDataLoaded.GameManager = new GameManagerData(this.mainVillage);
        this.GameDataLoaded.Player = new PlayerData(this.player, this.player.MaxHealth);
        var shooter = this.player.GetComponent<PlayerShooter>();
        this.GameDataLoaded.PlayerWeapon = new PlayerWeaponData(shooter, shooter.WeaponSlotCapacity);
        SaveSystem.SaveGameData(this.GameDataLoaded);
    }

    [Obsolete]
    private void LoadGameDataDeprecated()
    {
        try
        {
            this.GameDataLoaded = SaveSystem.LoadGameData();

            var data = GameDataLoaded.GameManager;

            if (data == null)
                throw new System.IO.FileNotFoundException("파일이 존재하지 않음.");

            this.Gold = this.debugMode ? this.goldDebugMode : data.gold;
            this.ClearCount = data.clearCount;
            this.DeathCount = data.deathCount;
            this.LastClearRecord = data.lastClearRecord;
            this.InGameStartSceneBuildIndex = data.sceneBuildIndex;
            this.StageRunningTime = data.stageRunningTime;
            //AudioListener.volume = data.audioVolume;

            this.clearRecords.RemoveAll(r => true);
            for (int i = 0; i < data.clearRecords.Length; i++)
            {
                TryAddClearRecord(data.clearRecords[i]);
            }

            Debug.Log($"<{typeof(GameManager)}> {data}");

            string clearRecordsTxt = string.Empty;
            int k = 0;
            foreach (var record in this.ClearRecords)
            {
                clearRecordsTxt += $"[{k + 1}] - {record}\n";
                k++;
            }
            Debug.Log($"<{typeof(GameManager)}> 클리어 Top 10 기록 \n{clearRecordsTxt}");
        }
        catch (System.IO.FileNotFoundException e)
        {
            this.Gold = this.debugMode ? this.goldDebugMode : this.gold;
            this.InGameStartSceneBuildIndex = this.mainVillage;
            AudioListener.volume = 0.5f;
            this.GameDataLoaded = new GameData();
            SaveSystem.SaveGameData(this.GameDataLoaded);
            Debug.Log($"<{typeof(GameManager)}> {e.Message}");
        }
    }

    #endregion
}

#endregion
