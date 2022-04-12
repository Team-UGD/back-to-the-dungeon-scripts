using UnityEngine;

public class EnemyStealthZone : MonoBehaviour
{
    [SerializeField] private Enemy[] enemies;

    private Rigidbody2D[] rigids;

    private void Awake()
    {
        rigids = new Rigidbody2D[enemies.Length];

        for (int i = 0; i < enemies.Length; i++)
        {
            rigids[i] = enemies[i].GetComponent<Rigidbody2D>();
            rigids[i].constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    private void Start()
    {
        StealthOn();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StealthOff();

            for (int i = 0; i < rigids.Length; i++)
            {
                rigids[i].constraints = RigidbodyConstraints2D.FreezeRotation;
            }

            Destroy(gameObject);
        }
    }

    private void StealthOn()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].gameObject.SetActive(false);
        }
    }

    private void StealthOff()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].gameObject.SetActive(true);
        }
    }
}
