
namespace MathTek.Utils
{
    // unity class
    using UnityEngine;
    using System.Linq;
    using System.Collections.Generic;

    // mathtek class
    using MathTek.Exceptions;
    using MathTek.Generics;

    // third party class
    using SimpleJSON;

    /// <summary>
    /// Utilities Class
    /// handles all internal calculations and loading of itembanks using JSON
    /// NOTE: do not modify this class ask the developer first: - Kenneth Salazar
    /// </summary>
    public static class Utilities
    {
        #region Initializing Values
        /// <summary>
        /// Get the game id by parsing the last part of the namespace
        /// </summary>
        /// <param name="namespaceString">namespace of the script</param>
        /// <returns>int of the GameID</returns>
        public static int GetIDFromNamespace(string namespaceString)
        {
            var _id = namespaceString.Split('.')
                .Last()
                .Replace(GameConstants.ID_NAMESPACE_KEY, "");

            int id = int.Parse(_id);
            LoadContentJSON(id);
            return id;
        }

        /// <summary>
        /// Load JSON from Resources folder
        /// </summary>
        /// <param name="gameID">target game id</param>
        private static void LoadContentJSON(int gameID)
        {
            TextAsset content = Resources.Load(string.Format("{0}/content", gameID), typeof(TextAsset)) as TextAsset;
            if (content == null)
                throw new InvalidOrMissingJSONFile();
            else
                ContentJSON = JSON.Parse(content.text) as JSONObject;
        }

        /// <summary>
        /// The JSON Object parsed from Resources folder
        /// </summary>
        private static JSONObject ContentJSON { set; get; }

        /// <summary>
        /// Get the GlobalID from JSON
        /// </summary>
        public static string GetGlobalID()
        {
            return ContentJSON[GameConstants.GLOBAL_ID_KEY];
        }

        /// <summary>
        /// Load Instructions from content.json in Resources folder
        /// </summary>
        /// <returns>instructions string</returns>
        public static string GetInstructions()
        {
            return ContentJSON[GameConstants.INSTRUCTIONS_KEY];
        }
        #endregion


        #region Level Items (Itembank)
        /// <summary>
        /// Get the questions for the current level
        /// </summary>
        /// <param name="currentLevel">level of the game</param>
        /// <returns>string List of Questions</returns>
        public static List<string> GetQuestions(string currentLevel)
        {
            return GetData(currentLevel, GameConstants.QUESTION_KEY);
        }

        /// <summary>
        /// Get the corrects for the current level
        /// </summary>
        /// <param name="currentLevel">level of the game</param>
        /// <returns>string List of Corrects</returns>
        public static List<string> GetCorrects(string currentLevel)
        {
            return GetData(currentLevel, GameConstants.CORRECT_KEY);
        }

        /// <summary>
        /// Get the wrongs 1 for the current level
        /// </summary>
        /// <param name="currentLevel">level of the game</param>
        /// <returns>string List of Wrongs1</returns>
        public static List<string> GetWrongs1(string currentLevel)
        {
            return GetData(currentLevel, GameConstants.WRONG_1_KEY);
        }

        /// <summary>
        /// Get the wrongs 2 for the current level
        /// </summary>
        /// <param name="currentLevel">level of the game</param>
        /// <returns>string List of Wrongs2</returns>
        public static List<string> GetWrongs2(string currentLevel)
        {
            return GetData(currentLevel, GameConstants.WRONG_2_KEY);
        }

        /// <summary>
        /// Get the list of data from the given key
        /// </summary>
        /// <param name="currentLevel">level of the game</param>
        /// <param name="key">target key</param>
        /// <returns>string List of the modified data</returns>
        public static List<string> GetModifiedData(string currentLevel, string key)
        {
            return GetData(currentLevel, key);
        }

        /// <summary>
        /// Get the level data from the given level and key
        /// </summary>
        /// <param name="level">level of the game</param>
        /// <param name="key">target key</param>
        /// <returns></returns>
        public static List<string> GetData(string level, string key)
        {
            List<string> data = new List<string>();
            var levelData = ContentJSON[level].AsArray;
            foreach (var l in levelData)
            {
                data.Add(l.Value.AsObject[key]);
            }

            return data;
        }
        #endregion


        #region Useful Functions and Computations
        /// <summary>
        /// Clear childs of the given parent transform
        /// </summary>
        /// <param name="parent"></param>
        public static void ClearChilds(Transform parent)
        {
            if (parent.childCount != 0)
            {
                for (int i = 0; i < parent.childCount; i++)
                {
                    Object.Destroy(parent.GetChild(i).gameObject);
                }
            }
        }

        /// <summary>
        /// Calculate the accumulated stars based on the scores divided by total items
        /// </summary>
        /// <param name="score">score of the player</param>
        /// <param name="total">total items</param>
        /// <returns></returns>
        public static int GetStars(int score, int total)
        {
            float percentage = (float)score / (float) total;
            int stars = 0;

            if (percentage <= 0.1f)
                stars = 0;
            else if (percentage <= 0.4f)
                stars = 1;
            else if (percentage <= 0.7f)
                stars = 2;
            else if (percentage <= 0.8f)
                stars = 3;
            else if (percentage <= 0.9f)
                stars = 4;
            else
                stars = 5;            

            return stars;
        }

        /// <summary>
        /// Calculate the feedback index, to show different feedback text and background
        /// </summary>
        /// <param name="stars">current stars of the player</param>
        /// <returns></returns>
        public static int GetFeedbackIndex(int stars)
        {
            int index = 0;
            if (stars < 3)
                index = 0;
            else if (stars < 4)
                index = 1;
            else if (stars < 5)
                index = 2;
            else
                index = 3;

            return index;
        }

        /// <summary>
        /// Shuffle/Randomize a list, to use just type .Suffle() after the list
        /// ex. choices.Suffle();
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Shuffle<T>(this IList<T> list)
        {
            System.Random rng = new System.Random();

            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        /// <summary>
        /// Create a dummy int list starting from 0 to count - 1
        /// </summary>
        /// <param name="count">count of the list</param>
        /// <returns>int list</returns>
        public static List<int> CreateIntList(int count)
        {
            List<int> intList = new List<int>();
            for (int i = 0; i < count; i++)
            {
                intList.Add(i);
            }
            return intList;
        }

        public static void LoadScene(string name)
        {
            SceneLoader loader = (SceneLoader) Object.Instantiate( Resources.Load(GameConstants.LOADING_SCREEN_KEY, typeof(SceneLoader)));
            loader.Load(name);
        }
        #endregion
    }

    public static class GameConstants
    {
        public const string ID_NAMESPACE_KEY = "ID";
        public const string GLOBAL_ID_KEY = "global_id";
        public const string INSTRUCTIONS_KEY = "instructions";
        public const string LEVEL_1_KEY = "level 1";
        public const string LEVEL_2_KEY = "level 2";
        public const string LEVEL_3_KEY = "level 3";
        public const string QUESTION_KEY = "Question";
        public const string CORRECT_KEY = "Correct";
        public const string WRONG_1_KEY = "Wrong 1";
        public const string WRONG_2_KEY = "Wrong 2";
        public const string WRONG_3_KEY = "Wrong 3";
        public const string WRONG_4_KEY = "Wrong 4";
        public const string PICTURE_KEY = "Picture";
        public const string LOADING_SCREEN_KEY = "Loading Screen";
    }
}