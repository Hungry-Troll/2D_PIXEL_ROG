using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBase : MonoBehaviour, IPanel
{
    /// <summary>
    /// �ٱ��� ������ ���� ������ ������ string���̺� ������
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
    /// �����ÿ� ȣ��ǰ� ��� ������ �ٲ� �� ȣ��
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
        //if (flagMatchWH)//ȭ�� ������ ���� ��ġ �� ����
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
