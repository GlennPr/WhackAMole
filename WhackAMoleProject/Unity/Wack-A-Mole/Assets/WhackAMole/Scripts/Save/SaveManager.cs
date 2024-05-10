using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlennKorver.Utility;
using WhackAMole.Data;

namespace WhackAMole.Save
{
	/// <summary>
	/// Single point of entry to hande Save data
	/// </summary>
	public static class SaveManager
	{
		private static readonly string _saveFileName = "WhackAMoleSave";
		private static HighScoreSaveData _sortedActiveSaveData = null;

		public static bool DoesSaveExistOnDisk()
		{
			if (Application.isEditor)
			{
				return JsonLoading.DoesJsonExist(_saveFileName, Application.streamingAssetsPath);
			}
			else
			{
				return JsonLoading.DoesJsonExist(_saveFileName, Application.persistentDataPath);
			}
		}

		public static void Save(HighScoreSaveData value)
		{
			if (Application.isEditor)
			{
				JsonWriter.SaveToStreaming(value, _saveFileName);
			}
			else
			{
				JsonWriter.SaveToPersistent(value, _saveFileName);
			}
		}

		public static void Load(bool forceLoadFromdisk = false)
		{
			if(_sortedActiveSaveData != null && forceLoadFromdisk == false)
			{
				return;
			}


			if (Application.isEditor)
			{
				_sortedActiveSaveData = JsonLoading.LoadDataFromStreaming<HighScoreSaveData, UserHighScore>(_saveFileName);
			}
			else
			{
				_sortedActiveSaveData = JsonLoading.LoadDataFromPersistent<HighScoreSaveData, UserHighScore>(_saveFileName);
			}

			// fallback, create new save data on the spot
			if(_sortedActiveSaveData == null)
			{
				_sortedActiveSaveData = new HighScoreSaveData();
			}

			_sortedActiveSaveData.Sort(true);
		}

		public static HighScoreSaveData GetActiveSaveData()
		{
			if(_sortedActiveSaveData == null)
			{
				Load();
			}

			return _sortedActiveSaveData;
		}

		public static void AddEntry(string userName, DateTime dateTime, int score, out int entryIndex)
		{
			// attempt load if not present
			var activeSaveData = GetActiveSaveData();
			entryIndex = 0;

			if (activeSaveData != null)
			{
				activeSaveData.AddEntry(userName, dateTime, score, out entryIndex);
			}
		}
	}
}