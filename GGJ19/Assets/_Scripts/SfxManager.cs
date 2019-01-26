using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SFXClips
{
    Pot_Plant,
    Trophy,
    Rubiks_Cube,
    Toy_Car,
    Mug,
    Laptop_Press,
    Footstep
}


public class SfxManager : MonoBehaviour
{

    [SerializeField] List<AudioClip> prefabClips = new List<AudioClip>();
    static public Dictionary<SFXClips, List<AudioClip>> clips = new Dictionary<SFXClips, List<AudioClip>>();

    void Start()
    {
        int currentIndex = 0;
        for (int i = 0; i < 6; i++)
        {
            clips.Add((SFXClips)i, prefabClips.GetRange(currentIndex, 2));
            currentIndex += 2;
        }

        clips.Add(SFXClips.Footstep, prefabClips.GetRange(currentIndex, (prefabClips.Count - currentIndex) - 1));

    }

    public  AudioClip GetPickUpDropOffAudioClip(SFXClips SFX, bool pickUp)
    {
        List<AudioClip> returnClip = null;
        clips.TryGetValue(SFX, out returnClip);
        if (pickUp)
        {
            return returnClip[0];
        }
        else
        {
            return returnClip[1];
        }
    }

    public  AudioClip GetRandomFootStep()
    {
        List<AudioClip> returnClip = null;
        clips.TryGetValue(SFXClips.Footstep, out returnClip);
        return returnClip[Random.Range(0, returnClip.Count - 1)];
    }


}
