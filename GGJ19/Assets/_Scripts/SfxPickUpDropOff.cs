using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxPickUpDropOff : MonoBehaviour
{
    [SerializeField] SFXClips sfx;
    private SfxManager sfxManager;
    private AudioSource audioPlayer;

    private void Start()
    {
        sfxManager = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SfxManager>();
        audioPlayer = GetComponent<AudioSource>();
    }

    public void PlayPickUpSFX()
    {
        audioPlayer.clip = sfxManager.GetPickUpDropOffAudioClip(sfx, true);
        audioPlayer.Play();
    }

    public void PlayDropOffSFX()
    {
        audioPlayer.clip = sfxManager.GetPickUpDropOffAudioClip(sfx, false);
        audioPlayer.Play();
    }

}
