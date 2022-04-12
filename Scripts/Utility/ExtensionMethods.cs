using System;
using UnityEngine;
using static UnityEngine.Mathf;

public static class ExtensionMethods
{
    /// <summary>
    /// 투사체와 타겟 사이의 위치를 고려해 발사 방향을 보정함.
    /// </summary>
    /// <param name="target">타겟</param>
    /// <param name="maxAngle">투사체로부터 타겟까지의 방향벡터와 수평축이 이루는 최대 허용 각도</param>
    /// <returns>보정된 방향</returns>
    /// <exception cref="System.ArgumentNullException">호출 객체나 target이 null일 경우</exception>
    public static Vector2 FireDirectionCorrection(this Bullet bullet, Transform target, float maxAngle)
    {
        if (bullet == null)
        {
            throw new ArgumentNullException(nameof(bullet));
        }
        else if (target == null)
        {
            throw new ArgumentNullException(nameof(target));
        }

        var targetCollider = target.GetComponent<Collider2D>();

        Vector2 firePosition = bullet.transform.position;
        Vector2 targetPosition = target.transform.position;
        Vector2 direction = (targetPosition - firePosition).normalized;

        if (targetCollider == null)
            return direction;

        Bounds bounds = targetCollider.bounds;
        targetPosition = bounds.center;
        direction = (targetPosition - firePosition).normalized;

        float angle = Vector2.Angle(Vector2.right, direction);
        if (angle > maxAngle && angle < 180f - maxAngle)
        {
            targetPosition.y += bounds.extents.y;
            direction = (targetPosition - firePosition).normalized;
        }

        return direction;
    }

    /// <summary>
    /// 벡터의 회전 변환
    /// </summary>
    /// <param name="theta">회전할 각도(라디안)</param>
    /// <returns></returns>
    public static Vector2 Rotation(this Vector2 v, float theta)
    {
        return new Vector2(v.x * Cos(theta) - v.y * Sin(theta), v.x * Sin(theta) + v.y * Cos(theta));
    }

    /// <summary>
    /// 문자열을 인스펙터 표시 용 문자열로 변경해준다.
    /// </summary>
    /// <returns>인스펙터 표시 용 문자열</returns>
    public static string InspectorLabel(this string variableName)
    {
        if (string.IsNullOrEmpty(variableName))
        {
            return string.Empty;
        }

        string temp = variableName.Trim();
        temp = char.ToUpper(variableName[0]) + variableName.Substring(1); // 첫 글자 대문자화

        // 소문자 -> 대문자로 변할 시 공백 삽입
        for (int i = 0; i < temp.Length - 1; i++)
        {
            if (!char.IsLetterOrDigit(temp[i]) || !char.IsLetterOrDigit(temp[i + 1]))
                continue;

            if ((char.IsLower(temp[i]) && !char.IsLower(temp[i + 1])) || (!char.IsUpper(temp[i]) && char.IsUpper(temp[i + 1])))
            {
                temp = temp.Insert(i + 1, " ");
            }
        }

        return temp;
    }
}
