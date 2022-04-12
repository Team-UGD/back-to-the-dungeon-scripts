public class CoroutineManager : Singleton<CoroutineManager>
{
    private void Awake()
    {
        if (!CheckSingletonInstance(true))
            return;

        GameManager.Instance.OnSceneLoaded += CoroutineAllStop;
    }

    private void CoroutineAllStop(UnityEngine.SceneManagement.Scene? previous, UnityEngine.SceneManagement.Scene? loaded, SceneLoadingTiming when)
    {
        if (when == SceneLoadingTiming.BeforeLoading)
            StopAllCoroutines();
    }

    protected CoroutineManager() { }
}
