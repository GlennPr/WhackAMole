using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

namespace GlennKorver.Utility
{
	[Serializable]
	public class SerializableList<T>
	{
		public List<T> DataList;
	}

	public static class JsonLoading
	{
		private static readonly string _fileExtenstion = ".json";

		public static T LoadDataFromStreaming<T, U>(string fileName) where T : SerializableList<U>, new()
		{
			return LoadData<T, U>(fileName, Application.streamingAssetsPath);
		}

		public static T LoadDataFromPersistent<T, U>(string fileName) where T : SerializableList<U>, new()
		{
			return LoadData<T, U>(fileName, Application.persistentDataPath);
		}


		private static T LoadData<T, U>(string fileName, string folderPath) where T : SerializableList<U>, new()
		{ 
			string fullFilePath = Path.Combine(folderPath, fileName) + _fileExtenstion;
			if (File.Exists(fullFilePath))
			{
				try
				{
					string loadedText = File.ReadAllText(fullFilePath);
					try
					{
						T loadedObject = JsonUtility.FromJson<T>(File.ReadAllText(fullFilePath));
						return loadedObject;
					}
					catch
					{
						Debug.Log("From Json conversion failed for : " + fullFilePath);
					}
				}
				catch
				{
					Debug.Log("Data not Readable at : " + fullFilePath);
				}
			}
			else
			{
				Debug.Log("Data not Found at : " + fullFilePath);
			}

			return null;
		}

		public static bool DoesJsonExist(string fileName, string folderPath)
		{
			string fullFilePath = Path.Combine(folderPath, fileName) + _fileExtenstion;
			return File.Exists(fullFilePath);
		}




		//		private string _androidDataText;
		//		private bool _isReady = false;
		//		public bool IsReady { get => _isReady; }


		//		private void Awake()
		//		{
		//			if (Directory.Exists(Application.persistentDataPath) == false)
		//			{
		//				Directory.CreateDirectory(Application.persistentDataPath);
		//			}
		//		}

		//		/// <summary>
		//		/// 
		//		/// </summary>
		//		/// <param name="fileName"></param> "Words.json"
		//		public void LoadData(string fileName)
		//		{
		//#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_EDITOR_OSX
		//			var text = LoadLocalizedText(fileName);

		//#elif UNITY_ANDROID
		//			StartCoroutine(LoadAndroidData());
		//#endif
		//		}

		//		IEnumerator LoadAndroidData(string fileName)
		//		{
		//			yield return LoadLocalizedTextOnAndroid(fileName);
		//			//_loadedRadicalData = JsonUtility.FromJson<RadicalWithWordsFull>("{\"content\":" + _androidDataText + "}");

		//			_isReady = true;
		//		}


		//		public string LoadLocalizedText(string fileName)
		//		{
		//			var path = Application.streamingAssetsPath;
		//			if (Application.isEditor == false && Directory.Exists(Application.persistentDataPath) && Directory.GetFiles(Application.persistentDataPath).Length > 0)
		//			{
		//				path = Application.persistentDataPath;
		//			}

		//			string filePath = Path.Combine(path + "/", fileName);
		//			if (File.Exists(filePath))
		//			{
		//				return File.ReadAllText(filePath);
		//			}
		//			else
		//			{
		//				Debug.LogError("Cannot find file! Path:: " + filePath);
		//				Debug.LogError("Cannot find file! Name:: " + fileName);
		//			}

		//			return null;
		//		}


		//		IEnumerator LoadLocalizedTextOnAndroid(string fileName)
		//		{
		//			Debug.Log(Application.persistentDataPath);

		//			var path = Application.streamingAssetsPath;
		//			if (Application.isEditor == false && Directory.Exists(Application.persistentDataPath) && Directory.GetFiles(Application.persistentDataPath).Length > 0)
		//			{
		//				path = Application.persistentDataPath;
		//			}

		//			string filePath = Path.Combine(path + "/", fileName);

		//			_androidDataText = null;

		//			if (filePath.Contains("://") || filePath.Contains(":///"))
		//			{
		//				//debugText.text += System.Environment.NewLine + filePath;
		//				Debug.Log("UNITY:" + System.Environment.NewLine + filePath);
		//				UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(filePath);
		//				yield return www.Send();
		//				_androidDataText = www.downloadHandler.text;
		//			}
		//			else
		//			{
		//				try
		//				{
		//					_androidDataText = File.ReadAllText(filePath);
		//				}
		//				catch
		//				{
		//					Debug.Log("Data not read : " + System.Environment.NewLine + filePath);
		//				}
		//			}
		//		}
	}
}
