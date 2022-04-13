using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MovableGroundTrap : MonoBehaviour
{
    private bool isTouched = false;

    [Range(0, 1)]
    public float weight = 0;
    public float speed = 5f;

    Rigidbody2D rigid;

    [MoveTool]
    public List<Vector2> dataSets = new List<Vector2>();
    private void Start()
    {
        dataSets[0] = this.transform.position;
    }
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isTouched)
        {
            isTouched = true;
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
            StartCoroutine(Moveback());
        }
    }
    private IEnumerator Moveback()
    {
        while (weight < 1)
        {
            float sp = speed * Time.fixedDeltaTime;
            rigid.position = PhysicsUtility.BezierCurve(dataSets, weight);
            weight = Mathf.Clamp01(weight + sp);
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForSeconds(1f);
        while (weight > 0)
        {
            float sp = speed * Time.fixedDeltaTime;
            weight = Mathf.Clamp01(weight - sp);
            rigid.position = PhysicsUtility.BezierCurve(dataSets, weight);
            yield return new WaitForFixedUpdate();
        }
        isTouched = false;
        rigid.constraints = RigidbodyConstraints2D.FreezeAll;
    }

}

#if UNITY_EDITOR
[CustomEditor(typeof(MovableGroundTrap)), CanEditMultipleObjects]
public class AnotherEditor : Editor
{
    private MoveToolEditor moveToolEditor;

    private void OnSceneGUI()
    {
        if (moveToolEditor == null)
            moveToolEditor = Editor.CreateEditor(target, typeof(MoveToolEditor)) as MoveToolEditor;

        moveToolEditor.OnEnable();
        moveToolEditor.OnSceneGUI(); // It's also okay to call moveToolEditor.SetMoveTool() instead of it.
    }

    private void OnDisable()
    {
        DestroyImmediate(moveToolEditor);
    }
}
#endif