using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingButton : MonoBehaviour
{
    [SerializeField] private SettingButtonUI ui;
    private bool isSetting = false;
    public bool IsSettingActive { get { return isSetting; } set { isSetting = value; } }
    //setting 버튼 클릭시 다른 버튼 제어 프로퍼티
    public void OnClickGameSettingButton()
    {
        isSetting = true;
        ui.transform.GetChild(0).gameObject.SetActive(true);
        ui.IsStart = true;
    }
}
