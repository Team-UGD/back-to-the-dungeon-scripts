using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbingSickle : MonoBehaviour
{
    public TrailRenderer effect;
    public bool isTouch = false;
    public Vector3 playerPosInHook;

    [SerializeField] private Transform bladePostion;

    public Vector2 BladePosition { get => bladePostion.position; }

    private void Start()
    {
        isTouch = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isTouch = true;
            playerPosInHook = collision.transform.position;
            Debug.Log("플레이어 닿음");
        }
        else
        {
            isTouch = false;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isTouch = true;
            playerPosInHook = collision.transform.position;
            Debug.Log("플레이어 닿음");
        }
        else
        {
            isTouch = false;
        }
    }
}
