using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GlennKorver.Utility
{
    /// <summary>
    /// This script makes a couple of asumptions
    /// - it assumes you want to make a perfectly round collider inside the Image Rect, regardless of the look/strech of the image
    /// - the radius is based on the smallest dimension (width or height), thus its "fits" inside the rect
    /// 
    /// </summary>
    public class CircularRaycastFilter : MonoBehaviour, ICanvasRaycastFilter
    {
        [SerializeField] Image _image;

        private void Start()
        {
            _image = GetComponent<Image>();
        }

        public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            Vector2 size = _image.rectTransform.rect.size;
            float radius = Mathf.Min(size.x, size.y) * 0.5f;

            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_image.rectTransform, screenPoint, eventCamera, out localPoint);

            // offset the localPoint later based on the Pivot values to work from the center of the circle 
            Vector2 pivot = _image.rectTransform.pivot;
            Vector2 localPointOffset = Vector2.zero;
            localPointOffset.x = ((pivot.x * 2) - 1) * radius;
            localPointOffset.y = ((pivot.y * 2) - 1) * radius;

            return (localPoint + localPointOffset).magnitude < radius;
        }
    }
}