using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

namespace WhackAMole.Gameplay
{
    /// <summary>
    /// Visual assist class to Mole.cs
    /// Facilitates visual state changes and animations
    /// </summary>
    [RequireComponent(typeof(Mole))]
    public class MoleVisuals : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform _interactionRect;
        [SerializeField] private Image _visualComponent;

        private Mole _mole;
        private Sequence _visualSequence;


		private void Awake()
		{
            _mole = GetComponent<Mole>();
        }

        private void Kill()
        {
            DOTween.Kill(gameObject, false);
        }

        public void AnimateReveal()
		{
            _visualComponent.sprite = _mole.CurrentData.ActiveVisual;
            _visualComponent.enabled = true;

            _visualComponent.transform.localScale = Vector3.zero;
            _interactionRect.transform.localScale = Vector3.one;

            Kill();
            _visualSequence = DOTween.Sequence(gameObject);
            _visualSequence.Append(_visualComponent.transform.DOScale(1.0f, 0.15f).SetEase(Ease.InOutBounce));
        }

        public void DefeatState()
		{
            _visualComponent.sprite = _mole.CurrentData.DefeatVisual;
        }

        public void LeaveState()
		{
            _visualComponent.sprite = _mole.CurrentData.EscapeVisual;
        }

        public void AnimateLeave(bool instant, Action onComplete)
		{
            Kill();
            _visualSequence = DOTween.Sequence(gameObject);
            _visualSequence.SetDelay(0.1f);// pacing, allow user to clearly see the escape visual
            _visualSequence.Append(_visualComponent.transform.DOScale(0.0f, 0.15f).SetEase(Ease.InOutBounce));

            if (onComplete != null)
            {
                _visualSequence.AppendCallback(() => onComplete.Invoke());
            }

            if (instant)
            {
                _visualSequence.Complete(true);
            }
        }

        public void AnimateHit()
		{
            float punchScaleDuration = 0.2f;
            _interactionRect.DOPunchScale(Vector3.one * 1.15f, punchScaleDuration, 10, 0.1f).SetId(gameObject);
        }
    }
}