using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceBall : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private float restartTime = 10f;
    [SerializeField] private float damage = 10f;
    public TrailRenderer effect;

    private float startTime;

    private void Start()
    {
        startTime=Time.time;
    }
    private void Update()
    {
        float time = Time.time;
        if (time - startTime > restartTime)
        {
            Destroy(gameObject, 0.5f);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.collider.CompareTag("Player"))
        {
            Attack(collision);
        }
    }
    private void Attack(Collision2D collision)
    {
        Vector2 hitPosition = collision.transform.position;
        Vector2 hitNormal = collision.GetContact(0).normal;
        collision.collider.GetComponent<Entity>()?.TakeDamage(damage, hitPosition, hitNormal, false);
    }
}
