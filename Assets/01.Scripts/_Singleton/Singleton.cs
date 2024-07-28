using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour, ISingleton where T : Component
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    SetupInstance();
                }
            }
            return _instance;
        }
    }

    private static void SetupInstance()
    {
        _instance = FindObjectOfType<T>();
        if (_instance == null)
        {
            GameObject gameObj = new GameObject();
            gameObj.name = typeof(T).Name;
            _instance = gameObj.AddComponent<T>();
            DontDestroyOnLoad(gameObj);
        }
    }

    public virtual void Init()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public virtual void UnInit() { }
}
