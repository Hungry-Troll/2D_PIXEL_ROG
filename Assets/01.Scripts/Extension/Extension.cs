using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extension
{
    public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
    {
        return Util.GetOrAddComponent<T>(go);
    }

    // Transform�� ã�� �޼���
    public static Transform FindChild(this GameObject gameObject, string name)
    {
        return Util.FindChild(name, gameObject.transform);
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
}
