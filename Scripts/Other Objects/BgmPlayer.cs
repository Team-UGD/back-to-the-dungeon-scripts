using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BgmPlayer : MonoBehaviour
{
    [SerializeField] private bool afterFadeIn = true;
    [SerializeField, Min(0f)] private float delay;

    private AudioSource player;

    private void Awake()
    {
        player = GetComponent<AudioSource>();
        GameManager.Instance.OnSceneLoaded += PlayBGM;
        if (!afterFadeIn)
        {
            player.PlayDelayed(delay);
        }
    }

    private void PlayBGM(UnityEngine.SceneManagement.Scene? previous, UnityEngine.SceneManagement.Scene? loaded, SceneLoadingTiming when)
    {
        if (when == SceneLoadingTiming.AfterFadeIn && this.afterFadeIn)
        {
            player.PlayDelayed(delay);
        }
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnSceneLoaded -= PlayBGM;
    }
}
