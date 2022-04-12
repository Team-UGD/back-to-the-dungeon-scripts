using UnityEngine;

public class ExplosionBullet : Bullet
{
    [SerializeField] GameObject explosion;

    //폭발 데미지는 Explosion프리펩에서 따로 설정
    
    protected override void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Ground"))
        {
            Instantiate(explosion, transform.position, Quaternion.identity);
            DestroyBullet();
            return;
        }

        switch (target)
        {
            case Target.Player:
                if (collision.CompareTag("Player"))
                {
                    Instantiate(explosion, transform.position, Quaternion.identity);
                    Attack(collision);
                }
                break;
            case Target.Enemy:
                if (collision.CompareTag("Enemy"))
                {
                    Instantiate(explosion, transform.position, Quaternion.identity);
                    Attack(collision);
                }
                break;
            case Target.Both:
                Attack(collision); 
                Instantiate(explosion, transform.position, Quaternion.identity);
                break;
        }
    }

}