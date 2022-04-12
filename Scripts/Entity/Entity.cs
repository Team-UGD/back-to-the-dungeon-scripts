using System;
using UnityEngine;

/// <summary>
/// 생명체의 체력 기능을 관리하는 추상 클래스
/// </summary>
public abstract class Entity : MonoBehaviour
{
    [SerializeField]
    private float maxHealth; // 최대 체력
    [SerializeField]
    protected float health; // 현재 체력

    /// <summary>
    /// 최대 체력
    /// </summary>
    public float MaxHealth
    {
        get => maxHealth;
        set
        {
            maxHealth = value >= 0f ? value : 0f;
            if (this.Health > this.maxHealth)
                this.Health = this.maxHealth;
        }
    }
    
    /// <summary>
    /// 현재 체력
    /// </summary>
    public virtual float Health
    {
        get => health;
        protected set
        {
            if (IsDead)
                return;

            //Debug.Log(health);

            health = Mathf.Clamp(value, 0f, maxHealth);
        }
    }

    /// <summary>
    /// 사망 상태
    /// </summary>
    public bool IsDead { get; protected set; }

    /// <summary>
    /// 사망 시 발동되는 이벤트
    /// </summary>
    public event Action OnDeath;

    protected virtual void OnEnable()
    {
        IsDead = false;
        Health = MaxHealth;
    }

    /// <summary>
    /// 체력을 회복한다.
    /// </summary>
    /// <param name="healthToHeal">회복할 체력</param>
    public virtual void Heal(float healthToHeal)
    {
        Health += healthToHeal;
    }

    /// <summary>
    /// 데미지를 입힌다.
    /// </summary>
    /// <param name="damage">데미지</param>
    /// <param name="hitPosition">맞은 위치</param>
    /// <param name="hitNormal">맞은 표면의 법선벡터</param>
    public virtual void TakeDamage(float damage, Vector2 hitPosition, Vector2 hitNormal, bool isContinuous = false, float power = 1, float use = 1)
    {
        Health -= damage;

        if (Health <= 0f && !IsDead)
        {
            Die();
        }
    }

    // 사망 처리
    protected virtual void Die()
    {
        IsDead = true;

        if (OnDeath != null)
        {
            OnDeath();
        }       
    }
}
