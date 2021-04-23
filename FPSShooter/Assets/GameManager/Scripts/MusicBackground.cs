using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MusicBackground : MonoBehaviour
{
    public List<AudioClip> clips;
    public AudioSource source;

    private State state;
    private float elapsedTimeInClip;

    private void Awake()
    {
        if (source == null) { 
            source = GetComponent<AudioSource>();
        }
        state = State.Stopped;
    }

    private void Update()
    {
        if (state == State.Playing && !source.isPlaying)
        {
            PlayRandomClip();
        }
    }

    public void Play()
    {
        if (state == State.Paused)
        {
            source.UnPause();
        }
        else if (state == State.Stopped)
        {
            PlayRandomClip();
        }
    }

    public void Pause()
    {
        state = State.Paused;
        source.Pause();
    }

    public void Stop()
    {
        state = State.Stopped;
        source.Stop();
    }

    private void PlayRandomClip()
    {
        source.clip = GetRandomClip();
        source.Play();
        state = State.Playing;
        elapsedTimeInClip = 0;
    }

    private AudioClip GetRandomClip()
    {
        return clips[Random.Range(0, clips.Count)];
    }

    private enum State
    {
        Playing, Paused, Stopped
    }
}
