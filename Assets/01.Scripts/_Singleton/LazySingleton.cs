using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

public class LazySingleton<T> where T : class
{
    private static readonly Lazy<T>
        _instance = new Lazy<T>(CreateInstanceOf, LazyThreadSafetyMode.ExecutionAndPublication);

    private static T CreateInstanceOf()
    {
        return Activator.CreateInstance(typeof(T), true) as T;
    }

    public static T getInstance { get { return _instance.Value; } }
    /// <summary>
    /// Ŭ������ �ʱ�ȭ
    /// </summary>
    protected LazySingleton()
    {
    }
    /// <summary>
    /// Ŭ���� �����
    /// </summary>
    ~LazySingleton()
    {
        Util.Log("LazySingleton Destroy");

    }
}
