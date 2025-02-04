﻿using System;
using Elektronik.Settings.Bags;
using Elektronik.UI.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI.Fields
{
    public class SettingsField : MonoBehaviour
    {
        public SettingsBag SettingsBag;
        public string FieldToolTip;
        public string FieldName;
        public Type FieldType;
        
        [SerializeField]
        private Text Tooltip;
        
        protected virtual void Start()
        {
            Tooltip.SetLocalizedText(FieldToolTip);
        }
    }
}