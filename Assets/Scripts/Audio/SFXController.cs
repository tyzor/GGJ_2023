using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

namespace GGJ.Audio
{
    public enum SFX
    {
        NONE = 0,
        TAKE_DAMAGE = 1,
        PLAYER_CHARGING = 2,
        PLAYER_ATTACK = 3,
        PLAYER_ATTACK_CHARGED = 4,
        GAME_OVER = 5,
        ENEMY_SHOOT = 6,
        ENEMY_DEATH = 7,
        BACKGROUND_CLIP1 = 8,
        BACKGROUND_CLIP2 = 9,
        BACKGROUND_CLIP3 = 10,
        START_UP = 11

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
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            
            _instance = this;
            
            _sfxDatas = new Dictionary<SFX, SFXData>();
            foreach (var sfxData in sfx)
            {
                _sfxDatas.Add(sfxData.type, sfxData);
            }
        }

        private void Start()
        {

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
        
        public static void SetVolume(float volume)
        {
            _instance.audioMixer.SetFloat("Volume", volume);
        }
    }
}