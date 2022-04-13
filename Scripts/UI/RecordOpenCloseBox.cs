using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecordOpenCloseBox : MonoBehaviour, IUiActiveCheck
{
    PlayerInput playerInput;
    [SerializeField] private UIManager uIManager;
    private bool isPlayerOutInBoard = true;
    private bool isBoardActive;
    Animator animator;

    public bool IsUIActive { get => this.isBoardActive; }
    private void Start()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
    }
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        if (isBoardActive && playerInput.Cancel)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            isBoardActive = false;
        }
        if (!isPlayerOutInBoard)
        {
            if (playerInput.Interact)
            {
                transform.GetChild(0).gameObject.SetActive(true);
                isBoardActive = true;
                //uIManager.RecordBoardAction();
            }
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(false);
            isBoardActive = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            transform.GetChild(1).gameObject.SetActive(true);
            animator.SetBool("isBoxOpen", true);
            Debug.Log("플레이어 인식");
            playerInput = collision.GetComponent<PlayerInput>();
            isPlayerOutInBoard = false;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Exit");
            animator.SetBool("isBoxOpen", false);
            isPlayerOutInBoard = true;
            transform.GetChild(1).gameObject.SetActive(false);
        }
    }
}
