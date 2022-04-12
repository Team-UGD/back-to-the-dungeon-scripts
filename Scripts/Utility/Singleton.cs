using UnityEngine;

/// <summary>
/// 싱글톤 패턴을 위한 베이스 클래스
/// </summary>
/// <remarks>
/// protected MySingleton() { } 을 반드시 선언.
/// </remarks>
/// <typeparam name="T">싱글톤으로 사용하려는 클래스</typeparam>
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static bool isShuttingDown = false;
    private static object lockObj = new object();
    private static T instance;

    /// <summary>
    /// <typeparamref name="T"/> 인스턴스를 반환한다.
    /// </summary>
    public static T Instance
    {
        get
        {
            if (isShuttingDown)
            {
                Debug.LogWarning($"[Singleton<{typeof(T)}>] Instance {typeof(T)} already destroyed. Returns null.");
                return null;
            }

            lock (lockObj)
            {

                if (instance == null)
                {
                    instance = FindObjectOfType<T>();

                    if (instance == null)
                    {
                        Debug.LogWarning($"[Singleton<{typeof(T)}>] Can't find the instance of {typeof(T)}. Make sure to create a game object including this component. Returns null.");
                    }
                }

                return instance;
            }
        }
    }

    /// <summary>
    /// Instance 프로퍼티의 반환값이 자기 자신이 아니라면 유니티 오브젝트를 파괴한다. 또한 아직 Instance 프로퍼터가 싱글톤화되지 않았다면 싱글톤화 시킨다.
    /// </summary>
    /// <returns>Instance가 객체 자기 자신이면 true를 반환한다.</returns>
    protected bool CheckSingletonInstance()
    {
        return CheckSingletonInstance(false);
    }

    /// <summary>
    /// Instance 프로퍼티의 반환값이 자기 자신이 아니라면 유니티 오브젝트를 파괴한다. 또한 아직 Instance 프로퍼터가 싱글톤화되지 않았다면 싱글톤화 시킨다.
    /// </summary>
    /// <param name="isDontDestroyOnLoad">싱글톤 유니티 오브젝트를 DontDestroyOnLoad 처리 할지 여부</param>
    /// <returns>Instance가 객체 자기 자신이면 true를 반환한다.</returns>
    protected bool CheckSingletonInstance(bool isDontDestroyOnLoad)
    {
        if (Instance != this)
        {
            Debug.LogWarning($"Scene내에 동일한 {typeof(T)} 싱글톤 오브젝트가 존재해 파괴합니다.\n Object Name : {this.name}, Instance ID : {this.GetInstanceID()}");
            DestroyImmediate(gameObject);
            return false;
        }

        if (isDontDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }

        return true;
    }

    private void OnApplicationQuit()
    {
        isShuttingDown = true;
    }

    private void OnDestroy()
    {
        if (this == instance)
            isShuttingDown = true;

        OnDestroyed();
    }

    protected virtual void OnDestroyed() { }
}