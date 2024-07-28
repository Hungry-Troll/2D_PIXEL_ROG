using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BGM,SFX,Emotion SFX ���̺� ����
/// </summary>
public class SoundDataTable : IDataTable, IDataLoader<int, SoundData>
{
    // ���� ������ �߰� �� / AppManager PLAY_CONTENT ����
    private const string SOUND_TABLE_PATH = "UnripeApples/DataTables/SoundTable";

    private Dictionary<int, SoundData> _audioTable = new Dictionary<int, SoundData>();
    public void Load()
    {
        var table = GenericTableData.getInstance;
        var audioTable = table.Load<SoundData>(SOUND_TABLE_PATH);
        Set(audioTable);
    }

    public void Clear()
    {
        _audioTable.Clear();
    }

    public SoundData Get(int key)
    {
        if (_audioTable.ContainsKey(key) == true)
        {
            return _audioTable[key];
        }
        Util.Log($"{this.GetType()} ���̺� Key : {key}�� �����ϴ�.", Color.red);
        return null;
    }

    public IDictionary<int, SoundData> GetDictionary()
    {
        return _audioTable;
    }

    public void Set(TableData<SoundData> _datas)
    {
        var array = _datas.array;
        int count = array.Count;
        for (int i = 0; i < count; i++)
        {
            _audioTable.Add(array[i].ID, array[i]);
        }
    }

    public void Set(int key, SoundData value)
    {
        _audioTable[key] = value;
    }
}
