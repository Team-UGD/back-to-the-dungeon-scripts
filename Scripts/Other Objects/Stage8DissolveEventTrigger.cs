using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage8DissolveEventTrigger : MonoBehaviour
{
    [Header("Event Property")]
    [SerializeField] private Stage8FlameEventTrigger trigger;
    [SerializeField] private Enemy enemy;
    [SerializeField] private PlayerRestrictionArea area;

    [Header("Move")]
    [SerializeField] private Transform target;
    [SerializeField, MoveTool] private Vector3 start;
    [SerializeField, MoveTool] private Vector3 goal;
    [SerializeField] private float moveTime;
    [SerializeField] AnimationCurve moveCurve;

    [Header("Effect")]
    [SerializeField] private float effectTime;
    [SerializeField] private float cameraMoveDelay;  
    [SerializeField] private float cameraMoveTime;

    private Rigidbody2D playerRigid;
    private PlayerShooter playerShooter;
    private PlayerMovement playerMovement;

    private SpawnEffect effect;
    private Collider2D m_collider;
    private bool isMove;
    private float time = 0f;
    private bool isTriggered;

    private Vector3 lastCameraPosition;

    private void Awake()
    {
        m_collider = GetComponent<Collider2D>();
        effect = target.GetComponent<SpawnEffect>();
    }

    private void Start()
    {
        enemy.OnDeath += () =>
        {
            playerMovement = FindObjectOfType<PlayerMovement>();
            playerRigid = playerMovement.GetComponent<Rigidbody2D>();
            playerShooter = playerMovement.GetComponent<PlayerShooter>();
        };
        enemy.OnDeath += () => ControlPlayer(true);
        IEnumerator Run()
        {
            var scale = playerMovement.transform.localScale;
            scale.x *= Mathf.Sign(scale.x) * Mathf.Sign(this.goal.x - playerMovement.transform.position.x);
            playerMovement.transform.localScale = scale;
            yield return MoveCamera(this.goal);
            this.isMove = true;
        }
        enemy.OnDeath += () => StartCoroutine(Run());
        enemy.OnDeath += () => UIManager.Instance.SetPlayerUI(false);
    }

    private void Update()
    {
        if (!isMove)
            return;

        if (time > moveTime)
        {
            isMove = false;
            IEnumerator Run()
            {
                yield return MoveCamera(this.playerMovement.transform.position);
                ControlPlayer(false);
                m_collider.enabled = true;
                area.gameObject.SetActive(true);
            }         
            StartCoroutine(Run());
            return;
        }

        time += Time.deltaTime;
        target.position = Vector3.Lerp(start, goal, moveCurve.Evaluate(Mathf.InverseLerp(0f, moveTime, time)));
    }

    private void ControlPlayer(bool isDisable)
    {
        if (isDisable)
            playerRigid.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        else
            playerRigid.constraints = RigidbodyConstraints2D.FreezeRotation;

        playerMovement.JumpControl = !isDisable;
        playerShooter.CanAim = !isDisable;
        playerShooter.CanFire = !isDisable;
        playerShooter.CanSwap = !isDisable;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isTriggered && collision.CompareTag("Player"))
        {
            this.isTriggered = true;
            effect.enabled = true;
            StartCoroutine(TriggerOn());
        }
    }

    private IEnumerator MoveCamera(Vector2 goal)
    {
        yield return new WaitForSeconds(this.cameraMoveDelay);
        Vector2 start = Camera.main.transform.position;
        float time = 0f;
        while (time <= this.cameraMoveTime)
        {
            yield return null;
            time += Time.deltaTime;
            Vector2 current = Vector2.Lerp(start, goal, Mathf.InverseLerp(0f, this.cameraMoveTime, time));
            Camera.main.transform.position = new Vector3(current.x, current.y, Camera.main.transform.position.z);
        }
    }

    private IEnumerator TriggerOn()
    {
        ControlPlayer(true);
        float time = 0f;
        float start_x = playerMovement.transform.position.x;
        while (time <= this.effectTime / 4f)
        {
            yield return null;
            time += Time.deltaTime;
            Vector2 cur = new Vector2(Mathf.Lerp(start_x, this.goal.x, Mathf.InverseLerp(0f, this.effectTime / 4f, time)), playerMovement.transform.position.y);
            playerMovement.transform.position = cur;
        }
        yield return new WaitForSeconds(this.effectTime);
        ControlPlayer(false);
        playerRigid.constraints = RigidbodyConstraints2D.FreezeAll;
        trigger.gameObject.SetActive(true);

        yield return null;
        Destroy(this.gameObject);
    }
}
