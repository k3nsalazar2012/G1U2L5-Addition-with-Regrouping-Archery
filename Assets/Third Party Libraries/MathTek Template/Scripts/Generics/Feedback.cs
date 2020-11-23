// c# / unity class
using UnityEngine;
using UnityEngine.UI;

// mathtek class
using MathTek.Utils;

// third party class
using DG.Tweening;
using UniRx;

namespace MathTek.Generics
{
    /// <summary>
    /// Show Feedbacks depending on scores and stars
    /// </summary>
    public class Feedback : MonoBehaviour
    {
        #region Editor Variables
        [Header("Feedback")]
        [Tooltip("Feedback Panel")]
        [SerializeField] private CanvasGroup _feedbackPanel = null;
        [Tooltip("Prompt Box")]
        [SerializeField] private RectTransform _promptBox = null;
        [Tooltip("Feedback Score Text")]
        [SerializeField] private Text _feedbackScoreText = null;
        [Tooltip("Feedback Background")]
        [SerializeField] private Image _feedbackBackground = null;
        [Tooltip("Feedback Text")]
        [SerializeField] private Image _feedbackText = null;
        [Tooltip("Feedback Background Sprites")]
        [SerializeField] private Sprite[] _feedbackBackgrounds = null;
        [Tooltip("Feedback Text Sprites")]
        [SerializeField] private Sprite[] _feedbackTexts = null;
        [Tooltip("Star prefab")]
        [SerializeField] private GameObject _starPrefab = null;
        [Tooltip("Star guides")]
        [SerializeField] private Transform[] _starGuides = null;
        [Tooltip("Stars parent")]
        [SerializeField] private Transform _starsParent = null;

        [Header("Animation and Presentation")]
        [Tooltip("Animation ease type")]
        [SerializeField] private Ease _ease = Ease.InOutBounce;
        [Tooltip("Animation duration")]
        [SerializeField] private float _animationDuration = 0.5f;

        [Header("Sound Effects")]
        [SerializeField] private AudioSource _sfx = null;
        [SerializeField] private SoundData _soundData = null;
        #endregion

        private ReactiveProperty<int> _feedbackScore = new ReactiveProperty<int>();

        #region Public Static Variables
        public static int Stars {set; private get;}
        public static int Score { set; private get; }
        public static int TotalItems { set; private get; }
        #endregion

        /// <summary>
        /// Subscribe to game events
        /// </summary>
        private void OnEnable()
        {
            GameEvents.OnEndGame += ShowFeedback;
        }

        /// <summary>
        /// Unsubscribe to game events
        /// </summary>
        private void OnDisable()
        {
            GameEvents.OnEndGame -= ShowFeedback;
        }

        #region Feedback Sequence
        /// <summary>
        /// Initiating Feedback Sequence
        /// </summary>
        private void ShowFeedback()
        {
            _feedbackPanel.DOFade(1f, _animationDuration);
            _feedbackPanel.blocksRaycasts = true;

            _feedbackScore.Value = 0;
            _feedbackScore.Subscribe(_ => UpdateScoreText());

            int index = Utilities.GetFeedbackIndex(Stars);

            _feedbackBackground.sprite = _feedbackBackgrounds[index];
            _feedbackBackground.SetNativeSize();
            _feedbackText.sprite = _feedbackTexts[index];
            _feedbackText.SetNativeSize();

            _feedbackText.DOFade(0f, 0f);
            _feedbackText.rectTransform.DOAnchorPosY(-64f, 0f);

            _sfx.clip = _soundData.sfxScoring;
            _sfx.loop = true;
            _sfx.Play();

            DOTween.To(() => _feedbackScore.Value,
                x => _feedbackScore.Value = x, Score,
                _animationDuration * 2f).SetDelay(_animationDuration)
                .OnComplete(() => ShowStars());

            // unsubscribe to game events
            GameEvents.OnEndGame -= ShowFeedback;
        }

        /// <summary>
        /// feedback score animation
        /// </summary>
        private void UpdateScoreText()
        {
            _feedbackScoreText.text = string.Format("{0}/<size=30>{1}</size>", _feedbackScore.Value, TotalItems);
        }

        /// <summary>
        /// Show stars, if star is 0, show feedback texts instead
        /// </summary>
        private void ShowStars()
        {
            _sfx.Stop();
            _sfx.clip = _soundData.sfxStar;
            _sfx.loop = false;

            if (Stars == 0)
                ShowFeedbackText();
            else
            {
                Transform starGuide = _starGuides[Stars];
                float delay = 0f;

                for (int i = 0; i < Stars; i++)
                {
                    int _index = i;

                    RectTransform targetStar = (starGuide.GetChild(i) as RectTransform);
                    RectTransform _star = (RectTransform)Instantiate(_starPrefab, _starsParent).transform as RectTransform;
                    _star.DOAnchorPosY(-64f, 0f);

                    _star.DOSizeDelta(targetStar.sizeDelta, _animationDuration).SetDelay(delay).SetEase(_ease);
                    _star.DORotateQuaternion(targetStar.rotation, _animationDuration).SetDelay(delay).SetEase(_ease);
                    _star.DOAnchorPos(targetStar.anchoredPosition, _animationDuration).SetDelay(delay).SetEase(_ease).OnComplete(() => OnStarComplete(_index));

                    delay += 0.5f;
                }
            }
        }

        /// <summary>
        /// Triggers after showing all stars
        /// </summary>
        /// <param name="index"></param>
        private void OnStarComplete(int index)
        {
            _sfx.Play();

            _promptBox.DOShakeScale(0.25f, 0.05f, 10, 90f, true);
            if (index == Stars - 1)
                ShowFeedbackText();
        }

        /// <summary>
        /// Show feedback text
        /// </summary>
        private void ShowFeedbackText()
        {
            _feedbackText.DOFade(1f, _animationDuration);
            _feedbackText.rectTransform.DOAnchorPosY(150f, _animationDuration);
        }

        /// <summary>
        /// Load title scene when Try Again button is clicked
        /// </summary>
        public void TryAgain()
        {
            Utilities.LoadScene("Title");
        }
        #endregion
    }
}