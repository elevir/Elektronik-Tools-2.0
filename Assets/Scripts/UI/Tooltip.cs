﻿using Elektronik.UI.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Elektronik.UI
{
    public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public GameObject TooltipPrefab;
        public string TooltipText;
        public Vector2 Offset = new Vector2(0, 40);

        private GameObject _tooltip;
        private bool _isActive;
        private TMP_Text _label;

        #region Unity events

        private void Start()
        {
            _tooltip = Instantiate(TooltipPrefab, transform);
            _label = _tooltip.GetComponentInChildren<TMP_Text>();
            _tooltip.SetActive(false);
            _tooltip.GetComponent<RectTransform>().anchoredPosition = Offset;
        }

        #endregion

        #region IPoint*Handler

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isActive = true;
            _tooltip.SetActive(_isActive);
            _label.SetLocalizedText(TooltipText);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isActive = false;
            _tooltip.SetActive(_isActive);
        }

        #endregion
    }
}