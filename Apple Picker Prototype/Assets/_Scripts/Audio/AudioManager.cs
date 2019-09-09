using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The audio manager
/// </summary>
public class AudioManager {

    static bool initialized = false;
    static AudioSource audioSource;
    static Dictionary<AudioClipName, AudioClip> audioClips =
        new Dictionary<AudioClipName, AudioClip>();

    /// <summary>
    /// Gets whether or not the audio manager has been initialized
    /// </summary>
    public static bool Initialized
    {
        get { return initialized; }
    }

    /// <summary>
    /// Initializes the audio manager
    /// </summary>
    /// <param name="source">audio source</param>
    public static void Initialize(AudioSource source)
    {
        initialized = true;
        audioSource = source;
        audioClips.Add(AudioClipName.Catch,
            Resources.Load<AudioClip>("Catch"));
        audioClips.Add(AudioClipName.Drop,
            Resources.Load<AudioClip>("Drop"));
        audioClips.Add(AudioClipName.Reset,
            Resources.Load<AudioClip>("Reset"));
        audioClips.Add(AudioClipName.NewGame,
            Resources.Load<AudioClip>("NewGame"));
        audioClips.Add(AudioClipName.Aerosol,
            Resources.Load<AudioClip>("Aerosol"));

    }

    /// <summary>
    /// Plays the audio clip with the given name
    /// </summary>
    /// <param name="name">name of the audio clip to play</param>
    public static void Play(AudioClipName name)
    {
        float volumescale;
        if (name == AudioClipName.Catch)
            volumescale = 0.5f;
        else
            volumescale = 1;
        audioSource.PlayOneShot(audioClips[name], volumescale);
    }
}