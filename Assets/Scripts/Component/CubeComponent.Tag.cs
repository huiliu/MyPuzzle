using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyPuzzle;

enum TagStatus
{
    None,
    Include,
    Absence,
}

public partial class CubeComponent
    : MonoBehaviour
{
    [SerializeField] private GameObject TagNode;
    [SerializeField] private Text TagTemplate;

    private Button Button;
    private void Awake()
    {
        this.Button = this.GetComponent<Button>();
        this.Button.onClick.AddListener(this.HandleClick);
    }

    private void InitTags(List<MyColor> myColors)
    {
        foreach(var c in myColors)
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
    public void HandleClick()
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
        foreach(var kvp in this.colorStatus)
        {
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
