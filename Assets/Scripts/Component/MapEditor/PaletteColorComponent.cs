using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using MyPuzzle;

namespace MapEditor
{
    public class PaletteColorComponent
        : MonoBehaviour
    {
        public Action<bool, MyColor>       OnColorChanged;
        public Action<MyColor>    OnColorSelected;

        [SerializeField] private Toggle ColorToggle;
        [SerializeField] private Toggle SelectToggle;

        private void Awake()
        {
            this.ColorImage = this.ColorToggle.gameObject.GetComponentInChildren<Image>();

            this.ColorToggle.isOn = false;
            this.ColorToggle.onValueChanged.AddListener((b) =>
            {
                this.OnColorChanged.SafeInvoke(b, this.MyColor);
            });

            this.SelectToggle.isOn = false;
            this.SelectToggle.onValueChanged.AddListener((b) =>
            {
                if (b && this.ColorToggle.isOn)
                    this.OnColorSelected.SafeInvoke(this.MyColor);
                else if (b)
                    Debug.Log(string.Format("当前颜色[{0}]无效!", this.MyColor));
            });
        }

        private MyColor MyColor;
        private Image ColorImage;
        public void SetColor(MyColor myColor)
        {
            this.ColorImage.color = myColor.ToColor();
            this.MyColor = myColor;
        }

        public void SetColorState(bool isOn)
        {
            this.ColorToggle.isOn = isOn;
        }

        public void SetSelected(bool select)
        {
            this.SelectToggle.isOn = select;
        }

        public void SetToggleGroup(ToggleGroup toggleGroup)
        {
            this.SelectToggle.group = toggleGroup;
        }
    }
}
