using UnityEngine;

public class DannyScream : MonoBehaviour
{

    public AudioClip SoundToPlay;
    //public float Volume;
    public AudioSource audio;
    public bool alreadyPlayed = false;


    void Start()
    {
        
    }

    void OnTriggerEnter()
    {
        if (!alreadyPlayed)
        {
            audio.PlayOneShot(SoundToPlay);
            alreadyPlayed = true;
        }
    }
}