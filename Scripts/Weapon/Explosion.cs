using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField]float explosionDamage;
    [SerializeField] AudioClip fireSound;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Destroy(this.gameObject, 0.6f);
        audioSource.PlayOneShot(fireSound);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != 8)
        {
            Entity entity = collision.GetComponent<Entity>();
            if (entity == null)
                return;

            Vector2 hitPosition = collision.ClosestPoint(transform.position);
            Vector2 hitNormal = new Vector2(hitPosition.x - collision.transform.position.x, collision.transform.position.y).normalized;

            entity.TakeDamage(explosionDamage, hitPosition, hitNormal);
        }
        Debug.Log("Exp");
    }

    public float ExplosionDamage
    {
        get { return explosionDamage; }
        set { explosionDamage = value; }
    }
}
