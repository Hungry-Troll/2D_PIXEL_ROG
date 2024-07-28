using System.Collections.Generic;
using UnityEngine;
using System;

public class EventManager : Singleton<EventManager>
{
    /// <summary>
    /// �̺�Ʈ ������ ������Ʈ �迭
    /// </summary>
    private Dictionary<Enum, List<IListener>> _Listeners = new Dictionary<Enum, List<IListener>>();

    public override void Init()
    {
        base.Init();
    }

    /// <summary>
    /// �̺�Ʈ ������ �߰� �Լ�
    /// </summary>
    /// <param name="EventType"></param>
    /// <param name="Listener"></param>
    public void AddListener(Enum EventType, IListener Listener)
    {
        // �̺�Ʈ ���� ������
        List<IListener> ListenList = null;
        // �̺�Ʈ ���� Ű�� �ִ��� Ȯ�� ������ �߰�
        if (_Listeners.TryGetValue(EventType, out ListenList))
        {
            ListenList.Add(Listener);
            return;
        }
        // ������ ���ο� ����Ʈ ���� �� �߰�
        ListenList = new List<IListener>();
        ListenList.Add(Listener);
        _Listeners.Add(EventType, ListenList);
    }

    /// <summary>
    /// �̺�Ʈ ������ ���� �Լ�
    /// </summary>
    /// <param name="EventType"></param>
    /// <param name="Listener"></param>
    public void RemoveListener(Enum EventType, IListener Listener)
    {
        // �̺�Ʈ ���� ������
        List<IListener> ListenList = null;
        // �̺�Ʈ ���� Ű�� �ִ��� Ȯ�� ������ �߰�
        if (_Listeners.TryGetValue(EventType, out ListenList))
        {
            ListenList.Remove(Listener);
            return;
        }
    }
    /// <summary>
    /// �̺�Ʈ �����ʿ��� �����ϱ� ���� �Լ�
    /// </summary>
    /// <param name="EventType">�ҷ��� �̺�Ʈ</param>
    /// <param name="Sender">�̺�Ʈ�� �θ��� ������Ʈ</param>
    /// <param name="Param">���� ������ �Ķ����</param>
    public void PostNotification<T>(Enum EventType, Component Sender, params T[] Param)
    {
        List<IListener> ListenList = null;
        // �̺�Ʈ �׸��� ������ ����
        if (!_Listeners.TryGetValue(EventType, out ListenList))
            return;
        // �׸��� �����ϸ� �����ʿ��� �˷���
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
    /// ��ųʸ��� ������� �׸� ����
    /// </summary>
    public void RemoveRedundancies()
    {
        Dictionary<Enum, List<IListener>> TmpListeners = new Dictionary<Enum, List<IListener>>();

        // ��� ��ųʸ� �׸� ��ȸ
        foreach (var item in _Listeners)
        {
            // ����Ʈ�� ��� ������ ������Ʈ ��ȸ null ����
            for (int i = 0; i < item.Value.Count - 1; i--)
            {
                //null �̸� �׸��� �����
                if (item.Value[i].Equals(null))
                    item.Value.RemoveAt(i);
            }
            // �˸��� �ޱ� ���� �׸�鸸 ����Ʈ�� ������ �� �׸���� �ӽ� ��ųʸ��� ��´�
            if (item.Value.Count > 0)
            {
                TmpListeners.Add(item.Key, item.Value);
            }
        }
        // ����ȭ�� ��ųʸ��� ��ü
        _Listeners = TmpListeners;
    }

    // ���� �ڵ� ����
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
/// PostNotification �� �� ���ĸŰ����� �뵵�θ� ���
/// </summary>
public struct EMPTY { }
