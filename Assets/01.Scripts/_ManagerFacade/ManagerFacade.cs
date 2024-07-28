using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerFacade : MonoBehaviour
{
    public static bool _isInit;

    public static void ManagerInit()
    {
        if (_isInit)
            return;

        _isInit = true;
        // �Ŵ��� Init();
        GameManager.Instance.Init();
        UIManager.Instance.Init();
        SoundManager.Instance.Init();
        EventManager.Instance.Init();

    }

    public static void ManagerUnInit()
    {
        if (_isInit == false)
            return;

        _isInit = false;
        // �Ŵ��� UnInit();
        GameManager.Instance.UnInit();
        UIManager.Instance.UnInit();
        SoundManager.Instance.UnInit();
        EventManager.Instance.UnInit();
    }
}
