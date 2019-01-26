using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxFootsteps : MonoBehaviour {

    private SfxManager sfxManager;
    private AudioSource audioPlayer;

    private void Start()
    {
        sfxManager = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SfxManager>();
        audioPlayer = GetComponent<AudioSource>();
    }

    public void PlayFootStep()
    {
        audioPlayer.clip = sfxManager.GetRandomFootStep();
        audioPlayer.Play();
    }
}
