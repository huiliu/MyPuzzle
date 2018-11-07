using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using MyPuzzle;

namespace MapEditor
{
    public class ColorInputField
        : MonoBehaviour
    {
        private InputField InputField;
        private void Awake()
        {
            this.InputField = this.GetComponent<InputField>();
            this.InputField.onEndEdit.AddListener(this.HandleOnValueChanged);
        }

        private void OnDisable()
        {
            this.InputField.text = "";
        }

        public MyColor MyColor;
        public int idx;
        public void Setup(MyColor myColor, int idx)
        {
            this.MyColor = myColor;
            this.idx = idx;

            this.Refresh();
        }

        private void Refresh()
        {
            var l = PuzzleComponent.Instance.Puzzle.Config.TagNums[this.MyColor];
            var v = this.idx < l.Count ? l[idx] : -1;

            var t = this.GetComponentInChildren<Text>();
            t.color = this.MyColor.ToColor();
            t.text = v.ToString();
        }

        private void HandleOnValueChanged(string v)
        {
            if (string.IsNullOrEmpty(v))
                return;

            PuzzleComponent.Instance.Puzzle.SetNum(this.idx, int.Parse(v), this.MyColor);
        }

        private void Update()
        {
            this.Refresh();
        }
    }
}
