using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyPuzzle;

public class PaletteComponent
    : MonoBehaviour
{
    [SerializeField] private GameObject ToggleTemplate;
    [HideInInspector] public MyColor CurrentColor { get; private set; }

    private GridLayoutGroup GridLayoutGroup;
    private RectTransform RectTransform;
    private ToggleGroup ToggleGroup;
    private void Awake()
    {
        this.GridLayoutGroup = this.GetComponent<GridLayoutGroup>();
        this.RectTransform = this.GetComponent<RectTransform>();
        this.ToggleGroup = this.gameObject.GetOrAddComponent<ToggleGroup>();
    }

    private MyColor DefaultColor;
    private List<GameObject> colors = new List<GameObject>();
    public void Init(List<MyColor> myColors, Vector2 sizeDelta)
    {
        this.ResetPalette();
        this.InitPosition(myColors.Count, sizeDelta);

        foreach(var c in myColors)
        {
            var go = Instantiate(this.ToggleTemplate);
            go.name = c.ToString();
            go.transform.SetParent(this.transform, false);
            go.SetActiveEx(true);

            var toggle = go.GetOrAddComponent<Toggle>();
            toggle.group = this.ToggleGroup;
            toggle.isOn = false;
            toggle.onValueChanged.AddListener(delegate { this.ToggleValueChanged(toggle, c); });

            var image = go.GetComponentInChildren<Image>();
            image.color = c.ToColor();

            this.colors.Add(go);
            this.DefaultColor = this.DefaultColor == MyColor.None ? c : this.DefaultColor;
        }

        this.CurrentColor = this.DefaultColor;
    }

    private void InitPosition(int cellCount, Vector2 wh)
    {
        var sizeDelta = this.RectTransform.sizeDelta;
        sizeDelta.x = this.GridLayoutGroup.cellSize.x;
        sizeDelta.y = cellCount * this.GridLayoutGroup.cellSize.y + this.GridLayoutGroup.spacing.y;
        this.RectTransform.sizeDelta = sizeDelta;

        var pos = wh / 2;
        pos.x += 64;
        pos.y = 0;

        this.transform.localPosition = pos;
    }

    public void ResetPalette()
    {
        this.DefaultColor = MyColor.None;
        foreach (var go in this.colors)
            GameObject.Destroy(go);

        this.colors.Clear();
    }

    private void ToggleValueChanged(Toggle toggle, MyColor myColor)
    {
        if (toggle.isOn)
            this.CurrentColor = myColor;
    }
}
