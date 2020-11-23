// c# / unity class
using UnityEngine;
using UnityEngine.UI;

// third party class
using DG.Tweening;
using System;

namespace MathTek.Generics
{
    public class ItemsFeedback : MonoBehaviour
    {
        #region Editor Variables
        [SerializeField] private CanvasGroup _prompt = null;

        [Header("Correct")]
        [SerializeField] private Image _correctImage = null;
        [SerializeField] private Image _correctText = null;

        [Header("Wrong")]
        [SerializeField] private Image _wrongImage = null;
        [SerializeField] private Image _wrongText = null;

        [Header("Timer")]
        [SerializeField] private Image _clockImage = null;
        [SerializeField] private Image _timeText = null;

        [Header("Animation and Transitions")]
        [Tooltip("Animation ease type")]
        [SerializeField] private Ease _ease = Ease.Linear;
        [Tooltip("Animation duration")]
        [SerializeField] private float _animationDuration = 0.25f;
        [Tooltip("Achor Y Offset")]
        [SerializeField] private float _yOffset = 10f;
        #endregion

        private Image _feedbackImage = null;
        private Image _feedbackText = null;

        /// <summary>
        /// Subscribe to game events
        /// </summary>
        private void OnEnable()
        { 
            GameEvents.OnCorrect += ShowCorrect;
            GameEvents.OnWrong += ShowWrong;
            GameEvents.OnTimeEnd += ShowTimesUp;
        }

        /// <summary>
        /// Unsubscribe to game events
        /// </summary>
        private void OnDisable()
        {
            GameEvents.OnCorrect -= ShowCorrect;
            GameEvents.OnWrong -= ShowWrong;
            GameEvents.OnTimeEnd -= ShowTimesUp;
        }

        /// <summary>
        /// Show correct feedback
        /// </summary>
        private void ShowCorrect()
        {
            _feedbackImage = _correctImage;
            _feedbackText = _correctText;
            ShowItemFeedback();
        }

        /// <summary>
        /// Show wrong feedback
        /// </summary>
        private void ShowWrong()
        {
            _feedbackImage = _wrongImage;
            _feedbackText = _wrongText;
            ShowItemFeedback();
        }

        /// <summary>
        /// Show Times Up Prompt
        /// </summary>
        private void ShowTimesUp()
        {
            float _duration = _animationDuration * 5f;
            _clockImage.rectTransform.DOAnchorPosY(_clockImage.rectTransform.anchoredPosition.y - _yOffset, 0f);
            _clockImage.rectTransform.DOAnchorPosY(_clockImage.rectTransform.anchoredPosition.y, _duration).SetEase(_ease).OnComplete(()=>OnTimesUpComplete());

            _timeText.rectTransform.DOAnchorPosY(_timeText.rectTransform.anchoredPosition.y + _yOffset, 0f);
            _timeText.rectTransform.DOAnchorPosY(_timeText.rectTransform.anchoredPosition.y, _duration).SetEase(_ease);

            _clockImage.DOFade(0f, 0f);
            _timeText.DOFade(0f, 0f);

            _clockImage.DOFade(1f, _duration).SetEase(_ease);
            _timeText.DOFade(1f, _duration).SetEase(_ease);
        }

        /// <summary>
        /// Show target item feedback
        /// </summary>
        private void ShowItemFeedback()
        {
            _prompt.DOFade(1f, _animationDuration).SetEase(_ease);
            _prompt.DOFade(0f, _animationDuration).SetEase(_ease).SetDelay(_animationDuration * 4f).OnComplete(() => OnItemFeedbackComplete());

            float _duration = _animationDuration * 2f;
            _feedbackImage.rectTransform.DOAnchorPosY(_feedbackImage.rectTransform.anchoredPosition.y - _yOffset, 0f);
            _feedbackImage.rectTransform.DOAnchorPosY(_feedbackImage.rectTransform.anchoredPosition.y, _duration).SetEase(_ease);

            _feedbackText.rectTransform.DOAnchorPosY(_feedbackText.rectTransform.anchoredPosition.y + _yOffset, 0f);
            _feedbackText.rectTransform.DOAnchorPosY(_feedbackText.rectTransform.anchoredPosition.y, _duration).SetEase(_ease);

            _feedbackImage.DOFade(0f, 0f);
            _feedbackText.DOFade(0f, 0f);

            _feedbackImage.DOFade(1f, _duration).SetEase(_ease);
            _feedbackText.DOFade(1f, _duration).SetEase(_ease);
        }

        /// <summary>
        /// Hide target feedback when complete
        /// </summary>
        private void OnItemFeedbackComplete()
        {
            _feedbackImage.DOFade(0f, 0f);
            _feedbackText.DOFade(0f, 0f);
        }

        /// <summary>
        /// Hide times up feedback when complete
        /// </summary>
        private void OnTimesUpComplete()
        {
            _clockImage.DOFade(0f, 0f);
            _timeText.DOFade(0f, 0f);
        }
    }
}