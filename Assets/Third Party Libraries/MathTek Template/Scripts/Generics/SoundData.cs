using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MathTek.Generics
{
    /// <summary>
    /// SoundData Scriptable Object
    /// Holds information about all audio clips needed in the game
    /// </summary>
    [CreateAssetMenu(fileName = "SoundData", menuName = "MathTek/SoundData", order = 1)]
    public class SoundData : ScriptableObject
    {
        [Header("BGM")]
        [Tooltip("Title BGM")]
        public AudioClip bgmTitle;
        [Tooltip("Game BGM")]
        public AudioClip bgmGame;

        [Header("SFX")]
        [Tooltip("Button Hover SFX")]
        public AudioClip sfxButtonHover;
        [Tooltip("Button Click SFX")]
        public AudioClip sfxButtonClick;
        [Tooltip("Correct SFX")]
        public AudioClip sfxCorrect;
        [Tooltip("Wrong SFX")]
        public AudioClip sfxWrong;
        [Tooltip("Time's Up SFX")]
        public AudioClip sfxTimesUp;
        [Tooltip("Scoring SFX")]
        public AudioClip sfxScoring;
        [Tooltip("Star SFX")]
        public AudioClip sfxStar;

        [Header("Custom SFX (add new sfx here)")]
        public List<CustomAudioSoundEffects> customSfx;
    }

    /// <summary>
    /// Custom Sound Effects Class
    /// </summary>
    [Serializable]
    public class CustomAudioSoundEffects
    {
        [Tooltip("Name of the clip")]
        public string name;
        [Tooltip("Target audioclip")]
        public AudioClip sfx;
    }
}