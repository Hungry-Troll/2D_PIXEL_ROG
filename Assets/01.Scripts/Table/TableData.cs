using System;
using System.Collections.Generic;

[Serializable]
public class TableData<T>
{
    public string md5;
    public List<T> array;
}
