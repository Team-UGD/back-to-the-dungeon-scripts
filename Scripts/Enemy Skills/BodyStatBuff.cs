using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Skills/Body Stat Buff")]
public class BodyStatBuff : EnemySkill
{
    [Header("Buff Interpolation")]
    [SerializeField] private AnimationCurve distribution = new AnimationCurve();
    [SerializeField, Min(0f)] private float changeTime;
    [SerializeField, Min(0f)] private float duration;
    [SerializeField, Min(0f)] private float restoreTime;
    [SerializeField] private ColorSetting colorSetting = new ColorSetting();

    [Header("Buff Value")]
    [SerializeField] private RandomRange speedUp;
    [SerializeField] private RandomRange jumpHeightUP;
    [SerializeField] private RandomRange jumpWidthUp;
    [SerializeField] private RandomRange maxHealthUp;
    [SerializeField] private RandomRange strUp;

    [System.Serializable]
    protected struct RandomRange
    {
        [Min(0f)] public float randMin;
        [Min(0f)] public float randMax;

        public float Value { get => Random.Range(randMin, randMax); }
    }

    [System.Serializable]
    private class ColorSetting
    {
        public bool enabled = false;
        public Color goal = Color.white;
    }

    public override void TriggerSkill(Attacker user, GameObject target)
    {
        user.StartCoroutine(StatUp(user));
        if (colorSetting.enabled)
            user.StartCoroutine(ColorChange(user));
    }

    private IEnumerator StatUp(Attacker user)
    {
        float previousMaxHelath = user.EnemyComponent.MaxHealth;
        float previousSpeed = user.Pathfinder.MaxSpeed;
        float previousJumpWidth = user.Pathfinder.MaxJumpWidth;
        float previousJumpHeight = user.Pathfinder.JumpHeight;
        uint previousStr = user.EnemyComponent.STR;

        float goalMaxHealth = previousMaxHelath + this.maxHealthUp.Value;
        float goalSpeed = previousSpeed + this.speedUp.Value;
        float goalJumpHeight = previousJumpHeight + this.jumpHeightUP.Value;
        float goalJumpWidth = previousJumpWidth + this.jumpWidthUp.Value;
        uint goalStr = previousStr + (uint)this.strUp.Value;

        float t = 0f;
        while (t < this.changeTime)
        {
            t += Time.deltaTime;
            float evaluated = distribution.Evaluate(Mathf.InverseLerp(0f, this.changeTime, t));
            float oldMaxHealth = user.EnemyComponent.MaxHealth;
            user.EnemyComponent.MaxHealth = Mathf.Lerp(previousMaxHelath, goalMaxHealth, evaluated);
            user.EnemyComponent.Heal(user.EnemyComponent.MaxHealth - oldMaxHealth);
            user.Pathfinder.MaxSpeed = Mathf.Lerp(previousSpeed, goalSpeed, evaluated);
            user.Pathfinder.JumpHeight = Mathf.Lerp(previousJumpHeight, goalJumpHeight, evaluated);
            user.Pathfinder.MaxJumpWidth = Mathf.Lerp(previousJumpWidth, goalJumpWidth, evaluated);
            user.EnemyComponent.STR = (uint)Mathf.RoundToInt(Mathf.Lerp(previousStr, goalStr, evaluated));
            yield return null;
        }

        yield return new WaitForSeconds(this.duration);

        t = this.restoreTime;
        while (t > 0f)
        {
            t -= Time.deltaTime;
            float evaluated = distribution.Evaluate(Mathf.InverseLerp(0f, this.restoreTime, t));
            user.EnemyComponent.MaxHealth = Mathf.Lerp(previousMaxHelath, goalMaxHealth, evaluated);
            //user.EnemyComponent.Heal(user.EnemyComponent.MaxHealth - oldMaxHealth);
            user.Pathfinder.MaxSpeed = Mathf.Lerp(previousSpeed, goalSpeed, evaluated);
            user.Pathfinder.JumpHeight = Mathf.Lerp(previousJumpHeight, goalJumpHeight, evaluated);
            user.Pathfinder.MaxJumpWidth = Mathf.Lerp(previousJumpWidth, goalJumpWidth, evaluated);
            user.EnemyComponent.STR = (uint)Mathf.RoundToInt(Mathf.Lerp(previousStr, goalStr, evaluated));
            yield return null;
        }
    }

    private IEnumerator ColorChange(Attacker user)
    {
        SpriteRenderer sprite = user.Sprite;
        float t = 0f;
        Color start = sprite.color;
        while (t < this.changeTime)
        {
            t += Time.deltaTime;
            sprite.color = Color.Lerp(start, colorSetting.goal, Mathf.InverseLerp(0f, this.changeTime, t));
            yield return null;
        }

        yield return new WaitForSeconds(duration);

        t = this.restoreTime;
        while (t > 0)
        {
            t -= Time.deltaTime;
            sprite.color = Color.Lerp(start, colorSetting.goal, Mathf.InverseLerp(0f, this.restoreTime, t));
            yield return null;
        }
    }
}
