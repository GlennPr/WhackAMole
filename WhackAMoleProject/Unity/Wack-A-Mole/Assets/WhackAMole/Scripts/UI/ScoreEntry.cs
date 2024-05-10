using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace WhackAMole.UI
{
    /// <summary>
    /// Basic Entry component to fill in names and scores
    /// </summary>
    public class ScoreEntry : MonoBehaviour
    {
        [SerializeField] private TMP_Text _nameField;
        [SerializeField] private TMP_Text _scoreField;
        [SerializeField] private Image _bgImageComponent;

        public void SetValues(string userName, int score, Color bgColor)
        {
            _nameField.text = userName.ToLowerInvariant();
            _scoreField.text = score.ToString().ToLowerInvariant();
            _bgImageComponent.color = bgColor;
        }
    }
}