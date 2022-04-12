using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextMeshController : MonoBehaviour
{
    [SerializeField] string text;

    TextMesh textMesh;

    void Start()
    {
        textMesh = GetComponent<TextMesh>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            textMesh.text = text;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            textMesh.text = "";
        }
    }

}
