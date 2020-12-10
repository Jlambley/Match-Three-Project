using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoundManager : MonoBehaviour
{

    public AudioSource[] destroyNoises;
    public AudioSource backgroundMusic;

    public void PlayRandomDestroyNoise()
    {
        if(PlayerPrefs.HasKey("Sound"))
        {

            if (PlayerPrefs.GetInt("Sound") == 1) 
                //Only plays sound if we have sound enabled (key is at 1)
            {
                //Choose a random number
                int clipToPlay = Random.Range(0, destroyNoises.Length);
                //Play that clip
                destroyNoises[clipToPlay].Play();
            }


        }
        else //if they have no key, then enable sound by default
        {
            //Choose a random number
            int clipToPlay = Random.Range(0, destroyNoises.Length);
            //Play that clip
            destroyNoises[clipToPlay].Play();
        }
    }

    public void adjustMusicVolume()
    {
        if (PlayerPrefs.HasKey("Music"))
        {
            if (PlayerPrefs.GetInt("Music") == 0)
            {
                MuteMusic();
            }
            if (PlayerPrefs.GetInt("Music") == 1)
            {
                UnmuteMusic();
            }
        }
    }

    public void MuteMusic()
    {
        backgroundMusic.volume = 0;
    }
    public void UnmuteMusic()
    {
        backgroundMusic.volume = 1;

    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("Music"))
        {
            if (PlayerPrefs.GetInt("Music") == 0)
            {
                backgroundMusic.Play();
                backgroundMusic.volume = 0;
            }
            else
            {
                backgroundMusic.Play();
                backgroundMusic.volume = 1;
            }

        }
        else
        {
            backgroundMusic.Play();
            backgroundMusic.volume = 1;
        }
    }


}
