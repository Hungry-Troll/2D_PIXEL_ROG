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

    // Transform을 찾는 메서드
    public static Transform FindChild(this GameObject gameObject, string name)
    {
        return FindChild(name, gameObject.transform);
    }

    // 특정 컴포넌트를 찾는 메서드
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
    /// 게임오브젝트 최상단 루트를 찾고 알아서 재귀로 찾아줌
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
    /// 태그로 루트 찾고 알아서 재귀로 찾아줌
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

    // "UI" 레이어 이름을 정적 필드로 저장하여 매번 변환하지 않도록 함
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
        // 포인터 데이터를 재사용할 수 있도록 Static 변수를 사용
        // 필요 시 각 게임 루프에서 이벤트 시스템을 업데이트
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
