using System.Collections.Generic;
using UnityEngine;
using System;

public class EventManager : Singleton<EventManager>
{
    /// <summary>
    /// 이벤트 리스너 오브젝트 배열
    /// </summary>
    private Dictionary<Enum, List<IListener>> _Listeners = new Dictionary<Enum, List<IListener>>();

    public override void Init()
    {
        base.Init();
    }

    /// <summary>
    /// 이벤트 리스너 추가 함수
    /// </summary>
    /// <param name="EventType"></param>
    /// <param name="Listener"></param>
    public void AddListener(Enum EventType, IListener Listener)
    {
        // 이벤트 수신 리스너
        List<IListener> ListenList = null;
        // 이벤트 형식 키가 있는지 확인 있으면 추가
        if (_Listeners.TryGetValue(EventType, out ListenList))
        {
            ListenList.Add(Listener);
            return;
        }
        // 없으면 새로운 리스트 생성 후 추가
        ListenList = new List<IListener>();
        ListenList.Add(Listener);
        _Listeners.Add(EventType, ListenList);
    }

    /// <summary>
    /// 이벤트 리스너 제거 함수
    /// </summary>
    /// <param name="EventType"></param>
    /// <param name="Listener"></param>
    public void RemoveListener(Enum EventType, IListener Listener)
    {
        // 이벤트 수신 리스너
        List<IListener> ListenList = null;
        // 이벤트 형식 키가 있는지 확인 있으면 추가
        if (_Listeners.TryGetValue(EventType, out ListenList))
        {
            ListenList.Remove(Listener);
            return;
        }
    }
    /// <summary>
    /// 이벤트 리스너에게 전달하기 위한 함수
    /// </summary>
    /// <param name="EventType">불려질 이벤트</param>
    /// <param name="Sender">이벤트를 부르는 오브젝트</param>
    /// <param name="Param">선택 가능한 파라미터</param>
    public void PostNotification<T>(Enum EventType, Component Sender, params T[] Param)
    {
        List<IListener> ListenList = null;
        // 이벤트 항목이 없으면 끝냄
        if (!_Listeners.TryGetValue(EventType, out ListenList))
            return;
        // 항목이 존재하면 리스너에게 알려줌
        for (int i = 0; i < ListenList.Count; i++)
        {
            if (!ListenList[i].Equals(null))
                ListenList[i].OnEvent<T>(EventType, Sender, Param);
        }
    }

    public void RemoveEvent(Enum EventType)
    {
        _Listeners.Remove(EventType);
    }
    /// <summary>
    /// 딕셔너리에 쓸모없는 항목 제거
    /// </summary>
    public void RemoveRedundancies()
    {
        Dictionary<Enum, List<IListener>> TmpListeners = new Dictionary<Enum, List<IListener>>();

        // 모든 딕셔너리 항목 순회
        foreach (var item in _Listeners)
        {
            // 리스트의 모든 리스너 오브젝트 순회 null 제거
            for (int i = 0; i < item.Value.Count - 1; i--)
            {
                //null 이면 항목을 지운다
                if (item.Value[i].Equals(null))
                    item.Value.RemoveAt(i);
            }
            // 알림을 받기 위한 항목들만 리스트에 남으면 이 항목들을 임시 딕셔너리에 담는다
            if (item.Value.Count > 0)
            {
                TmpListeners.Add(item.Key, item.Value);
            }
        }
        // 최적화된 딕셔너리로 교체
        _Listeners = TmpListeners;
    }

    // 추후 코드 보강
    //private void OnLevelWasLoaded()
    //{
    //    RemoveRedundancies();
    //}

    public override void UnInit()
    {
        base.UnInit();
    }
}

/// <summary>
/// PostNotification 의 빈 형식매개변수 용도로만 사용
/// </summary>
public struct EMPTY { }
