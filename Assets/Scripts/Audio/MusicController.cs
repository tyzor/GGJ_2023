using UnityEngine;
using UnityEngine.Audio;

namespace GGJ.Audio
{
    public class MusicController : MonoBehaviour
    {
        [SerializeField]
        private AudioSource audioSource;

        [SerializeField]
        private AudioMixer audioMixer;
    }
}