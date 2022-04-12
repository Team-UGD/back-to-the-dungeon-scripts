using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Mathf;
using static UnityEngine.Vector2;

[CreateAssetMenu(menuName = "Enemy Skills/Throw Boomerang")]
public class ThrowBoomerang : EnemySkill, ISkillFirePosition
{
    [Header("Skill Info")]
    [SerializeField] private MeleeWeapon sword;
    [SerializeField] private float baseDamage;
    [SerializeField] private float strWeight;
    [SerializeField] private float fireWaitTime;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Vector2 extentsFromUser;
    [SerializeField] private int segmentCount;

    [Header("Path Settings")]
    [SerializeField] private float sectionDistance;

    [Space]
    [SerializeField] private List<PathProperty> path1Property = new List<PathProperty>();
    [SerializeField] private List<PathProperty> path2Property = new List<PathProperty>();
    [SerializeField] private List<PathProperty> path3Property = new List<PathProperty>();
    [SerializeField] private List<PathProperty> path4Property = new List<PathProperty>();


    public Transform FirePosition { get; set; }

    public override void TriggerSkill(Attacker user, GameObject target)
    {
        var parent = new GameObject();
        float dir = Mathf.Sign(user.transform.position.x - target.transform.position.x);

        if (FirePosition == null)
            parent.transform.position = user.transform.position + new Vector3(-dir * extentsFromUser.x, extentsFromUser.y, 0f);
        else
            parent.transform.position = this.FirePosition.position;

        var parentScale = parent.transform.localScale;
        parentScale.x *= dir;
        parent.transform.localScale = parentScale;

        MeleeWeapon sword = Instantiate(this.sword, parent.transform);
        sword.StartCoroutine(Attack(user, target, parent, sword));
    }

    private IEnumerator Attack(Attacker user, GameObject target, GameObject parent, MeleeWeapon sword)
    {
        var fade = sword as IFade;
        var bezier = parent.AddComponent<BezierMoveTool>();
        bezier.SegmentCount = this.segmentCount;

        sword.StartCoroutine(Rotate(sword));

        float t = fade?.FadeIn() ?? -1;
        if (t > 0)
            yield return new WaitForSeconds(t);

        if (sword is IAttackTime)
            (sword as IAttackTime).AttackTime = 100f;
        sword.Attack(baseDamage + strWeight * GetSTR(user));
        yield return new WaitForSeconds(fireWaitTime);

        for (int i = 0; i < 4; i++)
        {
            bezier.Add(new BezierPath2());
        }

        bezier[0].property = path1Property;
        bezier[1].property = path2Property;
        bezier[2].property = path3Property;
        bezier[3].property = path4Property;

        bezier[0].point.Add(parent.transform.position);
        bezier[0].point.Add(target.transform.position);
        bezier.MovePathOnce(0);

        bezier.OnPathMove += (p, c, t) =>
        {
            if (p == bezier[3] && c.T > 0.7f)
                bezier.StartCoroutine(FadeOut(fade, parent));

            if (!t)
                return;

            if (p == bezier[0])
            {
                var path1Goal = bezier[0].point.Last();
                Vector2 direction = (path1Goal - bezier[0].point[0]).normalized;
                var path2 = bezier[1].point;
                path2.Add(path1Goal);
                path2.Add(sectionDistance * direction + path2[0]);
                path2.Add(sectionDistance * Sign(direction.x) * right + path2[1]);
                path2.Add(sectionDistance / 2f * Sign(target.transform.position.y - path2[0].y) * up + path2[2]);

                bezier.MovePathOnce(1);
            }
            else if (p == bezier[1])
            {
                var path2 = bezier[1].point;
                var path3 = bezier[2].point;
                var path2Goal = path2.Last();
                Vector2 targetPos = target.transform.position;

                path3.Add(path2Goal);
                path3.Add(sectionDistance / 2f * Sign(path2[3].y - path2[2].y) * up + path3[0]);
                path3.Add(new Vector2((targetPos.x - path3[1].x) / 2f, 0f) + path3[1]);
                path3.Add(targetPos);

                bezier.MovePathOnce(2);
            }
            else if (p == bezier[2])
            {
                var path3 = bezier[2].point;
                var path4 = bezier[3].point;

                float h = 1e-5f;
                Vector2 direction = (PhysicsUtility.BezierCurve(path3, c.T) - PhysicsUtility.BezierCurve(path3, c.T - h)).normalized;

                path4.Add(bezier.transform.position);
                path4.Add(sectionDistance * 1.5f * direction + path4[0]);

                bezier.MovePathOnce(3);
            }
        };             

        //Vector2 firePosition = parent.transform.position;
        //Vector3 direction = ((Vector2)target.transform.position + new Vector2(0f, 1f) - firePosition).normalized;

        //while (Vector2.Distance(firePosition, parent.transform.position) <= attackRange)
        //{
        //    yield return null;
        //    parent.transform.position += moveSpeed * direction * Time.deltaTime;
        //}     
    }

    private IEnumerator FadeOut(IFade fade, GameObject destroied)
    {
        float time = fade?.FadeOut() ?? -1;
        if (time > 0)
            yield return new WaitForSeconds(time);

        Destroy(destroied);
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
