using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beez : Enemy
{
    //SpriteRenderer spriteRenderer;
    Animator animator;
    public override float Health { get => base.Health; protected set => base.Health = value; }
    
    public override void TakeDamage(float damage, Vector2 hitPosition, Vector2 hitNormal , bool isContinuous = false, float power = 1, float use = 1)
    {
        base.TakeDamage(damage, hitPosition, hitNormal);
    }

    public override void Heal(float healthToHeal)
    {
        base.Heal(healthToHeal);
    }

    protected override void Die()
    {
        base.Die();

        animator.SetTrigger("Die");
    }

    void Awake()
    {
        //spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
