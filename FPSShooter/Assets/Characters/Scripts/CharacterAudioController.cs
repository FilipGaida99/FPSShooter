using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAudioController : MonoBehaviour
{

    public CharacterState state;

    [SerializeField]
    private AudioClip walkingSound;
    [SerializeField]
    private AudioClip runningSound;
    [SerializeField]
    private AudioClip jumpingSound;
    [SerializeField]
    private AudioClip landingSound;
    [SerializeField]
    private AudioClip attackingSound;
    [SerializeField]
    private AudioClip sufferingSound;
    [SerializeField]
    private AudioClip dyingSound;

    private AudioClip[] sounds;
    private AudioSource audioSource;

    private void Awake()
    {
        sounds = GetSoundsArray();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    virtual public void UpdateSound()
    {
        AudioClip choosedClip = sounds[(int)state];
        state = CharacterState.Idle;
        PlaySound(choosedClip);
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource.clip != clip || !audioSource.isPlaying)
        {
            if (clip != null)
            {
                audioSource.clip = clip;
                audioSource.Play();
            }
        }
    }

    private AudioClip[] GetSoundsArray()
    {
        return new AudioClip[] { null, walkingSound, runningSound, jumpingSound, landingSound, attackingSound, sufferingSound, dyingSound };
    }
}