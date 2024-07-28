using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Util 
{

    public static bool _isLog = true;

    public static void Log(object message)
    {
        if (_isLog)
            Debug.Log(message);
    }

    public static void Log(object message, Color color)
    {
        if (_isLog)
        {
            string htmlColor = ColorUtility.ToHtmlStringRGBA(color);
            Debug.Log($"<color=#{htmlColor}>{message}</color>");
        }
    }

    public static void LogError(object message)
    {
        if (_isLog)
            Debug.LogError(message);
    }

    private static Transform FindChild(string name, Transform _tr)
    {
        if (_tr.name == name)
        {
            return _tr;
        }
        for (int i = 0; i < _tr.childCount; i++)
        {
            Transform findTr = FindChild(name, _tr.GetChild(i));
            if (findTr != null)
                return findTr;
        }
        return null;
    }

    // Transform�� ã�� �޼���
    public static Transform FindChild(this GameObject gameObject, string name)
    {
        return FindChild(name, gameObject.transform);
    }

    // Ư�� ������Ʈ�� ã�� �޼���
    public static T FindChildComponent<T>(this GameObject gameObject, string name) where T : Component
    {
        Transform childTransform = gameObject.FindChild(name);
        if (childTransform != null)
        {
            return childTransform.GetComponent<T>();
        }
        return null;
    }

    /// <summary>
    /// ���ӿ�����Ʈ �ֻ�� ��Ʈ�� ã�� �˾Ƽ� ��ͷ� ã����
    /// </summary>
    /// <param name="_objName"></param>
    /// <returns></returns>
    public static GameObject Find(string _objName)
    {
        GameObject result = null;
        foreach (GameObject root in Object.FindObjectsOfType(typeof(GameObject)))
        {
            if (root.transform.parent == null)
            {
                result = Find(root, _objName, 0);
                if (result != null)
                {
                    break;
                }
            }
        }
        return result;
    }

    /// <summary>
    /// �±׷� ��Ʈ ã�� �˾Ƽ� ��ͷ� ã����
    /// </summary>
    /// <param name="_objName"></param>
    /// <param name="_tag"></param>
    /// <returns></returns>
    public static GameObject Find(string _objName, string _tag)
    {
        GameObject result = null;
        foreach (GameObject parent in GameObject.FindGameObjectsWithTag(_tag))
        {
            result = Find(parent, _objName, 0);
            if (result != null)
            {
                break;
            }
        }

        return result;
    }

    private static GameObject Find(GameObject _item, string _objName, int _index)
    {
        if (_index == 0 && _item.name == _objName)
            return _item;

        if (_index < _item.transform.childCount)
        {
            GameObject result = Find(_item.transform.GetChild(_index).gameObject, _objName, 0);
            if (result == null)
            {
                return Find(_item, _objName, ++_index);
            }
            else
            {
                return result;
            }
        }
        return null;
    }

    public static T GetOrAddComponent<T>(GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();
        if (component == null)
        {
            component = gameObject.AddComponent<T>();
        }
        return component;
    }

    // "UI" ���̾� �̸��� ���� �ʵ�� �����Ͽ� �Ź� ��ȯ���� �ʵ��� ��
    private static readonly int UILayer = LayerMask.NameToLayer("UI");
    public static bool PointerIsOverUI(Vector2 screenPos)
    {
        var pointerData = ScreenPosToPointerData(screenPos);
        var hitObject = UIRaycast(pointerData);
        return hitObject != null && hitObject.layer == UILayer;
    }

    private static PointerEventData _pointerEventData;
    private static PointerEventData ScreenPosToPointerData(Vector2 screenPos)
    {
        // ������ �����͸� ������ �� �ֵ��� Static ������ ���
        // �ʿ� �� �� ���� �������� �̺�Ʈ �ý����� ������Ʈ
        if (_pointerEventData == null)
        {
            _pointerEventData = new PointerEventData(EventSystem.current);
        }
        _pointerEventData.position = screenPos;
        return _pointerEventData;
    }

    private static GameObject UIRaycast(PointerEventData pointerData)
    {
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        return results.Count > 0 ? results[0].gameObject : null;
    }
}
