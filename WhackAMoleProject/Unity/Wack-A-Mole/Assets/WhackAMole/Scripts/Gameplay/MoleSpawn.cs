using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using WhackAMole.Data;

namespace WhackAMole.Gameplay
{
    /// <summary>
    /// Spawns Mole.cs
    /// Will reveal and hide if instructed by other classes
    /// Holds a OnHidden subscribable event 
    /// </summary>
    public class MoleSpawn : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private MoleSpawnVisuals _visuals;
        [SerializeField] private Mole _mole;

        private enum State
        {
            Hiding,
            IsHidden,
            Showing,
            IsShown
        }

        private State _state = State.IsHidden;

        private float _nextMoleSpawnTimestamp;
        private bool _isMoleSpawned = false;
        private event Action<MoleSpawn> _onMoleCycleCompleted;

        public event Action<MoleSpawn> OnHidden;


        private void Update()
        {
            if (IsActive())
            {
                if (_isMoleSpawned == false && Time.time > _nextMoleSpawnTimestamp)
                {
                    SpawnMole();
                }
            }
        }

        public void Initialize(Action<MoleSpawn> onMoleCycleCompleted, Action<int> onMoleDefeat)
        {
            _onMoleCycleCompleted = onMoleCycleCompleted;
            _mole.Initialize(onMoleDefeat);

            Hide(true);
        }

        public void Show()
        {
            if (_state == State.Showing)
            {
                return;
            }

            _state = State.Showing;
            gameObject.SetActive(true);
            _mole.gameObject.SetActive(false);

            // ensure no lingering animation / callbacks will be executed
            StopAllCoroutines();

            _visuals.AnimateReveal(false, ActivateAfterReveal);

            void ActivateAfterReveal()
            {
                _nextMoleSpawnTimestamp = 0.0f;
                _state = State.IsShown;
            }

        }

        /// <summary>
        /// Make the Spawn hide away and disable itself, after hiding away the Mole its spawned
        /// </summary>
        /// <param name="instant"></param>
        public void Hide(bool instant = false)
        {
            if(_state == State.IsHidden && instant == false)
			{
                return;
			}

            _state = State.Hiding;

            // ensure no lingering animation / callbacks will be executed
            StopAllCoroutines();
            _visuals.Kill();

            if (instant)
            {
                HideMole();
                HideSelf();
            }
            else
            {
                HideMole();
                StartCoroutine(ContinueAfterMoleHidingRoutine(HideSelf));
            }

            //Local functions
            void HideMole()
            {
                if (_isMoleSpawned)
                {
                    _mole.Despawn(instant);
                }
            }

            void HideSelf()
            {
                _visuals.AnimateHide(instant, HideSelfCompleted);     
            }

            void HideSelfCompleted()
            {
                gameObject.SetActive(false);
                _state = State.IsHidden;

                if (OnHidden != null)
                {
                    OnHidden.Invoke(this);
                }
            }

            IEnumerator ContinueAfterMoleHidingRoutine(Action onCompletion)
            {
                while (_isMoleSpawned)
                {
                    yield return null;
                }

                onCompletion.Invoke();
            }
        }


        public void SetMoleType(MoleDataObject dataObject)
        {
            _mole.SetStats(dataObject);
        }
        public bool IsActive()
        {
            return _state == State.IsShown;
        }



        private void SpawnMole()
        {
            if (_isMoleSpawned == false)
            {
                _isMoleSpawned = true;
                _mole.Spawn(OnMoleLifecycleCompleted);
            }
        }

        private void OnMoleLifecycleCompleted(Mole mole)
        {
            _isMoleSpawned = false;
            _onMoleCycleCompleted.Invoke(this);

            _nextMoleSpawnTimestamp = Time.time + UnityEngine.Random.Range(0.45f, 1.25f);
        }



    }
}