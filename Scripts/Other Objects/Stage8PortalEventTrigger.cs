using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage8PortalEventTrigger : MonoBehaviour
{
    [Header("Flame Thrower")]
    [SerializeField] private ParticleSystem flameThrowerPrefab;
    [SerializeField, MoveTool(LocalMode = true)] private Vector2 flameThrowerStart;
    [SerializeField, MoveTool(LocalMode = true)] private Vector2 flameThrowerEnd;
    [SerializeField] private float flameThrowerDistanceInterval;
    [SerializeField] private float flameThrowerTimeInterval;

    [Header("Explosion")]
    [SerializeField] private ParticleSystem bigExplosion;
    [SerializeField, MoveTool] private Vector2 explosionCreation;
    //[SerializeField] private ParticleSystem firePrefab;

    [Header("Drop Grounds")]
    [SerializeField] private List<Rigidbody2D> grounds = new List<Rigidbody2D>();
    [SerializeField] private float fallingTimeInterval;

    [Header("Portal)")]
    [SerializeField] private Portal portal;
    [SerializeField] private float portalDisabledTime;

    // Start is called before the first frame update
    private float previousSize;

    private bool isRun;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isRun && collision.CompareTag("Player"))
        {
            this.isRun = true;
            StartCoroutine(Run(collision));
        }
    }

    private IEnumerator Run(Collider2D player)
    {
        previousSize = Camera.main.orthographicSize;
        Camera.main.orthographicSize = 12;
        GameManager.Instance.OnSceneLoaded += OnSceneLoaded;

        StartCoroutine(CreateFlameThrower());
        StartCoroutine(DropGrounds());
        StartCoroutine(DisablePortal());
        Instantiate(bigExplosion, explosionCreation, Quaternion.identity);    
        //Vector2 teleport = (Vector2)transform.position + this.flameThrowerStart;
        //teleport.y += 5f;
        //player.transform.position = teleport;
        player.transform.position = new Vector3(transform.position.x, player.transform.position.y);
        player.attachedRigidbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        yield return new WaitForSeconds(1f);
        player.attachedRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene? previous, UnityEngine.SceneManagement.Scene? loaded, SceneLoadingTiming when)
    {
        if (when == SceneLoadingTiming.BeforeLoading)
        {
            Camera.main.orthographicSize = this.previousSize;
        }
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnSceneLoaded -= OnSceneLoaded;
    }

    private IEnumerator DisablePortal()
    {
        portal.enabled = false;
        yield return new WaitForSeconds(this.portalDisabledTime);
        portal.enabled = true;
    }

    private IEnumerator CreateFlameThrower()
    {
        if (flameThrowerPrefab == null)
            yield break;

        Vector2 s = this.flameThrowerEnd - this.flameThrowerStart;
        Vector2 direction = s.normalized;
        Vector2 current = this.flameThrowerStart;

        float l = s.magnitude;
        while (Vector2.Distance(this.flameThrowerStart, current) <= l)
        {
            Instantiate(this.flameThrowerPrefab, current + (Vector2)transform.position, this.flameThrowerPrefab.transform.rotation);
            current += this.flameThrowerDistanceInterval * direction;
            yield return new WaitForSeconds(this.flameThrowerTimeInterval);
        }
    }

    private IEnumerator DropGrounds()
    {
        for (int i = 0; i < grounds.Count; i++)
        {
            grounds[i].constraints = RigidbodyConstraints2D.FreezeRotation;
            grounds[i].AddForce(new Vector2(0f, 0.1f), ForceMode2D.Impulse);
            Destroy(grounds[i], 5f);
            yield return new WaitForSeconds(this.fallingTimeInterval);
        }
    }
}
