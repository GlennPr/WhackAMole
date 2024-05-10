using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;
using WhackAMole.Data;
using WhackAMole.Gameplay;
using WhackAMole.UI;

namespace WhackAMole.AppFlow
{
    /// <summary>
    /// Gamescreen facilitates the high level game flow, which inludes updating score and making spawn points avaialable
    /// once the times has run out we clear up all the spawn and display the final score
    /// </summary>
    public class GameScreen : BaseScreen
    {
        [SerializeField] private GameSettingsDataObject _settingsObject;

        [Header("References-Internal")]
        [SerializeField] private TMP_Text _timeRemainingField;
        [SerializeField] private GameScore _score;
        [SerializeField] private CanvasGroup _inGameStatsBar;
        [SerializeField] private CanvasGroup _resultPanel;
        [SerializeField] private GameObject _moleSpawnsContainer;

        [SerializeField] private Button _highScoreScreenButton;

        [Header("References-External")]
        [SerializeField] private BaseScreen _highScoreScreen;

        private List<MoleSpawn> _allSpawns = new List<MoleSpawn>();
        private List<MoleSpawn> _activeSpawns = new List<MoleSpawn>();
        private Queue<MoleSpawn> _deactivatedSpawns = new Queue<MoleSpawn>();

        private float _gameEndTimeStamp;

        private void Update()
        {
            UpdateTimeRemaining(_gameEndTimeStamp - Time.time);

            if (Time.time > _gameEndTimeStamp)
            {
                EndGame();
                return;
            }
        }

        protected override void OnSetup()
        {
            this.enabled = false;

            _moleSpawnsContainer.GetComponentsInChildren(true, _allSpawns);

            foreach (var spawn in _allSpawns)
            {
                spawn.Initialize(OnMoleCycleCompleted, OnMoleDefeat);
                spawn.OnHidden += OnSpawnHidden;
            }

            _highScoreScreenButton.onClick.AddListener(() => ChangeToScreen(_highScoreScreen));
        }

        protected override void OnShow()
        {
            DOTween.Kill(gameObject, true);

            _canvasGroup.alpha = 1;
            _inGameStatsBar.alpha = 1;
            _resultPanel.alpha = 0;
            _resultPanel.blocksRaycasts = false;

            _score.Set(0, false);
            UpdateTimeRemaining(_settingsObject.Data.PlaySessionDurationSec);

            // TODO add small delay

            StartGame();
        }

        protected override void OnHide(bool instantly)
        {
            _canvasGroup.alpha = 0;
        }

        /// <summary>
        /// Reset variables and reveal a couple of spawn points
        /// </summary>
        private void StartGame()
        {
            if (_allSpawns.Count == 0)
            {
                EndGame();
                return;
            }

            // initialize lifecycle variables
            this.enabled = true;
            _gameEndTimeStamp = Time.time + _settingsObject.Data.PlaySessionDurationSec;
            _activeSpawns.Clear();
            _deactivatedSpawns.Clear();

            SetupSpawns();
            StartCoroutine(SpawnsAppearRoutine());


            // Local Functions
            void SetupSpawns()
			{
                // decide which spawns are used from the start
                var allSpawnsTempCopy = new List<MoleSpawn>(_allSpawns);
                foreach (var spawn in allSpawnsTempCopy)
                {
                    spawn.Hide(true);
                }

                int activeCount = Mathf.Min(_settingsObject.Data.MaxSpawnActiveCount, allSpawnsTempCopy.Count);

                //active spawns
                for (int i = 0; i < activeCount; i++)
                {
                    var randomIndex = UnityEngine.Random.Range(0, allSpawnsTempCopy.Count);
                    var selectedSpawn = allSpawnsTempCopy[randomIndex];
                    allSpawnsTempCopy.RemoveAt(randomIndex);

                    _activeSpawns.Add(selectedSpawn);
                }

                //deactive spawns
                while (allSpawnsTempCopy.Count != 0)
                {
                    var randomIndex = UnityEngine.Random.Range(0, allSpawnsTempCopy.Count);
                    var selectedSpawn = allSpawnsTempCopy[randomIndex];
                    allSpawnsTempCopy.RemoveAt(randomIndex);

                    _deactivatedSpawns.Enqueue(selectedSpawn);
                }
            }

            IEnumerator SpawnsAppearRoutine()
            {
                var activationList = new List<MoleSpawn>(_activeSpawns);

                WaitUntil waitUntilReady;

                while (activationList.Count != 0)
                {
                    waitUntilReady = new WaitUntil(activationList[0].IsActive);
                    activationList[0].Show();

                    yield return waitUntilReady;

                    activationList.RemoveAt(0);
                }
            }
        }

        /// <summary>
        /// Hide away active spawn points and display final score
        /// </summary>
        private void EndGame()
        {
            StopAllCoroutines();
            this.enabled = false;

            var fadeTween = _inGameStatsBar.DOFade(0, 1f).SetId(gameObject);

            StartCoroutine(ContinueAfterHideSpawnsRoutine(ShowResultsPanel));


            //Local Functions
            IEnumerator ContinueAfterHideSpawnsRoutine(Action onComplete)
            {
                foreach (var spawn in _activeSpawns)
                {
                    spawn.Hide();
                }

                // wait
                while (_activeSpawns.Count != 0)
                {
                    yield return null;
                }

                // pacing
                yield return new WaitForSeconds(0.5f);

                // esnure all elements have faded out, incase they havent yet
                yield return fadeTween;

                if (onComplete != null)
                {
                    onComplete.Invoke();
                }
            }

            void ShowResultsPanel()
			{
                _resultPanel.blocksRaycasts = true;
                _resultPanel.DOFade(1, 0.5f).SetEase(Ease.InOutSine).SetId(gameObject);
            }
        }

        /// <summary>
        /// Given SpawnLocation is ready for new input, we either swap it out with another or provide it with a new type of Mole to spawn
        /// </summary>
        /// <param name="spawnLocation"></param>
        private void OnMoleCycleCompleted(MoleSpawn spawnLocation)
        {
            bool isGameStillRunning = this.enabled;
            if (isGameStillRunning)
            {
                if (UnityEngine.Random.value < 0.15f)
                {
                    SwapOutSpawnPoints();
                    return;
                }

                spawnLocation.SetMoleType(_settingsObject.Data.GetMole());
            }

            // Local functions

            /// <summary>
            /// Disable the current spawn and reveal a new one
            /// </summary>
            void SwapOutSpawnPoints()
			{
                MoleSpawn activatableSpawn;
                if (_deactivatedSpawns.TryDequeue(out activatableSpawn))
                {
                    spawnLocation.Hide();

                    activatableSpawn.Show();
                    _activeSpawns.Add(activatableSpawn);
                }
            }
        }

        private void OnMoleDefeat(int value)
        {
            // TODO screen shake / global effect

            _score.Add(value * _settingsObject.Data.ScoreMultiplier);
        }


        private void OnSpawnHidden(MoleSpawn value)
        {
            if (_activeSpawns.Contains(value))
            {
                _activeSpawns.Remove(value);
                _deactivatedSpawns.Enqueue(value);
            }
        }

        private void UpdateTimeRemaining(float timeRemaining)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds((double)Mathf.Max(timeRemaining, 0));
            _timeRemainingField.text = timeSpan.ToString(@"mm\:ss");
        }

    }

}
