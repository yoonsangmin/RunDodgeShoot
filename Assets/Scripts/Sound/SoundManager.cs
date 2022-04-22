using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance = null;
    public static SoundManager Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType(typeof(SoundManager)) as SoundManager;

                if (instance == null)
                    Debug.Log("SoundManager does not exist");
            }
            return instance;
        }
    }

    public enum Sound
    {
        Bgm = 0,
        Sfx,
        MaxCount,
    }

    [SerializeField] private AudioMixer mixer;
    public AudioMixer Mixer { get => mixer; }

    private AudioSource[] audioSources = new AudioSource[(int)Sound.MaxCount];
    private Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        
        instance = this;
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        Initialize();
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        Play(arg0.name, Sound.Bgm);
    }

    private void Initialize()
    {
        for (int i = 0; i < (int)Sound.MaxCount; i++)
        {
            audioSources[i] = this.gameObject.AddComponent<AudioSource>();
            audioSources[i].outputAudioMixerGroup = mixer.FindMatchingGroups(System.Enum.GetName(typeof(Sound), i))[0];
        }

        audioSources[(int)Sound.Bgm].loop = true;
    }

    public void Clear()
    {
        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.clip = null;
            audioSource.Stop();
        }

        audioClips.Clear();
    }

    public void Play(AudioClip audioClip, Sound type = Sound.Sfx, float volume = 1.0f)
    {
        if (audioClip == null)
            return;

        if (type == Sound.Bgm)
        {
            AudioSource audioSource = audioSources[(int)Sound.Bgm];
            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.clip = audioClip;
            audioSource.volume = volume;
            audioSource.Play();
        }
        else
        {
            AudioSource audioSource = audioSources[(int)Sound.Sfx];
            audioSource.PlayOneShot(audioClip, volume);
        }
    }

    public void Play(string path, Sound type = Sound.Sfx, float volume = 1.0f)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, type);
        Play(audioClip, type, volume);
    }

    private AudioClip GetOrAddAudioClip(string path, Sound type)
    {
        if (path.Contains("Sounds/") == false)
            path = $"Sounds/{path}";

        AudioClip audioClip = null;

        if (type == Sound.Bgm)
            audioClip = Resources.Load<AudioClip>(path);
        else
        {
            if (audioClips.TryGetValue(path, out audioClip) == false)
            {
                audioClip = Resources.Load<AudioClip>(path);
                audioClips.Add(path, audioClip);
            }
        }

        if (audioClip == null)
            Debug.Log(path + " AudioClip is Missing!");

        return audioClip;
    }
}
