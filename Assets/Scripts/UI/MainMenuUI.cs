using System.Collections;
using System.Collections.Generic;
using GGJ.Audio;
using GGJ.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GGJ.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField]
        private Button loginButton;
        [SerializeField]
        private Button settingsButton;

        [SerializeField, Header("Settings Window")]
        private GameObject settingsWindow;
        [SerializeField]
        private Slider sfxVolumeSlider;
        [SerializeField]
        private Slider musicVolumeSlider;
        
        [SerializeField]
        private Button backButton;
        [SerializeField]
        private Button closeButton;
        
        
        // Start is called before the first frame update
        private void Start()
        {

            InitButtons();
            settingsWindow.SetActive(false);
        }
        //============================================================================================================//

        private void InitButtons()
        {
            loginButton.onClick.AddListener(OnLoginPressed);
            settingsButton.onClick.AddListener(OnSettingsPressed);
            
            closeButton.onClick.AddListener(OnBackButtonPressed);
            backButton.onClick.AddListener(OnBackButtonPressed);
            
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            
            sfxVolumeSlider.value = 0f;
            musicVolumeSlider.value = 0f;
        }

        //============================================================================================================//

        private void OnLoginPressed()
        {
            RoomTransition.FadeScreen(1f, () =>
            {
                SceneManager.LoadScene(1);
            });
        }

        private void OnSettingsPressed()
        {
            settingsWindow.SetActive(true);
        }

        private void OnBackButtonPressed()
        {
            settingsWindow.SetActive(false);
        }
        //============================================================================================================//

        private void OnSFXVolumeChanged(float volume)
        {
            SFXController.SetVolume(volume);
        }

        private void OnMusicVolumeChanged(float volume)
        {
            MusicController.SetVolume(volume);
        }
        
        //============================================================================================================//
        
    }
}
