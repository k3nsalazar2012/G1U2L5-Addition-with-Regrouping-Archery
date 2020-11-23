// c# / unity class
using System;

namespace MathTek.Generics
{
    /// <summary>
    /// Hanndles all game events
    /// </summary>
    public static class GameEvents
    {
        /// <summary>
        /// Scene loaded event
        /// </summary>
        public static event Action OnSceneLoaded;
        public static void SceneLoaded()
        {
            OnSceneLoaded?.Invoke();
        }

        /// <summary>
        /// Next item event
        /// </summary>
        public static event Action OnNextItem;
        public static void LoadNextItem()
        {
            OnNextItem?.Invoke();
        }

        /// <summary>
        /// Game pause event, when clicking instructions / help
        /// </summary>
        public static event Action OnPauseGame;
        public static void PauseGame()
        {
            OnPauseGame?.Invoke();
        }

        /// <summary>
        /// Game pause event, when releasing from instructions / help
        /// </summary>
        public static event Action OnResumeGame;
        public static void ResumeGame()
        {
            OnResumeGame?.Invoke();
        }

        /// <summary>
        /// Life lost event (for chances based game)
        /// </summary>
        public static event Action OnLifeLost;
        public static void LifeLost()
        {
            OnLifeLost?.Invoke();
        }

        /// <summary>
        /// Allow input event (for chances based game, upon showing wrong answer)
        /// </summary>
        public static event Action OnAllowInput;
        public static void AllowInput()
        {
            OnAllowInput?.Invoke();
        }

        /// <summary>
        /// Time end event (for time based game)
        /// </summary>
        public static event Action OnTimeEnd;
        public static void TimeEnd()
        {
            OnTimeEnd?.Invoke();
        }

        /// <summary>
        /// Item checking event
        /// </summary>
        public static event Action OnChecking;
        public static void Checking()
        {
            OnChecking?.Invoke();
        }

        /// <summary>
        /// Showing correct answer event
        /// </summary>
        public static event Action OnCorrect;
        public static void ShowCorrect()
        {
            OnCorrect?.Invoke();
        }

        /// <summary>
        /// Showing wrong answer event
        /// </summary>
        public static event Action OnWrong;
        public static void ShowWrong()
        {
            OnWrong?.Invoke();
        }

        /// <summary>
        /// End game event
        /// </summary>
        public static event Action OnEndGame;
        public static void EndGame()
        {
            OnEndGame?.Invoke();
        }
    }
}