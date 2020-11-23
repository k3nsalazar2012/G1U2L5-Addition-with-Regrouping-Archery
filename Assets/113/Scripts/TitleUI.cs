// unity class
using UnityEngine;
using UnityEngine.UI;

// mathtek class
using MathTek.Utils;
using MathTek.Generics;

// third party class
using DG.Tweening;

namespace TFI.MathTek.ID113
{
    /// <summary>
    /// Title Script
    /// Manages showing instructions and level selection ( and unlocking)
    /// </summary>
    public class TitleUI : MonoBehaviour
    {
        #region Editor Variables
        [Header("UI Elements")]
        [Tooltip("Blur Panel in the UI, will serve as dimmer for instructions and level selection")]
        [SerializeField] GameObject blurPanel = null;
        [Tooltip("Instructions Panel in the UI")]
        [SerializeField] RectTransform instructionsPanel = null;
        [Tooltip("InstructionsText in the UI")]
        [SerializeField] Text instructionsText = null;
        [Tooltip("Level Selection Panel in the UI")]
        [SerializeField] RectTransform levelSelectionPanel = null;
        [Tooltip("Level toggles")]
        [SerializeField] Toggle[] levelButtons = null;
        [Tooltip("Play button, the one in level selection")]
        [SerializeField] Button playButton = null;


        [Header("Animations and Transitions")]
        [Tooltip("Reference to one panel, to get the screen size")]
        [SerializeField] RectTransform viewport = null;
        [Tooltip("Easing type of transitions")]
        [SerializeField] Ease ease = Ease.InBounce;
        [Tooltip("Duration of transitions")]
        [SerializeField] float transitionDuration = 1f;


        [Header("For development")]
        [Tooltip("Unlock all levels")]
        [SerializeField] bool unlockAllLevels = false;
        #endregion

        /// <summary>
        /// Initialize GameManager
        /// </summary>
        private void Awake()
        {
            GameManager.Initialize();
        }

        /// <summary>
        /// Initialize Values
        /// </summary>
        private void Start()
        {
            GameManager.ScreenWidth = viewport.rect.width;

            playButton.interactable = false;
            blurPanel.SetActive(false);
            instructionsPanel.anchoredPosition = Vector2.right * GameManager.ScreenWidth;
            levelSelectionPanel.anchoredPosition = Vector2.right * GameManager.ScreenWidth;

            // setting audio manager
            GameManager.SoundManager = FindObjectOfType<SoundManager>();
        }

        /// <summary>
        /// Show instructions after clicking the play button or clicking the arrow left button
        /// </summary>
        public void ShowInstructions()
        {
            if (instructionsText.text != GameManager.Instructions)
                instructionsText.text = GameManager.Instructions;

            ShowPanel(instructionsPanel);
            if(levelSelectionPanel.anchoredPosition.x != GameManager.ScreenWidth)
                HidePanel(levelSelectionPanel, GameManager.ScreenWidth);
        }

        /// <summary>
        /// Showing level selection after clicking the arrow right button
        /// </summary>
        public void ShowLevelSelection()
        {
            UnlockLevels();
            ShowPanel(levelSelectionPanel);
            HidePanel(instructionsPanel, -GameManager.ScreenWidth);
        }

        #region Level Unlocking
        /// <summary>
        /// Checking levels to unlock
        /// </summary>
        private void UnlockLevels()
        {
            #if UNITY_WEBGL && !UNITY_EDITOR
                Debug.Log("webgl");
                if (MTManager.IsCMS)
                {                    
                    GameManager.LevelReached = MTManager.CurrentLevel;
                    if(GameManager.LevelReached == 0)
                        GameManager.LevelReached = 1;
                }
                else
                    unlockAllLevels = true;
            #endif

            #if UNITY_STANDALONE || UNITY_EDITOR
            if (PlayerPrefs.GetInt("Levels") == 0)
                PlayerPrefs.SetInt("Levels", 1);

                GameManager.LevelReached = PlayerPrefs.GetInt("Levels");
            #endif

            if (unlockAllLevels)
                GameManager.LevelReached = 3;

            for (int i = 0; i < GameManager.LevelReached; i++)
            {
                Toggle levelToggle = levelButtons[i];
                levelToggle.interactable = true;
                levelToggle.onValueChanged.AddListener(delegate {
                    SelectLevel(levelToggle);
                });
            }
        }

        /// <summary>
        /// Toggle value changed event
        /// </summary>
        /// <param name="levelToggle">the toggle clicked</param>
        private void SelectLevel(Toggle levelToggle)
        {
            if (!levelToggle.isOn)
            {
                GameManager.CurrentLevel = null;
                playButton.interactable = false;
            }
            else
            {
                switch (levelToggle.transform.GetSiblingIndex())
                {
                    case 0: GameManager.CurrentLevel = GameConstants.LEVEL_1_KEY; break;
                    case 1: GameManager.CurrentLevel = GameConstants.LEVEL_2_KEY; break;
                    case 2: GameManager.CurrentLevel = GameConstants.LEVEL_3_KEY; break;
                    default: GameManager.CurrentLevel = GameConstants.LEVEL_1_KEY; break;
                }
                playButton.interactable = true;
            }
            Debug.Log(GameManager.CurrentLevel);
        }
        #endregion

        /// <summary>
        /// Shows panel with transition
        /// </summary>
        /// <param name="panel">Target panel to show</param>
        private void ShowPanel(RectTransform panel)
        {
            if (!blurPanel.activeSelf)
                blurPanel.SetActive(true);  // blur background

            panel.DOAnchorPosX(0f, transitionDuration).SetEase(ease);
        }

        /// <summary>
        /// Hides panel with transition
        /// </summary>
        /// <param name="panel">Target panel to hide</param>
        /// <param name="anchorX">Anchor destination on the x axis</param>
        private void HidePanel(RectTransform panel, float anchorX)
        {
            panel.DOAnchorPosX(anchorX, transitionDuration).SetEase(ease);
        }

        /// <summary>
        /// Load Game scene upon clicking play button
        /// </summary>
        public void Play()
        {
            if (GameManager.CurrentLevel == null)
                return;

            GameManager.LoadLevelData();
            GameManager.LoadScene("Game");
        }
    }
}