using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extension
{
    public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
    {
        return Util.GetOrAddComponent<T>(go);
    }

    // Transform을 찾는 메서드
    public static Transform FindChild(this GameObject gameObject, string name)
    {
        return Util.FindChild(name, gameObject.transform);
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
}
