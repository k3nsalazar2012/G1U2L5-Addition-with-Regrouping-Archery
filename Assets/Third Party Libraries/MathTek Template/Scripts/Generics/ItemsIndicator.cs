// c# / unity class
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// third party class
using DG.Tweening;

namespace MathTek.Generics
{
    /// <summary>
    /// Visual indicator showing the current item and total items to accomplish
    /// </summary>
    public class ItemsIndicator : MonoBehaviour
    {
        #region Editor Variables
        [Header("References")]
        [Tooltip("Items parent")]
        [SerializeField] private Transform _itemsParent = null;
        [Tooltip("Item Prefab")]
        [SerializeField] private GameObject _itemPrefab = null;
        [Tooltip("Item on sprite")]
        [SerializeField] private Sprite _itemOnSprite = null;
        [Tooltip("Item off sprite")]
        [SerializeField] private Sprite _itemOffSprite = null;

        [Header("Animation and Presentation")]
        [Tooltip("Animation ease type")]
        [SerializeField] private Ease _ease = Ease.InOutBounce;
        [Tooltip("Animation duration")]
        [SerializeField] private float _animationDuration = 0.25f;
        #endregion

        public static int TotalItems { set; private get; }

        private void Start()
        {
            StartCoroutine( CreateItemsIndicator());
        }

        /// <summary>
        /// Subscribe to game events
        /// </summary>
        private void OnEnable()
        {
            GameEvents.OnNextItem += UpdateItemIndicator;
            GameEvents.OnSceneLoaded += ShowItemsIndicator;
        }

        /// <summary>
        /// Unsubscribe to game events
        /// </summary>
        private void OnDisable()
        {
            GameEvents.OnNextItem -= UpdateItemIndicator;
            GameEvents.OnSceneLoaded -= ShowItemsIndicator;
        }

        #region Item Indicator Sequence
        /// <summary>
        /// Create items indicator
        /// </summary>
        /// <returns>IEnumerator</returns>
        private IEnumerator CreateItemsIndicator()
        {
            for (int i = 0; i < TotalItems; i++)
            {
                RectTransform dot = Instantiate(_itemPrefab, _itemsParent).transform as RectTransform;
            }
            yield return new WaitForEndOfFrame();
            _itemsParent.GetComponent<HorizontalLayoutGroup>().enabled = false;
        }

        /// <summary>
        /// Show Items Indicator
        /// </summary>
        /// <returns></returns>
        private void ShowItemsIndicator()
        {
            float delay = 0f;

            for (int i = 0; i < _itemsParent.childCount; i++)
            {
                (_itemsParent.GetChild(i).transform as RectTransform).DOAnchorPosY(0f, _animationDuration).SetEase(_ease).SetDelay(delay);
                delay += 0.05f;
            }
        }

        /// <summary>
        /// Update Item, activate one per item
        /// </summary>
        private void UpdateItemIndicator()
        {
            for (int i = 0; i < _itemsParent.childCount; i++)
            {
                Image itemIndicator = _itemsParent.GetChild(i).GetComponent<Image>();
                if (itemIndicator.sprite == _itemOffSprite)
                {
                    itemIndicator.sprite = _itemOnSprite;
                    itemIndicator.transform.DOScale(Vector3.one * 1.2f, _animationDuration).SetEase(_ease).OnComplete(() => OnItemIndicatorComplete(itemIndicator));
                    break;
                }
            }
        }

        /// <summary>
        /// On Item Indicator animation complete
        /// </summary>
        /// <param name="itemIndicator"></param>
        private void OnItemIndicatorComplete(Image itemIndicator)
        {
            itemIndicator.transform.DOScale(Vector3.one, _animationDuration).SetEase(_ease);
        }
        #endregion   
    }
}