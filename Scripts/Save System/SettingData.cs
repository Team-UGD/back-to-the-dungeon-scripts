using UnityEngine;


[System.Serializable]
public sealed class SettingData
{
    public readonly GameLevel lastSelectedLevel;
    public readonly float audioVolume;

    public SettingData()
    {
        this.lastSelectedLevel = GameManager.Instance.Level;
        this.audioVolume = AudioListener.volume;
    }
}
