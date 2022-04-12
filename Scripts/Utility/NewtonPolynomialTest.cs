using System.Collections.Generic;
using UnityEngine;

public class NewtonPolynomialTest : MonoBehaviour
{
    public List<Vector2> dataSets;
    public float startX = -5;
    public float endX = 5;
    public byte pointCount = 20;

    private Function f;
    private Vector2[] points;

    // Start is called before the first frame update
    void Start()
    {
        SetPoints();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetPoints();
        }

        for (int i = 0; i < points.Length - 1; i++)
        {
            Vector2 objectPosition = transform.position;
            Debug.DrawLine(points[i], points[i + 1], Color.red);
        }
    }

    private void SetPoints()
    {
        f = PhysicsUtility.NewtonPolynomial(dataSets.ToArray());
        points = new Vector2[pointCount];

        // 시작점, 끝점 정의
        points[0] = new Vector2(startX, f(startX));
        points[points.Length - 1] = new Vector2(endX, f(endX));

        // 나머지 점들 채워넣음
        float interval_x = (endX - startX) / pointCount;
        float current_x = startX + interval_x;
        for (int i = 1; i < points.Length - 1; i++)
        {
            points[i] = new Vector2(current_x, f(current_x));
            current_x += interval_x;
        }
    }
}
