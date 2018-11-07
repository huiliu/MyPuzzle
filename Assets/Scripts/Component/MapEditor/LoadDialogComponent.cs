using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using MyPuzzle;

namespace MapEditor
{
    public class LoadDialogComponent
        : MonoBehaviour
    {
        [SerializeField] private GameObject PuzzleNode;
        [SerializeField] private Dropdown DifficultDropdown;
        [SerializeField] private Dropdown LevelDropdown;
        [SerializeField] private Text ErrorText;

        private List<string> diffcultDatas = new List<string>();
        private List<string> levelDatas = new List<string>();

        private void Awake()
        {
            this.LevelDropdown.ClearOptions();
            this.DifficultDropdown.ClearOptions();
            this.DifficultDropdown.onValueChanged.AddListener((v) =>
            {
                var i = this.DifficultDropdown.value;
                this.RefreshLevelOptions(this.DifficultDropdown.options[i].text);
            });

            this.ErrorText.gameObject.SetActiveEx(false);
        }

        private void OnEnable()
        {
            this.InitDifficulOptions();
        }

        private void OnDisable()
        {
            this.diffcultDatas.Clear();
            this.DifficultDropdown.ClearOptions();
        }

        private void InitDifficulOptions()
        {
            var diffs = Config.GetDiffcultTypes();
            foreach(var item in diffs)
                this.diffcultDatas.Add(item);

            this.DifficultDropdown.AddOptions(diffcultDatas);

            this.RefreshLevelOptions(this.DifficultDropdown.options[0].text);
        }

        private void RefreshLevelOptions(string difficult)
        {
            if (string.IsNullOrEmpty(difficult))
                return;

            this.levelDatas.Clear();
            var levels = Config.GetQuizsByDifficulty(difficult);
            foreach (var i in levels)
                this.levelDatas.Add(i);

            this.LevelDropdown.ClearOptions();
            this.LevelDropdown.AddOptions(this.levelDatas);
        }

        public void LoadQuiz()
        {
            var i = this.DifficultDropdown.value;
            var j = this.LevelDropdown.value;

            if (i > this.DifficultDropdown.options.Count ||
                j > this.LevelDropdown.options.Count)
                return;

            var difficult = this.DifficultDropdown.options[i].text;
            if (string.IsNullOrEmpty(difficult))
                return;

            var level = 0;
            var r = int.TryParse(this.LevelDropdown.options[j].text, out level);
            if (!r)
            {
                this.ShowErrorMessage(string.Format("解析Level[{0}]失败!", this.LevelDropdown.options[j].text));
                return;
            }

            this.PuzzleNode.SetActiveEx(true);
            PuzzleComponent.Instance.LoadPuzzle(difficult, level);
            this.HideErrorMessage();
            this.gameObject.SetActiveEx(false);
        }

        private void ShowErrorMessage(string msg)
        {
            this.ErrorText.text = msg;
            this.ErrorText.gameObject.SetActiveEx(true);
        }

        private void HideErrorMessage()
        {
            this.ErrorText.gameObject.SetActiveEx(false);
        }
    }
}
