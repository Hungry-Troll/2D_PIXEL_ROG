using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBase : MonoBehaviour, IPanel
{
    /// <summary>
    /// 다국어 지원을 위한 문장을 얻어오는 string테이블 데이터
    /// </summary>
    protected StringTable _stringTable = null;
    private GraphicRaycaster _raycaster = null;
    protected Canvas _canvas = null;
    private bool _isShow = false;

    public virtual void Init()
    {
        _raycaster = GetComponent<GraphicRaycaster>();
        _canvas = GetComponent<Canvas>();
        _isShow = _canvas.enabled;
        _stringTable = GenericTableData.getInstance.GetTable<StringTable, string, StringData>() as StringTable;
        OnChangeLocalize();
    }

    /// <summary>
    /// 생성시에 호출되고 언어 설정이 바뀔 때 호출
    /// </summary>
    protected virtual void OnChangeLocalize()
    {

    }

    public void Hide()
    {
        if (_canvas == null)
            return;

        _isShow = false;
        _canvas.enabled = false;
        _raycaster.enabled = false;
    }

    public bool IsShow()
    {
        if (_canvas == null)
            return false;

        return _isShow;
    }

    public void Show(bool flagMatchWH = true)
    {
        if (_canvas == null)
        {
            return;
        }
        Util.Log($"Show flagMatchWH {flagMatchWH}");
        //if (flagMatchWH)//화면 비율에 따른 매치 값 적용
        //{
        //    if ((float)Screen.height / Screen.width >= 1.5f)
        //    {
        //        GetComponent<CanvasScaler>().matchWidthOrHeight = 0f;
        //    }
        //    else
        //    {
        //        GetComponent<CanvasScaler>().matchWidthOrHeight = 1f;
        //    }
        //}
        //else
        //{
        //    GetComponent<CanvasScaler>().matchWidthOrHeight = 0f;
        //}
        _isShow = true;
        _canvas.enabled = true;
        _raycaster.enabled = true;
    }
    public void SetSortOrder(int _order)
    {
        if (_canvas == null)
            return;

        _canvas.sortingOrder = _order;
    }

    public int GetSortOrder()
    {
        if (_canvas == null)
            return int.MaxValue;

        return _canvas.sortingOrder;
    }
}
