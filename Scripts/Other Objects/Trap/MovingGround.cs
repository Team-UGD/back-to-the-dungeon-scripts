using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MovingGround : MonoBehaviour
{
    [Range(0, 1)]
    public float weight = 0;
    public float speed = 1f;

    [MoveTool]
    public List<Vector2> dataSets = new List<Vector2>();

    private Transform box;

    private void Start()
    {
        dataSets[0] = this.transform.position;
        StartCoroutine(Move());
        //Box Tag를 갖는 오브젝트 찾기
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Box");
        if (gameObjects.Length > 1)
            Debug.LogError("Box 태그를 가진 오브젝트는 두 개 이상 존재할 수 없습니다.");
        box = GameObject.FindGameObjectWithTag("Box").transform;

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Bounds playerBound = collision.collider.bounds;
            Vector2 playerFootPosition = new Vector2(playerBound.center.x, playerBound.min.y);
            if (Vector2.Angle(playerFootPosition, Vector2.up) < 110f)
            {
                //parent가 있을 경우에만 parent 해제
                if (transform.parent != null)
                    transform.SetParent(null);
                //movingGround를 dontdestroy로 설정
                DontDestroyOnLoad(this.gameObject);
                //Player의 부모를 movingGround로 설정
                collision.transform.SetParent(this.transform);
            }
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Bounds playerBound = collision.collider.bounds;
            Vector2 playerFootPosition = new Vector2(playerBound.center.x, playerBound.min.y);
            if (Vector2.Angle(playerFootPosition, Vector2.up) < 110f)
            {
                //parent가 있을 경우에만 parent 해제
                if (transform.parent != null)
                    transform.SetParent(null);
                //movingGround를 dontdestroy로 설정
                DontDestroyOnLoad(this.gameObject);
                //Player의 부모를 movingGround로 설정
                collision.transform.SetParent(this.transform);
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player"))
            return;
        if (collision.transform.parent != null)
        {
            //Player의 parent 해제
            collision.transform.SetParent(null);
            //movingGround의 부모를 box로 설정
            this.transform.SetParent(box);
        }
    }
    private IEnumerator Move()
    {
        while (true)
        {
            while (weight < 1)
            {
                float sp = speed * Time.fixedDeltaTime;
                this.transform.position = PhysicsUtility.BezierCurve(dataSets, weight);
                weight = Mathf.Clamp01(weight + sp);
                yield return new WaitForFixedUpdate();
            }
            while (weight > 0)
            {
                float sp = speed * Time.fixedDeltaTime;
                weight = Mathf.Clamp01(weight - sp);
                this.transform.position = PhysicsUtility.BezierCurve(dataSets, weight);
                yield return new WaitForFixedUpdate();
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MovingGround)), CanEditMultipleObjects]
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

}