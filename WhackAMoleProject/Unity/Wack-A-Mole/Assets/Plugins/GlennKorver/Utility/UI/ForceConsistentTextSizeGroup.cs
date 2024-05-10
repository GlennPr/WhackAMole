using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace GlennKorver.Utility
{
    [DefaultExecutionOrder(1)] // run after default execution
    public class ForceConsistentTextSizeGroup : MonoBehaviour
    {
        [SerializeField] private bool _includeAllChildren;
        [SerializeField] private List<TMP_Text> _aditionalTextFields = new List<TMP_Text>();

		private void LateUpdate()
		{
			if (Application.isPlaying)
			{
				ForceConsistentTextSize();
				this.enabled = false;
			}
		}

		[ContextMenu("ForceConsistentTextSize")]
        private void ForceConsistentTextSize()
        {
            if (_includeAllChildren)
            {
                _aditionalTextFields.AddRange(gameObject.GetComponentsInChildren<TMP_Text>());
            }

            if (_aditionalTextFields.Count == 0)
            {
                return;
            }

            // force update all text components with no limit in text size
            for (int i = 0; i < _aditionalTextFields.Count; i++)
            {
                var textComponent = _aditionalTextFields[i];

                textComponent.enableAutoSizing = true;
                textComponent.fontSizeMin = 1;
                textComponent.fontSizeMax = 10000;

                textComponent.ForceMeshUpdate(true, true);
            }

            // find smallest text size
            float smallestTextSize = 0;
            for (int i = 0; i < _aditionalTextFields.Count; i++)
            {
                var textComponent = _aditionalTextFields[i];
                var textSize = textComponent.fontSize;

                if (i == 0 || textSize < smallestTextSize)
                {
                    smallestTextSize = textSize;
                }
            }

            // force update all text components to match the smallest text size
            for (int i = 0; i < _aditionalTextFields.Count; i++)
            {
                var textComponent = _aditionalTextFields[i];
                textComponent.enableAutoSizing = false;
                textComponent.fontSize = smallestTextSize;
                textComponent.ForceMeshUpdate(true, true);
            }
        }
    }
}