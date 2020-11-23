// c# / unity class
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// mathtek class
using MathTek.Generics;
using MathTek.Utils;

// third party class
using DG.Tweening;

namespace TFI.MathTek.ID113
{
    [RequireComponent(typeof(GameUI))]
    public class GameType : MonoBehaviour
    {
        [Tooltip("Reference to the Game UI")]
        [SerializeField] private GameUI _gameUI = null;


        [Header("Multiple Choice Set Up")]
        [Space]
        [Tooltip("Level 1 Choice Count")]
        [SerializeField] private int _level1ChoiceCount = 0;
        [Tooltip("Level 2 Choice Count")]
        [SerializeField] private int _level2ChoiceCount = 0;
        [Tooltip("Level 3 Choice Count")]
        [SerializeField] private int _level3ChoiceCount = 0;
        [Tooltip("Choices parent")]
        [SerializeField] private Transform _choicesParent = null;
        [Tooltip("Choice prefab")]
        [SerializeField] private GameObject _choicePrefab = null;


        [Header("Typing Set Up")]
        [Space]
        [Tooltip("Answer input parent")]
        [SerializeField] private Transform _inputParent = null;
        [Tooltip("Answer input prefab")]
        [SerializeField] private GameObject _inputPrefab = null;


        [Header("Lives / Chances Set Up")]
        [Space]
        [Tooltip("Is life per item or for whole game")]
        [SerializeField] private bool _isChancesPerItem = false;
        [Tooltip("Number of lives")]
        [Range(3, 5)]
        [SerializeField] private int _livesCount = 3;
        [Tooltip("Lives background")]
        [SerializeField] private Image _livesBackground = null;
        [Tooltip("Life background sprites")]
        [SerializeField] private Sprite[] _livesSprites = null;
        [Tooltip("Life prefab")]
        [SerializeField] private GameObject _lifePrefab = null;


        [Header("Timer Set Up")]
        [Space]
        [Tooltip("Is time per item or for whole game")]
        [SerializeField] private bool _isTimePerItem = false;
        [Tooltip("Level 1 Time in seconds")]
        [SerializeField] private int _level1Time = 30;
        [Tooltip("Level 2 Time in seconds")]
        [SerializeField] private int _level2Time = 25;
        [Tooltip("Level 3 Time in seconds")]
        [SerializeField] private int _level3Time = 15;
        [Tooltip("Clock hand")]
        [SerializeField] private Transform _clockHand = null;
        [Tooltip("Time Text")]
        [SerializeField] private Text _timeText = null;
        [Tooltip("Timer GameObject")]
        [SerializeField] private GameObject _timer = null;


        // multile choice
        private List<Button> _choicesButton = null;
        private List<Text> _choicesText = null;
        private int _choiceCount = 0;

        // typing
        private InputField _answerField = null;
        private Button _submitButton = null;

        // lives / chances
        private List<Toggle> _lives = null;
        private int _currentLives = 0;

        // timer
        private int _currentTime = 0;
        private float _currentAngle = 0f;
        private float _angleIncrement = 0f;

        public void InitializeGameType()
        {
            if (_gameUI.IsMultipleChoice)
                CreateMultipleChoiceButtons();

            if (_gameUI.IsTyping)
                CreateAnswerInput();

            if (_gameUI.IsLifeBased)
                CreateLivesCounter();

            if (_gameUI.IsTimeBased)
            {
                InitializeTimer();
                InvokeRepeating("UpdateTimer", 1.5f, 1f);
            }
        }

        public void UpdateStatus()
        {
            if (_gameUI.IsMultipleChoice)
                UpdateChoices();

            if (_gameUI.IsTyping)
                UpdateAnswerInput();

            if (_gameUI.IsLifeBased && _isChancesPerItem)
                ResetLives();

            if (_gameUI.IsTimeBased && _isTimePerItem)
                ResetTimer();
        }

        #region Events Detection
        /// <summary>
        /// Subscribe to game events
        /// </summary>
        private void OnEnable()
        {
            GameEvents.OnPauseGame += StopTimer;
            GameEvents.OnResumeGame += StartTimer;
            GameEvents.OnChecking += DeactivateInput;
            GameEvents.OnLifeLost += UpdateLives;
            GameEvents.OnEndGame += StopTimer;
        }

        /// <summary>
        /// Unsubscribe to game events
        /// </summary>
        private void OnDisable()
        {
            GameEvents.OnPauseGame -= StopTimer;
            GameEvents.OnResumeGame -= StartTimer;
            GameEvents.OnChecking -= DeactivateInput;
            GameEvents.OnLifeLost -= UpdateLives;
            GameEvents.OnEndGame -= StopTimer;
        }
        #endregion

        #region Situational Methods: Multiple Choice
        /// <summary>
        /// Creating buttons for multiple choice
        /// </summary>
        private void CreateMultipleChoiceButtons()
        {
            _choicesButton = new List<Button>();
            _choicesText = new List<Text>();

            switch (GameManager.CurrentLevel)
            {
                case GameConstants.LEVEL_1_KEY: _choiceCount = _level1ChoiceCount; break;
                case GameConstants.LEVEL_2_KEY: _choiceCount = _level2ChoiceCount; break;
                case GameConstants.LEVEL_3_KEY: _choiceCount = _level3ChoiceCount; break;
            }

            for (int i = 0; i < _choiceCount; i++)
            {
                var _choiceButton = Instantiate(_choicePrefab, _choicesParent).GetComponent<Button>();
                _choiceButton.gameObject.name = "Choice";
                _choicesButton.Add(_choiceButton);

                Text _choiceText = _choiceButton.GetComponentInChildren<Text>();
                _choicesText.Add(_choiceText);
                _choiceButton.onClick.AddListener(() => OnChoiceClick(_choiceText));
            }
        }

        /// <summary>
        /// Update choices text and activate button
        /// </summary>
        private void UpdateChoices()
        {
            if (GameManager.Choices == null)
            {
                Debug.Log("null");
                return;
            }

            for (int i = 0; i < _choicesButton.Count; i++)
            {
                _choicesButton[i].interactable = true;
                _choicesText[i].text = GameManager.Choices[i];
            }
        }

        /// <summary>
        /// Deactivate input
        /// </summary>
        private void DeactivateInput()
        {
            if (_gameUI.IsMultipleChoice)
                foreach (Button b in _choicesButton) { b.interactable = false; }
        }

        /// <summary>
        /// When a choice is clicked
        /// </summary>
        /// <param name="_choiceText">the target Text</param>
        private void OnChoiceClick(Text _choiceText)
        {
            if (_gameUI.IsTimeBased)
                StopTimer();

            GameManager.Answer = _choiceText.text;
            GameEvents.Checking();
        }
        #endregion

        #region Situational Methods: Typing 
        /// <summary>
        /// Creating input for typing
        /// </summary>
        private void CreateAnswerInput()
        {
            var _input = Instantiate(_inputPrefab, _inputParent);
            _answerField = _input.GetComponentInChildren<InputField>();
            _submitButton = _input.GetComponentInChildren<Button>();
            _submitButton.onClick.AddListener(() => OnSubmit());
        }

        /// <summary>
        /// Invoke when clicking submit button
        /// </summary>
        private void OnSubmit()
        {
            if (_answerField.text == "")
                return;

            _answerField.interactable = false;
            _submitButton.interactable = false;
            GameManager.Answer = _answerField.text;
            GameEvents.Checking();
        }

        /// <summary>
        /// Reset answer input upon loading next item
        /// </summary>
        private void UpdateAnswerInput()
        {
            _answerField.interactable = true;
            _submitButton.interactable = true;
            _answerField.text = "";
        }
        #endregion

        #region Situational Methods: Lives
        /// <summary>
        /// Create lives counter for life-based game
        /// </summary>
        private void CreateLivesCounter()
        {
            _livesBackground.gameObject.SetActive(true);

            GameManager.IsLifeBased = true;

            Utilities.ClearChilds(_livesBackground.transform);

            _livesBackground.sprite = _livesSprites[_livesCount - 3];
            _livesBackground.SetNativeSize();

            _lives = new List<Toggle>();

            for (int i = 0; i < _livesCount; i++)
            {
                var life = Instantiate(_lifePrefab, _livesBackground.transform);
                life.name = "Life";
                _lives.Add(life.GetComponent<Toggle>());
            }
            _currentLives = _livesCount;
        }

        /// <summary>
        /// Update lifes if wrong answer
        /// </summary>
        private void UpdateLives()
        {
            if (_currentLives == 0)
                return;

            GameEvents.ShowWrong();

            _currentLives--;
            foreach (Toggle t in _lives)
            {
                if (t.isOn)
                {
                    t.isOn = false;
                    break;
                }
            }

            if (_currentLives == 0)
                Invoke("AllLivesUsed", 1.5f);
            else
                Invoke("ReactivateInput", 1.5f);
        }

        /// <summary>
        /// Invoke when all lives has been used
        /// </summary>
        private void AllLivesUsed()
        {
            if (_isChancesPerItem)
                GameEvents.LoadNextItem();
            else
                GameEvents.EndGame();
        }

        /// <summary>
        /// Reset lives if lives is per item and not for whole game
        /// </summary>
        private void ResetLives()
        {
            foreach (Toggle t in _lives) { t.isOn = true; }
            _currentLives = _livesCount;
        }

        /// <summary>
        /// Reactivate input after showing wrong feedback
        /// </summary>
        private void ReactivateInput()
        {
            if (_gameUI.IsMultipleChoice)
                UpdateChoices();

            if (_gameUI.IsTyping)
                UpdateAnswerInput();

            GameEvents.AllowInput();
        }
        #endregion


        #region Situational Methods: Time
        /// <summary>
        /// Initialize Timer upon start or resetting
        /// </summary>
        private void InitializeTimer()
        {
            if (!_timer.activeSelf)
                _timer.SetActive(true);
            
            int _time = 0;


            switch (GameManager.CurrentLevel)
            {
                case GameConstants.LEVEL_1_KEY: _time = _level1Time; break;
                case GameConstants.LEVEL_2_KEY: _time = _level2Time; break;
                case GameConstants.LEVEL_3_KEY: _time = _level3Time; break;
            }
            DOTween.Pause(_clockHand);
            _clockHand.DORotate(Vector3.forward * 0f, 0f);

            _currentAngle = 0;
            _angleIncrement = 360f / _time;
            _currentTime = _time;
            _timeText.text = GetTimeString(_currentTime);
        }

        /// <summary>
        /// Count down timer per second
        /// </summary>
        private void UpdateTimer()
        {
            _currentTime--;
            _timeText.text = GetTimeString(_currentTime);
            _currentAngle += _angleIncrement;

            _clockHand.DORotate(Vector3.forward * _currentAngle, 1f);

            if (_currentTime == 0)
            {
                TimerEnd();
            }
        }

        /// <summary>
        /// Reset timer when invoking next item
        /// </summary>
        private void ResetTimer()
        {
            StopTimer();

            InitializeTimer();
            StartTimer();
        }

        /// <summary>
        /// get the time string with the given int
        /// </summary>
        /// <param name="_seconds">time in seconds</param>
        /// <returns>time string</returns>
        private string GetTimeString(int _seconds)
        {
            int _min = _seconds / 60;
            int _sec = _seconds % 60;
            return string.Format("{0}:{1}", _min, _sec.ToString("00"));
        }

        /// <summary>
        /// Stopping the timer
        /// </summary>
        private void StopTimer()
        {
            CancelInvoke("UpdateTimer");
        }

        /// <summary>
        /// Resume Timer
        /// </summary>
        private void StartTimer()
        {
            InvokeRepeating("UpdateTimer", 1f, 1f);
        }

        /// <summary>
        /// Invoke when timer reaches 0
        /// </summary>
        private void TimerEnd()
        {
            StopTimer();
            GameManager.SoundManager.SfxTimesUp();
            GameEvents.TimeEnd();

            // TODO time's up prompt
            Invoke("RestartTimer", 1.5f);
        }

        /// <summary>
        /// Load next item when time's up prompt is complete
        /// </summary>
        private void RestartTimer()
        {
            GameEvents.LoadNextItem();
        }
        #endregion
    }
}