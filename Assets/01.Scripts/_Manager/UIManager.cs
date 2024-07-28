using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIManager : Singleton<UIManager>
{
    private const int BASE_SORTING_ORDER = 100;
    private Dictionary<Type, UIBase> _panelDict = null;
    private Stack<UIBase> _panelStack = null;
    private Stack<UIBase> _hideSavePanelStack = null;

    public T Show<T>(string _panelName = "", int order = 0) where T : UIBase
    {
        var panel = GetCachedPanel<T>(_panelName);

        if (!panel.IsShow())
        {
            panel.Show();
            _panelStack.Push(panel);
            //return panel;
        }
#if UNITY_EDITOR
        Debug.Log($"show panel : {typeof(T).Name} > Count : {_panelStack.Count}");
#endif
        panel.SetSortOrder(BASE_SORTING_ORDER + _panelStack.Count + order);
        //ĵ���� ������ �����Ҷ� ��� Ipanel SetSortOrder() �������̽� �߰�

        return panel;
    }

    public void Hide()
    {
        if (_panelStack.Count > 0)
        {
            var panel = _panelStack.Pop();
            panel.Hide();

#if UNITY_EDITOR
            Debug.Log($"hide panel : {panel.GetType().Name} > Remaind Count : {_panelStack.Count}");
#endif
        }
    }

    public void HideAll()
    {
        int cnt = _panelStack.Count;
        for (int i = 0; i < cnt; i++)
        {
            var panel = _panelStack.Peek();
            panel.Hide();
        }
    }

    /// <summary>
    /// Ư�� �г��� ���ö����� ���� �гε� ����
    /// </summary>
    /// <param name="panelName"></param>
    public void HideUntilFound(string panelName)
    {
        int cnt = _panelStack.Count;
        for (int i = 0; i < cnt; i++)
        {
            var panel = _panelStack.Peek();
            if (panel.GetType().Name != panelName)
                Hide();
            else
                break;
        }
    }

    /// <summary>
    /// Ư�� �г��� ���ö����� ���� �гε��� ����鼭 ���ؿ� ������
    /// </summary>
    /// <param name="panelName"></param>
    public void HideAndSaveUntilFound(string panelName)
    {
        int cnt = _panelStack.Count;
        for (int i = 0; i < cnt; i++)
        {
            var panel = _panelStack.Peek();
            if (panel.GetType().Name != panelName)
            {
                _hideSavePanelStack.Push(panel);
                Hide();
            }
            else
                break;
        }
    }
    /// <summary>
    /// ����鼭 ���ؿ� ������ �г��� �ٽ� ����
    /// </summary>
    public void ShowSavePanel()
    {
        var panel = _panelStack.Peek();
        for (int i = 0; i < _hideSavePanelStack.Count; i++)
        {
            var one = _hideSavePanelStack.Pop();
            one.Show();
        }
    }


    /// <summary>
    /// UIManager�� ĳ�� �Ǿ��ִ� Panel�� �����´�.
    /// </summary>
    /// <typeparam name="T">Panel Type</typeparam>
    /// <returns></returns>
    private T GetCachedPanel<T>(string _panelName = "") where T : UIBase
    {
        Type type = typeof(T);
        if (_panelDict.ContainsKey(type) == false)
        {
            AddCachePanel<T>(_panelName);
            //throw new Exception($"not cached panel : {type.Name}");
        }

        return (T)_panelDict[type];
    }

    /// <summary>
    /// Scene�� ��ġ�� Panel�� UIManager�� �̸� ĳ�� �Ѵ�.
    /// </summary>
    /// <typeparam name="T">ĳ���� Panel Type</typeparam>
    /// <param name="_panelName">ĳ���� Scene�� ��ġ�� Panel Object�� �̸�</param>
    public void AddCachePanel<T>(string _panelName = "") where T : UIBase
    {
        T panel = FindPanel<T>(_panelName);
        if (panel != null)
        {
            AddCachePanel<T>(panel);
            return;
        }

        Type type = typeof(T);
        string prefabName = _panelName == string.Empty ? type.Name : _panelName;
        GameObject panelPrefab = null;

        // ���� ������ �߰��� ��� �߰�
        //if (AppManager.Instance._PLAY_CONTENT == PLAY_CONTENT.UNRIPEAPPLES)
        //    panelPrefab = (GameObject)Resources.Load($"UnripeApples/Prefabs/UI/{prefabName}", typeof(GameObject));

        GameObject panelObj = GameObject.Instantiate(panelPrefab);
        string[] nameArr = _panelName.Split('/');
        panelObj.name = $"{nameArr[nameArr.Length - 1]}";
        panel = panelObj.GetComponent<T>();
        panel.Init();
        AddCachePanel<T>(panel);
    }

    private void AddCachePanel<T>(T _panel) where T : UIBase
    {
        Type type = typeof(T);
        if (_panelDict.ContainsKey(type) == true)
        {
            return;
            //throw new Exception($"already cached panel : {type.Name}");
        }
        _panelStack.Push(_panel);
        _panelDict.Add(type, _panel);
    }

    public T FindPanel<T>(string _panelName = "") where T : UIBase
    {
        Type type = typeof(T);
        string panelName = _panelName == string.Empty ? type.Name : _panelName;
        GameObject panelObj = Util.Find(panelName);
        if (panelObj == null)
        {
            return null;
        }

        T panel = panelObj.GetComponent<T>();
        if (panel == null)
        {
            throw new Exception($"must have {type.Name} component");
        }

        return panel;
    }
}
