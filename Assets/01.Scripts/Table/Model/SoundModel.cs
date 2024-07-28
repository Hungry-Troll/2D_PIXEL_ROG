using System;

/// <summary>
/// ����� Ŭ���� Ÿ������ ȿ����, ������� ����
/// </summary>
public enum SOUND_TYPE
{
    /// <summary>
    /// ȿ����
    /// </summary>
    SFX = 0,
    /// <summary>
    /// �����
    /// </summary>
    BGM = 1,
    /// <summary>
    /// ��Ҹ���
    /// </summary>
    VOICE = 2,
}

[Serializable]
public class SoundData
{
    /// <summary>
    /// ���� Ŭ���� ���̵�
    /// </summary>
    public int ID;
    /// <summary>
    /// ���� Ŭ���� Ÿ�� [0]:���� ����Ʈ,  [1]:�����
    /// </summary>
    public int Type;
    /// <summary>
    /// ���� Ŭ�� �̸����� Resource���� �����Ŭ���̸�
    /// </summary>
    public string SoundName;
    /// <summary>
    /// ���� Ŭ���� ���� ����(������)
    /// </summary>
    public string Description;
}
