using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using MyPuzzle;

namespace MapEditor
{
    public enum SaveResult
    {
        Success,
        NoPuzzle,
        NoDifficult,
        NoLevel,
    }

    public class SaveComponent
        : MonoBehaviour
    {
        [SerializeField] private Dropdown DifficultDropdown;
        [SerializeField] private InputField LevelText;
        [SerializeField] private Text ErrorText;

        private void Awake()
        {
            this.ErrorText.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            var diffs = Config.GetDiffcultTypes();
            var l = new List<string>();
            foreach (var d in diffs)
                l.Add(d);

            this.DifficultDropdown.ClearOptions();
            this.DifficultDropdown.AddOptions(l);

            var currentDifficult = PuzzleComponent.Instance.CurrentDifficult;
            if (!string.IsNullOrEmpty(currentDifficult))
            {
                var i = this.DifficultDropdown.options.FindIndex(item => item.text == currentDifficult);
                if (i != -1)
                    this.DifficultDropdown.value = i;
            }

            var currentLevel = PuzzleComponent.Instance.CurrentLevel;
            this.LevelText.text = currentLevel.ToString();

        }

        private SaveResult CanSave()
        {
            if (PuzzleComponent.Instance.Puzzle == null)
                return SaveResult.NoPuzzle;

            if (DifficultDropdown.value < 0 || this.DifficultDropdown.options.Count < this.DifficultDropdown.value)
                return SaveResult.NoDifficult;

            if (string.IsNullOrEmpty(this.LevelText.text))
                return SaveResult.NoLevel;

            return SaveResult.Success;
        }

        private void DoSave()
        {
            var i = this.DifficultDropdown.value;
            var difficult = this.DifficultDropdown.options[i].text;
            var level = int.Parse(this.LevelText.text);

            PuzzleComponent.Instance.Puzzle.Save(difficult, level);
        }

        public void Save()
        {
            var r = this.CanSave();
            if (r != SaveResult.Success)
            {
                this.ShowError(r.ToString());
                return;
            }

            this.DoSave();
            this.HideError();
            this.gameObject.SetActive(false);
        }

        private void ShowError(string msg)
        {
            this.ErrorText.text = string.Format("Failed to Save! Reason: [{0}].", msg);
            this.ErrorText.gameObject.SetActive(true);
        }
        
        private void HideError()
        {
            if (this.ErrorText.gameObject.activeSelf)
                this.ErrorText.gameObject.SetActive(false);
        }
    }
}
