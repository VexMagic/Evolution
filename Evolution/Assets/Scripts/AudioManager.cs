using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource pitchSource;

    [SerializeField] private float pitchShiftAmount;
    [SerializeField] private List<AudioClip> music = new List<AudioClip>();

    [SerializeField] private List<SoundEffectData> effectsData = new List<SoundEffectData>();

    private Dictionary<string, SoundEffectData> effectsMap = new Dictionary<string, SoundEffectData>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            CreateEffectsMap();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        CreateEffectsMap();
    }

    private void CreateEffectsMap()
    {
        effectsMap.Clear();
        foreach (var effect in effectsData)
        {
            effectsMap.Add(effect.code, effect);
        }
    }

    public void PlayMusic(int index)
    {
        AudioClip sound = music[index % music.Count];

        if (musicSource.clip == sound)
            return;

        musicSource.clip = sound;
        musicSource.Play();
    }

    public void PlaySFX(string code)
    {
        if (effectsMap[code].pitchShift)
        {
            float pitchShift = Random.Range(-pitchShiftAmount, pitchShiftAmount);
            pitchSource.pitch = 1 + pitchShift;
            pitchSource.PlayOneShot(effectsMap[code].clip);
        }
        else
        {
            sfxSource.PlayOneShot(effectsMap[code].clip);
        }
    }
}
