using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    private const string _SFX_SOUND_PATH = "UnripeApples/Sounds/SFX/";
    private const string _BGM_SOUND_PATH = "UnripeApples/Sounds/BGM/";
    private const string _VOICE_SOUND_PATH = "UnripeApples/Sounds/VOICE/";
    private const string _TEST_SOUND_PATH = "Sounds/TEST/";
    IDictionary<int, SoundData> _soundDataTable;//사운드 데이터 전체
    private Dictionary<string, AudioClip> _sfxs = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> _voices = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> _bgms = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> _testSounds = new Dictionary<string, AudioClip>(); //테이블에 추가 하지 않고 테스트 하기 위함
    private List<AudioSource> _sfxPool = new List<AudioSource>();
    private AudioSource _bgmPool;
    private AudioSource _voicePool;
    private List<AudioSource> _testSoundPool = new List<AudioSource>();
    private float _volumeBgm;//BGM볼륨
    private float _volume;//볼륨
    /// <summary>
    /// 사운드 볼륨 값 get, set
    /// </summary>
    public float SoundVolume
    {
        get { return _volume = PlayerPrefs.GetFloat("SoundVolume", 1f); }
        set
        {
            _volume = value;
            PlayerPrefs.SetFloat("SoundVolume", _volume);
            SetVolume();
        }
    }
    public float BgmVolume
    {
        get
        {
            return _volumeBgm = PlayerPrefs.GetFloat("BgmVolume", 1f);
        }
        set
        {
            _volumeBgm = value;
            PlayerPrefs.SetFloat("BgmVolume", _volumeBgm);
            SetVolumeBgm();
        }
    }

    public string BGMName
    {
        get
        {
            if (_bgmPool.clip)
                return _bgmPool.clip.name;
            else
                return string.Empty;
        }
    }
    private bool _onsfx;//효과음 목소리음 on, off
    /// <summary>
    /// 효과음, 목소리음 on, off 설정 get, set
    /// </summary>
    public bool OnSFX
    {
        get { return _onsfx = PlayerPrefs.GetInt("OnSFX", 1) == 1; }
        set
        {
            _onsfx = value;
            PlayerPrefs.SetInt("OnSFX", _onsfx ? 1 : 0);
            if (_onsfx == false) { StopSFX(); StopVoice(); }//진행되고 있는 소리를 멈춘다.
        }
    }
    private bool _onbgm;//배경음 on, off
    /// <summary>
    /// 배경음 on, off 설정 get, set
    /// </summary>
    public bool OnBGM
    {
        get { return _onbgm = PlayerPrefs.GetInt("OnBGM", 1) == 1; }
        set
        {
            _onbgm = value;
            PlayerPrefs.SetInt("OnBGM", _onbgm ? 1 : 0);
            if (_onbgm == false)
            { StopBGM(); }//진행되고 있는 소리를 멈춘다.
            else
            {
                if (_bgmPool.isPlaying == false)
                    _bgmPool.Play();
            }
        }
    }

    public SoundManager()
    {
        Util.Log($"SoundManager Init");

    }
    ~SoundManager()
    {
        Util.Log("SoundManager Destroy");
    }

    public override void Init()
    {
        base.Init();
        //_bgmPool = Util.GetOrAddComponent<AudioSource>(gameObject);
        float soundVolume = SoundVolume;
        SoundVolume = soundVolume;
        float bgSoundVolume = BgmVolume;
        BgmVolume = bgSoundVolume;
        bool onbgm = OnBGM;
        bool onsfx = OnSFX;
        Util.Log($"soundVolume {soundVolume} bgSoundVolume {bgSoundVolume} onbgm {onbgm} onsfx {onsfx}");
        GenericTableData.getInstance.LoadTable<SoundDataTable>();//오디오 테이블 데이터 로드
        _soundDataTable = GenericTableData.getInstance.GetTable<SoundDataTable, int, SoundData>().GetDictionary();//오디오 데이터 딕셔너리로 가져오기
    }

    /// <summary>
    /// 배경음을 플레이
    /// </summary>
    /// <param name="soundID">사운드 아이디</param>
    public void PlayBGM(int soundID)
    {
        if (_soundDataTable.TryGetValue(soundID, out SoundData soundData))
        {
            if (soundData.Type == (int)SOUND_TYPE.BGM)//사운드 타입이 배경음인지 확인
            {
                PlayBGM(soundData);
            }
            else
            {
                Util.LogError($"SoundID {soundID} is exist but Sound Type is not BGM");
            }
        }
        else
        {
            Util.LogError($"SoundID {soundID} is not exist");
        }
    }

    public void PlayBGM(string soundName)
    {
        var data = _soundDataTable.FirstOrDefault(x => x.Value.SoundName == soundName);
        if (data.Value.Type == (int)SOUND_TYPE.BGM)//사운드 타입이 배경음인지 확인
        {
            var targetData = data.Value as SoundData;
            PlayBGM(targetData);
        }
        else
        {
            Util.LogError($"SoundName {soundName} is exist but Sound Type is not BGM");
        }
    }

    public void StopBGM()
    {
        if (_bgmPool == null)
            return;

        if (_bgmPool.isPlaying == false)
            return;

        _bgmPool.Stop();
    }
    public void RestartBGM()
    {
        if (_bgmPool != null && _bgmPool.clip != null && _bgmPool.isPlaying == false)
        {
            _bgmPool.Play();
        }
    }
    /// <summary>
    /// 효과음과 목소리음을 Play<br/>
    /// 사운드ID는 _SoundTable.xlsx 파일을 참조
    /// </summary>
    /// <param name="soundID"></param>
    /// <param name="loop"></param>
    public void Play(int soundID, bool loop = false)
    {
        //XosoftUtil.Log($"Play soundID {soundID}");
        if (soundID == 0) return;
        if (_soundDataTable.TryGetValue(soundID, out SoundData soundData))
        {
            switch (soundData.Type)
            {
                case (int)SOUND_TYPE.SFX:
                    PlaySFX(soundData, loop);
                    break;
                case (int)SOUND_TYPE.VOICE:
                    PlayVoice(soundData);
                    break;
            }
        }
        else
        {
            Util.LogError($"SoundID {soundID} is not exist");
        }

    }

    public void PlayShot(int soundID, bool loop = false)
    {
        //XosoftUtil.Log($"Play soundID {soundID}");
        if (soundID == 0) return;
        if (_soundDataTable.TryGetValue(soundID, out SoundData soundData))
        {
            switch (soundData.Type)
            {
                case (int)SOUND_TYPE.SFX:
                    PlaySFXShot(soundData, loop);
                    break;
                case (int)SOUND_TYPE.VOICE:
                    PlayVoice(soundData);
                    break;
            }
        }
        else
        {
            Util.LogError($"SoundID {soundID} is not exist");
        }

    }

    public void StopSFX(int soundID)
    {
        if (_soundDataTable.TryGetValue(soundID, out SoundData soundData))
        {
            switch (soundData.Type)
            {
                case (int)SOUND_TYPE.SFX:
                    StopSFX(soundData);
                    break;
            }
        }
    }
    /// <summary>
    /// 사운드 볼륨을 설정
    /// </summary>
    /// <param name="value"></param>
    public void SetVolume()
    {
        foreach (var sfx in _sfxPool)
        {
            sfx.volume = _volume;
        }
        if (_voicePool != null) _voicePool.volume = _volume;
    }
    public void SetVolumeBgm()
    {
        if (_bgmPool != null)
        {
            _bgmPool.volume = _volumeBgm;
        }
    }
    /// <summary>
    /// 오디오 클립 가져오기
    /// </summary>
    /// <param name="soundData">사운드 데이터</param>
    /// <returns></returns>
    private AudioClip GetClip(SoundData soundData)
    {
        AudioClip clip = null;
        switch (soundData.Type)
        {
            case (int)SOUND_TYPE.SFX:
                if (_sfxs.TryGetValue(soundData.SoundName, out clip) == false)//효과음을 가져오고 만일 효과음 오디오 클립이 없다면 
                {
                    clip = Resources.Load<AudioClip>(_SFX_SOUND_PATH + soundData.SoundName);//효과음 로드
                    if (clip == null)
                        Util.LogError($"SoundManager ==> GetSfxClip Error==> {soundData.SoundName}");
                    else
                        _sfxs.Add(soundData.SoundName, clip);//추가된 효과음을 효과음 Pool에 Add
                }
                break;
            case (int)SOUND_TYPE.BGM:
                if (_bgms.TryGetValue(soundData.SoundName, out clip) == false)//배경음 오디오 클립이 없다면 로드
                {
                    clip = Resources.Load<AudioClip>(_BGM_SOUND_PATH + soundData.SoundName);
                    if (clip == null)
                        Util.LogError($"SoundManager ==> GetSfxClip Error==> {soundData.SoundName}");
                    else
                        _bgms.Add(soundData.SoundName, clip);
                }
                break;
            case (int)SOUND_TYPE.VOICE:
                if (_voices.TryGetValue(soundData.SoundName, out clip) == false)//목소리음 오디오 클립이 없다면 로드
                {
                    clip = Resources.Load<AudioClip>(_VOICE_SOUND_PATH + soundData.SoundName);
                    if (clip == null)
                        Util.LogError($"SoundManager ==> GetSfxClip Error==> {soundData.SoundName}");
                    else
                        _voices.Add(soundData.SoundName, clip);
                }
                break;
        }

        return clip;
    }

    private AudioClip GetTestClip(string SoundName)
    {
        AudioClip t_clip = null;
        if (_testSounds.TryGetValue(SoundName, out t_clip) == false)
        {
            t_clip = Resources.Load<AudioClip>(_TEST_SOUND_PATH + SoundName);
            if (t_clip == null)
                Util.LogError($"SoundManager ==> GetSfxTestClip Error==> {SoundName}");
            else
                _testSounds.Add(SoundName, t_clip);
        }

        return t_clip;
    }

    private AudioSource GetSFXPool(SoundData soundData)
    {
        var sameNameAudio = _sfxPool.FirstOrDefault(v => v.clip.name == soundData.SoundName);

        if (sameNameAudio != null)
            return sameNameAudio;

        foreach (var audio in _sfxPool)
        {
            if (audio.isPlaying == false)
                return audio;
        }

        var a = gameObject.AddComponent<AudioSource>();
        a.loop = false;
        a.volume = _volume;
        _sfxPool.Add(a);
        return a;
    }

    private void PlayBGM(SoundData soundData)
    {
        if (_onbgm)
        {
            if (_bgmPool == null)
            {
                _bgmPool = gameObject.AddComponent<AudioSource>();
                _bgmPool.loop = true;
                float volume = BgmVolume;
                _bgmPool.volume = volume;
            }
            if (_bgmPool.clip != null && _bgmPool.clip.name == soundData.SoundName && _bgmPool.isPlaying == true)
                return;

            _bgmPool.clip = GetClip(soundData);
            _bgmPool.Play();
        }
    }

    private void PlayVoice(SoundData soundData)
    {
        if (_onsfx)
        {
            var clip = GetClip(soundData);

            if (clip == null)
                return;

            if (_voicePool == null)
            {
                _voicePool = gameObject.AddComponent<AudioSource>();
                _voicePool.loop = false;
                _voicePool.volume = _volume;
            }
            StopVoice();

            _voicePool.clip = clip;
            _voicePool.Play();
        }

    }
    public void StopVoice()
    {
        _voicePool?.Stop();
    }

    public void PlaySFXShot(SoundData soundData, bool loop)
    {
        if (_onsfx)
        {
            var clip = GetClip(soundData);

            if (clip == null)
                return;

            var source = GetSFXPool(soundData);
            source.clip = clip;
            source.loop = loop;
            source.PlayOneShot(clip);
        }
    }

    private void PlaySFX(SoundData soundData, bool loop)
    {
        if (_onsfx)
        {
            var clip = GetClip(soundData);

            if (clip == null)
                return;

            var source = GetSFXPool(soundData);
            source.clip = clip;
            source.loop = loop;
            source.Play();
        }
    }

    private void StopSFX(SoundData soundData)
    {
        if (_sfxs.ContainsKey(soundData.SoundName) == false)
            return;

        foreach (var sound in _sfxPool)
        {
            if (sound.clip == null)
                continue;

            if (sound.isPlaying == true && sound.clip.name == _sfxs[soundData.SoundName].name)
                sound.Stop();
        }
    }
    public void StopSFX()
    {
        foreach (var sound in _sfxPool)
        {
            sound.Stop();
        }
    }

    public void TestPlaySound(string testSoundName, float volume, bool isLoop, float startPlayPoint = 0.0f)
    {
        var t_clip = GetTestClip(testSoundName);

        AudioSource _source = null;

        for (int i = 0; i < _testSoundPool.Count; i++)
        {
            if (!_testSoundPool[i].isPlaying)
            {
                _source = _testSoundPool[i];
                break;
            }
        }

        if (_source == null)
        {
            _source = gameObject.AddComponent<AudioSource>();
            _testSoundPool.Add(_source);
        }

        _source.clip = t_clip;
        _source.volume = volume;
        _source.loop = isLoop;
        _source.time = startPlayPoint;
        _source.Play();
    }

    public void StopTestSound(string testSoundName = null)
    {
        if (testSoundName == null)
        {
            foreach (var sound in _testSoundPool)
            {
                sound.Stop();
            }
        }
        else
        {
            if (_testSounds.ContainsKey(testSoundName) == false)
                return;
            else
            {
                foreach (var sound in _testSoundPool)
                {
                    if (sound.clip == null)
                        continue;

                    if (sound.isPlaying == true && sound.clip.name == _testSounds[testSoundName].name)
                        sound.Stop();
                }
            }
        }

    }

    public override void UnInit()
    {
        base.UnInit();
    }
}
