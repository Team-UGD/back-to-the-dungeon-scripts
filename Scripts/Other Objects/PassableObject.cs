using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassableObject : MonoBehaviour
{
    SpriteRenderer sprite;
    private Color originCol = new Color(1, 1, 1, 1);
    private Color col = new Color(0, 0, 0, 0);
    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            StartCoroutine(Enter());
            ChangeLayer();
        }
    }
    private IEnumerator Enter()
    {
        float cnt = 0;
        while (true)
        {
            cnt += 0.01f;
            sprite.color = Color.Lerp(originCol, col, cnt);
            if (cnt >= 1)
            {
                sprite.color = col;
                break;
            }
            yield return null;
        }
    }
   
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            StartCoroutine(Exit());
            Invoke("ReturnLayer", 1f);
        }
    }
    private IEnumerator Exit()
    {
        float cnt = 0;
        while (true)
        {
            cnt += 0.005f;
            sprite.color = Color.Lerp(col, originCol, cnt);
            if (cnt >= 1)
            {
                sprite.color = originCol;
                break;
            }
            yield return null;
        }
    }
    public void ChangeLayer()
    {
        gameObject.layer = 11;
    }
    private void ReturnLayer()
    {
        gameObject.layer = 6;
    }
}

