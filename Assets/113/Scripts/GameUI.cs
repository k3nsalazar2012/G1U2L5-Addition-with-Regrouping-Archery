// c# / unity class
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

// mathtek class
using MathTek.Utils;
using MathTek.Generics;

// third party class
using DG.Tweening;

namespace TFI.MathTek.ID113
{
    [RequireComponent(typeof(GameType))]
    public class GameUI : MonoBehaviour
    {
        #region Editor Variables
        [Header("For Development (active only on Editor, will be disregarded on build)")]
        [Space]
        [Tooltip("Bypass Level Selection")]
        [SerializeField] private bool _bypassLevel = false;
        [Tooltip("Target Level (NOTE: will work only if bypassLevel is true)")]
        [SerializeField] private LevelSelection _levelSelect = LevelSelection.Level1;
        public enum LevelSelection { Level1, Level2, Level3 }

        [Header("HUD")]
        [Space]
        [Tooltip("Question Text on UI")]
        [SerializeField] private Text _questionText = null;
        [Tooltip("Choices Text on UI")]
        [SerializeField] private Text[] _choicesText = null;

        [Header("Game Type")]
        [Space]
        [SerializeField] private GameType _gameType = null;

        [Header("Multiple Choice")]
        [Tooltip("Is multiple choice?")]
        [SerializeField] public bool IsMultipleChoice = false;

        [Header("Typing Input")]
        [Tooltip("Is typing game?")]
        [SerializeField] public bool IsTyping = false;

        [Header("Chances (if the game is based on lives or chances)")]
        [Tooltip("Is life / chances based?")]
        [SerializeField] public bool IsLifeBased = false;

        [Header("Time (if the game is time-based)")]
        [Tooltip("Is time-based?")]
        [SerializeField] public bool IsTimeBased = false;

        [Header("Scoring")]
        [Space]
        [Tooltip("Score Text on UI")]
        [SerializeField] private Text _scoreText = null;
        [Tooltip("Add score text")]
        [SerializeField] private Text _addScoreText = null;

        [Header("Animation and Presentation")]
        [Space]
        [Tooltip("Animation ease type")]
        [SerializeField] Ease _ease = Ease.InOutBounce;
        [Tooltip("Animation duration")]
        [SerializeField] private float _animationDuration = 0.5f;
        #endregion

        /// <summary>
        /// Initialize sound manager and game events
        /// </summary>
        private void Awake()
        {
            GameManager.SoundManager = FindObjectOfType<SoundManager>();
        }
      
        /// <summary>
        /// Initialize Game UI states
        /// </summary>
        IEnumerator Start()
        {
            // for development, if bypassLevel is checked from the inspector
            #if UNITY_EDITOR
            if (_bypassLevel)
            {
                GameManager.Initialize();

                switch (_levelSelect)
                {
                    case LevelSelection.Level1: GameManager.CurrentLevel = GameConstants.LEVEL_1_KEY; break;
                    case LevelSelection.Level2: GameManager.CurrentLevel = GameConstants.LEVEL_2_KEY; break;
                    case LevelSelection.Level3: GameManager.CurrentLevel = GameConstants.LEVEL_3_KEY; break;
                }
                GameManager.LoadLevelData();
            }
            #endif

            ItemsIndicator.TotalItems = GameManager.TotalItems;
            Settings.Instructions = GameManager.Instructions;

            _gameType.InitializeGameType();

            yield return new WaitForSeconds(0.5f);

            GameManager.NextItem();
            GameEvents.LoadNextItem();

            #if UNITY_EDITOR
            if (_bypassLevel)
                GameEvents.SceneLoaded();
            #endif
        }



        /// <summary>
        /// Update texts for next item
        /// </summary>
        private void NextItem()
        {
            _gameType.UpdateStatus();

            string[] question = GameManager.Question.Split(',');
            _questionText.text = question[0] + "\n" + question[1];

            for (int i = 0; i < GameManager.Choices.Count; i++)
            {
                _choicesText[i].text = GameManager.Choices[i];
            }
        }

        #region Scoring and Item Feedback
        /// <summary>
        /// Update score and UI details when correct
        /// </summary>
        public void Correct()
        {
            GameManager.Score++;
            _scoreText.transform.DOScale(Vector3.one * 1.2f, _animationDuration);
            _addScoreText.DOFade(1f, _animationDuration);
            _addScoreText.rectTransform.DOAnchorPosY(0f, _animationDuration).OnComplete(()=>OnUpdateHUDScore());
            GameManager.SoundManager.SfxCorrect();
        }

        /// <summary>
        /// Play just play wrong sfx
        /// </summary>
        public void Wrong()
        {
            GameManager.SoundManager.SfxWrong();
        }

        /// <summary>
        /// Animation for updated HUD score
        /// </summary>
        private void OnUpdateHUDScore()
        {
            _scoreText.transform.DOScale(Vector3.one, _animationDuration);
            _scoreText.text = GameManager.Score.ToString();
            _addScoreText.DOFade(0f, 0f);
            _addScoreText.rectTransform.DOAnchorPosY(-50f, 0f).SetEase(_ease);
        }
        #endregion

        #region Events Detection
        /// <summary>
        /// Subscribe to game events
        /// </summary>
        private void OnEnable()
        {
            GameEvents.OnNextItem += NextItem;
            GameEvents.OnCorrect += Correct;
            GameEvents.OnWrong += Wrong;
        }

        /// <summary>
        /// Unsubscribe to game events
        /// </summary>
        private void OnDisable()
        {
            GameEvents.OnNextItem -= NextItem;
            GameEvents.OnCorrect -= Correct;
            GameEvents.OnWrong -= Wrong;
        }
        #endregion
    }
}