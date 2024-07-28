using System;

/// <summary>
/// 오디오 클립의 타입으로 효과음, 배경음을 구분
/// </summary>
public enum SOUND_TYPE
{
    /// <summary>
    /// 효과음
    /// </summary>
    SFX = 0,
    /// <summary>
    /// 배경음
    /// </summary>
    BGM = 1,
    /// <summary>
    /// 목소리음
    /// </summary>
    VOICE = 2,
}

[Serializable]
public class SoundData
{
    /// <summary>
    /// 사운드 클립의 아이디
    /// </summary>
    public int ID;
    /// <summary>
    /// 사운드 클립의 타입 [0]:사운드 이펙트,  [1]:배경음
    /// </summary>
    public int Type;
    /// <summary>
    /// 사운드 클립 이름으로 Resource내에 오디오클립이름
    /// </summary>
    public string SoundName;
    /// <summary>
    /// 사운드 클립에 대한 설명(관리용)
    /// </summary>
    public string Description;
}
