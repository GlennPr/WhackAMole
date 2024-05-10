using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WhackAMole.Data
{
    /// <summary>
    /// Highscore data collection, persistent on disk
    /// </summary>
    [Serializable]
    public class HighScoreSaveData : GlennKorver.Utility.SerializableList<UserHighScore>
    {
        public HighScoreSaveData()
        {
            if(DataList == null)
		    {
                DataList = new List<UserHighScore>();
            }
        }

        public void AddEntry(string userName, DateTime dateTime, int score, out int entryIndex)
        {
            UserHighScore entry = new UserHighScore(userName, dateTime, score);

            //TODO binary search, add element without the need for sorting
            DataList.Add(entry);
            Sort(true);

            entryIndex = DataList.IndexOf(entry);
        }

        public void Sort(bool fullReSorting)
        {
            // simple sort by for now, TODO make more effecient
            DataList = DataList.OrderByDescending(x => x.Score).ThenByDescending(x => x.Ticks).ToList();
        }
    }

    [Serializable]
    public class UserHighScore
    {
        public string UserName;
        public long Ticks;
        public int Score;

        public UserHighScore(string userName, DateTime dateTime, int score)
		{
            UserName = userName;
            Ticks = dateTime.Ticks;
            Score = score;
        }
    }
}