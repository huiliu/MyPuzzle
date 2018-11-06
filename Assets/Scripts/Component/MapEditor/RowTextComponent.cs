using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace MapEditor
{
    public class RowTextComponent
        : MonoBehaviour
    {
        private static string ObjectPoolTag = "ColorText";

        [SerializeField] private GameObject Template;
        [SerializeField] private bool IsRow;

        private RectTransform RectTransform;
        private GridLayoutGroup GridLayoutGroup;
        private Vector2 cellSize;
        private Vector2 sizeDelta;
        private void Awake()
        {
            this.RectTransform = this.GetComponent<RectTransform>();
            this.sizeDelta = this.RectTransform.sizeDelta;

            this.GridLayoutGroup = this.GetComponent<GridLayoutGroup>();
            this.GridLayoutGroup.startAxis = this.IsRow ? GridLayoutGroup.Axis.Vertical : GridLayoutGroup.Axis.Horizontal;
            this.cellSize = this.GridLayoutGroup.cellSize;
        }

        private void Update()
        {
            if (this.IsRow)
                this.RefreshRow();
            else
                this.RefreshCol();
        }

        private List<GameObject> rowTexts = new List<GameObject>();
        private int lastRowCount = 0;
        private void RefreshRow()
        {
            var count = PuzzleComponent.Instance.Puzzle.Config.Row;
            if (this.lastRowCount == count)
                return;

            this.lastRowCount = count;
            foreach (var rt in this.rowTexts)
                ObjectPool.Instance.Return(ObjectPoolTag, rt);

            this.rowTexts.Clear();

            for (var i = 0; i < count; ++i)
            {
                var go = ObjectPool.Instance.Take(ObjectPoolTag);
                go.GetComponent<ColorTextComponent>().Setup(i, true);
                go.transform.SetParent(this.transform, false);
                go.SetActive(true);

                this.rowTexts.Add(go);
            }

            this.sizeDelta.y = count * cellSize.y;
            this.RectTransform.sizeDelta = sizeDelta;
        }

        private List<GameObject> colTexts = new List<GameObject>();
        private int lastColCount = 0;
        private void RefreshCol()
        {
            var count = PuzzleComponent.Instance.Puzzle.Config.Col;
            if (this.lastColCount == count)
                return;

            this.lastColCount = count;
            foreach (var rt in this.colTexts)
                ObjectPool.Instance.Return(ObjectPoolTag, rt);

            this.colTexts.Clear();

            for (var i = 0; i < count; ++i)
            {
                var go = ObjectPool.Instance.Take(ObjectPoolTag);
                go.GetComponent<ColorTextComponent>().Setup(i + this.lastColCount, false);
                go.transform.SetParent(this.transform, false);
                go.SetActive(true);

                this.colTexts.Add(go);
            }

            this.sizeDelta.x = count * cellSize.x;
            this.RectTransform.sizeDelta = sizeDelta;
        }
    }
}
