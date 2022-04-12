using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class EnemyDetection : MonoBehaviour
{
    [SerializeField]
    LayerMask PlayerLayer;

    [SerializeField] private float agroRange = 10.0f;             //도발 범위
    [SerializeField] private float permittedRange = 5.0f;         //허용 범위

    [SerializeField]
    LayerMask GroundLayer;


    [SerializeField] private float CoroutineTime = 0.5f;
    [SerializeField] private float waitTime = 1f;
    [SerializeField] private float sceneWaitTime = 5f;
    [SerializeField] private bool isSceneLoad = false;

    //[System.NonSerialized]
    private Collider2D Collider2D;
    public Transform castPoint;                // 광선의 시작 위치
    private float startTime;
    private float curTime;
    private float sceneStartTime;
    private float sceneCurTime;

    private void Start()
    {
        GameManager.Instance.OnSceneLoaded += Instance_OnSceneLoaded;
    }

    private void Instance_OnSceneLoaded(UnityEngine.SceneManagement.Scene? previous, UnityEngine.SceneManagement.Scene? loaded, SceneLoadingTiming when)
    {
        switch (when)
        {
            case SceneLoadingTiming.AfterFadeIn:
                isSceneLoad = true;
                break;
        }
        //throw new System.NotImplementedException();
    }
    
    private void OnEnable()
    {
        StartCoroutine(Detection());
    }

    private void Awake()
    {
        Collider2D = GetComponent<Collider2D>();
    }
    /// <summary>
    /// 벽의 감지 유무를 판단 후 Target의 정보를 반환한다.
    /// </summary>
    public GameObject Target { get; private set; }
    private IEnumerator Detection()
    {
        while (true)
        {
            //Debug.Log("isSceneLoad = " + isSceneLoad);
            sceneStartTime = Time.time;
            if (isSceneLoad)
            {
                while (true)
                {
                    Target = null;
                    //Debug.Log("sceneCurTime - sceneStartTime = " + (sceneCurTime - sceneStartTime));
                    sceneCurTime = Time.time;
                    if (sceneCurTime - sceneStartTime >= sceneWaitTime)
                    {
                        //Debug.Log("기다림 끝");
                        isSceneLoad = false;
                        break;
                    }
                    yield return null;
                }
            }
            else
            {
                Bounds bounds = Collider2D.bounds;
                Vector2 pos = new Vector2(bounds.center.x, bounds.center.y);
                Collider2D Playercollider2Ds = Physics2D.OverlapCircle(pos, agroRange, PlayerLayer);

                if (Playercollider2Ds != null)              //플레이어가 적의 레이더에 인식이 된 경우
                {
                    Vector2 endPos = Playercollider2Ds.transform.position + new Vector3(0f, 0.75f); //광선의 끝 위치
                    RaycastHit2D hit = Physics2D.Linecast(castPoint.position, endPos, GroundLayer);
                    Debug.DrawLine(castPoint.position, endPos, Color.cyan);

                    float checkPermit = Mathf.Sqrt(Mathf.Pow((Playercollider2Ds.transform.position.x - this.transform.position.x), 2) + Mathf.Pow((Playercollider2Ds.transform.position.y - this.transform.position.y), 2));

                    if (checkPermit > permittedRange)       //만약 플레이어가 허용 범위를 벗어났다면,
                    {
                        if (hit.collider != null)           //만약 플레이어와 Enemy 사이에 벽이 있다면,
                        {
                            //Debug.Log($"인식X");
                            startTime = Time.time;
                            while (true)
                            {
                                //Debug.Log("curTime - startTime = " + (curTime - startTime));
                                curTime = Time.time;
                                if (curTime - startTime >= waitTime)
                                {
                                    break;
                                }
                                yield return null;
                            }
                            Target = null;
                        }
                        else                               //만약 플레이어와 Enemy 사이에 벽이 없다면,
                        {
                            Target = Playercollider2Ds?.gameObject;
                            //Debug.Log($"인식O");
                        }
                    }
                    else                                   //만약 플레이어가 허용범위 안에 있다면,
                    {
                        if (hit.collider != null)           //만약 플레이어와 Enemy 사이에 벽이 있다면,
                        {
                            //Debug.Log($"인식X");
                            startTime = Time.time;
                            while (true)
                            {
                                //Debug.Log("curTime - startTime = " + (curTime - startTime));
                                curTime = Time.time;
                                if (curTime - startTime >= waitTime)
                                {
                                    break;
                                }
                                yield return null;
                            }
                            Target = null;
                        }
                        else                               //만약 플레이어와 Enemy 사이에 벽이 없다면,
                        {
                            Target = Playercollider2Ds?.gameObject;
                            //Debug.Log($"인식O");
                        }
                    }
                }
                else                                      //플레이어가 적의 레이더에 인식이 안됐을 경우
                {
                    Target = null;
                }
                yield return new WaitForSeconds(CoroutineTime);
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.magenta;
        Handles.DrawWireDisc(this.transform.position, Vector3.forward, agroRange);
    }
#endif

    private void OnDestroy()
    {
        try
        {
            GameManager.Instance.OnSceneLoaded -= Instance_OnSceneLoaded;
        }
        catch { }
    }
}
