﻿using System;
using System.Globalization;
using System.Reflection;
using Elektronik.Settings;
using UniRx;
using UnityEngine.UI;

namespace Elektronik.UI.Fields
{
    public class StringSettingsField : SettingsField
    {
        public InputField Field;

        protected override void Start()
        {
            var input = GetComponent<InputWithBrowse>();
            base.Start();
            if (input != null && IsDirectory())
            {
                input.FolderMode = true;
            }
            else if (input != null)
            {
                input.Filters = SettingsBag.GetType().GetField(FieldName).GetCustomAttribute<PathAttribute>().Extensions;
            }

            Field.OnValueChangedAsObservable().Subscribe(OnTextChanged);
        }

        private bool IsDirectory()
        {
            var attr = SettingsBag.GetType().GetField(FieldName).GetCustomAttribute<PathAttribute>();
            return attr != null && attr.PathType == PathAttribute.PathTypes.Directory;
        }

        private void OnTextChanged(string text)
        {
            if (FieldType == typeof(int))
            {
                if (int.TryParse(text, out int val))
                {
                    SettingsBag.GetType().GetField(FieldName).SetValue(SettingsBag, val);
                }
            }
            else if (FieldType == typeof(float))
            {
                if (float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out float val))
                {
                    SettingsBag.GetType().GetField(FieldName).SetValue(SettingsBag, val);
                }
            }
            else if (FieldType == typeof(string))
            {
                SettingsBag.GetType().GetField(FieldName).SetValue(SettingsBag, text);
            }
            else
            {
                throw new NotSupportedException("Other field types not implemented yet");
            }
        }
    }
}