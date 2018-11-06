using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using MyPuzzle;

namespace MapEditor
{
    public class ColorTextComponent
        : MonoBehaviour
    {
        private static string ObjectPoolTag = "TextInput";

        public int idx;
        public void Setup(int idx, bool isRow)
        {
            this.idx = idx;
            this.GetComponent<GridLayoutGroup>().startAxis = isRow ? GridLayoutGroup.Axis.Vertical : GridLayoutGroup.Axis.Horizontal;
            this.Refresh();
        }

        private int lastColorCount = 0;
        private void Update()
        {
            var count = PuzzleComponent.Instance.Puzzle.Config.TagNums.Keys.Count;
            if (this.lastColorCount != count)
            {
                this.lastColorCount = count;
                this.Refresh();
            }
        }

        private List<GameObject> inputs = new List<GameObject>();
        public void Refresh()
        {
            foreach (var t in this.inputs)
                ObjectPool.Instance.Return(ObjectPoolTag, t);
            this.inputs.Clear();
            var colors = PuzzleComponent.Instance.Puzzle.Config.TagNums.Keys;

            foreach (var c in colors)
            {
                var go = ObjectPool.Instance.Take(ObjectPoolTag);
                go.transform.SetParent(this.transform, false);
                go.SetActiveEx(true);

                var input = go.GetComponent<ColorInputField>();
                input.Setup(c, this.idx);

                this.inputs.Add(go);
            }
        }
    }
}
