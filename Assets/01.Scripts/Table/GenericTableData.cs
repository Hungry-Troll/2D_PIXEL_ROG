using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class GenericTableData : LazySingleton<GenericTableData>
{
    protected Dictionary<System.Type, object> tables = new Dictionary<System.Type, object>();

    public T LoadTable<T>() where T : IDataTable, new()
    {
        if (tables.ContainsKey(typeof(T)) == true)
        {
            return (T)tables[typeof(T)];
        }

        T table = new T();
        table.Load();
        tables.Add(typeof(T), table);

        return table;
    }

    public T GetTableByType<T>() where T : class, IDataTable
    {
        System.Type type = typeof(T);
        if (tables.ContainsKey(type))
        {
            return (T)tables[type];
        }

        Util.Log($"{type} 테이블이 없습니다.", Color.red);

        return null;
    }

    public IDataLoader<TKey, TValue> GetTable<T, TKey, TValue>() where T : class, IDataLoader<TKey, TValue>
    {
        System.Type type = typeof(T);
        if (tables.ContainsKey(type))
        {
            return (IDataLoader<TKey, TValue>)tables[type];
        }

        Util.Log($"{type} 테이블이 없습니다.", Color.red);

        return null;
    }

    public TValue GetTableData<T, TKey, TValue>(TKey _key) where T : class, IDataLoader<TKey, TValue>
    {
        IDataLoader<TKey, TValue> table = GetTable<T, TKey, TValue>();
        return table.Get(_key);
    }

    public void SetTableData<T, TKey, TValue>(TKey _key, TValue _Value) where T : class, IDataLoader<TKey, TValue>
    {
        IDataLoader<TKey, TValue> table = GetTable<T, TKey, TValue>();
        table.Set(_key, _Value);
    }

    #region Json Loader
    public TableData<T> Load<T>(string _path)
    {
        TableData<T> TableData;
        TableData = LoadJsonFile<TableData<T>>(_path);
        return TableData;
    }
    public T LoadJsonFile<T>(string _path)
    {
        TextAsset json = (TextAsset)Resources.Load(_path, typeof(TextAsset));
        if (json == null)
        {
            throw new System.Exception($"{_path}");
        }
        return JsonConvert.DeserializeObject<T>(json.text);
    }
    #endregion

}