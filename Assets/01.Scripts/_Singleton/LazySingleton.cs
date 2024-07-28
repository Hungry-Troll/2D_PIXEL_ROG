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
    /// 클래스를 초기화
    /// </summary>
    protected LazySingleton()
    {
    }
    /// <summary>
    /// 클래스 종료시
    /// </summary>
    ~LazySingleton()
    {
        Util.Log("LazySingleton Destroy");

    }
}
