using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace WhackAMole.UI
{
    public class GameScore : MonoBehaviour
    {   
        [Header("Settings")]
        [SerializeField] private Color _scoreGainedColor = Color.yellow;
        [SerializeField, Range(0.1f, 2f)] private float _scoreGainedEffectDuration = 0.8f;
        [SerializeField] private GlennKorver.Utility.Ease.EaseType _scoreGainedEffectEase;

        [Header("References")]
        [SerializeField] private TMP_Text _scoreFieldInGame;
        [SerializeField] private TMP_Text _scoreFieldFinal;

        private int _score;

        private Color _scoreColorOriginal;
        private float _scoreChangeTimeStamp;


        private void Awake()
		{
            _scoreColorOriginal = _scoreFieldInGame.color;
            _scoreChangeTimeStamp = Time.time - _scoreGainedEffectDuration;
        }

        //TODO make this not run when color is not going to change anyway
		private void Update()
		{
            float progress = Time.time - _scoreChangeTimeStamp;

            progress = Mathf.Clamp01(progress /= _scoreGainedEffectDuration);
            progress = 1 - Mathf.Abs((progress * 2) - 1);// map 0-1  to 0-1-0

            progress = GlennKorver.Utility.Ease.ApplyEase(progress, _scoreGainedEffectEase);
            _scoreFieldInGame.color = Color.Lerp(_scoreColorOriginal, _scoreGainedColor, progress);
        }

		public int Get()
        {
            return _score;
        }

        public void Set(int value, bool triggerEffect = true)
        {
            _score = value;
            _scoreFieldFinal.text = _scoreFieldInGame.text = _score.ToString();

            if (triggerEffect)
            {
                _scoreChangeTimeStamp = Time.time;
            }
        }

        public void Add(int value)
        {
            Set(_score + value, true);
        }
    }
}