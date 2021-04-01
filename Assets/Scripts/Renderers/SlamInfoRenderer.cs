﻿using System.Collections.Generic;
using Elektronik.Data.PackageObjects;
using Elektronik.UI;
using TMPro;
using UnityEngine;

namespace Elektronik.Renderers
{
    public class SlamInfoRenderer : MonoBehaviour, IDataRenderer<(string message, IEnumerable<ICloudItem> points)>
    {
        public TMP_Text MessageLabel;
        public TMP_Text PointMessageLabel;
        public ListBox PointButtonsBox;

        #region Unity events
        
        private void OnEnable()
        {
            IsShowing = true;
        }

        private void OnDisable()
        {
            IsShowing = false;
        }

        #endregion

        public bool IsShowing { get; private set; }

        public void Render((string message, IEnumerable<ICloudItem> points) data)
        {
            MessageLabel.text = $"Package message: {data.message}";
            foreach (var point in data.points)
            {
                var lbi = (SpecialInfoListBoxItem) PointButtonsBox.Add();
                lbi.Point = point;
                lbi.MessageLabel = PointMessageLabel;
            }
        }

        public void Clear()
        {
            MessageLabel.text = "";
            PointMessageLabel.text = "";
            PointButtonsBox.Clear();
        }
    }
}