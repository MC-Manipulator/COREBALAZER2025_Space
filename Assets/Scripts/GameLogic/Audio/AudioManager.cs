using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    //DeepSeek写的
    public static AudioManager Instance { get; private set; }

    [System.Serializable]
    public class Sound
    {
        //Sound类的name必须与clip音频文件名称一致，否则会导致音量调整报错
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
        [Range(0.1f, 3f)] public float pitch = 1f;
        public bool loop = false;
    }

    [Header("音频设置")]
    [SerializeField] private List<Sound> musicList = new List<Sound>();
    [SerializeField] private List<Sound> soundEffects = new List<Sound>();

    [Header("音效池设置")]
    [SerializeField] private int initialPoolSize = 10;
    [SerializeField] private float spatialBlend = 0f; // 0=2D, 1=3D

    private AudioSource musicSource;
    private List<AudioSource> soundPool = new List<AudioSource>();
    private Dictionary<string, Sound> soundDict = new Dictionary<string, Sound>();
    private Dictionary<string, Sound> musicDict = new Dictionary<string, Sound>();

    [Header("音量控制")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    public bool isMuted = false;

    private Coroutine musicFadeCoroutine;

    public AudioSettingBoard audioSettingBoard;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Initialize()
    {
        // 初始化音乐源
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;

        // 初始化音效池
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewSoundSource();
        }

        // 创建音频字典
        foreach (Sound s in soundEffects)
        {
            soundDict[s.name] = s;
        }
        foreach (Sound m in musicList)
        {
            musicDict[m.name] = m;
        }

        LoadVolumeSettings();
    }

    private AudioSource CreateNewSoundSource()
    {
        GameObject soundObject = new GameObject("SoundSource");
        soundObject.transform.SetParent(transform);
        AudioSource source = soundObject.AddComponent<AudioSource>();
        source.spatialBlend = spatialBlend;
        soundPool.Add(source);
        return source;
    }

    #region 音乐控制
    public void PlayMusic(string musicName, float fadeTime = 1f)
    {
        if (!musicDict.ContainsKey(musicName))
        {
            Debug.LogWarning($"音乐 {musicName} 不存在");
            return;
        }

        Sound music = musicDict[musicName];
        if (musicFadeCoroutine != null) StopCoroutine(musicFadeCoroutine);
        musicFadeCoroutine = StartCoroutine(FadeMusic(music, fadeTime));
    }

    private IEnumerator FadeMusic(Sound newMusic, float fadeTime)
    {
        float startVolume = musicSource.volume;

        // 淡出当前音乐
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeTime);
            yield return null;
        }

        // 切换音乐
        musicSource.clip = newMusic.clip;
        musicSource.volume = newMusic.volume * musicVolume * masterVolume;
        musicSource.pitch = newMusic.pitch;
        musicSource.loop = newMusic.loop;
        musicSource.Play();

        // 淡入新音乐
        float targetVolume = newMusic.volume * musicVolume * masterVolume;
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(0f, targetVolume, t / fadeTime);
            yield return null;
        }

        musicSource.volume = targetVolume;
    }

    public void PauseMusic(float fadeTime = 0.5f)
    {
        if (musicFadeCoroutine != null) StopCoroutine(musicFadeCoroutine);
        musicFadeCoroutine = StartCoroutine(FadeMusicVolume(0f, fadeTime, true));
    }

    public void ResumeMusic(float fadeTime = 0.5f)
    {
        if (musicFadeCoroutine != null) StopCoroutine(musicFadeCoroutine);
        musicFadeCoroutine = StartCoroutine(FadeMusicVolume(musicDict[musicSource.clip.name].volume, fadeTime));
    }

    private IEnumerator FadeMusicVolume(float targetVolume, float fadeTime, bool pauseAfter = false)
    {
        float startVolume = musicSource.volume;
        targetVolume *= musicVolume * masterVolume;

        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVolume, targetVolume, t / fadeTime);
            yield return null;
        }

        musicSource.volume = targetVolume;
        if (pauseAfter) musicSource.Pause();
    }
    #endregion

    #region 音效控制
    public void PlaySound(string soundName, Vector3 position = default)
    {
        if (!soundDict.ContainsKey(soundName))
        {
            Debug.LogWarning($"音效 {soundName} 不存在");
            return;
        }

        AudioSource source = GetAvailableSoundSource();
        Sound sound = soundDict[soundName];

        source.clip = sound.clip;
        source.volume = sound.volume * sfxVolume * masterVolume * (isMuted ? 0 : 1);
        source.pitch = sound.pitch;
        source.loop = sound.loop;
        source.transform.position = position;
        source.Play();
    }

    private AudioSource GetAvailableSoundSource()
    {
        foreach (AudioSource source in soundPool)
        {
            if (!source.isPlaying) return source;
        }
        return CreateNewSoundSource();
    }

    public void StopAllSounds()
    {
        foreach (AudioSource source in soundPool)
        {
            source.Stop();
        }
    }
    #endregion

    #region 音量控制
    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        UpdateAllVolumes();
        SaveVolumeSettings();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        UpdateMusicVolume();
        SaveVolumeSettings();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        UpdateSFXVolume();
        SaveVolumeSettings();
    }

    private void UpdateAllVolumes()
    {
        UpdateMusicVolume();
        UpdateSFXVolume();
    }

    private void UpdateMusicVolume()
    {
        if (musicSource != null && musicSource.clip != null)
        {
            Sound currentMusic = musicDict[musicSource.clip.name];
            musicSource.volume = currentMusic.volume * musicVolume * masterVolume * (isMuted ? 0 : 1);
        }
    }

    private void UpdateSFXVolume()
    {
        foreach (AudioSource source in soundPool)
        {
            if (source.isPlaying && soundDict.ContainsKey(source.clip.name))
            {
                Sound sound = soundDict[source.clip.name];
                source.volume = sound.volume * sfxVolume * masterVolume * (isMuted ? 0 : 1);
            }
        }
    }

    private void UpdateAudioSettingBoard()
    {
        audioSettingBoard.UpdateBoard();
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;
        UpdateAllVolumes();
        SaveVolumeSettings();
    }
    #endregion

    #region 持久化存储
    private void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.SetInt("IsMuted", isMuted ? 1 : 0);
    }

    private void LoadVolumeSettings()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        isMuted = PlayerPrefs.GetInt("IsMuted", 0) == 1;
        UpdateAllVolumes();
        UpdateAudioSettingBoard();
    }
    #endregion
}
