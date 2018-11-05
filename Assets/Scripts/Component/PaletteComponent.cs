using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyPuzzle;

public class PaletteComponent
    : MonoBehaviour
{
    [SerializeField] private GameObject ToggleTemplate;
    [HideInInspector] public MyColor CurrentColor { get; private set; }

    private ToggleGroup ToggleGroup;
    private void Awake()
    {
        this.ToggleGroup = this.gameObject.GetOrAddComponent<ToggleGroup>();
    }

    private MyColor DefaultColor;
    private List<GameObject> colors = new List<GameObject>();
    public void Init(List<MyColor> myColors)
    {
        this.ResetPalette();

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
