using UnityEngine;
using UnityEngine.UI;

public class SaveArea : MonoBehaviour
{
    [SerializeField, Min(0f)] private float saveTime;
    [SerializeField, Min(0f)] private float delay;
    [SerializeField] private Slider saveProgressBar;

    private PlayerInput playerInput;
    private Rigidbody2D playerRigid;
    private PlayerMovement playerMovement;

    private float time;
    private bool save;
    private bool playerDisable;

    private void Awake()
    {
        saveProgressBar.maxValue = saveTime;
    }

    private void SaveData()
    {
        //SaveSystem.SaveGameManagerData(new GameManagerData());
        //SaveSystem.SavePlayerWeapon(new PlayerWeaponData(shooter, shooter.WeaponSlotCapacity));
        //SaveSystem.SavePlayerData(new PlayerData(playerEntity));

        //var shooter = playerInput.GetComponent<PlayerShooter>();
        //var playerEntity = playerInput.GetComponent<Hero>();

        //GameData data = GameManager.Instance.GameDataLoaded;
        //data.GameManager = new GameManagerData(gameObject.scene.buildIndex);
        //data.Player = new PlayerData(playerEntity);
        //data.PlayerWeapon = new PlayerWeaponData(shooter, shooter.WeaponSlotCapacity);
        //SaveSystem.SaveGameData(data);
        GameManager.Instance.InGameStartSceneBuildIndex = gameObject.scene.buildIndex;
        GameManager.Instance.SaveGameData();
    }

    private void Update()
    {
        // 세이브 시작
        if (!save && playerInput != null && playerInput.Interact)
        {
            save = true;

            // 플레이어 움직임 제한
            playerDisable = true;
            playerMovement.JumpControl = false;
            playerRigid.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;

            saveProgressBar.gameObject.SetActive(true);

            SaveData();
        }

        // 세이브 진행 중
        if (save)
        {
            time += Time.deltaTime;
            saveProgressBar.value = time;
        }

        // 플레이어 움직임 제한 풀기
        if (playerDisable && time > saveTime)
        {
            playerMovement.JumpControl = true;
            playerRigid.constraints = RigidbodyConstraints2D.FreezeRotation;
            playerDisable = false;
            saveProgressBar.gameObject.SetActive(false);
        }

        // 세이브 종료
        if (time > saveTime + delay)
        {
            time = 0f;
            save = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInput = collision.GetComponent<PlayerInput>();
            playerMovement = collision.GetComponent<PlayerMovement>();
            playerRigid = collision.attachedRigidbody;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInput = null;
        }
    }
}
