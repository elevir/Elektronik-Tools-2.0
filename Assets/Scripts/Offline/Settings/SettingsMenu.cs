﻿using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Elektronik.Common.Settings;
using Elektronik.Common.UI;
using System.IO;
using System.Linq;
using Elektronik.Offline.UI;

namespace Elektronik.Offline.Settings
{
    public class SettingsMenu : MonoBehaviour
    {
        public UIListBox lbRecentFiles;
        public Button buCancel;
        public Button buLoad;
        public InputField filePathField;
        public InputField imagePathField;
        public int MaxCountOfRecentFiles = 10;
        
        private SettingsHistory<OfflineSettingsBag> _settingsHistory;

        private void Awake()
        {
            ModeSelector.Mode = Mode.Offline;
        }

        // Use this for initialization
        private void Start()
        {
            _settingsHistory = new SettingsHistory<OfflineSettingsBag>(MaxCountOfRecentFiles);
            AttachBehavior2FileInput();
            AttachBehavior2Cancel();
            AttachBehavior2Load();
            AttachBehavior2RecentFiles();
        }

        private void AttachBehavior2FileInput()
        {
            filePathField
                .ObserveEveryValueChanged(o => o.text)
                .Do(t => buLoad.enabled = File.Exists(t))
                .Subscribe();
        }

        private void AttachBehavior2RecentFiles()
        {
            foreach (var recentFile in _settingsHistory.Recent)
            {
                var recentFileItem = lbRecentFiles.Add() as RecentFileListBoxItem;
                recentFileItem.Path = $"{recentFile.FilePath}\n{recentFile.ImagePath}";
                recentFileItem.DateTime = recentFile.ModificationTime;
            }
            if (_settingsHistory.Recent.Count > 0)
            {
                SettingsBag.SetCurrent(_settingsHistory.Recent.First());
            }
            else
            {
                SettingsBag.SetCurrent(new OfflineSettingsBag());
            }
            lbRecentFiles.OnSelectionChanged += RecentFileChanged;
        }

        private void RecentFileChanged(object sender, UIListBox.SelectionChangedEventArgs e)
        {
            var current = _settingsHistory.Recent[e.Index];
            SettingsBag.SetCurrent(current);
            filePathField.text = current.FilePath;
            imagePathField.text = current.ImagePath;
            var scalingField = GameObject.Find("Input scaling").GetComponent<InputField>();
            scalingField.text = current.Scale.ToString(CultureInfo.CurrentCulture);
        }

        private void AttachBehavior2Cancel()
        {
            buCancel.OnClickAsObservable()
                .Do(_ => SettingsBag.RemoveCurrent(typeof(OfflineSettingsBag)))
                .Do(_ => UnityEngine.SceneManagement.SceneManager.LoadScene("Main menu", UnityEngine.SceneManagement.LoadSceneMode.Single))
                .Subscribe();
        }

        private void AttachBehavior2Load()
        {
            var scalingField = GameObject.Find("Input scaling").GetComponent<InputField>();
            buLoad.OnClickAsObservable()
                .Select(_ => new OfflineSettingsBag())
                .Do(SettingsBag.SetCurrent)
                .Do(ofb => ofb.Scale = scalingField.text.Length == 0 ? 1.0f : float.Parse(scalingField.text))
                .Do(ofb => ofb.FilePath = filePathField.text)
                .Do(ofb => ofb.ImagePath = imagePathField.text)
                .Do(ofb => _settingsHistory.Add(ofb))
                .Do(_ => _settingsHistory.Save())
                .Do(_ => UnityEngine.SceneManagement.SceneManager.LoadScene("Empty", UnityEngine.SceneManagement.LoadSceneMode.Single))
                .Subscribe();
        }
    }
}