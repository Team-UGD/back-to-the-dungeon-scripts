using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealthPlatform : MonoBehaviour
{
    [SerializeField] GameObject platform;

    public void ReleaseStealth()
    {
        platform.SetActive(true);
    }
}
