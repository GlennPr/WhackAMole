using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

namespace WhackAMole.Gameplay
{
    /// <summary>
    /// Visual assist class to MoleSpawn.cs
    /// Facilitates visual state changes and animations
    /// </summary>
    [RequireComponent(typeof(MoleSpawn))]
    public class MoleSpawnVisuals : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image _visualComponent;

        private Sequence _visualSequence;

        public void Kill()
		{
            DOTween.Kill(gameObject, false);
        }

        public void AnimateReveal(bool instant, Action onComplete)
        {
            _visualComponent.color = new Color(1, 1, 1, 0);

            Kill();
            _visualSequence = DOTween.Sequence(gameObject);
            _visualSequence.Append(_visualComponent.DOFade(1, 2f));

            if (onComplete != null)
            {
                _visualSequence.AppendCallback(() => onComplete.Invoke());
            }

            if (instant)
            {
                _visualSequence.Complete(true);
            }
        }

        public void AnimateHide(bool instant, Action onComplete)
        {
            Kill();
            _visualSequence = DOTween.Sequence(gameObject);
            _visualSequence.Append(_visualComponent.DOFade(0, 2f));

            if (onComplete != null)
            {
                _visualSequence.AppendCallback(() => onComplete.Invoke());
            }

            if (instant)
            {
                _visualSequence.Complete(true);
            }
        }

    }
}