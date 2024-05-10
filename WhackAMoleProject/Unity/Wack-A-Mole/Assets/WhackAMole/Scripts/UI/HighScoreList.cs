using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WhackAMole.Data;
using WhackAMole.Utility;

namespace WhackAMole.UI
{
    /// <summary>
    /// Manages a highscore list
    /// ensure all fields are created and filled in properly
    /// </summary>
    public class HighScoreList : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int _displayScoreCap = 30;
        [SerializeField] private Color _entryColorTintA = Color.white;
        [SerializeField] private Color _entryColorTintB = Color.grey;
        [SerializeField] private Color _personalEntryColorTint = Color.yellow;
        [SerializeField] private Color _personalRecentEntryColorTint = Color.red;

        [Header("References-Internal")]
        [SerializeField] private RectTransform _scoreEntryContainer;
        [SerializeField] private ScrollBarToScrollRectController _scrollController;
   
        [Header("References-Project")]
        [SerializeField] private ScoreEntry _scoreEntryPrefab;

        /// <summary>
        /// Create a list of Scores no greater then '_displayScoreCap'
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="highScores"> List of data from which a section or all will be translated to the UI</param>
        /// <param name="tryCenterOnIndex"> Index around which we will select other entries </param>
        public void RecreateList<T>(IReadOnlyList<T> highScores, int tryCenterOnIndex) where T : UserHighScore
        {
            // Ensure list is properly filled
            int targetEntryCount = Mathf.Min(highScores.Count, _displayScoreCap);
            if (_scoreEntryContainer.childCount < targetEntryCount)
            {
                for (int i = _scoreEntryContainer.childCount; i < targetEntryCount; i++)
                {
                    Instantiate(_scoreEntryPrefab, _scoreEntryContainer);
                }
            }
            else if (_scoreEntryContainer.childCount > targetEntryCount)
            {
                for (int i = _scoreEntryContainer.childCount - 1; i > targetEntryCount - 1; i--)
                {
                    Destroy(_scoreEntryContainer.GetChild(i).gameObject);
                }
            }

            //center entries around "tryCenterOnIndex"
            int lowerBoundsIndex = tryCenterOnIndex - Mathf.CeilToInt((float)_displayScoreCap / 2);
            lowerBoundsIndex = Mathf.Max(0, lowerBoundsIndex);

            // Update available entries
            for (int i = 0; i < _scoreEntryContainer.childCount; i++)
            {
                var bgColor = (i % 2 == 0) ? _entryColorTintA: _entryColorTintB;

                var entryDataIndex = lowerBoundsIndex + i; // offset entry index by lowerBoundsIndex
                var scoreData = highScores[entryDataIndex];
                var dateTime = new System.DateTime(scoreData.Ticks);
                var userNameWithDate = scoreData.UserName + "|" + dateTime.ToShortDateString();

                if (entryDataIndex == tryCenterOnIndex)
                {
                    bgColor *= _personalRecentEntryColorTint;
                }
                else
                {
                    bool isEntryOfCurrentPlayer = scoreData.UserName.ToLowerInvariant() == AppFlow.Main.PlayerName.ToLowerInvariant();
                    if (isEntryOfCurrentPlayer)
                    {
                        bgColor *= _personalEntryColorTint;
                    }
                }

                var scoreEntry = _scoreEntryContainer.GetChild(i).GetComponent<ScoreEntry>();
                scoreEntry.SetValues(userNameWithDate, scoreData.Score, bgColor);
            }

            // force container to be updated (trigger its contentSize fitter)
            LayoutRebuilder.ForceRebuildLayoutImmediate(_scoreEntryContainer);

            // force scrolling to start at 'tryCenterOnIndex'
            var scrollStartValue = Mathf.Clamp01((float)(tryCenterOnIndex - lowerBoundsIndex) / (float)targetEntryCount);
            _scrollController.SetBarHandlePosition(scrollStartValue, true, true);
        }
    }
}
