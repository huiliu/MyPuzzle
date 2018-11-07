using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyPuzzle;

namespace MapEditor
{

    enum TagStatus
    {
        None,
        Include,
        Absence,
    }

    public partial class CubeComponent
    {
        [SerializeField] private GameObject TagNode;
        [SerializeField] private Text TagTemplate;

        private void OnEnable()
        {
            PuzzleComponent.Instance.OnColorUpdate += this.HandleColorUpdate;
        }

        private void OnDisable()
        {
            PuzzleComponent.Instance.OnColorUpdate -= this.HandleColorUpdate;
        }

        private void HandleColorUpdate()
        {
            this.InitTags(new List<MyColor>(PuzzleComponent.Instance.Puzzle.Config.TagNums.Keys));
        }

        private void InitTags(List<MyColor> myColors)
        {
            foreach (var kvp in this.colorTag)
                Destroy(kvp.Value.gameObject);
            this.colorTag.Clear();

            foreach (var c in myColors)
            {
                var go = Instantiate(this.TagTemplate);
                go.color = c.ToColor();
                go.transform.SetParent(this.TagNode.transform);
                go.gameObject.SetActive(true);
                this.colorTag[c] = go;
                this.colorStatus[c] = TagStatus.None;
            }

            this.RefreshTags();
        }

        private void ResetTags()
        {
            foreach (var s in this.colorTag)
                this.colorStatus[s.Key] = TagStatus.None;

            this.RefreshTags();
        }

        private Dictionary<MyColor, TagStatus> colorStatus = new Dictionary<MyColor, TagStatus>();
        private Dictionary<MyColor, Text> colorTag = new Dictionary<MyColor, Text>();
        private void HandleClick()
        {
            if (this.Cube.IsBlock)
                return;

            var currentColor = PuzzleComponent.Instance.CurrentColor;
            var currentStatus = this.colorStatus.ContainsKey(currentColor) ? this.colorStatus[currentColor] : TagStatus.None;

            switch (currentStatus)
            {
                case TagStatus.None:
                    colorStatus[currentColor] = TagStatus.Include;
                    break;
                case TagStatus.Include:
                    colorStatus[currentColor] = TagStatus.Absence;
                    break;
                case TagStatus.Absence:
                    colorStatus[currentColor] = TagStatus.None;
                    break;
                default:
                    break;
            }

            this.RefreshTags();
        }

        private void RefreshTags()
        {
            foreach (var kvp in this.colorStatus)
            {
                if (!this.colorTag.ContainsKey(kvp.Key))
                    continue;

                this.colorTag[kvp.Key].text = this.GetTag(kvp.Value);
                this.colorTag[kvp.Key].enabled = kvp.Value != TagStatus.None;
            }
        }

        private string GetTag(TagStatus status)
        {
            switch (status)
            {
                case TagStatus.None:
                    return "";
                case TagStatus.Include:
                    return "✔";
                case TagStatus.Absence:
                    return "✘";
                default:
                    return "";
            }
        }
    }
}
