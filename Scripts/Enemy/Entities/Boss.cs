using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Boss : Enemy
{
    [SerializeField] private BossEventTrigger eventTrigger;
    [SerializeField] private ReapingHook reapingHook;
    [SerializeField] GameObject DieEffect;
    private SpriteRenderer reapingHookSprite;

    [SerializeField] private List<RandomSpawner> spanwers = new List<RandomSpawner>();

    [Header("Phase2 Options")]
    [SerializeField, Range(0f, 1f)] private float phase2HealthWeight;
    [SerializeField] private List<GameObjectGenerator> generators = new List<GameObjectGenerator>();
    [SerializeField, Tooltip("Enemy Attacker에 등록된 스킬만 적용됩니다")] private List<EnemySkillCondition> skillChange = new List<EnemySkillCondition>();
    [SerializeField] private List<GameObject> objectsToActivate = new List<GameObject>();

    private Animator animator;
    private Animator reapingHookAnimator;
    private EnemyAttacker attacker;
    private AudioSource audioPlayer;
    private SpriteRenderer sprite;

    private bool weaponFadedOut;
    private float weaponRemainedTimeToFadeIn;
    private Phase currentPhase = Phase.Phase1;
    private float deathTime;

    private Color defaultColor;
    private Hero player;

    private enum Phase
    {
        Phase1,
        Phase2
    }

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        reapingHookAnimator = reapingHook.GetComponent<Animator>();
        attacker = GetComponent<EnemyAttacker>();
        audioPlayer = GetComponent<AudioSource>();

        reapingHookSprite = reapingHook.gameObject.GetComponent<SpriteRenderer>();

        attacker.OnSkillTriggered += OnSkillTriggered;
        eventTrigger.OnBossAppearFinished += () => audioPlayer.Play();
    }

    private void Start()
    {

        UIManager.Instance.EnabledBossHealthUI(true);
        UIManager.Instance.EnabledRemainEnemyUI(false);

        player = FindObjectOfType<Hero>();
        if (player != null)
            player.OnDeath += RemoveBossHealthBar;
    }

    private void RemoveBossHealthBar()
    {
        UIManager.Instance.EnabledBossHealthUI(false);
        player.OnDeath -= RemoveBossHealthBar;
        player = null;
    }

    private void Update()
    {
        if (IsDead)
        {
            deathTime += Time.deltaTime;
            sprite.color = Color.Lerp(defaultColor, Color.clear, Mathf.InverseLerp(0f, this.timeToDestroy, deathTime));
            reapingHookSprite.color = Color.Lerp(defaultColor, Color.clear, Mathf.InverseLerp(0f, this.timeToDestroy, deathTime));
        }
    }

    private void OnSkillTriggered(EnemySkill skillTriggered)
    {
        switch (skillTriggered)
        {
            case SingleSwordSwing _:
            case DoubleSwordSwing _:
                animator.SetBool("Is Using Skill", true);
                this.weaponRemainedTimeToFadeIn = skillTriggered.castingTime;
                if (!this.weaponFadedOut)
                    StartCoroutine(FadeInOutReapingHook());
                break;
            case ThrowBoomerang _:

            default:
                break;
        }
    }

    private IEnumerator FadeInOutReapingHook()
    {
        reapingHook.FadeOut();
        this.weaponFadedOut = true;

        while (this.weaponRemainedTimeToFadeIn > 0)
        {
            yield return null;
            this.weaponRemainedTimeToFadeIn = Mathf.Max(0f, this.weaponRemainedTimeToFadeIn - Time.deltaTime);
        }

        animator.SetBool("Is Using Skill", false);

        yield return new WaitForSeconds(0.4f);

        float t = reapingHook.FadeIn();
        yield return new WaitForSeconds(t);

        this.weaponFadedOut = false;
    }


    //for test UIBossHealth
    public override float Health
    {
        get => base.Health;
        protected set
        {
            if (IsDead)
                return;

            //Debug.Log(health);
            health = Mathf.Clamp(value, 0f, MaxHealth);

            Debug.Log($"<{nameof(Boss)}> 체력: {this.Health}", this);

            UIManager.Instance.SetBossHealthUI(Health, MaxHealth);

            if (currentPhase == Phase.Phase1 && this.Health < this.MaxHealth * phase2HealthWeight)
            {
                RunPhase2();
            }
        }
    }

    protected override void Die()
    {
        base.Die();

        Destroy(Instantiate<GameObject>(DieEffect,this.transform.position,Quaternion.identity),timeToDestroy);

        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;

        for (int i = 0; i < spanwers.Count; i++)
        {
            spanwers[i].gameObject.SetActive(false);
        }

        var enemies = FindObjectsOfType<Enemy>();
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].TakeDamage(enemies[i].MaxHealth, enemies[i].transform.position, Vector2.zero);
        }
        UIManager.Instance.EnabledBossHealthUI(false);
        UIManager.Instance.EnabledRemainEnemyUI(true);

        defaultColor = sprite.color;
        Destroy(gameObject, timeToDestroy);

    }

    // 체력이 일정 이하가 되면 2페이즈 시작
    private void RunPhase2()
    {
        currentPhase = Phase.Phase2;
        Debug.Log($"<{nameof(Boss)}.{nameof(RunPhase2)}()> 페이즈2 진입", this);
        for (int i = 0; i < generators.Count; i++)
        {
            generators[i].Run();
        }

        for (int i = 0; i < objectsToActivate.Count; i++)
        {
            objectsToActivate[i].SetActive(true);
        }

        for (int i = 0; i < skillChange.Count; i++)
        {
            try
            {
                var condition = attacker.skills.Find(s => s.skill == skillChange[i].skill);
                condition.skillUseWeight = skillChange[i].skillUseWeight;
            }
            catch { }
        }       
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Boss)), CanEditMultipleObjects]
public class BossEditor : EnemyEditor
{
    private MoveToolEditor moveToolEditor;

    private void OnSceneGUI()
    {
        if (moveToolEditor == null)
        {
            moveToolEditor = Editor.CreateEditor(target, typeof(MoveToolEditor)) as MoveToolEditor;
            moveToolEditor.OnEnable();
        }       
        moveToolEditor.OnSceneGUI();       
    }

    private void OnDisable()
    {
        DestroyImmediate(moveToolEditor);
    }
}
#endif