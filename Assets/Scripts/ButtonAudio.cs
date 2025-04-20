using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAudio : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip buttonElAudio;
    public AudioClip buttonMechAudio;

    public void playButtonElAudio()
    {
        audioSource.PlayOneShot(buttonElAudio);
    }
    public void playButtonMechAudio()
    {
        audioSource.PlayOneShot(buttonMechAudio);
    }
}
