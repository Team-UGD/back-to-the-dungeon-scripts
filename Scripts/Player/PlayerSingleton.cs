public class PlayerSingleton : Singleton<PlayerSingleton>
{
    protected PlayerSingleton() { }

    private void Awake()
    {
        if (!CheckSingletonInstance(true))
        {
            return;
        }
    }
}
