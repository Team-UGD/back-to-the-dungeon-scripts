#define DEBUG

using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class PhysicsUtility
{
    /// <summary>
    /// 조합구하는 공식
    /// </summary>
    /// <param name="n"></param>
    /// <param name="i"></param>
    /// <returns></returns>

    public static float Combination(int n, int i)
    {
        if (i < 0)
        {
            return 1;
        }
        else if (i > n)
        {
            return 1;
        }
        float p = 1;
        for (int j = 0; j < i; j++)
        {
            p *= (n - j);
        }
        float rP = 1;
        for (int k = 1; k <= i; k++)
        {
            rP *= k;
        }
        return p / rP;
    }
    /// <summary>
    /// 베지에 곡선의 최종 보간된 위치 <br/>
    /// 잘 사용하시길...
    /// </summary>
    /// <param name="dataset"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public static Vector2 BezierCurve(List<Vector2> dataset, float t)
    {
        if (dataset.Count <= 1)
        {
            throw new InvalidOperationException("두 개 이상의 점의 개수를 넣어야합니다.");
        }
        int n = dataset.Count - 1;
        Vector2 B = new Vector2();
        for (int i = 0; i <= n; i++)
        {
            B += Combination(n, i) * Mathf.Pow(t, i) * Mathf.Pow((1 - t), n - i) * dataset[i];
        }
        return B;
    }

    public static Vector2 BezierCurve(Vector2[] dataset, float t)
    {
        if (dataset.Length <= 1)
        {
            throw new InvalidOperationException("두 개 이상의 점의 개수를 넣어야합니다.");
        }
        int n = dataset.Length - 1;
        Vector2 B = new Vector2();
        for (int i = 0; i <= n; i++)
        {
            B += Combination(n, i) * Mathf.Pow(t, i) * Mathf.Pow((1 - t), n - i) * dataset[i];
        }
        return B;
    }

    /// <summary>
    /// 주어진 데이터 집합을 통해 베지어 곡선을 보간해주는 함수
    /// </summary>
    /// <param name="dataSet">데이터 집합</param>
    /// <returns>보간된 베지어 함수</returns>
    public static BezierFunction<Vector2> BezierInterpolation(IEnumerable<Vector2> dataSet)
    {
        var set = dataSet.ToArray();

        if (set.Length < 2)
            throw new InvalidOperationException("두 개 이상의 데이터가 필요합니다.");

        // 0 ~ n
        int n = set.Length - 1;
        float[] combinations = new float[n + 1];
        for (int i = 0; i <= n; i++)
        {
            combinations[i] = Combination(n, i);
        }

        Vector2 Bezier(float t)
        {
            Vector2 result = Vector2.zero;
            for (int i = 0; i <= n; i++)
            {
                result += combinations[i] * Mathf.Pow(t, i) * Mathf.Pow(1 - t, n - i) * set[i];
            }
            return result;
        }

        return new BezierFunction<Vector2>(Bezier);
    }

    /// <summary>
    /// 포물선 형태의 Linecast를 발동합니다. start ~ vertex, vertex ~ end 사이 각각에 포물선 형태의 Linecast가 발동됩니다.<br/>
    /// 현재는 y = ax^2 꼴 형태의 포물선만 지원합니다.
    /// </summary>
    /// <remarks>
    /// 이 메서드는 연산 비용이 비쌉니다. 특히 intersectionCount 값에 비례합니다.
    /// </remarks>
    /// <param name="start">시작점의 월드 좌표</param>
    /// <param name="vertex">꼭짓점의 월드 좌표</param>
    /// <param name="end">끝점의 월드 좌표</param>
    /// <param name="layerMask">감지할 레이어</param>
    /// <param name="intersectionCount">각 포물선의 구간에서의 교점 개수. 값이 클 수록 정밀도가 높아짐.</param>
    /// <returns>ParabolaLinecast의 감지된 정보</returns>
    public static RaycastHit2D ParabolaLinecast(Vector2 start, Vector2 vertex, Vector2 end, int layerMask, int intersectionCount = 2)
    {
        RaycastHit2D hitInfo = ParabolaLinecast(start, vertex, true, layerMask, intersectionCount);
        if (hitInfo.collider != null)
            return hitInfo;

        return ParabolaLinecast(end, vertex, false, layerMask, intersectionCount);
    }

    /// <summary>
    /// 포물선 형태의 Linecast를 발동합니다. point ~ vertex 사이에 포물선 형태의 Linecast가 발동됩니다.<br/>
    /// 현재는 y = ax^2 꼴 형태의 포물선만 지원합니다.
    /// </summary>
    /// <remarks>
    /// 이 메서드는 연산 비용이 비쌉니다. 특히 intersectionCount 값에 비례합니다.
    /// </remarks>
    /// <param name="point">포물선 위 정점의 월드 좌표</param>
    /// <param name="vertex">꼭짓점의 월드 좌표</param>
    /// <param name="point2Vertex">Linecast가 정점에서 꼭짓점으로 향할 지 여부</param>
    /// <param name="layerMask">감지할 레이어</param>
    /// <param name="intersectionCount">포물선의 구간에서의 교점 개수. 값이 클 수록 정밀도가 높아짐.</param>
    /// <returns>ParabolaLinecast의 감지된 정보</returns>
    public static RaycastHit2D ParabolaLinecast(Vector2 point, Vector2 vertex, bool point2Vertex, int layerMask, int intersectionCount = 2)
    {
        // 교점 개수가 0일 때는 기본 Linecast를 사용한다.
        if (intersectionCount == 0)
        {
            if (point2Vertex)
            {
#if DEBUG
                Debug.DrawLine(point, vertex);
#endif
                return Physics2D.Linecast(point, vertex, layerMask);
            }
            else
            {
#if DEBUG
                Debug.DrawLine(vertex, point);
#endif
                return Physics2D.Linecast(vertex, point, layerMask);
            }
        }
        // 교점 개수가 음수값이면 ArgumentException을 발생시킨다.
        else if (intersectionCount < 0)
        {
            throw new System.ArgumentException($"The argument value of {nameof(intersectionCount)} must be more than zero.", nameof(intersectionCount));
        }


        float leadingCoefficient = GetLeadingCoefficient(point, vertex); // 2차함수의 최고차항 계수
        float xInterval = (point2Vertex ? 1 : -1) * (vertex.x - point.x) / intersectionCount; // point와 vertex 사이의 방향 정보가 포함된 간격.

        Vector2 p1 = point2Vertex ? point : vertex; // 시작 위치

        for (int i = 0; i < intersectionCount; i++)
        {
            float p2_x = p1.x + xInterval;
            Vector2 p2 = new Vector2(p2_x, leadingCoefficient * Mathf.Pow(p2_x - vertex.x, 2) + vertex.y); // 끝 위치

            float intersection_x = (p2.x + p1.x) - (p2.y - p1.y) / (2 * leadingCoefficient * (p2.x - p1.x)) - vertex.x;
            Vector2 intersection = new Vector2(intersection_x, 2 * leadingCoefficient * (p1.x - vertex.x) * (intersection_x - p1.x) + p1.y); // p1과 p2의 각 접선의 교점.

#if DEBUG
            Debug.DrawLine(p1, intersection, Color.Lerp(Color.cyan, Color.magenta, i / Mathf.Max(intersectionCount - 1, 1)));
#endif
            RaycastHit2D hitInfo = Physics2D.Linecast(p1, intersection, layerMask);
            if (hitInfo.collider != null)
                return hitInfo;

#if DEBUG
            Debug.DrawLine(intersection, p2, Color.Lerp(Color.cyan, Color.magenta, i / Mathf.Max(intersectionCount - 1, 1)));
#endif
            hitInfo = Physics2D.Linecast(intersection, p2, layerMask);
            if (hitInfo.collider != null)
                return hitInfo;

            p1 = p2; // 갱신
        }

        return new RaycastHit2D();
    }

    public static void DrawParabolaLine(Vector2 start, Vector2 vertex, Vector2 end, Color color, int intersectionCount = 2, float duration = 0f)
    {
        DrawParabolaLine(start, vertex, true, color, intersectionCount, duration);
        DrawParabolaLine(end, vertex, false, color, intersectionCount, duration);
    }

    public static void DrawParabolaLine(Vector2 point, Vector2 vertex, bool point2Vertex, Color color, int intersectionCount = 2, float duration = 0f)
    {
        // 교점 개수가 0일 때는 기본 Line을 그린다.
        if (intersectionCount == 0)
        {
            if (point2Vertex)
            {
                Debug.DrawLine(point, vertex, color, duration);
                return;
            }
            else
            {
                Debug.DrawLine(vertex, point, color, duration);
                return;
            }
        }
        // 교점 개수가 음수값이면 ArgumentException을 발생시킨다.
        else if (intersectionCount < 0)
        {
            throw new System.ArgumentException($"The argument value of {nameof(intersectionCount)} must be more than zero.", nameof(intersectionCount));
        }

        float leadingCoefficient = GetLeadingCoefficient(point, vertex); // 2차함수의 최고차항 계수
        float xInterval = (point2Vertex ? 1 : -1) * (vertex.x - point.x) / intersectionCount; // point와 vertex 사이의 방향 정보가 포함된 간격.

        Vector2 p1 = point2Vertex ? point : vertex; // 시작 위치

        for (int i = 0; i < intersectionCount; i++)
        {
            float p2_x = p1.x + xInterval;
            Vector2 p2 = new Vector2(p2_x, leadingCoefficient * Mathf.Pow(p2_x - vertex.x, 2) + vertex.y); // 끝 위치

            float intersection_x = (p2.x + p1.x) - (p2.y - p1.y) / (2 * leadingCoefficient * (p2.x - p1.x)) - vertex.x;
            Vector2 intersection = new Vector2(intersection_x, 2 * leadingCoefficient * (p1.x - vertex.x) * (intersection_x - p1.x) + p1.y); // p1과 p2의 각 접선의 교점.

            Debug.DrawLine(p1, intersection, color, duration);
            Debug.DrawLine(intersection, p2, color, duration);

            p1 = p2; // 갱신
        }
    }

    /// <summary>
    /// 한 정점과 꼭짓점에 의해 결정되는 2차 방정식의 임의의 y좌표에서의 해를 구한다.
    /// </summary>
    /// <param name="point">2차함수 위 정점의 월드 좌표</param>
    /// <param name="vertex">꼭짓점의 월드 좌표</param>
    /// <param name="y">해의 y좌표</param>
    /// <param name="solutions">해가 담긴 배열. 허근일 시 null. 해의 개수에 따라 배열 크기 결정됨.</param>
    /// <returns>허근이 아니면 true를 반환합니다.</returns>
    public static bool QuadraticFormula(Vector2 point, Vector2 vertex, float y, out float[] solutions)
    {
        float a = GetLeadingCoefficient(point, vertex); // 2차항의 계수
        float b = -2f * a * vertex.x; // 1차항의 계수
        float c = a * vertex.x * vertex.x + vertex.y - y; // 상수

        float discriminant = b * b - 4 * a * c; // 판별식
        if (discriminant < 0f)
        {
            solutions = null;
            return false;
        }
        else if (discriminant == 0f)
        {
            solutions = new float[1];
            solutions[0] = -b / (2f * a);
        }
        else
        {
            solutions = new float[2];
            float x1 = (-b - Mathf.Sqrt(discriminant)) / (2f * a);
            float x2 = (-b + Mathf.Sqrt(discriminant)) / (2f * a);
            solutions[0] = Mathf.Min(x1, x2);
            solutions[1] = Mathf.Max(x1, x2);
        }

        return true;
    }

    /// <summary>
    /// 뉴턴 다항식 보간법을 활용한 데이터 셋 보간
    /// </summary>
    /// <param name="points">데이터 셋</param>
    /// <returns>보간된 함수</returns>
    public static Function NewtonPolynomial(params Vector2[] points)
    {      
        if (points.Length < 2)
        {
            throw new ArgumentException($"뉴턴 다항식 보간을 위해서는 최소 2개의 Point가 필요합니다. Point 개수: {points.Length}개");
        }

        int n = points.Length; // 데이터 개수 == 보간하려는 다항함수 식의 계수 개수
        float[,] dividedDifferenceTable = new float[n, n]; // 뉴턴 분할차분표

        // 0번째 열에 y좌표(함수값) 복사
        for (int i = 0; i < n; i++)
        {
            dividedDifferenceTable[i, 0] = points[i].y;
        }

        // 분할 차분 값들을 이전 분할 차분 값으로부터 순차적으로 구함
        for (int j = 1; j < n; j++)
        {
            for (int i = 0; i < n - j; i++)
            {
                dividedDifferenceTable[i, j] = (dividedDifferenceTable[i + 1, j - 1] - dividedDifferenceTable[i, j - 1]) / (points[i + j].x - points[i].x);
            }
        }

        // 보간하려는 다항함수의 계수: 분할차분표의 0번째 행
        float[] coef = new float[n];
        for (int i = 0; i < n; i++)
        {
            coef[i] = dividedDifferenceTable[0, i];
        }

        // 필요한 데이터의 x값(마지막 x값은 필요 없음)
        float[] x_points = new float[n - 1];
        for (int i = 0; i < n - 1; i++)
        {
            x_points[i] = points[i].x;
        }

        // 기존 dividedDifferenceTable과 points 배열 변수를 사용해도 되지만 아래 Interpolate() 로컬 함수에서 변수 캡쳐 시
        // 필요없는 값들까지 유지되기 떄문에 위 coef와 x_points 배열을 새로 생성해 필요한 값만 할당한 후 이 배열들을 캡쳐시켜
        // 기존에 뉴턴 분할차분표에 있던 필요없는 값들을 갈비지 컬렉터가 수집하도록 하기 위함임.
        // 즉 메모리 절약을 위해서 새롭게 배열을 할당함. 자세한 내용은 C# 클로저 개념을 참조.
        float Interpolate(float x)
        {
            float functionValue = coef[0]; // 반환할 함수값
            float newtonBasisPoly = 1f; // 뉴턴 기반 다항식

            for (int k = 1; k < coef.Length; k++)
            {
                newtonBasisPoly *= x - x_points[k - 1]; // 누적곱을 사용함
                functionValue += coef[k] * newtonBasisPoly;
            }

            return functionValue;
        }

        // 위 로컬함수 Interpolate(float)를 대리자 인스턴스에 추가후 반환함.
        return new Function(Interpolate);
    }

    private static float GetLeadingCoefficient(Vector2 point, Vector2 vertex)
    {
        return (point.y - vertex.y) / Mathf.Pow(point.x - vertex.x, 2);
    }
}

/// <summary>
/// 보간된 베지어 함수
/// </summary>
/// <typeparam name="T">함수의 반환 형식</typeparam>
/// <param name="t">비중(0 ~ 1)</param>
/// <returns>비중에 따른 베지어 함숫값</returns>
public delegate T BezierFunction<out T>(float t);

/// <summary>
/// 2차원에서의 함수
/// </summary>
/// <param name="x">x좌표</param>
/// <returns>x좌표에 따른 함숫값</returns>
public delegate float Function(float x);