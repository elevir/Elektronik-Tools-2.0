﻿using UniRx;
using UnityEngine.UI;

namespace Elektronik.UI.Fields
{
    public class BoolSettingField : SettingsField
    {
        public Toggle CheckBox;

        protected override void Start()
        {
            base.Start();
            if (CheckBox != null)
            {
                CheckBox.OnValueChangedAsObservable()
                        .Subscribe(v => SettingsBag.GetType().GetField(FieldName).SetValue(SettingsBag, v));
            }
        }
    }
}