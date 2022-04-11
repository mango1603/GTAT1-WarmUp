using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public AudioSource EffectsSoundSource;
    public AudioSource EffectsMusicSource;
    public static SoundManager Instance = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public void PlayMusic(AudioClip clip, bool loop)
    {
        EffectsMusicSource.clip = clip;
        EffectsMusicSource.loop = loop;
        EffectsMusicSource.Play();
    }

    public void PlaySoundOnce(AudioClip clip)
    {
        EffectsSoundSource.PlayOneShot(clip);
    }
}