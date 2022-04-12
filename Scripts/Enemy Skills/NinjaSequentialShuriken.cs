using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Skills/Ninja Sequential Shuriken Skill")]
public class NinjaSequentialShuriken : SequentialProjectileFire
{
    protected override Vector2[] GetCreationPositions(Vector2 user, Vector2 target, Vector2 firePosition)
    {
        // 투사체 생성을 위한 위치 전처리
        int projectileCount = 5;
        int backProjectileCount = 3;
        Vector2 middlePosition = firePosition;
        Vector2 intervalControl = Mathf.Sign((int)creationDirection) * Mathf.Sign(90f - layoutAngle)
            * creationDistanceInterval * (Quaternion.Euler(0f, 0f, layoutAngle) * Vector2.right).normalized;
        Vector2 current = middlePosition - (backProjectileCount - 1) / 2f * intervalControl;

        if (creationDirection == CreationDirection.Automatic)
        {
            Vector2 lhs = current;
            Vector2 rhs = middlePosition + (backProjectileCount - 1) / 2f * intervalControl;
            if ((rhs - target).magnitude < (lhs - target).magnitude)
            {
                current = rhs;
                intervalControl *= -1f;
            }
        }

        Vector2[] creationPositions = new Vector2[projectileCount];
        for (int i = 0; i < backProjectileCount; i++)
        {
            creationPositions[i] = current;
            current += intervalControl;
        }

        middlePosition.x += Mathf.Sign(firePosition.x - user.x) * intervalControl.magnitude;
        current = middlePosition - (projectileCount - backProjectileCount - 1) / 2f * intervalControl;

        for (int i = backProjectileCount; i < projectileCount ; i++)
        {
            creationPositions[i] = current;
            current += intervalControl;
        }

        return creationPositions;
    }
}
