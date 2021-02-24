﻿using System;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.Common.Settings
{
    public class ModeSelector : MonoBehaviour
    {
        public static Mode Mode = Mode.Online;// Mode.Invalid;
    
        public GameObject filePlayer;
        public GameObject onlinePlayer;
        public Text Label;

        private void Start()
        {
            switch (Mode)
            {
                case Mode.Offline:
                {
                    if (filePlayer != null) filePlayer.SetActive(true);
                    if (Label != null) Label.text = $"Offline {Label.text}";
                    break;
                }
                case Mode.Online:
                {
                    if (onlinePlayer != null) onlinePlayer.SetActive(true);
                    if (Label != null) Label.text = $"Online {Label.text}";
                    break;
                }
                default:
                    throw new NullReferenceException("No modes are initialized");
            }
        }
    }
}
