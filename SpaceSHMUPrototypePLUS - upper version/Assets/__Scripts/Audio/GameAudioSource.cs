using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Audio Source
/// </summary>
public class GameAudioSource : MonoBehaviour {


    private void Awake()
    {
        if (!AudioManager.Initialized)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            AudioManager.Initialize(audioSource);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
