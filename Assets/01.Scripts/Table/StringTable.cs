using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringData
{
    public string StringKey;
    public string Kr;
    public string Eng;
    public string Jp;
    public string Ch;
}

public class StringTable : IDataTable, IDataLoader<string, StringData>
{
    // 추후 컨텐츠 추가 시 / AppManager PLAY_CONTENT 참고
    private const string LOCALIZE_TABLE_PATH = "UnripeApples/DataTables/StringTable";
    public static event System.Action OnChangeLocalize;

    private SystemLanguage systemLanguage = Application.systemLanguage;
    private Dictionary<string, StringData> localizeStringTable = new Dictionary<string, StringData>();
    public void Clear()
    {
        localizeStringTable.Clear();
    }

    public void Load()
    {
        if (PlayerPrefs.HasKey("LOCALIZE"))
        {
            systemLanguage = (SystemLanguage)PlayerPrefs.GetInt("LOCALIZE");
        }
        else
        {
            switch (systemLanguage)
            {
                case SystemLanguage.Korean:
                    systemLanguage = SystemLanguage.Korean;
                    break;
                case SystemLanguage.Japanese:
                    systemLanguage = SystemLanguage.Japanese;
                    break;
                case SystemLanguage.Chinese:
                    systemLanguage = SystemLanguage.Chinese;
                    break;
                default:
                    systemLanguage = SystemLanguage.English;
                    break;
            }
            PlayerPrefs.SetInt("LOCALIZE", (int)systemLanguage);
        }
        systemLanguage = (SystemLanguage)PlayerPrefs.GetInt("LOCALIZE", (int)SystemLanguage.English);
        Util.Log($"현재 설정된 시스템 언어 : {systemLanguage.ToString()}");
        var table = GenericTableData.getInstance;
        var stringTable = table.Load<StringData>(LOCALIZE_TABLE_PATH);
        Set(stringTable);
    }

    public void OnChangeLanguege(SystemLanguage _language)
    {
        systemLanguage = _language;
        Util.Log($"OnChangeLanguege : {systemLanguage.ToString()}");
        PlayerPrefs.SetInt("LOCALIZE", (int)systemLanguage);
        OnChangeLocalize?.Invoke();
    }
    public SystemLanguage GetSystemLanguage()
    {
        return systemLanguage;
    }

    public string GetLocalizeText(string key)
    {
        var stringData = Get(key);
        if (stringData == null)
        {
            return key;
        }
        string ret = string.Empty;
        switch (systemLanguage)
        {
            case SystemLanguage.Korean:
                ret = stringData.Kr.Replace("\\n", "<br>");
                return ret;
            case SystemLanguage.Japanese:
                ret = stringData.Jp.Replace("\\n", "<br>");
                return ret;
            case SystemLanguage.Chinese:
                ret = stringData.Ch.Replace("\\n", "<br>");
                return ret;
            default:
                ret = stringData.Eng.Replace("\\n", "<br>");
                return ret;
        }
    }

    public StringData Get(string key)
    {
        if (localizeStringTable.ContainsKey(key))
            return localizeStringTable[key];

        return null;
    }

    public IDictionary<string, StringData> GetDictionary()
    {
        return localizeStringTable;
    }

    public void Set(TableData<StringData> _datas)
    {
        var array = _datas.array;
        int arrayCount = array.Count;
        for (int index = 0; index < arrayCount; index++)
        {
            var data = array[index];
#if UNITY_EDITOR
            if (localizeStringTable.ContainsKey(data.StringKey))
            {
                Util.Log($"StringTable Key Overlap !!! {data.StringKey}", Color.red);
                continue;
                //throw new System.Exception("StringTalbe key Overlap");
            }
#endif
            localizeStringTable.Add(data.StringKey, data);
        }
    }

    public void Set(string key, StringData value)
    {
        localizeStringTable[key] = value;
    }

}
