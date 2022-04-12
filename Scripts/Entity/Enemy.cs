using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif


public abstract class Enemy : Entity, IStrikingPower
{
    [SerializeField] private uint str;
    [SerializeField] protected float timeToDestroy = 5f;
    [SerializeField] protected bool isAutoDestroy = true;
    [SerializeField] protected bool unconditionallyDestroy = false;

    [SerializeField] public Slider healthBar;               //체력바 슬라이더

    private Rigidbody2D enemyRigid;

    public override float Health
    {
        get => base.Health;

        protected set
        {
            base.Health = value;

            if (healthBar != null)
                healthBar.value = Health / MaxHealth;
            else
                throw new System.NullReferenceException($"Enemy class - healthBar field is null. Please check inspector");
        }
    }

    private Action onGroundAfterDie;
    public uint STR { get => str; set => str = value; }

    protected override void Die()
    {
        base.Die();

        if (!isAutoDestroy)
            return;

        gameObject.layer = 12; // Enemy Dead Layer
        enemyRigid = GetComponent<Rigidbody2D>();
        enemyRigid.gravityScale = 1f;
        enemyRigid.velocity = Vector2.zero;
        enemyRigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        // non-fly monster의 경우 항상 Ground를 밟고 있기 때문에 OnCollisionEnter2D를 발생시키기 위해 임의로 미세한 힘을 가함.
        enemyRigid.AddForce(new Vector2(0f, enemyRigid.mass * 0.5f), ForceMode2D.Impulse);

        // 적 사망 시 땅에 닿을 수 없는 상황일 때 고려해야함!
        // 속도가 먼저 0에 근접한 상태를 몇초간 유지했을 때에 파괴?        

        onGroundAfterDie += () => enemyRigid.constraints = RigidbodyConstraints2D.FreezeAll;
        onGroundAfterDie += () => enemyRigid.velocity = Vector2.zero;
        onGroundAfterDie += () =>
        {
            Collider2D[] colliders = GetComponents<Collider2D>();
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].enabled = false;
            }
        };

        if (unconditionallyDestroy)
            Destroy(gameObject, timeToDestroy);
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (!this.IsDead || !isAutoDestroy)
            return;

        if (Vector2.Angle(collision.GetContact(0).normal, Vector2.up) <= 60f)
        {
            //Debug.Log($"[{typeof(Enemy)}] {this.name} 사망으로 인한 추락 후 접촉한 표면의 법선벡터 : {collision.GetContact(0).normal}");

            if (collision.collider.CompareTag("Ground"))
            {
                this.onGroundAfterDie();
                this.onGroundAfterDie = null;
                Destroy(gameObject, timeToDestroy);
            }
            else
            {
                StartCoroutine(StopCheck());
            }

        }
        else if (collision.collider.CompareTag("DeadZone"))
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator StopCheck()
    {
        float time = 0f;
        while (time <= this.timeToDestroy + 1f)
        {
            yield return null;
            if (enemyRigid.velocity.magnitude <= 0.1f)
            {
                time += Time.deltaTime;
            }
            else
            {
                time = 0f;
            }
        }
        Destroy(gameObject);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Enemy), true), CanEditMultipleObjects]
public class EnemyEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
}
#endif