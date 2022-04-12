using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class RecordBoard : MonoBehaviour, IUiActiveCheck
{
    [SerializeField] Text UIrecordTimeText;
    [SerializeField] Text UIrecordLastTimeText;
    [SerializeField] Text UIrecordClearDeathText;
    private bool isPlayerOutInBoard = true;
    private bool isBoardActive;

    // Player Components
    PlayerInput playerInput;
    private PlayerShooter playerShooter;
    private Rigidbody2D playerRigid;
    private PlayerMovement playerMovement;
    public bool IsUIActive { get => this.isBoardActive; }
    private void Start()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }
    void Update()
    {
        if (isBoardActive && playerInput.Cancel)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            this.PlayerControl(true);
            isBoardActive = false;

            UIManager.Instance.IsOtherUIActive = false;
        }
        else if (!isBoardActive)
        {
            if (!isPlayerOutInBoard)
            {
                if (playerInput.Interact)
                {
                    transform.GetChild(0).gameObject.SetActive(true);
                    this.PlayerControl(false);
                    isBoardActive = true;
                    RecordBoardAction();

                    UIManager.Instance.IsOtherUIActive = true;
                }
            }
            else
            {
                transform.GetChild(0).gameObject.SetActive(false);
                playerInput = null;
                playerShooter = null;
                playerRigid = null;
                isBoardActive = false;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && playerInput == null)
        {
            Debug.Log("플레이어 인식");
            playerInput = collision.GetComponent<PlayerInput>();
            playerShooter = collision.GetComponent<PlayerShooter>();
            playerRigid = collision.GetComponent<Rigidbody2D>();
            playerMovement = collision.GetComponent<PlayerMovement>();
            isPlayerOutInBoard = false;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerOutInBoard = true;
        }
    }
    private void PlayerControl(bool enabled)
    {
        //playerRigid.velocity = enabled ? playerRigid.velocity : Vector2.zero;
        playerRigid.constraints = enabled ? RigidbodyConstraints2D.FreezeRotation : RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        playerShooter.CanAim = enabled;
        playerShooter.CanFire = enabled;
        playerShooter.CanSwap = enabled;
        playerMovement.JumpControl = enabled;
        playerMovement.MoveControl = enabled;
    }
    /// <summary>
    /// RecordList UI를 설정해주는 메서드
    /// </summary>
    public void RecordBoardAction()
    {
        if (!UIrecordTimeText || !UIrecordLastTimeText || !UIrecordClearDeathText)
            return;

        UIrecordTimeText.text = "";
        UIrecordLastTimeText.text = "";
        UIrecordClearDeathText.text = "";
        int i = 0;
        foreach (var record in GameManager.Instance.ClearRecords)
        {
            UIrecordTimeText.text += $"[Top{i + 1}]\n";
            UIrecordTimeText.text += $"Date : {record.Date.Year} years {record.Date.Month} months {record.Date.Day} days\n";
            UIrecordTimeText.text += $"         {record.Date.Hour} hours {record.Date.Minute} minutes {record.Date.Second} seconds\n";
            var time = TimeSpan.FromSeconds(record.ClearTime);
            UIrecordTimeText.text += $"Clear Time : {(time.Hours == 0 ? string.Empty : $"{time.Hours} hours ")}{(time.Minutes == 0 ? string.Empty : $"{time.Minutes} minutes ")}{$"{time.Seconds} seconds "}\n";
            UIrecordTimeText.text += "\n";
            i++;
        }
        if (i != 0)
        {
            UIrecordLastTimeText.text += $"[Last Clear Time] ";
            UIrecordLastTimeText.text += $" Date : { GameManager.Instance.LastClearRecord.Date.Year} years { GameManager.Instance.LastClearRecord.Date.Month} months { GameManager.Instance.LastClearRecord.Date.Day} days\n";
            UIrecordLastTimeText.text += $" {GameManager.Instance.LastClearRecord.Date.Hour} hours {GameManager.Instance.LastClearRecord.Date.Minute} minutes {GameManager.Instance.LastClearRecord.Date.Second} seconds\n";
            var time1 = TimeSpan.FromSeconds(GameManager.Instance.LastClearRecord.ClearTime);
            UIrecordLastTimeText.text += $"Clear Time : {(time1.Hours == 0 ? string.Empty : $"{time1.Hours} hours ")}{(time1.Minutes == 0 ? string.Empty : $"{time1.Minutes} minutes ")}{$"{time1.Seconds} seconds "}\n";
        }
        UIrecordClearDeathText.text += $"Death Count = {GameManager.Instance.DeathCount}\tClear Count = {GameManager.Instance.ClearCount}";
    }
}
