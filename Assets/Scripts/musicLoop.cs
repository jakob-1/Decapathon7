using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class musicLoop : MonoBehaviour
{

    public AudioClip MusicStart;
    public AudioClip MusicLoop;
    public bool MusicOn = true;
    public bool StartPlayed = false;

    private AudioSource speaker;
    // Start is called before the first frame update
    void Start()
    {
        speaker = GetComponent<AudioSource>();
        speaker.clip = MusicStart;
        speaker.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (!StartPlayed)
        {
            if (!speaker.isPlaying)
            {
                speaker.clip = MusicLoop;
                speaker.loop = true;
                speaker.Play();
                StartPlayed = true;
            }
        }
    }

    public void changeVolume(float volume)
    {
        speaker.volume = volume;
    }
}
