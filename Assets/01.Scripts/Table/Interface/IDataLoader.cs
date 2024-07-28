using System.Collections.Generic;

public interface IDataLoader<TKey, TValue>
{
    public IDictionary<TKey, TValue> GetDictionary();
    void Set(TableData<TValue> _datas);
    void Set(TKey key, TValue value);
    TValue Get(TKey key);
}
