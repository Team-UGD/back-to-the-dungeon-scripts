using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Skills/Laser Target Aim Skill")]
public sealed class LaserTargetAimSkill : LaserSkill
{
    [Header("Creation Options")]
    [SerializeField, Tooltip("User를 기준으로 한 경계 범위")] private Vector2 extents;
    [SerializeField] private Direction direction;

    public enum Direction
    {
        Horizontal,
        Vertical
    }

    public override Laser.Route GetRoute(Attacker user, GameObject target)
    {
        Vector2 displacement = target.transform.position - user.transform.position;
        var collider = target.GetComponent<Collider2D>();
        if (collider != null)
            displacement = collider.bounds.center - user.transform.position;

        Vector3 relativeStart;
        Vector3 relativeEnd;

        switch (direction)
        {
            case Direction.Horizontal:
                relativeStart = new Vector3(Mathf.Sign(displacement.x) * extents.x, Mathf.Clamp(displacement.y, -extents.y, extents.y));
                relativeEnd = new Vector3(-relativeStart.x, relativeStart.y);
                return new Laser.Route(user.transform.position + relativeStart, user.transform.position + relativeEnd);
            case Direction.Vertical:
                relativeStart = new Vector3(Mathf.Clamp(displacement.x, -extents.x, extents.x), Mathf.Sign(displacement.y) * extents.y);
                relativeEnd = new Vector3(relativeStart.x, -relativeStart.y);
                return new Laser.Route(user.transform.position + relativeStart, user.transform.position + relativeEnd);
            default:
                throw new System.InvalidOperationException();
        }
    }
}
