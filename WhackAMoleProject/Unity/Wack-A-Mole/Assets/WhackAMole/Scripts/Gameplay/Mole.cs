using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using WhackAMole.Data;

namespace WhackAMole.Gameplay
{
    /// <summary>
    /// Is spawned by a MoleSpawn and will leave/disable itself  after X time or when tapped by the user
    /// </summary>
    public class Mole : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private MoleDataObject _defaultDataObject;

        [Header("References")]
        [SerializeField] private MoleVisuals _visuals;
        [SerializeField] private Button _buttonComponent;

        private enum State
        {
            IsActive,
            IsDefeated,
            IsLeaving,
            IsHidden
        }

        private State _state = State.IsHidden;

        private int _hp;
        private float _leaveTimestamp = 0.0f;

        private event Action<int> _onIncreaseScore;
        private event Action<Mole> _onComplete;

        private MoleDataObject _currentDataObject;
        private MoleData _currentData;

		public MoleData CurrentData { get => _currentData; }
        

        private void Awake()
        {
            _buttonComponent.onClick.AddListener(OnHit);
            gameObject.SetActive(false);
        }

        private void Update()
        {
            bool canLeave = _state == State.IsActive || _state == State.IsDefeated;
            if (canLeave && Time.time >= _leaveTimestamp)
            {
                Leave();
            }
        }

        public void Initialize(Action<int> onMoleDefeat)
        {
            _onIncreaseScore = onMoleDefeat;
            SetStats(_defaultDataObject);
        }

        public void SetStats(MoleDataObject dataObject)
        {
            _currentDataObject = dataObject;
        }

        public void Spawn(Action<Mole> onComplete)
        {
            bool isSpawnable = _currentDataObject != null;
            if (isSpawnable)
            {
                // swap out all settings on Spawn, ensures no settings changes occur while active
                _currentData = _currentDataObject.Data;
                _hp = CurrentData.MaxHP;
                _leaveTimestamp = Time.time + UnityEngine.Random.Range(CurrentData.MinAppearTime, CurrentData.MaxAppearTime);
                _onComplete = onComplete;

                _state = State.IsActive;
                gameObject.SetActive(true);
                SetInteractable(true);

                _visuals.AnimateReveal();
            }
            else // fallback
            {
                OnLeaveCompleted();
            }
        }

        /// <summary>
        /// Force a removal of the Mole
        /// </summary>
        /// <param name="instant"></param>
        public void Despawn(bool instant = false)
        {
            Leave(instant);
        }


        private void OnHit()
        {
            if (_state == State.IsActive)
            {
                // impact particles
                // visual effects?
                _hp--;
                _visuals.AnimateHit();

                if (_hp <= 0)
                {
                    OnDefeat();
                }
            }
        }

        private void OnDefeat()
        {
            if (_state == State.IsActive)
            {
                _state = State.IsDefeated;
                SetInteractable(false);

                _visuals.DefeatState();
                _leaveTimestamp = Time.time + 0.25f;

                // trigger score increase
                if (_onIncreaseScore != null) _onIncreaseScore.Invoke(CurrentData.Score);
            }
        }

        private void Leave(bool instant = false)
        {
            if (_state == State.IsHidden) // inactive thus, cant leave to begin with
			{
                return;
			}

            if (_state != State.IsLeaving || instant)
            {
                if (_state != State.IsDefeated) // ensure we only change our visuals when applicable
                {
                    _visuals.LeaveState();
                }

                _state = State.IsLeaving;
                SetInteractable(false);

                _visuals.AnimateLeave(instant,  OnLeaveCompleted);
            }
        }

        /// <summary>
        /// The Mole Deactivate's itself and notifies others of it having left the game
        /// </summary>
        private void OnLeaveCompleted()
        {
            _state = State.IsHidden;
            gameObject.SetActive(false);
            _onComplete.Invoke(this);
        }

        private void SetInteractable(bool value)
        {
            _buttonComponent.interactable = value;
        }
    }
}