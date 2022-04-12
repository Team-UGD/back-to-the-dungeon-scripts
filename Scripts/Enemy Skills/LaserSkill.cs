using UnityEngine;

public abstract class LaserSkill : EnemySkill
{
    [SerializeField] private Laser laserPrefab;

    [Header("Attack Options")]
    [SerializeField] protected float baseDamage = 10f;
    [SerializeField] protected float strWeight = 1f;
    [SerializeField] protected float knockbackPower = 1f;

    public abstract Laser.Route GetRoute(Attacker user, GameObject target);

    public override void TriggerSkill(Attacker user, GameObject target)
    {
        var route = GetRoute(user, target);
        var laser = Instantiate(LaserPrefab, route.start, Quaternion.identity);
        laser.Damage = baseDamage + GetSTR(user) * strWeight;
        laser.AutoDestroy = true;
        laser.KnockbackPower = knockbackPower;
        laser.transform.position = route.start;

        Rotate(laser, route);

        if (laser.IsReady)
            laser.Fire(route);
        else
            Destroy(laser.gameObject);
    }

    public Laser LaserPrefab => this.laserPrefab;

    /// <summary>
    /// 레이저 발사 방향에 맞춰 laser 오브젝트 자체의 회전과 스케일 값을 조정함.
    /// </summary>
    protected void Rotate(Laser laser, in Laser.Route route)
    {
        Vector2 directionToRotate = route.end - route.start;

        float rotateDegree = Mathf.Atan2(directionToRotate.y, directionToRotate.x) * Mathf.Rad2Deg;
        laser.transform.rotation = Quaternion.Euler(0f, 0f, rotateDegree);

        var scale = laser.transform.localScale;
        scale.y *= scale.y * Mathf.Sign(directionToRotate.x);
        laser.transform.localScale = scale;
    }
}
