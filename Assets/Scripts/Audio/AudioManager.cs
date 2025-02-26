using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace CamKnight
{
    public class AudioManager : MonoBehaviour
    {
        private static AudioManager instance;

        [Header("Audio files")]
        public Sound[] music;
        [SerializeField]
        private Sound[] sfx;

        [Header("Audio Sources")]
        [SerializeField]
        private AudioSource musicSource;
        [SerializeField]
        private AudioSource sfxSource;

        public static AudioManager Instance { get => instance; private set => instance = value; }

        private void Awake()
        {
            // Singleton.
            if(instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if(instance != this) Destroy(gameObject);

        }

        /// <summary>
        /// Play a music clip by using its name parameter.
        /// </summary>
        /// <param name="name"></param>
        public void PlayMusic(string name)
        {
            Sound sound = Array.Find(music, x => x.Name == name);

            if (sound == null)
            {
                Debug.LogError("Sound not found");
                return;
            }

            else
            {
                musicSource.clip = sound.Clip;
                musicSource.Play();
            }
        }

        /// <summary>
        /// Stops the current music clip.
        /// </summary>
        public void StopMusic()
        {
            musicSource.Stop();
        }

        /// <summary>
        /// Play an SFX clip by using its name parameter.
        /// </summary>
        /// <param name="name"></param>
        public void PlaySFX(string name, bool oneShot)
        {
            Sound sound = Array.Find(sfx, x => x.Name == name);

            if (sound == null)
            {
                Debug.LogError("Sound not found");
                return;
            }

            else
            {
                sfxSource.clip = sound.Clip;

                if(!oneShot) sfxSource.Play();
                else sfxSource.PlayOneShot(sound.Clip);
            }
        }
    }
}
