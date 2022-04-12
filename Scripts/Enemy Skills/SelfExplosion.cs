using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfExplosion : MonoBehaviour
{
    [SerializeField] GameObject selfExplosion;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Vector3 pos = transform.position + new Vector3(0,1,0);
            Instantiate(selfExplosion, pos, Quaternion.identity);
            Destroy(transform.parent.gameObject, 0.5f);
        }
    }
}
