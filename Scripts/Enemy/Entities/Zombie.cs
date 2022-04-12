using System.Collections;
using UnityEngine;

public class Zombie : Enemy
{
    [Header("Spawn Option")]
    [SerializeField] private AnimationClip spawnAnimation;
    [SerializeField] private GameObject spawnEffect;

    private float deathTime;
    private Color defaultColor;

    private EnemyDetection detection;
    private Rigidbody2D rigid;
    private SpriteRenderer sprite;
    private Animator animator;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        detection = GetComponent<EnemyDetection>();
        rigid = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        defaultColor = sprite.color;
        StartCoroutine(DetectionStop());
    }

    private IEnumerator DetectionStop()
    {
        detection.enabled = false;
        rigid.constraints = RigidbodyConstraints2D.FreezeAll;
        spawnEffect.SetActive(true);

        yield return new WaitForSeconds(this.spawnAnimation.length);

        detection.enabled = true;
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        rigid.AddForce(new Vector2(0f, 0.1f), ForceMode2D.Impulse);
        spawnEffect.SetActive(false);
    }

    private void Update()
    {
        if (this.IsDead)
        {
            deathTime += Time.deltaTime;
            sprite.color = Color.Lerp(defaultColor, Color.clear, Mathf.InverseLerp(0f, this.timeToDestroy, deathTime));

            return;
        }

    }

    protected override void Die()
    {
        base.Die();
        defaultColor = sprite.color;
        animator.SetTrigger("Die");
    }
}
