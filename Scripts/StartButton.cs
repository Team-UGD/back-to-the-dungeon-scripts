using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    [SerializeField] private SettingButton setting;
    [SerializeField] private SettingButtonUI settingUI;
    [SerializeField] private GameStartController controller;
    public void OnClickGameStartButton()
    {
        if (!setting.IsSettingActive)
        {
            controller.StartGame(settingUI.SettingLevel);
            //GameManager.Instance.LoadScene(GameManager.Instance.InGameStartSceneBuildIndex);
            this.gameObject.GetComponent<Button>().interactable = false;
        }
    }
}
