using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class BossEventTrigger : MonoBehaviour
{
    [Header("Boss")]
    [MoveTool] public Vector3 startPosition;
    [MoveTool] public Vector3 endPosition;
    [SerializeField] private float appearTime;
    [SerializeField] private GameObject bossSprite;

    [Header("Other Object")]
    [SerializeField] private GameObject tilemapObject;
    [SerializeField] private Boss boss;
    [SerializeField] private GameObject portal;

    [Header("Events")]
    [SerializeField] private UnityEvent onBossAppeared;

    private PlayerMovement playerMovement;
    private PlayerShooter playerShooter;
    private float cameraSize;
    private Vector3 cameraPosition;

    public event System.Action OnBossAppearFinished;

    private bool triggered;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggered)
            return;

        if (!collision.CompareTag("Player"))
            return;

        triggered = true;

        playerMovement = collision.GetComponent<PlayerMovement>();
        playerShooter = collision.GetComponent<PlayerShooter>();

        playerShooter.enabled = false;
        playerMovement.SetConstraints(3);

        tilemapObject.SetActive(true);
        StartCoroutine(BossAppear(playerMovement));
    }

    //private void OnDestroy()
    //{
    //    boss.SetActive(true);
    //    portal.SetActive(true);
    //}

    System.Action cameraRestored;

    IEnumerator BossAppear(PlayerMovement playerMovement)
    {
        Camera cam = Camera.main;
        float time = 0;
        cameraSize = cam.orthographicSize;
        cameraPosition = cam.transform.position;

        Tilemap tilemap = tilemapObject.GetComponent<Tilemap>();
        //cameraMove 1
        //cam.transform.position = endPosition;
        cam.orthographicSize = 13f;

        while (true)
        {
            if (time > appearTime)
                break;

            time += 0.02f;
            bossSprite.transform.position = Vector2.Lerp(startPosition, endPosition, time / appearTime);
            tilemap.color = new Color(1, 1, 1, time / appearTime);

            //cameraMove 1
            cam.transform.position = Vector3.Lerp(cameraPosition, endPosition, time);

            yield return new WaitForFixedUpdate();
        }


        yield return new WaitForSeconds(0.2f);
        cam.transform.position = cameraPosition;
        cam.orthographicSize = cameraSize + 1;

        bool restored = false;
        var playerEntity = playerMovement.GetComponent<Entity>();
        cameraRestored = () =>
        {
            if (restored)
                return;
            Camera.main.orthographicSize -= 1;
            playerEntity.OnDeath -= cameraRestored;
        };
        boss.OnDeath += cameraRestored;
        playerEntity.OnDeath += cameraRestored;

        playerShooter.enabled = true;
        playerMovement.SetConstraints(2);

        boss.gameObject.SetActive(true);
        portal.SetActive(true);

        OnBossAppearFinished?.Invoke();
        onBossAppeared?.Invoke();

        yield return null;

        //Destroy(this.gameObject, 0.1f);
        gameObject.SetActive(false);
    }
}
