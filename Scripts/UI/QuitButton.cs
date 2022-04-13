using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitButton : MonoBehaviour
{
    [SerializeField] private SettingButton setting;
    public void OnClickGameQuitButton()
    {
        if (!setting.IsSettingActive)
            Application.Quit(); // 어플리케이션 종료
    }
}
