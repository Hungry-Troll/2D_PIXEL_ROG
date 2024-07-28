using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : Singleton<ObjectManager>
{
    //추후 딕셔너리로 만들어야 될 수도 있음 (서버 연동 시)
    //Dictionary<int, GameObject> _object = new Dictionary<int, GameObject>();


    public GameObject _target;

    public List<GameObject> _objects = new List<GameObject>(); //이걸 CretureController로 변경하는게 낫지 않나 추후 Find 함수 최적화

    public List<GameObject> _monsterObjects = new List<GameObject>();

    public List<GameObject> _itemObjects = new List<GameObject>();

    //중복 좌표 체크용
    public List<Vector3Int> _checkPath = new List<Vector3Int>();
    //중복 좌표 체크용 함수
    public void PosCheckPathAdd(Vector3Int randPos)
    {
        _checkPath.Add(randPos);
        //턴마다 지우고 다시 저장해야됨 //몬스터 컨트롤러 턴관련에 넣어놈
    }
    //중복 좌표 계산
    public bool PosCheck(List<Vector3Int> checkPath, Vector3Int randPos)
    {
        if (checkPath.Count > 1)
        {
            for (int i = 0; i < checkPath.Count - 1; i++)
            {
                if (checkPath[i] == randPos)
                {
                    //중복이면 false
                    return false;
                }

            }
        }
        //중복 아니면 true
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

    //플레이어를 찾는 ai에 사용함
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
