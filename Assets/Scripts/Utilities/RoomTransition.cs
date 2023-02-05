using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GGJ.Utilities
{
    public class RoomTransition : MonoBehaviour
    {
        private static RoomTransition _instance;
        
        public Image fadeImage;
        private bool _fading;

        private Color32 _transparent = new Color32();
        private Color32 _black = new Color32(0,0,0,255);

        //============================================================================================================//
        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            
            DontDestroyOnLoad(gameObject);
            _instance = this;
        }

        //============================================================================================================//
        public static void FadeScreen(float fadeTime, Action onFaded)
        {
            _instance.TryFade(fadeTime, onFaded);
        }
        
        private void TryFade(float fadeTime, Action onFaded)
        {
            if (_fading)
                return;

            StartCoroutine(FadeScreenCoroutine(fadeTime, onFaded));
        }

        private IEnumerator FadeScreenCoroutine(float fadeTime, Action onFaded)
        {
            _fading = true;
            
            float halfTime = fadeTime / 2f;
            fadeImage.color = _transparent;
            fadeImage.enabled = true;

            for (float t = 0; t < halfTime; t+=Time.deltaTime)
            {
                fadeImage.color = Color32.Lerp(_transparent, _black, t / halfTime);
                yield return null;
            }
            fadeImage.color = _black;
            onFaded?.Invoke();
            
            for (float t = 0; t < halfTime; t+=Time.deltaTime)
            {
                fadeImage.color = Color32.Lerp(_black, _transparent, t / halfTime);
                yield return null;
            }
            
            fadeImage.color = _transparent;
            fadeImage.enabled = false;
            _fading = false;
        }
    }
}