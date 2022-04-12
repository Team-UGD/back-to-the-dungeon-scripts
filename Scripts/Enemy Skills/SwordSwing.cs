using UnityEngine;
using System.Collections;


public abstract class SwordSwing : EnemySkill
{
    [Header("Basic Swing Info")]
    [SerializeField] protected float startAngle;
    [SerializeField] protected float endAngle;
    [SerializeField] protected bool isClockWise;
    [SerializeField] protected float swingTime;

    protected IEnumerator Swing(MeleeWeapon sword, Space rotationMode = Space.Self)
    {
        float t = 0f;
        //sword.transform.localRotation = Quaternion.Euler(0f, 0f, startAngle);
        sword.transform.Rotate(0f, 0f, startAngle, rotationMode);

        float theta = (endAngle - startAngle) - (isClockWise ? 360f : 0f);

        while (t <= swingTime)
        {
            yield return null;

            sword.transform.Rotate(0f, 0f, theta / swingTime * Time.deltaTime, rotationMode);
            t += Time.deltaTime;
        }
    }
}