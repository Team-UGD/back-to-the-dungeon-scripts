using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Stage8FlameEventTrigger : MonoBehaviour
{   
    [Header("Flame")]
    [SerializeField] private float flameSpreadTime;
    [SerializeField] private BezierMoveTool flameSpread;

    [Header("Fire")]
    [SerializeField] private ParticleSystem firePrefab;
    [SerializeField] private ParticleSystem smallExplosionPrefab;
    //[SerializeField, MoveTool] private List<Vector2> wildFireCreation = new List<Vector2>();
    [SerializeField, MoveTool(LocalMode = true)] private Vector2 origin;
    [SerializeField, MoveTool(LocalMode = true)] private Vector2 end1;
    [SerializeField, MoveTool(LocalMode = true)] private Vector2 end2;
    [SerializeField, Min(0.1f)] private float distanceInterval;
    [SerializeField] private float timeInterval;

    [Header("Explosion")]
    [SerializeField] private ParticleSystem explosionPrefab;
    [SerializeField] private AudioClip explosionSound;
    [SerializeField, MoveTool(LocalMode = true)] private Vector2 explosionCreation;
    [SerializeField, Range(0f, 1f)] private float explosionT;

    [Header("Camera")]
    [SerializeField] private BezierMoveTool cameraMove;
    [SerializeField] private float cameraSize;
    [SerializeField] private Door door;
    [SerializeField] private AudioClip doorOpenSound;
    [SerializeField] private Tilemap groundFadedOut;

    [Header("Other Objects")]
    [SerializeField] private PlayerRestrictionArea restrictionArea;

    private bool isRun;
    private ParticleSystem particle;
    private AudioSource audioSoruce;
    private AudioClip fireSound;

    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();
        audioSoruce = GetComponent<AudioSource>();
    }

    private void OnParticleCollision(GameObject other)
    {
        //Debug.Log("실행");
        if (!isRun && other.CompareTag("Player"))
        {
            isRun = true;
            StartCoroutine(Run(other));
        }
    }

    private IEnumerator Run(GameObject player)
    {
        //playerRigid.constraints = RigidbodyConstraints2D.FreezeAll;
        var playerRigid = player.GetComponent<Rigidbody2D>();
        var shooter = player.GetComponent<PlayerShooter>();

        // 플레이어 조작 불가능 상태로
        shooter.CanAim = false;
        playerRigid.velocity = Vector2.zero;
        playerRigid.constraints = RigidbodyConstraints2D.FreezeAll;

        flameSpread.gameObject.SetActive(true);

        yield return null;
        
        // 주변에 불 생성
        List<ParticleSystem> wildFires = new List<ParticleSystem>();
        var spreadFires = StartCoroutine(SpreadWildFire(wildFires));

        // 불꽃 퍼트림
        flameSpread.StopAllPath();
        flameSpread.MovePathIteratively(0, BezierMoveDirection.Forward);
        flameSpread.MovePathIteratively(0, BezierMoveDirection.Backward);

        yield return new WaitForSeconds(flameSpreadTime);

        flameSpread.gameObject.SetActive(false);

        // 폭발 파티클 생성
        var explosion = Instantiate(explosionPrefab, (Vector2)transform.position + explosionCreation, Quaternion.identity);
        //explosion.Play(true);
        this.particle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        //this.audioSoruce.PlayOneShot(this.explosionSound);

        yield return new WaitForSeconds(6f);

        // 카메라 이동
        yield return MoveCamera();


        yield return new WaitForSeconds(2f);

        // 이벤트 종료(플레이어가 다시 조작 가능한 상태로)
        shooter.CanAim = true;
        playerRigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        //this.restrictionArea.RestrictionOff();
        UIManager.Instance.SetPlayerUI(true);

        // 불필요한 오브젝트 파괴
        Destroy(restrictionArea.gameObject);
        Destroy(explosion.gameObject);

        // 불 꺼지는 기능(나중에 빼거나 변경될 예정)
        //StopCoroutine(spreadFires);
        //for (int i = 0; i < wildFires.Count; i++)
        //{
        //    Destroy(wildFires[i].gameObject);
        //}

        Destroy(this.gameObject);
    }

    private IEnumerator SpreadWildFire(List<ParticleSystem> wildFires)
    {
        if (this.firePrefab == null || this.smallExplosionPrefab == null)
            yield break;

        Vector2 s1 = (end1 - origin).normalized;
        Vector2 s2 = (end2 - origin).normalized;

        Vector2 current1 = this.distanceInterval * s1 + this.origin;
        Vector2 current2 = this.distanceInterval * s2 + this.origin;

        Queue<ParticleSystem> explosions = new Queue<ParticleSystem>();
        wildFires.Add(Instantiate(firePrefab, this.origin + (Vector2)transform.position, Quaternion.identity));

        float d1 = (end1 - origin).magnitude;
        float d2 = (end2 - origin).magnitude;
        while (Vector2.Distance(current1, this.origin) <= d1 || Vector2.Distance(current2, this.origin) <= d2)
        {
            yield return new WaitForSeconds(this.timeInterval);

            if (Vector2.Distance(current1, this.origin) <= d1)
            {
                wildFires.Add(Instantiate(firePrefab, current1 + (Vector2)transform.position, Quaternion.identity));
                explosions.Enqueue(Instantiate(smallExplosionPrefab, current1 + (Vector2)transform.position, Quaternion.Euler(-90f, 0f, 0f)));
                //explosions.Peek().Play(true);
                current1 += this.distanceInterval * s1;
            }

            if (Vector2.Distance(current2, this.origin) <= d2)
            {
                wildFires.Add(Instantiate(firePrefab, current2 + (Vector2)transform.position, Quaternion.identity));
                explosions.Enqueue(Instantiate(smallExplosionPrefab, current2 + (Vector2)transform.position, Quaternion.Euler(-90f, 0f, 0f)));
                //explosions.Peek().Play(true);
                current2 += this.distanceInterval * s2;
            }
        }

        yield return new WaitForSeconds(3f);
        for (int i = 0; explosions.Count > 0; i++)
        {
            Destroy(explosions.Dequeue().gameObject);
        }
    }

    private IEnumerator MoveCamera()
    {
        if (cameraMove.Count < 1)
            yield break;

        var cameraMoveTool = Camera.main.gameObject.AddComponent<BezierMoveTool>();

        yield return null;

        float last = Camera.main.orthographicSize;
        //Camera.main.orthographicSize = this.cameraSize;

        // 목적지까지 이동
        for (int i = 0; i < cameraMove.Count; i++)
        {
            cameraMoveTool.MovePathOnce(cameraMove[i], BezierMoveDirection.Forward);
        }
      
        bool isTerminated = false;
        bool arrivedAtDoor = false;
        bool fadedIn = false;

        cameraMoveTool.OnPathMove += (p, c, t) =>
        {
            // 플레이어 근처에서 줌아웃인
            float zoomPlayer = 0.3f;
            if (p == cameraMove[0] && c.T <= zoomPlayer)
            {
                float zoomT = c.T / zoomPlayer;
                Camera.main.orthographicSize = Mathf.Lerp(last, this.cameraSize, zoomT);
            }

            // 카메라 무브워크 종료
            if (p == cameraMove[0] && c.T <= 0 && t)
                isTerminated = true;

            float groundFade = 0.7f;
            if (!fadedIn)
            {
                if (p == cameraMove[0] && c.T <= groundFade)
                {
                    float fadeT = c.T / groundFade;
                    Color color = this.groundFadedOut.color;
                    float a = Mathf.Lerp(0f, 1f, fadeT);
                    color.a = a;
                    this.groundFadedOut.color = color;
                }
                else
                {
                    Color color = this.groundFadedOut.color;
                    color.a = 1f;
                    this.groundFadedOut.color = color;
                    fadedIn = true;
                }
            }

            // 문 근처에서 줌인아웃
            float zoomDoor = 0.8f;
            if (p == cameraMove[cameraMove.Count - 1] && c.T >= zoomDoor)
            {
                float zoomT = (c.T - zoomDoor) / (1f - zoomDoor);
                Camera.main.orthographicSize = Mathf.Lerp(this.cameraSize, last, zoomT);
            }

            // 문에 도달했을 때 오픈
            if (p == cameraMove[cameraMove.Count - 1] && c.T >= 1f && t)
            {
                arrivedAtDoor = true;
            }            
        };

        this.groundFadedOut.gameObject.SetActive(true);

        while (!arrivedAtDoor)
        {
            yield return null;
        }

        yield return new WaitForSeconds(2f);

        this.door.Open();
        this.door.GetComponent<AudioSource>().PlayOneShot(this.doorOpenSound);

        yield return new WaitForSeconds(this.door.OpenTime + 2f);

        for (int i = cameraMove.Count - 1; i >= 0; i--)
        {
            cameraMoveTool.MovePathOnce(cameraMove[i], BezierMoveDirection.Backward);
        }

        while (!isTerminated)
        {
            yield return null;
        }

        Destroy(cameraMoveTool);
        Camera.main.transform.localPosition = new Vector3(0f, 0f, Camera.main.transform.localPosition.z);
        Camera.main.orthographicSize = last;
    }
}
