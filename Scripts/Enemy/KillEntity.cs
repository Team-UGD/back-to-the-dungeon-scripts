using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KillEntity : MonoBehaviour
{
    [SerializeField] private Entity entityToKill;
    [SerializeField] private float time;
    public UnityEvent OnKillEntity;

    private float currentTime;

    private void Update()
    {
        if (currentTime >= time)
        {
            OnKillEntity?.Invoke();
            entityToKill.TakeDamage(entityToKill.MaxHealth, entityToKill.transform.position, Vector2.zero);
        }

        currentTime += Time.deltaTime;
    }
}
