using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Enemy Skills/EnemyBicBallSkill")]
public class BigBallSkill : EnemySkill
{
    [SerializeField]
    private float waitTime = 1.0f;

    [NonSerialized]
    public float baseDamage = 15f;
    public Ball ball;
    public AudioClip creationSound;
    public AudioClip BallSound;

    public override void TriggerSkill(Attacker user, GameObject target)
    {
        Quaternion rotation = Quaternion.Euler(0f, 0f, 0f);
        Vector3 pos = new Vector3(user.transform.position.x, user.transform.position.y + 2.0f);
        Ball bulletBall = Instantiate(ball, pos, rotation);
        float damage = GetSTR(user);
        bulletBall.StartCoroutine(Attack(user, damage, target, bulletBall, pos));      //GameManager에서 코루틴 호출 금지 
    }
    private IEnumerator Attack(Attacker user, float damage, GameObject target, Ball BulletBall, Vector3 position)
    {
        BulletBall.GetComponent<Rigidbody2D>().gravityScale = 0;
        
        AudioSource userAudioPlayer = user.AudioSourceComponent;
        Rigidbody2D userRb = user.gameObject.GetComponent<Rigidbody2D>();
        userRb.constraints = RigidbodyConstraints2D.FreezeAll;

        
        if (BulletBall != null)
        {
            float count = 0;
            Vector2 curPos = BulletBall.transform.position;
            Vector2 toPos = new Vector2(user.transform.position.x + 2.0f * (target.transform.position - user.transform.position).normalized.x, user.transform.position.y + 3.0f);

            if (userAudioPlayer)
                userAudioPlayer.PlayOneShot(creationSound);

            while (true)
            {

                count += 0.01f;
                if (BulletBall == null)
                {
                    yield break;
                }
                BulletBall.transform.position = Vector2.Lerp(curPos, toPos, count);
                if (count >= 1)
                {
                    
                    BulletBall.transform.position = toPos;
                    break;
                }
                yield return null;
            }
        }
        yield return new WaitForSeconds(waitTime);
        if (BulletBall != null)
        {
            float moveDirection = (target.transform.position - position).normalized.x;
            Vector2 shootPos = new Vector2(3.5f * moveDirection, 2);
            if (userAudioPlayer)
                userAudioPlayer.PlayOneShot(BallSound);

            BulletBall.GetComponent<Rigidbody2D>().gravityScale = 1;
            BulletBall.SetBullet(shootPos, position);
        }
        if (user != null)
        {
            userRb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }
    private float GetSTR(GameObject user)
    {
        float temp = user?.GetComponent<IStrikingPower>().STR ?? 0f;
        float str = temp < 0f ? 0f : temp;

        return str;
    }
}