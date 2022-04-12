using UnityEngine;

public class MainVillagePortal : MonoBehaviour
{
    private bool isLoading;

    private void Start()
    {
        UIManager.Instance.EnabledRemainEnemyUI(false);
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

            if (playerInput != null && playerInput.Interact)
            {
                isLoading = true;
                GameManager.Instance.LoadScene(GameManager.Instance.StartStageBuildIndex, collision.gameObject);
            }
        }
    }
}
