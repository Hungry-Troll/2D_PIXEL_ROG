using System;
using UnityEngine;

public interface IListener
{
    /// <summary>
    /// �̺�Ʈ �߻� �� �����ʿ��� ȣ��
    /// </summary>
    /// <param name="Event_Type"></param>
    /// <param name="Sender"></param>
    /// <param name="Parm"></param>
    void OnEvent<T>(Enum Event_Type, Component Sender, params T[] Param);

    /// <summary>
    /// �̺�Ʈ�� �޴� �Լ�
    /// </summary>
    void AddListener();
}
