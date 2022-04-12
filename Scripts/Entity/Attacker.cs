using UnityEngine;

/// <summary>
/// Skill을 사용하는 MonoBehaviour 클래스
/// </summary>
public abstract class Attacker : MonoBehaviour
{
    protected Enemy enemy;
    protected Rigidbody2D rigid;
    protected AudioSource audioSource;
    private EnemyPathfinder pathfinder;
    private SpriteRenderer sprite;

    public EnemyPathfinder Pathfinder { get => pathfinder; }
    public SpriteRenderer Sprite { get => sprite; }

    public Enemy EnemyComponent { get => enemy; }
    public Rigidbody2D RigidbodyComponent { get => rigid; }
    public AudioSource AudioSourceComponent { get => audioSource; }

    protected virtual void Awake()
    {
        enemy = GetComponent<Enemy>();
        rigid = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        pathfinder = GetComponent<EnemyPathfinder>();
        sprite = GetComponent<SpriteRenderer>();
    }

}