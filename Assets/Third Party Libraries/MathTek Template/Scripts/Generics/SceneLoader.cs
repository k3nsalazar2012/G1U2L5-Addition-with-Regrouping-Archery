// unity class
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

// third party class
using DG.Tweening;
using UniRx;

namespace MathTek.Generics
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] CanvasGroup canvas = null;
        [SerializeField] Image mathSymbolPrefab = null; 
        [SerializeField] Sprite[] mathSymbols = null;
        [SerializeField] float xOffset = 0f;
        [SerializeField] float yOffset = 0f;
        [SerializeField] RectTransform[] dots = null;
        [SerializeField] Ease ease = Ease.InOutBounce;
        [SerializeField] Sprite dotOffSprite = null;
        [SerializeField] Sprite dotOnSprite = null;
        
        private float animationDuration = 0.5f;
        private ReactiveProperty<int> progress = new ReactiveProperty<int>();
        private string sceneName = "";

        private void Start()
        {
            progress.Subscribe(_ => UpdateProgress());
            DontDestroyOnLoad(this);
        }

        private void LoadMathSymbol()
        {
            var mathSymbol = GameObject.Instantiate(mathSymbolPrefab, transform).GetComponent<Image>();
            mathSymbol.sprite = mathSymbols[Random.Range(0, mathSymbols.Length)];
            mathSymbol.SetNativeSize();

            float x = Random.Range(-xOffset, xOffset);
            float y = Random.Range(-yOffset, yOffset);

            mathSymbol.rectTransform.anchoredPosition = new Vector2(x, y);
            mathSymbol.DOFade(0f, 0f);
            mathSymbol.transform.DOScale(Vector3.one * 1.5f, animationDuration);
            mathSymbol.DOFade(1f, animationDuration).OnComplete(()=>OnMathSymbolComplete(mathSymbol.gameObject));
        }

        private void OnShowComplete()
        {
            float delay = 0f;

            for (int i=0; i<dots.Length; i++)
            {
                dots[i].GetComponent<Image>().sprite = dotOffSprite;
                if(i==dots.Length-1)
                    dots[i].DOAnchorPosY(0f, 0.2f).SetDelay(delay).SetEase(ease).OnComplete(()=>StartCoroutine( OnDotsEntryComplete()));
                else
                    dots[i].DOAnchorPosY(0f, 0.2f).SetDelay(delay).SetEase(ease);
                delay += 0.05f;
            }
        }

        private IEnumerator OnDotsEntryComplete()
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
            while (!asyncOperation.isDone)
            {
                int _progress = (int) (asyncOperation.progress * 15f);
                progress.Value = _progress;
                yield return null;
            }
            progress.Value = 15;
        }

        private void OnDotOnComplete(Transform target)
        {
            DOTween.Pause(target);

            target.GetComponent<Image>().sprite = dotOnSprite;
            if(target.GetSiblingIndex() == target.parent.childCount - 1)
                target.DOScale(Vector3.one, 0.1f).OnComplete(()=>OnLoadingComplete());
            else
                target.DOScale(Vector3.one, 0.1f);
        }

        private void OnLoadingComplete()
        {
            canvas.DOFade(0f, animationDuration).OnComplete(() => StartCoroutine( Hide()));
        }

        public void Load(string name)
        {
            sceneName = name;
            Show();
            InvokeRepeating("LoadMathSymbol", 0f, 1f);
        }

        private void Show()
        {
            canvas.blocksRaycasts = true;
            canvas.DOFade(1f, animationDuration).OnComplete(() => OnShowComplete());
        }

        private IEnumerator Hide()
        {
            if (IsInvoking("LoadMathSymbol"))
                CancelInvoke("LoadMathSymbol");

            yield return new WaitForSeconds(0.25f);
            GameEvents.SceneLoaded();
            Destroy(gameObject);
        }

        private void OnMathSymbolComplete(GameObject target)
        {
            Destroy(target);
        }

        private void UpdateProgress()
        {
            float delay = 0f;
            for (int i = 0; i < progress.Value; i++)
            {
                Transform target = dots[i];
                if (target.GetComponent<Image>().sprite != dotOnSprite)
                {
                    target.DOScale(Vector3.one * 1.2f, 0.1f).SetDelay(delay).OnComplete(() => OnDotOnComplete(target));
                    delay += 0.05f;
                }
            }
        }
    }
}