using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WhackAMole.Save;
using WhackAMole.UI;

namespace WhackAMole.AppFlow
{
    /// <summary>
    /// Highscore screen ensures the Player's most recent score is added to the list and then displays the list
    /// </summary>
    public class HighScoreScreen : BaseScreen
    {
        [Header("References-Internal")]
        [SerializeField] private HighScoreList _highScoreList;
        [SerializeField] private Button _mainMenuButton;
        [SerializeField] private Button _gameButton;

        [Header("References-External")]
        [SerializeField] private BaseScreen _mainMenuScreen;
        [SerializeField] private BaseScreen _gameScreen;
        [SerializeField] private GameScore _gameScore;

        protected override void OnSetup()
        {
            _mainMenuButton.onClick.AddListener(() => ChangeToScreen(_mainMenuScreen));
            _gameButton.onClick.AddListener(() => ChangeToScreen(_gameScreen));
        }

        protected override void OnShow()
        {
            _canvasGroup.alpha = 1;

            // add current score
            int entryIndex;
            SaveManager.AddEntry(Main.PlayerName, System.DateTime.Now, _gameScore.Get(), out entryIndex);

            // save score to disk
            var currentSaveData = SaveManager.GetActiveSaveData();
            SaveManager.Save(currentSaveData);

            // get a block of values arround the player's score
            _highScoreList.RecreateList(currentSaveData.DataList, entryIndex);
        }

        protected override void OnHide(bool instantly)
        {
            _canvasGroup.alpha = 0;
        }
    }
}