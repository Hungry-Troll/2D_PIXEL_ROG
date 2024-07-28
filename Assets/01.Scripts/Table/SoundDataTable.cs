using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BGM,SFX,Emotion SFX 테이블 관리
/// </summary>
public class SoundDataTable : IDataTable, IDataLoader<int, SoundData>
{
    // 추후 컨텐츠 추가 시 / AppManager PLAY_CONTENT 참고
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
        Util.Log($"{this.GetType()} 테이블에 Key : {key}가 없습니다.", Color.red);
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
