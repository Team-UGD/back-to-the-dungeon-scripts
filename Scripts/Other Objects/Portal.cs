using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 다음 Scene으로 전환시키기 위한 매개체.
/// </summary>
public class Portal : MonoBehaviour
{
    #region [Fields for you to input value in inspector]

    [SerializeField, ScenePopup] private int nextSceneIndex = 0;
    [SerializeField] private bool enemyCountUiEnabled = true;

    #endregion

    #region [Private Fields]

    private List<Entity> enemies = new List<Entity>();
    private Action onAllEnemiesDeath;
    private SpriteRenderer spriteRenderer;
    private Collider2D portalCollider;
    private bool isLoading = false;
    private float startTime;
    private float curTime;

    #endregion

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        portalCollider = GetComponent<Collider2D>();

        Enemy[] enemies = FindObjectsOfType<Enemy>();

        for (int i = 0; i < enemies.Length; i++)
        {
            Enemy enemy = enemies[i];
            this.enemies.Add(enemy);
            this.enemies[i].OnDeath += () => this.enemies.Remove(enemy);
        }
        //enemies = monsters.Select(m => m.GetComponent<Entity>()).Where(e => e != null).Select(e => { e.OnDeath += () => enemies.Remove(e); return e; }).ToList();

        GameManager.Instance.OnSceneLoaded += EnemyCountUIControl;
    }

    private void EnemyCountUIControl(UnityEngine.SceneManagement.Scene? previous, UnityEngine.SceneManagement.Scene? loaded, SceneLoadingTiming when)
    {
        switch (when)
        {
            case SceneLoadingTiming.BeforeLoading:
                if (previous.Value == gameObject.scene)
                    UIManager.Instance.EnabledRemainEnemyUI(true);
                break;
            case SceneLoadingTiming.AfterLoading:
                if (loaded.Value == gameObject.scene)
                    UIManager.Instance.EnabledRemainEnemyUI(this.enemyCountUiEnabled);
                break;
            default:
                break;
        }
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnSceneLoaded -= EnemyCountUIControl;
    }

    private void Start()
    {
        portalCollider.enabled = false;
        spriteRenderer.enabled = false;

        onAllEnemiesDeath += () => portalCollider.enabled = true;
        onAllEnemiesDeath += () => spriteRenderer.enabled = true;
    }

    private void Update()
    {
        UIManager.Instance.SetRemainEnemyUI(enemies.Count);

        if (enemies.Count <= 0 && onAllEnemiesDeath != null)
        {
            onAllEnemiesDeath();
            onAllEnemiesDeath = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TryLoadScene(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        TryLoadScene(collision);
    }

    private void TryLoadScene(Collider2D collision)
    {
        if (isLoading)
            return;

        if (collision.CompareTag("Player"))
        {
            PlayerInput playerInput = collision.GetComponent<PlayerInput>();

            //playerinput.Fire ->상호작용키로 변경
            if (playerInput != null && playerInput.Interact)
            {
                isLoading = true;
                GameManager.Instance.LoadScene(nextSceneIndex, collision.gameObject);
            }
        }
    }
}
