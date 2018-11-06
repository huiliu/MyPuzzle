using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyPuzzle;

namespace MapEditor
{
    [RequireComponent(typeof(ToggleGroup))]
    public class PaletteComponent
        : MonoBehaviour
    {
        [SerializeField]
        private GameObject ColorTemplate;

        public MyColor CurrentColor { get; private set; }
        private ToggleGroup ToggleGroup;

        public void InitPalette(List<MyColor> colors)
        {
            this.ToggleGroup = this.GetComponent<ToggleGroup>();
            // Warning: 如果增加新颜色，此处需要调整

            this.InitColor(MyColor.Red);
            this.InitColor(MyColor.Green);
            this.InitColor(MyColor.Blue);
            this.InitColor(MyColor.Yellow);

            foreach (var c in colors)
                this.color2Go[c].SetColorState(true);
        }

        private Dictionary<MyColor, PaletteColorComponent> color2Go = new Dictionary<MyColor, PaletteColorComponent>();
        private void InitColor(MyColor color)
        {
            if (this.color2Go.ContainsKey(color))
                return;

            var go = Instantiate(this.ColorTemplate);
            go.transform.SetParent(this.transform, false);
            go.SetActiveEx(true);

            var pcc = go.GetComponent<PaletteColorComponent>();
            pcc.SetColor(color);
            pcc.SetToggleGroup(this.ToggleGroup);
            pcc.OnColorChanged = this.HanldeColorChanged;
            pcc.OnColorSelected = this.HandleColorSelected;

            this.color2Go.Add(color, pcc);
        }

        private void HanldeColorChanged(bool flag, MyColor myColor)
        {
            if (flag)
                PuzzleComponent.Instance.AddColor(myColor);
            else
                PuzzleComponent.Instance.RemoveColor(myColor);
        }

        private void HandleColorSelected(MyColor myColor)
        {
            this.CurrentColor = myColor;
            Debug.Log(string.Format("当前选中颜色为：{0}", this.CurrentColor));
        }
    }
}
