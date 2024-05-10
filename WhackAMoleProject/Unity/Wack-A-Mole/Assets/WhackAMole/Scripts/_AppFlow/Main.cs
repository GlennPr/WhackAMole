using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WhackAMole.Save;

namespace WhackAMole.AppFlow
{
    /// <summary>
    /// Starting point of the application
    /// Only responsibility is to gurantee the neccesary files for the application to run are created
    /// </summary>
    public class Main : MonoBehaviour
    {
        public static readonly string PlayerName = "YOU";

        [SerializeField] private MainScreen _mainScreen;

        private void Start()
        {
            // setup frame rate
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;

            // setup save data
            GuranteeHighScores();
            SaveManager.Load(true);

            // setup screens
            var allScreens = GetComponentsInChildren<BaseScreen>(true);
            foreach(var screen in allScreens)
			{
                screen.Hide(true);
            }

            // start the first Screen
            _mainScreen.Show();


            //Local Functions
            void GuranteeHighScores()
            {
                if (SaveManager.DoesSaveExistOnDisk() == false)
                {
                    HighScoreListGenerator.Generate();
                }
            }
        }
    }
}