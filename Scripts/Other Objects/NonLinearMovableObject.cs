using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class NonLinearMovableObject : MonoBehaviour
{
    /*public float moveSpeed = 5;
    public List<Vector2> dataSets = new List<Vector2>();
    public float startX = -5;
    public float endX = 5;
    public byte pointCount = 20;

    private InterpolatedFunction f;
    private Rigidbody2D rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        StartCoroutine(Move());
    }
    private IEnumerator Move()
    {
        f = PhysicsUtility.NewtonPolynomial(dataSets.ToArray());
        while (true)
        {
            float interval_x = (endX - startX) / (pointCount - 1);
            float current_x = startX + interval_x;
            for (int i = 0; i < pointCount - 1; i++)
            {

                float count = 0;
                Vector2 vector1 = new Vector2(current_x, f(current_x));
                current_x += interval_x;
                Vector2 vector2 = new Vector2(current_x, f(current_x));

                Vector2 spe = vector2 - vector1;
                Vector2 vel = spe.normalized * moveSpeed;
                float time = Vector2.Dot(vel, spe) / Mathf.Pow(moveSpeed, 2);

                while (true)
                {
                    Debug.Log(time);
                    count += Time.fixedDeltaTime / time;
                    if (rigid == null)
                    {
                        yield break;
                    }
                    rigid.position = Vector2.Lerp(vector1, vector2, count);
                    if (count >= 1f)
                    {
                        rigid.position = vector2;
                        break;
                    }
                    yield return new WaitForFixedUpdate();
                }
            }
            for (int i = pointCount - 1; i > 0; i--)
            {
                float count = 0;

                Vector2 vector1 = new Vector2(current_x, f(current_x));
                current_x -= interval_x;
                Vector2 vector2 = new Vector2(current_x, f(current_x));

                Vector2 spe = vector2 - vector1;
                Vector2 vel = spe.normalized * moveSpeed;
                float time = Vector2.Dot(vel, spe) / Mathf.Pow(moveSpeed, 2);
                while (true)
                {
                    count += Time.fixedDeltaTime / time;
                    if (rigid == null)
                    {
                        yield break;
                    }
                    rigid.transform.position = Vector2.Lerp(vector1, vector2, count);
                    if (count >= 1f)
                    {
                        rigid.transform.position = vector2;
                        break;
                    }
                    yield return new WaitForFixedUpdate();
                }
            }
            yield return new WaitForFixedUpdate();
        }
    }*/


    [Range(0, 1)]
    public float weight = 0;
    public float speed = 1f;

    Rigidbody2D rigid;

    [MoveTool]
    public List<Vector2> dataSets = new List<Vector2>();
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        StartCoroutine(Move());
    }
    private IEnumerator Move()
    {
        while (true)
        {
            while (weight < 1)
            {
                float sp = speed * Time.fixedDeltaTime;
                rigid.position = PhysicsUtility.BezierCurve(dataSets, weight);
                weight = Mathf.Clamp01(weight + sp);
                yield return new WaitForFixedUpdate();
            }
            while (weight > 0)
            {
                float sp = speed * Time.fixedDeltaTime;
                weight = Mathf.Clamp01(weight - sp);
                rigid.position = PhysicsUtility.BezierCurve(dataSets, weight);
                yield return new WaitForFixedUpdate();
            }
        }
    }

#if UNITY_EDITOR
    [CanEditMultipleObjects]
    [CustomEditor(typeof(NonLinearMovableObject))]
    public class Movable_Editor : Editor
    {
        private NonLinearMovableObject Generator;
        SerializedProperty dataset;
        private void OnEnable()
        {
            Generator = target as NonLinearMovableObject;
            dataset = serializedObject.FindProperty("dataSets");
        }

        private void OnSceneGUI()
        {
            if(Generator.dataSets.Count <= 1)
            {
                return;
            }

            EditorGUI.BeginChangeCheck();
            Queue<Vector2> buffer = new Queue<Vector2>();
            for(int i = 0; i < Generator.dataSets.Count; i++)
            {
                buffer.Enqueue(Handles.PositionHandle(Generator.dataSets[i], Quaternion.identity));
            }

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(Generator, $"Change {nameof(Generator.dataSets)}");

                for (int i = 0; i < Generator.dataSets.Count; i++)
                {
                    Generator.dataSets[i] = buffer.Dequeue();
                }
                EditorUtility.SetDirty(Generator);
            }

            for (int i = 0; i < Generator.dataSets.Count - 1; i++)
            {
                Handles.DrawLine(Generator.dataSets[i], Generator.dataSets[i + 1]);
            }
            int detail = 50;
            for (float i = 0; i < detail; i++)
            {
                float value_before = i / detail;
                Vector2 before = PhysicsUtility.BezierCurve(Generator.dataSets, value_before);

                float value_after = (i + 1) / detail;
                Vector2 after = PhysicsUtility.BezierCurve(Generator.dataSets, value_after); ;


                Handles.DrawLine(before, after);
            }
        }
    }
#endif
}
