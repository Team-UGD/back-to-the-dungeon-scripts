using UnityEngine;

public class AttackTrap : MonoBehaviour, IStrikingPower
{
    [SerializeField]
    private uint str;
    [SerializeField, Min(0f)] private float power = 1;
    public uint STR { get => str; set => str = value; }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            var entity = collision.collider.GetComponent<Entity>();
            var contact = collision.GetContact(0);
            Debug.Log(contact.normal);

            if (entity)                                         //null참조 error방지를 위해 추가한 if문
            {
                Debug.Log(entity);
                entity.TakeDamage(str, contact.point, contact.normal, power: power);
            }
        }
    }
}
