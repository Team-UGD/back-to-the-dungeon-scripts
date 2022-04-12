using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Skills/Spell Skill")]
public class SpellSkill : EnemySkill
{
    [Header("Settings")]
    [SerializeField] private BringerOfDeathSpell spellSkillPrefab;
    [SerializeField] private byte maxSpellCount = 5;

    [Header("Creation")]
    [SerializeField] private float creationWaitTime;
    [SerializeField] private float creationTimeInterval;
    [SerializeField] private float creationDistanceInterval;
    [SerializeField] private AudioClip creationSound;

    [Header("Attack")]
    [SerializeField] private float baseDamage;
    [SerializeField] private float attackTimeInterval;
    [SerializeField] private AudioClip fireSound;
    [SerializeField] private float fireSoundDelay;

    public override void TriggerSkill(Attacker user, GameObject target)
    {
        user.StartCoroutine(Attack(user, target));
    }

    private IEnumerator Attack(Attacker user, GameObject target)
    {
        if (creationWaitTime > 0)
            yield return new WaitForSeconds(creationWaitTime);

        var entity = user.EnemyComponent;

        if (entity.IsDead)
            yield break;

        Vector3 correctionForTargetPostion = new Vector2(0f, 0.75f);
        Vector2 targetPosition = target.transform.position + correctionForTargetPostion;

        //스펠 생성
        byte spellCount = (byte)Random.Range(1, maxSpellCount + 1);
        BringerOfDeathSpell[] spells = new BringerOfDeathSpell[spellCount];
        AudioSource userAudioPlayer = user.AudioSourceComponent;

        Vector2[] creationPostion = GetCreationPositions(user.transform.position, targetPosition, spellCount);
        int arrayLength = creationPostion.Length;
        if (arrayLength <= 0)
            yield break;

        float damage = baseDamage + GetSTR(user, true) * 2 / spellCount;
        for (int i = 0; i < spells.Length; i++)
        {
#if TEST
            Debug.LogWarning($"[{typeof(SequentialProjectileFire)}] {i + 1}번째 투사체 생성");
#endif
            if (entity.IsDead)
                break;

            targetPosition = target.transform.position + correctionForTargetPostion;
            Vector2 direction = new Vector2((targetPosition.x - user.transform.position.x), 0f).normalized; // 생성 방향
            spells[i] = Instantiate(spellSkillPrefab, creationPostion[i % arrayLength], Quaternion.identity);

            if (userAudioPlayer)
                userAudioPlayer.PlayOneShot(creationSound);


            spells[i].StartCoroutine(AttackCoroutine(spells[i], damage, userAudioPlayer));

            if (creationTimeInterval > 0)
                yield return new WaitForSeconds(creationTimeInterval);
        }
    }

    private IEnumerator AttackCoroutine(BringerOfDeathSpell spell, float damage, AudioSource audioSource)
    {
        yield return new WaitForSeconds(attackTimeInterval);

        spell.Attack(damage);
      
        if (audioSource)
        {
            yield return new WaitForSeconds(this.fireSoundDelay);
            audioSource.PlayOneShot(this.fireSound);
        }
    }

    protected virtual Vector2[] GetCreationPositions(Vector2 user, Vector2 target, byte positionCount)
    {
        // 투사체 생성을 위한 위치 전처리
        Vector2 middlePosition = target + new Vector2(0f, 2f);
        Vector2 intervalControl = creationDistanceInterval * Vector2.right;
        Vector2 current = middlePosition - (positionCount - 1) / 2f * intervalControl;

        Vector2 rhs = middlePosition + (positionCount - 1) / 2f * intervalControl;
        if (Random.value < 0.5f)
        {
            current = rhs;
            intervalControl *= -1f;
        }

        Vector2[] creationPositions = new Vector2[positionCount];
        for (int i = 0; i < positionCount; i++)
        {
            creationPositions[i] = current;
            current += intervalControl;
        }

        return creationPositions;
    }
}
