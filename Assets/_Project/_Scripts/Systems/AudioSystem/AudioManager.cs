using System;
using RV.Util;
using UnityEngine;

namespace RV.Systems.AudioSystem
{
    public class AudioManager : PersistentSingleton<AudioManager>
    {
        [SerializeField] private Sound[] _musicSounds, _sfxSounds;
        [SerializeField] private AudioSource _musicAudioSource, _sfxAudioSource;
    
        public void PlayMusic(string name)
        {
            Sound s = Array.Find(_musicSounds, x => x.name == name);
    
            if (s == null)
            {
                Debug.Log("Sound not found");
            }
            else
            {
                _musicAudioSource.clip = s.clip;
                _musicAudioSource.Play();
            }
        }
    
        public void StopMusic(string name)
        {
            Sound s = Array.Find(_musicSounds, x => x.name == name);
    
            if (s == null)
            {
                Debug.Log("Sound not found");
            }
            else
            {
                if (s.clip == _musicAudioSource.clip)
                {
                    _musicAudioSource.Stop();
                    _musicAudioSource.clip = null;
                }
            }
        }
    
        public void PlaySfx(string name)
        {
            Sound s = Array.Find(_sfxSounds, x => x.name == name);
    
            if (s == null)
            {
                Debug.Log("Sound not found");
            }
            else
            {
                _sfxAudioSource.clip = s.clip;
                _sfxAudioSource.Play();
            }
        }
    }
}