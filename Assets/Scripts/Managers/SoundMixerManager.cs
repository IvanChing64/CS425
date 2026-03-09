using UnityEngine;
using UnityEngine.Audio;


//Author: Ivan Ching
//Developed using guides from Youtube: Sasquatch B Studios
//Usage: Sets the float on mulitple sliders on the scene change the volume levels of master, music, and sound effects.
//The mathf level is used since unity's sound mixer calculates the volume levels based on logritimic.
public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    public void SetMasterVolume(float level)
    {
        audioMixer.SetFloat("masterVolume", Mathf.Log10(level)* 20f);
        //audioMixer.SetFloat("masterVolume", level);
    }
    public void SetSoundFXVolume(float level)
    {
        audioMixer.SetFloat("soundFXVolume", Mathf.Log10(level) * 20f);
        //audioMixer.SetFloat("soundFXVolume", level);
    }
    public void SetMusicVolume(float level)
    {
        audioMixer.SetFloat("musicVolume", Mathf.Log10(level) * 20f);
        //audioMixer.SetFloat("musicVolume", level);
    }
}
