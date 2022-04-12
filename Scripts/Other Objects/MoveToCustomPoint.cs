using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToCustomPoint : MonoBehaviour
{
    [MoveTool, SerializeField]
    private Vector2 startPoint;

    private void Awake()
    {
        if (GameManager.Instance.DeathCount > 0)
        {
            startPoint.x = -15;
            GameObject.FindGameObjectWithTag("Respawn").transform.position = startPoint;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.gameObject.transform.position = startPoint;
    }

}
