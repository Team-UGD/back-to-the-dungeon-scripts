using System;
using UnityEngine;

public class GameStartController : MonoBehaviour
{
    [SerializeField] private PlayerInput player;

    private void OnEnable()
    {
        player.enabled = false;
    }

    public void StartGame(GameLevel level)
    {
        try
        {
            GameManager.Instance.Level = level;
            GameManager.Instance.LoadGameData();
            GameManager.Instance.SaveGameData();
            SettingDataController.SaveSettingData();
            GameManager.Instance.LoadScene(GameManager.Instance.InGameStartSceneBuildIndex, player.gameObject);
        }
        catch (ArgumentException e)
        {

            Debug.LogException(e, this);
            return;
        }
    }

    private void OnDisable()
    {
        player.enabled = true;
    }
}
