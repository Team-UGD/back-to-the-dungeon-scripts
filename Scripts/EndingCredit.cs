using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class EndingCredit : MonoBehaviour
{

    // Player Components
    private GameObject playerGameObject;
    PlayerInput playerInput;
    private PlayerShooter playerShooter;
    private Rigidbody2D playerRigid;
    private PlayerMovement playerMovement;
    private bool start = false;
    private float startTime;
    private float curTime;
    private bool isPlayerOutInBoard = true;
    [SerializeField] Text UIEndingText;
    private void Start()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }
    void Update()
    {
        if (!isPlayerOutInBoard)
        {
            if (start)
            {
                startTime = Time.time;
                start = false;
            }
            curTime = Time.time;
            if (curTime - startTime >= 25f)
            {
                transform.GetChild(0).gameObject.SetActive(false);
                this.PlayerControl(true);
                isPlayerOutInBoard = true;
                GameManager.Instance.LoadScene(1, playerGameObject);
            }
            else
            {
                transform.GetChild(0).gameObject.SetActive(true);
                this.PlayerControl(false);
                EndingCreditAction();
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && playerInput == null)
        {
            playerGameObject = collision.gameObject;
            playerInput = collision.GetComponent<PlayerInput>();
            playerShooter = collision.GetComponent<PlayerShooter>();
            playerRigid = collision.GetComponent<Rigidbody2D>();
            playerMovement = collision.GetComponent<PlayerMovement>();
            isPlayerOutInBoard = false;
            start = true;
        }
    }
    private void PlayerControl(bool enabled)
    {
        playerRigid.constraints = enabled ? RigidbodyConstraints2D.FreezeRotation : RigidbodyConstraints2D.FreezeAll;
        playerShooter.CanAim = enabled;
        playerShooter.CanFire = enabled;
        playerShooter.CanSwap = enabled;
        playerMovement.JumpControl = enabled;
        playerMovement.MoveControl = enabled;
    }
    /// <summary>
    /// Ending Credit UI를 설정해주는 메서드
    /// </summary>
    public void EndingCreditAction()
    {
        if (!UIEndingText)
            return;
        UIEndingText.text = "";
        UIEndingText.text += $"THE END \n \n";
        UIEndingText.text += $"\n \n \nThank you for playing!\n \n \n";
        UIEndingText.text += $"\n[Your Clear Date]\n \n";
        UIEndingText.text += $"{GameManager.Instance.LastClearRecord.Date.Year} years { GameManager.Instance.LastClearRecord.Date.Month} months { GameManager.Instance.LastClearRecord.Date.Day} days\n";
        UIEndingText.text += $"{GameManager.Instance.LastClearRecord.Date.Hour} hours {GameManager.Instance.LastClearRecord.Date.Minute} minutes {GameManager.Instance.LastClearRecord.Date.Second} seconds\n";
        UIEndingText.text += $"\n[Your Clear Time]\n \n";
        var time3 = TimeSpan.FromSeconds(GameManager.Instance.LastClearRecord.ClearTime);
        UIEndingText.text += $"{(time3.Hours == 0 ? string.Empty : $"{time3.Hours} hours ")}{(time3.Minutes == 0 ? string.Empty : $"{time3.Minutes} minutes ")}{$"{time3.Seconds} seconds "}\n";
        UIEndingText.text += "\n \n \n \n";
        UIEndingText.text += "\n[Creative Director]\n \n";
        UIEndingText.text += "Kim Kyupyo\n \n";
        UIEndingText.text += "Park Jinyeong\n \n";
        UIEndingText.text += "Ko Hyeonseo\n \n";
        UIEndingText.text += "You Wonsock\n \n";
        UIEndingText.text += "Yunwoo\n \n";
        UIEndingText.text += "\n \n";
        UIEndingText.text += "\n[Programmers]\n \n";
        UIEndingText.text += "Park Jinyeong\n \n";
        UIEndingText.text += "Ko Hyeonseo\n \n";
        UIEndingText.text += "You Wonsock\n \n";
        UIEndingText.text += "\n \n";
        UIEndingText.text += "\n[Designer]\n \n";
        UIEndingText.text += "Yunwoo\n";

    }
}
