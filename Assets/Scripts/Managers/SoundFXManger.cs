using UnityEngine;

//Author: Ivan Ching
//Developed using guides from Youtube: Sasquatch B Studios
//This manager plays audio clip from an array of audio and then quickly deletes the source. 
public class SoundFXManager: MonoBehaviour
{
    public static SoundFXManager instance;

    [SerializeField] private AudioSource soundFXObject;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public void PlaySoundFXClip(AudioClip[] audioClip, Transform spawnTransform, float volume)
    {

        //random selection
        int rand = Random.Range(0, audioClip.Length);

        //Spawns audio source
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        //Plays the randomly selected audio
        audioSource.clip = audioClip[rand];

        //Sound volume is based on sfx volume
        audioSource.volume = volume;

        //plays sound
        audioSource.Play();
        //length of sound
        float clipLength = audioSource.clip.length;
        //destroys the audio source object
        Destroy(audioSource.gameObject, clipLength);
    }
}
