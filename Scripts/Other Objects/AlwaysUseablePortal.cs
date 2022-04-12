using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 다음 Scene으로 전환시키기 위한 매개체.
/// </summary>
public class AlwaysUseablePortal : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Collider2D portalCollider;

    [ScenePopup] public int nextSceneIndex = 0;
    private bool isLoading = false;
    private bool isKillMiniboss = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        portalCollider = GetComponent<Collider2D>();

        GameObject bod = GameObject.Find("Bringer Of Death");

        if (bod)
            bod.GetComponent<Enemy>().OnDeath += () => isKillMiniboss = true;
    }

    private void Start()
    {
        GameManager.Instance.OnSceneLoaded += SceneLoaded;
    }

    private void SceneLoaded(UnityEngine.SceneManagement.Scene? previous, UnityEngine.SceneManagement.Scene? loaded, SceneLoadingTiming when)
    {
        switch (when)
        {
            case SceneLoadingTiming.BeforeLoading:
                if (!isKillMiniboss)
                {
                    GameObject.FindWithTag("Respawn").transform.position = new Vector2(46, 56);
                }
                break;
            case SceneLoadingTiming.AfterLoading:
                if (!isKillMiniboss)
                {
                    UIManager.Instance.EnabledRemainEnemyUI(false);
                    //UIManager.Instance.SetStageText("Mini Boss");
                }
                else
                {
                    UIManager.Instance.EnabledRemainEnemyUI(true);
                    // UIManager.Instance.SetStageText("Stage3");

                    Destroy(GameObject.Find("AlwaysUseablePortal"), 1);
                }
                break;

        }
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnSceneLoaded -= SceneLoaded;
    }

    private void Update()
    {
        if (isKillMiniboss)
        {
            spriteRenderer.enabled = true;
            portalCollider.enabled = true;
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
        if (collision.CompareTag("Player"))
        {
            PlayerInput playerInput = collision.GetComponent<PlayerInput>();

            //playerinput.Fire ->상호작용키로 변경
            if (playerInput != null && playerInput.Interact && !isLoading)
            {
                if (!isKillMiniboss)
                    GameManager.Instance.LoadScene(nextSceneIndex, collision.gameObject, LoadSceneMode.Additive);        //Mini Boss stage 로드
                else if (isKillMiniboss)
                    GameManager.Instance.SwitchScene(3, collision.gameObject, true);                                     //Mini Boss stage에서 Stage3로 이동

                isLoading = true;
                Debug.Log("Loading 시도");
            }
        }
    }
}
