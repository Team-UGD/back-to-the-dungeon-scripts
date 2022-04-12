using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerRestrictionArea : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private bool canMove = true;
    [SerializeField] private bool canRun;
    [SerializeField] private float basicSpeed;
    [SerializeField, Min(0)] private int maxJumpCount;

    [Header("Shooter")]
    [SerializeField] private bool canFire;
    [SerializeField] private bool canAim;
    [SerializeField] private bool canSwap;

    [Header("UI")]
    //[SerializeField] private bool playerUIEnabled;

    private PlayerMovement movement;
    private PlayerShooter shooter;
    //private Hero hero;

    //private float originalSpeed;
    private int differenceJumpCount;
    private float playerSpeedDiffer;
    
    public void RestrictionOn()
    {
        // Movement
        if (movement != null)
        {
            float temp = movement.ChangeBasicSpeed;
            movement.ChangeBasicSpeed = basicSpeed;
            playerSpeedDiffer = Mathf.Round(movement.ChangeBasicSpeed - temp);
            movement.MoveControl = canMove;
            movement.RunControl = canRun;

            differenceJumpCount = maxJumpCount - movement.SettingMaxJumpCount;
            movement.SettingMaxJumpCount += differenceJumpCount;
        }      

        // Shooter
        if (shooter != null)
        {
            shooter.CanFire = this.canFire;
            shooter.CanAim = this.canAim;
            shooter.CanSwap = this.canSwap;
        }

        //UIManager.Instance.SetPlayerUI(playerUIEnabled);
    }

    public void RestrictionOff()
    {
        if (movement != null)
        {
            //ChangeSpeed만 실행할경우 speed값이 계속 감소
            movement.ChangeBasicSpeed -= this.playerSpeedDiffer;
            movement.SettingMaxJumpCount -= differenceJumpCount;
            movement.MoveControl = true;
            movement.RunControl = true;
        }

        if (shooter != null)
        {
            shooter.CanFire = true;
            shooter.CanAim = true;
            shooter.CanSwap = true;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            movement = collision.GetComponent<PlayerMovement>();
            shooter = collision.GetComponent<PlayerShooter>();

            this.RestrictionOn();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            RestrictionOff();
        }
    }
}
