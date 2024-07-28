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
        //캔버스 단위로 관리할때 사용 Ipanel SetSortOrder() 인터페이스 추가

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
    /// 특정 패널이 나올때까지 이전 패널들 숨김
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
    /// 특정 패널이 나올때까지 이전 패널들을 숨기면서 스텍에 저장함
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
    /// 숨기면서 스텍에 저장한 패널을 다시 복구
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
    /// UIManager에 캐싱 되어있는 Panel을 가져온다.
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
    /// Scene에 배치된 Panel을 UIManager에 미리 캐싱 한다.
    /// </summary>
    /// <typeparam name="T">캐싱할 Panel Type</typeparam>
    /// <param name="_panelName">캐싱할 Scene에 배치된 Panel Object의 이름</param>
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

        // 추후 컨텐츠 추가시 경로 추가
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
