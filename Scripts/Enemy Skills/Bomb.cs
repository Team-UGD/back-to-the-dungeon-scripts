using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] float time;
    [SerializeField] GameObject selfExplosion;

    void Start()
    {
        Destroy(this.gameObject, time);
    }

    private void OnDestroy()
    {
        Instantiate(selfExplosion, transform.position, Quaternion.identity);
    }

}
