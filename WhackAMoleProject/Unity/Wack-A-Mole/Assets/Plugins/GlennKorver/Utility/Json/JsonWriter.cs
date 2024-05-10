using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GlennKorver.Utility
{
	public static class JsonWriter
	{
		private static readonly string _fileExtenstion = ".json";

		public static void SaveToStreaming<T>(T Data, string fileName, string additionalPath = "")
		{
			Save(Data, fileName, Application.streamingAssetsPath, additionalPath);
		}

		public static void SaveToPersistent<T>(T Data, string fileName, string additionalPath = "")
		{
			Save(Data, fileName, Application.persistentDataPath, additionalPath);
		}

		private static void Save<T>(T Data, string fileName, string filePath, string additionalPath = "")
		{
			var fullFolderPath = Path.Combine(filePath, additionalPath);

			if (Directory.Exists(fullFolderPath) == false)
			{
				Directory.CreateDirectory(fullFolderPath);
			}

			var fullFilePath = Path.Combine(fullFolderPath, fileName) + _fileExtenstion;
			var json = JsonUtility.ToJson(Data, true);
			File.WriteAllText(fullFilePath, json);
		}
	}
}