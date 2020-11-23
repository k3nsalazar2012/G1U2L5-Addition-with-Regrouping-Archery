// c# / unity class
using UnityEngine;
using System.Collections;

// mathtek class
using MathTek.Generics;

namespace TFI.MathTek.ID113
{
    /// <summary>
    /// All game logic goes here
    /// Major difference is that it will be based on events
    /// To invoke next item, call GameEvents.LoadNextItem() event.
    /// To show correct feedback, call GameEvents.ShowCorrect() event.
    /// To show wrong feedback, call GameEvents.ShowWrong() event.
    /// To invoke end game, call GameEvents.EndGame() event.
    /// </summary>
    public class Game : MonoBehaviour
    {
        private bool _allowInput = false;

        private void Start()
        {
            
        }

        private void Update()
        {
            
        }

        /// <summary>
        /// Allow input when starting next item
        /// </summary>
        private void AllowInput()
        {
            _allowInput = true;
        }

        /// <summary>
        /// Preventing input when checking
        /// </summary>
        private void PreventInput()
        {
            _allowInput = false;
        }

        /// <summary>
        /// Answer checking
        /// </summary>
        public void CheckAnswer()
        {
            if (!_allowInput) return;
            PreventInput();

            if (GameManager.Answer == GameManager.Correct)
            {
                GameEvents.ShowCorrect();

                Invoke("LoadNextItem", 1.5f);
            }
            else
            {
                if (GameManager.IsLifeBased)
                    GameEvents.LifeLost();
                else
                {
                    GameEvents.ShowWrong();
                    Invoke("LoadNextItem", 1.5f);
                }
            }
        }

        #region Events Detection
        /// <summary>
        /// Subscribe to game events
        /// </summary>
        private void OnEnable()
        {
            GameEvents.OnNextItem += AllowInput;
            GameEvents.OnPauseGame += PreventInput;
            GameEvents.OnResumeGame += AllowInput;
            GameEvents.OnChecking += CheckAnswer;
            GameEvents.OnEndGame += ScoreCatch;
            GameEvents.OnAllowInput += AllowInput;
            GameEvents.OnTimeEnd += PreventInput;
        }

        /// <summary>
        /// Unsubscribe to game events
        /// </summary>
        private void OnDisable()
        {
            GameEvents.OnNextItem -= AllowInput;
            GameEvents.OnPauseGame -= PreventInput;
            GameEvents.OnResumeGame -= AllowInput;
            GameEvents.OnChecking -= CheckAnswer;
            GameEvents.OnEndGame -= ScoreCatch;
            GameEvents.OnAllowInput -= AllowInput;
            GameEvents.OnTimeEnd -= PreventInput;
        }
        #endregion

        /// <summary>
        /// Invoke next item
        /// </summary>
        private void LoadNextItem()
        {
            GameManager.NextItem();
            GameEvents.LoadNextItem();
        }

        /// <summary>
        /// Invoke score catching
        /// </summary>
        private void ScoreCatch()
        {
            _allowInput = false;

            if (GameManager.IsNextLevelUnlocked() && GameManager.LevelReached < 3)
                GameManager.LevelReached++;

            #if UNITY_EDITOR || UNITY_STANDALONE_WIN
                PlayerPrefs.SetInt("Levels", GameManager.LevelReached);
            #endif

            #if UNITY_WEBGL && !UNITY_EDITOR
                StartCoroutine(MTManager.UpdateScore(GameManager.Score, GameManager.CurrentLevel, GameManager.LevelReached));
            #endif

        }

        public bool IsReady => _allowInput;
    }
}