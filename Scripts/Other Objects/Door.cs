using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Door : MonoBehaviour
{

    [SerializeField] private GameObject[] doors;
    [SerializeField] private float openTime;
    [SerializeField] private float moveDistance;
    private AudioSource audio;

    public float OpenTime { get => this.openTime; set => this.openTime = value; }

    private void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    public void Open()
    {
        for (int i = 0; i < doors.Length; i++)
        {
            Vector3 current = doors[i].transform.position;
            Vector3 goal;
            if (doors[i].transform.localScale.y > 0)
                goal = current + new Vector3(0, -moveDistance, 0);
            else
                goal = current + new Vector3(0, moveDistance, 0);

            StartCoroutine(MoveDoor(i,current,goal));
        }
    }

    IEnumerator MoveDoor(int idx,Vector3 current, Vector3 goal)
    {
        if (audio.clip != null)
            audio.PlayOneShot(audio.clip);

        float time = 0;

        while (true)
        {
            if (time > openTime)
                break;

            doors[idx].transform.position = Vector2.Lerp(current, goal, time / openTime);

            time += 0.02f;
            yield return new WaitForFixedUpdate();
        }
    }
}
