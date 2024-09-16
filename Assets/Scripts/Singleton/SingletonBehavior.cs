using UnityEngine;

/// <summary> Base MonoBehaviour clas that implements the singleton pattern </summary>
/// <typeparam name="T"> Class type </typeparam>
public abstract class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static bool applicationIsQuitting { get; set; }

    private static T instance;

    /// <summary> Get singleton instance </summary>
    public static T Instance
    {
        get
        {
            // Debug.Log($"Instance");
            if (applicationIsQuitting) 
                return null;

            if (instance != null)
                return instance;

            instance = FindObjectOfType<T>();

            if (instance != null) 
                return instance;

            instance = new GameObject(typeof(T).Name).AddComponent<T>();
            return instance;
        }
    }

    protected virtual void OnApplicationQuit() =>
        applicationIsQuitting = true;

    protected virtual void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.Log($"There cannot be two '{typeof(T).Name}' active at the same time. Destroying the '{gameObject.name}' GameObject!");
            Destroy(gameObject);
            return;
        }

        instance = GetComponent<T>();
        DontDestroyOnLoad(gameObject);
    }
}