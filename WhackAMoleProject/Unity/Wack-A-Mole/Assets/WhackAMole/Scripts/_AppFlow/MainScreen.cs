using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WhackAMole.AppFlow
{
    /// <summary>
    /// Main screen
    /// </summary>
    public class MainScreen : BaseScreen
    {
        [Header("References-Internal")]
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _settingsButton;

        [Header("References-External")]
        [SerializeField] private BaseScreen _gameScreen;

        protected override void OnSetup()
        {
            _startButton.onClick.AddListener(() => ChangeToScreen(_gameScreen));
        }

        protected override void OnShow()
        {
            _canvasGroup.alpha = 1;
        }

        protected override void OnHide(bool instantly)
        {
            _canvasGroup.alpha = 0;
        }
    }
}