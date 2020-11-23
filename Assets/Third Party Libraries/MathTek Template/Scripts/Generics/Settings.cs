// c# / unity class
using UnityEngine;
using UnityEngine.UI;

// third party class
using DG.Tweening;

namespace MathTek.Generics
{
    /// <summary>
    /// Help and Audio Settings
    /// </summary>
    public class Settings : MonoBehaviour
    {
        #region Editor Variables
        [Header("UI References")]
        [Tooltip("Settings Menu")]
        [SerializeField] private RectTransform _settingsMenu = null;
        [Tooltip("Sound button")]
        [SerializeField] private Button _soundButton = null;
        [Tooltip("Sound button on sprite")]
        [SerializeField] private Sprite _soundOnSprite = null;
        [Tooltip("Sound button off sprite")]
        [SerializeField] private Sprite _soundOffSprite = null;

        [Header("Instructions")]
        [Tooltip("Instructions Panel")]
        [SerializeField] RectTransform _instructionsPanel = null;
        [Tooltip("Instructions Text")]
        [SerializeField] Text _instructionsText = null;

        [Header("Animation and Presentation")]
        [Tooltip("Reference to one panel, to get the screen size")]
        [SerializeField] RectTransform _viewport = null;
        [Tooltip("Animation ease type")]
        [SerializeField] Ease _ease = Ease.InOutBounce;
        [Tooltip("Animation duration")]
        [SerializeField] float _animationDuration = 0.25f;
        #endregion

        private float _screenWidth = 0f;
        private bool _settingsVisible = false;

        public static string Instructions { set; private get; }

        private void Start()
        {
            _screenWidth = _viewport.rect.width;
            _settingsVisible = false;
            _instructionsPanel.anchoredPosition = Vector2.right * _screenWidth;
        }

        #region Settings Menu
        /// <summary>
        /// When settings button is clicked
        /// </summary>
        public void ToggleSettings()
        {
            if (_settingsVisible)
                _settingsMenu.DOAnchorPosY(220f, _animationDuration).SetEase(_ease);
            else
                _settingsMenu.DOAnchorPosY(-41f, _animationDuration).SetEase(_ease);

            _settingsVisible = !_settingsVisible;
        }

        /// <summary>
        /// Show Instructions when help button is clicked
        /// </summary>
        public void ShowInstructions()
        {
            if (_instructionsText.text != Instructions)
                _instructionsText.text = Instructions;

            _instructionsPanel.anchoredPosition = Vector2.right * _screenWidth;
            _instructionsPanel.DOAnchorPosX(0f, _animationDuration).SetEase(_ease);

            GameEvents.PauseGame();
        }

        /// <summary>
        /// Hide instructions when ready button is clicked
        /// </summary>
        public void HideInstructions()
        {
            _instructionsPanel.DOAnchorPosX(-_screenWidth, _animationDuration).SetEase(_ease);

            GameEvents.ResumeGame();
        }

        /// <summary>
        /// Toggle audio on/off when clicking the sound button
        /// </summary>
        public void SetAudio()
        {
            Image buttonSprite = _soundButton.GetComponent<Image>();

            if (buttonSprite.sprite == _soundOnSprite)
            {
                buttonSprite.sprite = _soundOffSprite;
                AudioListener.volume = 0f;
            }
            else
            {
                buttonSprite.sprite = _soundOnSprite;
                AudioListener.volume = 1f;
            }
        }
        #endregion
    }
}