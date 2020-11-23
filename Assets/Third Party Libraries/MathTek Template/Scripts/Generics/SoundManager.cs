// unity class
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace MathTek.Generics
{
    public class SoundManager : MonoBehaviour
    {
        [Tooltip("Sound Data, holds information about all audioclips needed in the game")]
        [SerializeField] SoundData soundData = null;

        [Header("Audio Sources")]
        [Tooltip("audio source for bgm")]
        [SerializeField] AudioSource bgm = null;
        [Tooltip("audio source for sfx")]
        [SerializeField] AudioSource sfx = null;

        /// <summary>
        /// Update Audio When switching scene
        /// </summary>
        private void OnEnable()
        {
            var buttons = FindObjectsOfType<Button>();
            foreach (Button b in buttons)
            {
                AddListenerOnEventTrigger(b.gameObject.AddComponent<EventTrigger>());
            }

            var toggles = FindObjectsOfType<Toggle>();
            foreach (Toggle t in toggles)
            {
                AddListenerOnEventTrigger(t.gameObject.AddComponent<EventTrigger>());
            }

            if (SceneManager.GetActiveScene().name == "Title")
            { 
                bgm.clip = soundData.bgmTitle;
                bgm.loop = false;
            }
            else if (SceneManager.GetActiveScene().name == "Game")
            {
                bgm.clip = soundData.bgmGame;
                bgm.loop = true;
            }
            bgm.Play();
        }

        /// <summary>
        /// Add event on buttons and toggles to play audio when hovering and clicking
        /// </summary>
        /// <param name="trigger"></param>
        private void AddListenerOnEventTrigger(EventTrigger trigger)
        {
            trigger.AddListener(EventTriggerType.PointerEnter, OnPointerEnter);
            trigger.AddListener(EventTriggerType.PointerDown, OnPointerDown);
        }

        /// <summary>
        /// Hover event
        /// </summary>
        /// <param name="eventData"></param>
        private void OnPointerEnter(PointerEventData eventData)
        {
            var selectable = eventData.pointerEnter.GetComponent<Selectable>();
            if (selectable != null && selectable.interactable)
            {
                sfx.clip = soundData.sfxButtonHover;
                sfx.Play();
            }
        }

        /// <summary>
        /// Click event
        /// </summary>
        /// <param name="eventData"></param>
        private void OnPointerDown(PointerEventData eventData)
        {
            var selectable = eventData.pointerCurrentRaycast.gameObject.GetComponent<Selectable>();
            if (selectable != null && selectable.interactable)
            {
                sfx.clip = soundData.sfxButtonClick;
                sfx.Play();
            }
        }

        /// <summary>
        /// Play custom sfx added in the SoundData
        /// </summary>
        /// <param name="name">name of the clip</param>
        public void PlayCustomSFX(string name)
        {
            AudioClip clip = null;
            foreach (CustomAudioSoundEffects customSfx in soundData.customSfx)
            {
                if (customSfx.name == name)
                {
                    clip = customSfx.sfx;
                    break;
                }
            }

            if (clip != null)
            {
                sfx.clip = clip;
                sfx.Play();
            }
        }

        /// <summary>
        /// Play Correct SFX
        /// </summary>
        public void SfxCorrect()
        {
            sfx.clip = soundData.sfxCorrect;
            sfx.Play();
        }

        /// <summary>
        /// Play Wrong SFX
        /// </summary>
        public void SfxWrong()
        {
            sfx.clip = soundData.sfxWrong;
            sfx.Play();
        }

        /// <summary>
        /// Play Time's Up SFX
        /// </summary>
        public void SfxTimesUp()
        {
            sfx.clip = soundData.sfxTimesUp;
            sfx.Play();
        }

        /// <summary>
        /// Play Star SFX
        /// </summary>
        public void SfxStar()
        {
            sfx.loop = false;
            sfx.clip = soundData.sfxStar;
            sfx.Play();
        }

        /// <summary>
        /// Play Scoring SFX
        /// </summary>
        public void SfxScoring()
        {
            sfx.loop = true;
            sfx.clip = soundData.sfxScoring;
            sfx.Play();
        }
    }
}