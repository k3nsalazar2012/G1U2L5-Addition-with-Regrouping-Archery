// unity class
using System.Collections.Generic;
using System;
using UnityEngine;

// mathtek class
using MathTek.Utils;
using MathTek.Generics;

namespace TFI.MathTek.ID113
{
    public static class GameManager
    {
        #region Game Set Up
        /// <summary>
        /// Screen Width, can be used for responsive UI scaling
        /// </summary>
        public static float ScreenWidth { set; get; }

        /// <summary>
        /// Current level in the Game
        /// </summary>
        public static string CurrentLevel { set; get; }

        /// <summary>
        /// Current Game ID
        /// </summary>
        public static int GameID { private set; get; }

        /// <summary>
        /// Current Global ID from JSON
        /// </summary>
        public static string GlobalID { private set; get; }

        /// <summary>
        /// Determining Level Reached, for unlocking
        /// </summary>
        public static int LevelReached { set; get; }

        /// <summary>
        /// Is game life-based?
        /// </summary>
        public static bool IsLifeBased { set; get; }

        /// <summary>
        /// Is game time-based?
        /// </summary>
        public static bool IsTimeBased { set; get; }

        /// <summary>
        /// Determine if next level is unlocked
        /// </summary>
        /// <returns></returns>
        public static bool IsNextLevelUnlocked()
        {
            if (_stars >= 3)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Reference to the current sound manager in the scene
        /// </summary>
        public static SoundManager SoundManager { set; get; }
        #endregion

        #region Itembanks / Contents
        /// <summary>
        /// Game Instructions, used in Title and Game scene
        /// </summary>
        public static string Instructions { set; get; }

        /// <summary>
        /// Question for the current item
        /// </summary>
        public static string Question { private set; get; }

        /// <summary>
        /// Correct answer for the current item
        /// </summary>
        public static string Correct { private set; get; }

        /// <summary>
        /// Current answer of the player
        /// </summary>
        public static string Answer { set; get; }

        /// <summary>
        /// Choices for the current item
        /// </summary>
        public static List<string> Choices { private set; get; }
        #endregion

        #region Scoring and Feedback
        /// <summary>
        /// Current Item in the game
        /// </summary>
        public static int CurrentItem { set; get; }

        /// <summary>
        /// Total score of the player
        /// </summary>
        public static int Score { set; get; }

        /// <summary>
        /// Total number of items in the game
        /// </summary>
        public static int TotalItems { set; get; }
        #endregion

        #region Private Variables
        private static List<string> _questions = null;
        private static List<string> _corrects = null;
        private static List<string> _wrongs1 = null;
        private static List<string> _wrongs2 = null;
        private static List<string> _wrongs3 = null;
        private static List<string> _pictures = null;
        private static int _stars = 0;
        #endregion

        /// <summary>
        /// Initiallize Values 
        /// </summary> 
        public static void Initialize()
        {
            GameID = Utilities.GetIDFromNamespace(typeof(GameManager).Namespace);
            GlobalID = Utilities.GetGlobalID();
            Instructions = Utilities.GetInstructions();

            LevelReached = 1;
            Score = 0;
            TotalItems = 10;
            _stars = 0;
            CurrentItem = 0;
        }

        /// <summary>
        /// Loading level contents or itembanks
        /// automatically get the values from JSON for the current level
        /// </summary>
        public static void LoadLevelData()
        {
            _questions = Utilities.GetData(CurrentLevel, GameConstants.QUESTION_KEY);
            _corrects = Utilities.GetData(CurrentLevel, GameConstants.CORRECT_KEY);
            _wrongs1 = Utilities.GetData(CurrentLevel, GameConstants.WRONG_1_KEY);
            _wrongs2 = Utilities.GetData(CurrentLevel, GameConstants.WRONG_2_KEY);
            _wrongs3 = Utilities.GetData(CurrentLevel, GameConstants.WRONG_3_KEY);
            // NOTE: add Wrongs2, Wrongs3, etc if required by the game

            // if needed data is not is the above format (Question, Correct, Wrong1, Wrong2)
            // set the key to desired string
            _pictures = Utilities.GetData(CurrentLevel, "Picture"); // <- this is the modified key
        }

        /// <summary>
        /// Load Next Item
        /// </summary>
        public static void NextItem()
        {
            if (CurrentItem == TotalItems)
            {
                // showing feedback, this invokes end game event
                _stars = Utilities.GetStars(Score, TotalItems);
                Feedback.Stars = _stars;
                Feedback.Score = Score;
                Feedback.TotalItems = TotalItems;
                GameEvents.EndGame();
                return;
            }
            CurrentItem++;

            var indexList = Utilities.CreateIntList(_questions.Count);
            indexList.Shuffle(); // comment this part to disable randomized items/contents
            int index = indexList[0];

            Question = _questions[index];
            Correct = _corrects[index];

            // create randomized choices, add wrongs 1, wrongs2, etc.
            Choices = new List<string>();
            Choices.Add(Correct);
            Choices.Add(_wrongs1[index]);
            Choices.Add(_wrongs2[index]);
            Choices.Add(_wrongs3[index]);
            Choices.Shuffle(); // comment this part to disable randomized choices

            _questions.RemoveAt(index);
            _corrects.RemoveAt(index);
            _wrongs1.RemoveAt(index);
            _wrongs2.RemoveAt(index);
            _wrongs3.RemoveAt(index);
        }

        /// <summary>
        /// Loading scene
        /// </summary>
        /// <param name="name">scene name</param>
        public static void LoadScene(string name)
        {
            Utilities.LoadScene(name);
        }

        /// <summary>
        /// Automatically play audio, added from SoundData asset
        /// </summary>
        /// <param name="clipName">name of the clip</param>
        public static void PlayAudio(string clipName)
        {
            SoundManager.PlayCustomSFX(clipName);
        }
    }
}