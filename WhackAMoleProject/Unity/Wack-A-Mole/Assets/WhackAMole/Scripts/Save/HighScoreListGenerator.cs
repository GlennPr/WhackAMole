using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WhackAMole.Data;

namespace WhackAMole.Save
{
    /// <summary>
    /// Generates a dummy savefile
    /// </summary>
    public static class HighScoreListGenerator
    {
        /// <summary>
        /// Generates a dummy savefile
        /// </summary>
        public static void Generate()
        {
            var saveData = new HighScoreSaveData();
            saveData.DataList = new List<UserHighScore>();

            string[] names = new string[] { "Leonardo", "Picasso", "Rembrandt", "Monet", "Warhol", "Mondriaan", "Banksy" };

            for (int i = 0; i < names.Length; i++)
			{
                var randomUserName = names[i] + i.ToString("D3");
                var randomScore = Random.Range(2000, 8500);
               
                var currentDateTime = System.DateTime.UtcNow;
                var randomYear = Random.Range(currentDateTime.Year - 2, currentDateTime.Year - 1);
                var randomMonth = Random.Range(1, 13);
                var randomDay = Random.Range(1, 20);
                var randomHour = Random.Range(8, 17);
                var randomMinute = Random.Range(0, 60);
                var randomSeconds = Random.Range(0, 60);
                var randomDateTime = new System.DateTime(randomYear, randomMonth, randomDay, randomHour, randomMinute, randomSeconds);

                saveData.DataList.Add(new UserHighScore(randomUserName, randomDateTime, randomScore));
            }

            SaveManager.Save(saveData);
        }
    }
}