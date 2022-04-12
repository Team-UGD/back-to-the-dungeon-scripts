using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Skills/Throw Sword")]
public class ThrowRotatedSword : EnemySkill, ISkillFirePosition
{
    [Header("Skill Info")]
    [SerializeField] private MeleeWeapon sword;
    [SerializeField] private float baseDamage;
    [SerializeField] private float strWeight;
    [SerializeField] private float fireWaitTime;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float attackRange;
    [SerializeField] private Vector2 extentsFromUser;

    public Transform FirePosition { get; set; }

    public override void TriggerSkill(Attacker user, GameObject target)
    {
        var parent = new GameObject();
        float dir = Mathf.Sign(user.transform.position.x - target.transform.position.x);


        if (FirePosition == null)
        {
            parent.transform.position = user.transform.position + new Vector3(-dir * extentsFromUser.x, extentsFromUser.y, 0f);
        }
        else
        {
            parent.transform.position = this.FirePosition.position;
        }

        var parentScale = parent.transform.localScale;
        parentScale.x *= dir;
        parent.transform.localScale = parentScale;

        MeleeWeapon sword = Instantiate(this.sword, parent.transform);

        sword.StartCoroutine(Attack(user, target, parent, sword));
    }

    private IEnumerator Attack(Attacker user, GameObject target, GameObject parent, MeleeWeapon sword)
    {
        var fade = sword as IFade;

        sword.StartCoroutine(Rotate(sword));

        float t = fade?.FadeIn() ?? -1;
        if (t > 0)
            yield return new WaitForSeconds(t);

        if (sword is IAttackTime)
            (sword as IAttackTime).AttackTime = fireWaitTime + attackRange / moveSpeed;
        sword.Attack(baseDamage + strWeight * GetSTR(user));
        yield return new WaitForSeconds(fireWaitTime);

        Vector2 firePosition = parent.transform.position;
        Vector3 direction = ((Vector2)target.transform.position + new Vector2(0f, 1f) - firePosition).normalized;

        while (Vector2.Distance(firePosition, parent.transform.position) <= attackRange)
        {
            yield return null;
            parent.transform.position += moveSpeed * direction * Time.deltaTime;
        }

        t = fade?.FadeOut() ?? -1;
        if (t > 0)
            yield return new WaitForSeconds(t);

        Destroy(parent);
    }

    private IEnumerator Rotate(MeleeWeapon sword, Space rotationMode = Space.Self)
    {
        float t = 0f;
        while (true)
        {
            yield return null;
            sword.transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime, rotationMode);
            t += Time.deltaTime;
        }
    }
}
