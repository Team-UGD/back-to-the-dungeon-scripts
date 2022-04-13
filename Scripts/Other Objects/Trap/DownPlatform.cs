using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DownPlatform : MonoBehaviour
{
    private TilemapCollider2D thinGround;

    //기존 코드
    private void OnCollisionExit2D(Collision2D collision)
    {
        //Invoke("ReturnLayer",0.5f);
    }

    public void ChangeLayer()
    {
        gameObject.layer = 11;
    }
    private void ReturnLayer()
    {
        gameObject.layer = 14;
    }

    // 1/13일 변경된 코드들 
    private void Awake()
    {
        thinGround = GetComponent<TilemapCollider2D>();
        Debug.Log(thinGround);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            EnableCollider();
    }

    public void DiableCollider()
    {
        thinGround.isTrigger = true;
    }

    public void EnableCollider()
    {
        thinGround.isTrigger = false;
    }

}
