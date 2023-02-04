using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

namespace GGJ.Audio
{
    public enum SFX
    {
        NONE,
        TEMPLATE_SOUND_1,
        TEMPLATE_SOUND_2
    }

    public class SFXController : MonoBehaviour
    {
        //============================================================================================================//

        [Serializable]
        public struct SFXData
        {
            public string name;
            public SFX type;
            public AudioClip clip;
        }

        //============================================================================================================//

        private static SFXController _instance;
        private Dictionary<SFX, SFXData> _sfxDatas;

        public static void PlaySound(SFX sfx, float volume = 1f)
        {
            _instance.TryPlaySound(sfx, volume);
        }

        //============================================================================================================//

        [SerializeField]
        private AudioSource audioSource;

        [SerializeField]
        private AudioMixer audioMixer;

        [SerializeField]
        private SFXData[] sfx;

        //Unity Functions
        //============================================================================================================//

        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            _sfxDatas = new Dictionary<SFX, SFXData>();
            foreach (var sfxData in sfx)
            {
                _sfxDatas.Add(sfxData.type, sfxData);
            }
        }

        //============================================================================================================//

        private void TryPlaySound(SFX sfx, float volume = 1f)
        {
            if (sfx == SFX.NONE) { return; }

            // get specified sound
            SFXData data = _sfxDatas[sfx];

            // play sound
            audioSource?.PlayOneShot(data.clip, volume);
        }
    }
}