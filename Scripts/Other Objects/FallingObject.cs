using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingObject : MonoBehaviour
{
    public float destroyTime = 5.0f;
    private Rigidbody2D rigid;
    public Collider2D collider2d;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Player"))
        {
            rigid.constraints = RigidbodyConstraints2D.None;
            //rigid.gravityScale = 1;
            collider2d.isTrigger = true;
            Destroy(gameObject, destroyTime);
        }
    }
}
