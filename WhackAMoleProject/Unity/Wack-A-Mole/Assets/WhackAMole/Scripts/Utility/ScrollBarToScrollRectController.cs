using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Linking the 'Bar' to the 'Rect' in the 'Rect's' panel, makes it so that some control over the bar is lost, thus this custom implementation
/// </summary>

namespace WhackAMole.Utility
{
    public class ScrollBarToScrollRectController : MonoBehaviour
    {
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private Scrollbar _scrollBar;

        private void Awake()
        {
            if (_scrollRect == null || _scrollBar == null)
            {
                return;
            }

            _scrollRect.onValueChanged.AddListener(OnScrollRectValueChanged_ByTouch);
            _scrollBar.onValueChanged.AddListener(OnScrollBarValueChanged_ByTouch);
        }

		public void SetBarHandlePosition(float value, bool updateScrollRect, bool notifyListeners)
        {
            if (notifyListeners)
            {
                _scrollBar.value = value;

                if (updateScrollRect)
                {
                    OnScrollBarValueChanged_ByAny(value);
                }
            }
            else
            {
                _scrollBar.SetValueWithoutNotify(value);

                if (updateScrollRect)
                {
                    OnScrollBarValueChanged_ByAny(value);
                }
            }
        }


        #region Manipulate ScrollRect
        private void OnScrollBarValueChanged_ByTouch(float value)
        {
            OnScrollBarValueChanged_ByAny(value);
        }

        private void OnScrollBarValueChanged_ByAny(float value)
        {
            switch (_scrollBar.direction)
            {
                case Scrollbar.Direction.TopToBottom:
                    _scrollRect.verticalNormalizedPosition = 1f - value;
                    break;

                case Scrollbar.Direction.BottomToTop:
                    _scrollRect.verticalNormalizedPosition = value;
                    break;

                case Scrollbar.Direction.LeftToRight:
                    _scrollRect.verticalNormalizedPosition = 1f - value;
                    break;

                case Scrollbar.Direction.RightToLeft:
                    _scrollRect.verticalNormalizedPosition = value;
                    break;
            }
        }
        #endregion

        #region Manipulate ScrollBar
        private void OnScrollRectValueChanged_ByTouch(Vector2 value)
        {
            OnScrollRectValueChanged_ByAny(value);
        }

        private void OnScrollRectValueChanged_ByAny(Vector2 value)
        {
            if (_scrollRect.vertical)
            {
                float normalizedPos = _scrollRect.verticalNormalizedPosition;
                switch (_scrollBar.direction)
                {
                    case Scrollbar.Direction.TopToBottom:
                        SetBarHandlePosition(1f - normalizedPos, false, false);
                        break;

                    case Scrollbar.Direction.BottomToTop:
                        SetBarHandlePosition(normalizedPos, false, false);
                        break;
                }
            }

            if (_scrollRect.horizontal)
            {
                float normalizedPos = _scrollRect.horizontalNormalizedPosition;
                switch (_scrollBar.direction)
                {
                    case Scrollbar.Direction.LeftToRight:
                        SetBarHandlePosition(1f - normalizedPos, false, false);
                        break;

                    case Scrollbar.Direction.RightToLeft:
                        SetBarHandlePosition(normalizedPos, false, false);
                        break;
                }
            }
        }
		#endregion
	}
}
