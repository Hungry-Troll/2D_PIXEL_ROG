using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : Singleton<ObjectManager>
{
    //���� ��ųʸ��� ������ �� ���� ���� (���� ���� ��)
    //Dictionary<int, GameObject> _object = new Dictionary<int, GameObject>();


    public GameObject _target;

    public List<GameObject> _objects = new List<GameObject>(); //�̰� CretureController�� �����ϴ°� ���� �ʳ� ���� Find �Լ� ����ȭ

    public List<GameObject> _monsterObjects = new List<GameObject>();

    public List<GameObject> _itemObjects = new List<GameObject>();

    //�ߺ� ��ǥ üũ��
    public List<Vector3Int> _checkPath = new List<Vector3Int>();
    //�ߺ� ��ǥ üũ�� �Լ�
    public void PosCheckPathAdd(Vector3Int randPos)
    {
        _checkPath.Add(randPos);
        //�ϸ��� ����� �ٽ� �����ؾߵ� //���� ��Ʈ�ѷ� �ϰ��ÿ� �־��
    }
    //�ߺ� ��ǥ ���
    public bool PosCheck(List<Vector3Int> checkPath, Vector3Int randPos)
    {
        if (checkPath.Count > 1)
        {
            for (int i = 0; i < checkPath.Count - 1; i++)
            {
                if (checkPath[i] == randPos)
                {
                    //�ߺ��̸� false
                    return false;
                }

            }
        }
        //�ߺ� �ƴϸ� true
        return true;
    }


    public void Add(GameObject go)
    {
        _objects.Add(go);
    }

    public void Remove(GameObject go)
    {
        _objects.Remove(go);
    }

    public void ItemAdd(GameObject go)
    {
        _itemObjects.Add(go);
    }

    public void ItemRemove(GameObject go)
    {
        _itemObjects.Remove(go);
    }

    public GameObject Find(Vector3Int cellPos)
    {
        foreach (GameObject obj in _objects)
        {
            CreatureController cc = obj.GetComponent<CreatureController>();
            if (cc == null)
                continue;

            if (cc.CellPos == cellPos)
                return obj;
        }
        return null;
    }

    //�÷��̾ ã�� ai�� �����
    public GameObject Find(Func<GameObject, bool> condition)
    {
        foreach (GameObject obj in _objects)
        {
            if (condition.Invoke(obj))
                return obj;
        }
        return null;
    }

    public void Clear()
    {
        _objects.Clear();
    }

    public void ItemClear()
    {
        _itemObjects.Clear();
    }

}
