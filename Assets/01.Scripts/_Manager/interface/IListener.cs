using System;
using UnityEngine;

public interface IListener
{
    /// <summary>
    /// 이벤트 발생 시 리스너에서 호출
    /// </summary>
    /// <param name="Event_Type"></param>
    /// <param name="Sender"></param>
    /// <param name="Parm"></param>
    void OnEvent<T>(Enum Event_Type, Component Sender, params T[] Param);

    /// <summary>
    /// 이벤트를 받는 함수
    /// </summary>
    void AddListener();
}
