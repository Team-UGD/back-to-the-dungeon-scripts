using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    private PlayerInput playerInput;

    [SerializeField] bool isDoor;
    [SerializeField] bool isStealthPlatform;
    [SerializeField] GameObject Notice;

    [SerializeField, Tooltip("오브젝트 할당시 해당 오브젝트가 사망 시 비활성화된 스위치가 활성화됩니다." +
        "\nEnemy 컴포넌트를 가지고 있는 게임 오브젝트를 할당해주세요." +
        "\n 없을 시 시작 후 바로 활성화됩니다.")] GameObject enableTrigger;

    private void Start()
    {
        if (enableTrigger != null && GetEnemyComponent(out Enemy enemy))
        {
            this.gameObject.SetActive(false);
            enemy.OnDeath += () =>
            {
                this.gameObject.SetActive(true);
            };
        }

        if (isDoor && isStealthPlatform)
            isStealthPlatform = false;
        else if (!isDoor && !isStealthPlatform)
            isDoor = true;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInput = collision.GetComponent<PlayerInput>();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(playerInput == null)
                playerInput = collision.GetComponent<PlayerInput>();

            if (playerInput.Interact)
            {
                if (isDoor)
                {
                    Door door = transform.parent.gameObject.GetComponent<Door>();
                    transform.parent.gameObject.GetComponent<Door>().Open();
                }
                else
                {
                    transform.parent.gameObject.GetComponent<StealthPlatform>().ReleaseStealth();
                }
                if (Notice != null)
                    Notice.SetActive(true);

                this.GetComponent<BoxCollider2D>().enabled = false;
            }
        }
    }

    private bool GetEnemyComponent(out Enemy enemy)
    {
        enemy = enableTrigger.GetComponent<Enemy>();

        if (enemy == null)
        {
            this.gameObject.SetActive(true);
            return false;
        }
        return true;
    }

}
