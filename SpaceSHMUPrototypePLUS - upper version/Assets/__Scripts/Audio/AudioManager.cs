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
        audioClips.Add(AudioClipName.ButtonClick,
            Resources.Load<AudioClip>("button_click"));
        audioClips.Add(AudioClipName.Blaster,
            Resources.Load<AudioClip>("blaster"));
        audioClips.Add(AudioClipName.Gun,
            Resources.Load<AudioClip>("gun"));
        audioClips.Add(AudioClipName.Laser,
            Resources.Load<AudioClip>("laser"));
        audioClips.Add(AudioClipName.Missile,
            Resources.Load<AudioClip>("missile"));
        audioClips.Add(AudioClipName.Phaser,
            Resources.Load<AudioClip>("phaser"));
        audioClips.Add(AudioClipName.Spread,
            Resources.Load<AudioClip>("spread"));
        audioClips.Add(AudioClipName.Boom,
            Resources.Load<AudioClip>("boom"));
        audioClips.Add(AudioClipName.Health,
            Resources.Load<AudioClip>("health"));
        audioClips.Add(AudioClipName.Ouch,
            Resources.Load<AudioClip>("ouch"));
        audioClips.Add(AudioClipName.GameOver,
            Resources.Load<AudioClip>("game_over"));
        audioClips.Add(AudioClipName.Restart,
            Resources.Load<AudioClip>("restart"));
        audioClips.Add(AudioClipName.MissionCompleted,
            Resources.Load<AudioClip>("mission_completed"));

    }

    /// <summary>
    /// Plays the audio clip with the given name
    /// </summary>
    /// <param name="name">name of the audio clip to play</param>
    public static void Play(AudioClipName name)
    {
        audioSource.PlayOneShot(audioClips[name]);
        //audioSource.volume = 0.05f;
    }

    /// <summary>
    /// Stops all audio clips
    /// </summary>
    public static void Stop()
    {
        audioSource.Stop();
    }
}