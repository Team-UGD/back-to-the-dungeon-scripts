using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BossHand : MonoBehaviour
{
    // Boss Hand Components
    [SerializeField] private TextMeshPro spaceKeyInfo;
    [SerializeField] private float power = 3f;
    private Animator bossAnimator;
    private Collider2D handCollider;
    private SpriteRenderer sprite;
    public Rigidbody2D bossHandRigidbody;

    // Player Components
    private Hero player;
    private Rigidbody2D playerRigid;
    private PlayerShooter playerShooter;
    //public GameObject playerGameObject;
    private PlayerMovement playerMovement;

    // Hand Control Fields
    private string grabAnimationInstruction;
    private uint keyPressCount;
    private float lastAttackTime;
    private AttackType handAttackType;
    private bool isColorChanging;

    public bool isSmash = false;
    private void Start()
    {
        isSmash = false;
    }

    public enum AttackType
    {
        None,
        Hit,
        Grab
    }


    // Public Properties
    public AttackType HandAttackType
    {
        get => handAttackType;
        private set
        {
            handAttackType = value;

            switch (handAttackType)
            {
                case AttackType.None:
                    break;
                case AttackType.Hit:
                    handCollider.isTrigger = false;
                    break;
                case AttackType.Grab:
                    handCollider.isTrigger = true;
                    break;
                default:
                    break;
            }
        }
    }

    public void FadeIn(float duration)
    {
        Color from = sprite.color;
        Color to = sprite.color;
        from.a = 0f;
        to.a = 1f;
        StartCoroutine(ColorChanging(from, to, duration));
    }

    public void FadeOut(float duration)
    {
        Color from = sprite.color;
        Color to = sprite.color;
        from.a = 1f;
        to.a = 0f;
        StartCoroutine(ColorChanging(from, to, duration));
    }

    private IEnumerator ColorChanging(Color from, Color to, float duration)
    {
        if (isColorChanging)
            yield break;

        this.isColorChanging = true;
        float t = 0;

        while (t < duration)
        {
            yield return null;

            t += Time.deltaTime;
            sprite.color = Color.Lerp(from, to, t / duration);

        }

        sprite.color = to;
        this.isColorChanging = false;
    }

    public void SetGrabSkillInfo(float damage, float attackTimeInterval, uint totalKeyPressCount)
    {
        HandAttackType = AttackType.Grab;
        Damage = damage;
        AttackTimeInterval = attackTimeInterval;
        TotalKeyPressCount = totalKeyPressCount;
    }
    public void SetSmashSkillInfo(float damage, GameObject playerOb)
    {
        HandAttackType = AttackType.Hit;
        Damage = damage;
        player = playerOb.GetComponent<Hero>();
    }
    public bool IsHolding { get; private set; }

    public float Damage { get; set; }

    public uint TotalKeyPressCount { get; set; }

    public float AttackTimeInterval { get; set; }


    private void Awake()
    {
        bossAnimator = GetComponent<Animator>();
        handCollider = GetComponent<Collider2D>();
        sprite = GetComponent<SpriteRenderer>();
        playerMovement = GetComponent<PlayerMovement>();
        bossHandRigidbody = GetComponent<Rigidbody2D>();

        grabAnimationInstruction = "is Grab";
    }

    // Update is called once per frame
    private void Update()
    {
        if (!gameObject.activeInHierarchy)
            return;
        if (HandAttackType == AttackType.Hit)
        {
            // 애니메이션 재생
            bossAnimator.SetBool(grabAnimationInstruction, true);
        }
        if (HandAttackType == AttackType.Grab && IsHolding)
        {
            OnHoldingPlayer();
            spaceKeyInfo.enabled = true;
            Vector3 scale = spaceKeyInfo.rectTransform.localScale;
            scale.x *= Mathf.Sign(transform.localScale.x) * Mathf.Sign(spaceKeyInfo.rectTransform.localScale.x);
            spaceKeyInfo.rectTransform.localScale = scale;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                keyPressCount++;
                //Debug.Log($"[{typeof(BossHand)}] 키를 누른 횟수: {keyPressCount}");
            }
            spaceKeyInfo.text = $"Press Space Key\n{TotalKeyPressCount - keyPressCount} Times";
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //데미지가 2번 들어가는 현상 방지용
        if (handAttackType == AttackType.Hit)
        {
            Smash(collision);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (handAttackType == AttackType.Grab && collision.CompareTag("Player") && !IsHolding)
        {
            Grab(collision);
        }
    }

    private void OnHoldingPlayer()
    {
        if (keyPressCount < TotalKeyPressCount)
        {
            player.transform.position = transform.position;

            if (Time.time > lastAttackTime + AttackTimeInterval)
            {
                player.TakeDamage(Damage, player.transform.position + new Vector3(0f, 0.75f), Vector3.zero, true);
                lastAttackTime = Time.time;
            }
        }
        else
        {
            playerRigid.constraints = RigidbodyConstraints2D.FreezeRotation;
            playerShooter.enabled = true;
            player = null;
            playerRigid = null;
            playerShooter = null;

            bossAnimator.SetBool(grabAnimationInstruction, false);
            HandAttackType = AttackType.None;
            IsHolding = false;

            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }

    private void Grab(Collider2D collision)
    {
        player = collision.GetComponent<Hero>();
        playerShooter = collision.GetComponent<PlayerShooter>();
        playerRigid = collision.attachedRigidbody;
        playerRigid.velocity = Vector2.zero;
        playerRigid.constraints = RigidbodyConstraints2D.FreezeAll;
        playerShooter.enabled = false;

        IsHolding = true;
        keyPressCount = 0;

        // 애니메이션 재생
        bossAnimator.SetBool(grabAnimationInstruction, true);
    }

    private void Smash(Collision2D collision)
    {
        if (handAttackType == AttackType.Hit && collision.collider.CompareTag("Player") && !isSmash)
        {
            //플레이어를 가격한 후에도 직선으로 진행할 수 있도록 layer변경(일단 Passable Ground로 변경)
            //this.gameObject.layer = 11;
            //bossHandRigidbody.velocity = new Vector2(0, -20);
            isSmash = true;

            var playerEntity = collision.collider.GetComponent<Entity>();

            Vector2 dir = collision.transform.localScale.x > 0 ? Vector2.right : Vector2.left;

            playerEntity.TakeDamage(Damage, player.transform.position, dir);
        }
        else if (handAttackType == AttackType.Hit && (collision.collider.CompareTag("Ground")))
        {
            if (player.GetComponent<PlayerMovement>().Grounded && Mathf.Abs(collision.GetContact(0).point.y - player.transform.position.y) <= 0.5f && !isSmash)
            {
                player.TakeDamage(10f, player.transform.position, Vector2.down, false, power);
            }
            Destroy(gameObject, 0.5f);
        }
    }
}
