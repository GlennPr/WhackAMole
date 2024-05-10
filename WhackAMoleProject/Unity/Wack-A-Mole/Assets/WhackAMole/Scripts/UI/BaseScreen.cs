using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WhackAMole.AppFlow
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class BaseScreen : MonoBehaviour
    {
        protected CanvasGroup _canvasGroup;
        private bool _isSetup = false;

        private void Setup()
        {
            if (_isSetup)
            {
                return;
            }

            _isSetup = true;

            gameObject.SetActive(true);
            _canvasGroup = GetComponent<CanvasGroup>();
            OnSetup();
        }

        protected abstract void OnSetup();

        public void Show()
        {
            Setup();
            _canvasGroup.blocksRaycasts = true;
            OnShow();
        }

        public void Hide(bool instantly = false)
        {
            Setup();
            _canvasGroup.blocksRaycasts = false;
            OnHide(instantly);
        }

        protected abstract void OnShow();

        protected abstract void OnHide(bool instantly = false);

        protected void ChangeToScreen(BaseScreen target)
        {
            Hide();
            target.Show();
        }
    }
}