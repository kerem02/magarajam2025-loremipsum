using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [Header("------------ Main Music Settings ------------")]
    public AudioSource mainMusicSource;
    public AudioClip mainMusic;
    [Header("------------ Effects ------------")]

    public static AudioManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetMainMusicVolume(float value)
    {
        mainMusicSource.volume = value;
    }
    
    
}
