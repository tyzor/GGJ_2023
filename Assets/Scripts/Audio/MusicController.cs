using System;
using UnityEngine;
using UnityEngine.Audio;

namespace GGJ.Audio
{
    public class MusicController : MonoBehaviour
    {
        private static MusicController _instance;
        
        
        [SerializeField]
        private AudioSource audioSource;

        [SerializeField]
        private AudioMixer audioMixer;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            
            _instance = this;
        }

        //============================================================================================================//

        public static void SetVolume(float volume)
        {
            _instance.audioMixer.SetFloat("Volume", volume);
        }
    }
}