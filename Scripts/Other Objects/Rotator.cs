using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class Rotator : MonoBehaviour
{
    public float rotationPerSec;
    public bool isClockwise;

    [Tooltip("동적인 변경은 지원하지 않습니다.")]
    public RotationCondition rotationCondition;
    public CollisionTarget collisionTarget;

    private bool canRotate;
    private bool isStoppingRotation;

    private float lastStopTime;
    private float timeBetStop = 1.5f;

    public enum RotationCondition
    {
        Always,
        OnCollision,
        OnStepped,
        KeepingAfterOnCollision,
        KeepingAfterOnStepped   
    }

    public enum CollisionTarget
    {
        Player,
        Enemy,
        Both
    }

    private void Start()
    {
        canRotate = rotationCondition == RotationCondition.Always;
    }

    // Update is called once per frame
    void Update()
    {
        if (isStoppingRotation && Time.time >= lastStopTime + timeBetStop)
        {
            canRotate = false;
            lastStopTime = Time.time;
            isStoppingRotation = false;
        }

        if (canRotate)
        {
            transform.Rotate(0f, 0f, (isClockwise ? -1 : 1) * rotationPerSec * Time.deltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsCorrectTarget(collision))
            return;

        if (rotationCondition == RotationCondition.OnCollision || rotationCondition == RotationCondition.KeepingAfterOnCollision)
        {
            canRotate = true;
            isStoppingRotation = false;
            //Debug.Log("충돌 시작");
        }
        else
        {
            if (Vector2.Angle(collision.GetContact(0).normal, Vector2.down) <= 60f)
            {
                canRotate = true;
                isStoppingRotation = false;
            }
        }
    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        if (IsCorrectTarget(collision))
        {
            if (rotationCondition == RotationCondition.OnCollision || rotationCondition == RotationCondition.OnStepped)
            {
                isStoppingRotation = true;
                lastStopTime = Time.time;
                //Debug.Log("충돌 종료");
            }
        }
    }

    private bool IsCorrectTarget(Collision2D collision)
    {
        string tag = collision.collider.tag;
        return (collisionTarget == CollisionTarget.Both) ? (tag == "Player" || tag == "Enemy") : tag == collisionTarget.ToString();
    }
}
