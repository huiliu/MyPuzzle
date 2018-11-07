using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace MyPuzzle
{
    public class Cube
    {
        public MyColor UpColor { get; private set; }
        public MyColor DownColor { get; private set; }
        public MyColor LeftColor { get; private set; }
        public MyColor RightColor { get; private set; }
        public bool IsBlock { get; private set; }
        public bool IsDirty { get; set; }

        public Cube()
        {
            this.UpColor = MyColor.None;
            this.DownColor = MyColor.None;
            this.LeftColor = MyColor.None;
            this.RightColor = MyColor.None;
            this.IsBlock = false;
        }

        public Cube(MyColor[] colors, bool isBlock)
        {
            Debug.Assert(colors.Length == 4);
            this.UpColor = colors[0];
            this.DownColor = colors[1];
            this.LeftColor = colors[2];
            this.RightColor = colors[3];
            this.IsBlock = isBlock;
            this.IsDirty = true;
        }

        public bool IsConnectTo(Direction direct)
        {
            return getColorByDirect(direct) != MyColor.None;
        }

        public bool IsConnectTo(Direction direct, MyColor color)
        {
            return getColorByDirect(direct) == color;
        }

        public bool IsCross(MyColor color)
        {
            return this.UpColor == color && this.DownColor == color && this.LeftColor == color && this.RightColor == color;
        }

        public bool HasOtherColor(MyColor color)
        {
            if (this.UpColor != MyColor.None && this.UpColor != color)
                return true;

            if (this.DownColor != MyColor.None && this.DownColor != color)
                return true;

            if (this.LeftColor != MyColor.None && this.LeftColor != color)
                return true;

            if (this.RightColor != MyColor.None && this.RightColor != color)
                return true;

            return false;
        }

        // 该方块上有几条连线
        public int ConnectionNum()
        {
            int num = 0;
            if (this.UpColor != MyColor.None) num++;
            if (this.DownColor != MyColor.None) num++;
            if (this.LeftColor != MyColor.None) num++;
            if (this.RightColor != MyColor.None) num++;
            return num;
        }

        // 该方块上有几条指定颜色的连线
        public int ConnectionNum(MyColor color)
        {
            int num = 0;
            if (this.UpColor == color) num++;
            if (this.DownColor == color) num++;
            if (this.LeftColor == color) num++;
            if (this.RightColor == color) num++;
            return num;
        }

        public void SetColor(Direction direct, MyColor color)
        {
#if ! EditMode
            if (this.IsBlock)
                return;
#endif

            switch (direct)
            {
                case Direction.Up: this.UpColor = color; break;
                case Direction.Down: this.DownColor = color; break;
                case Direction.Left: this.LeftColor = color; break;
                case Direction.Right: this.RightColor = color; break;
                default: return;
            }
        }

        public void Clear(Direction direct)
        {
#if ! EditMode
            if (this.IsBlock)
                return;
#endif

            switch (direct)
            {
                case Direction.Up: this.UpColor = MyColor.None; break;
                case Direction.Down: this.DownColor = MyColor.None; break;
                case Direction.Left: this.LeftColor = MyColor.None; break;
                case Direction.Right: this.RightColor = MyColor.None; break;
                default: break;
            }

            this.IsDirty = true;
        }

        public void SetColors(MyColor[] colors)
        {
            this.UpColor = colors[0];
            this.DownColor = colors[1];
            this.LeftColor = colors[2];
            this.RightColor = colors[3];
        }

        public void SetBlockState(bool isBlock)
        {
            this.IsBlock = isBlock;
            this.IsDirty = true;
        }

        public void Reset()
        {
#if ! EditMode
            if (this.IsBlock)
                return;
#endif

            this.UpColor = MyColor.None;
            this.DownColor = MyColor.None;
            this.LeftColor = MyColor.None;
            this.RightColor = MyColor.None;
            this.IsBlock = false;
            this.IsDirty = true;
        }

        private MyColor getColorByDirect(Direction direct)
        {
            switch (direct)
            {
                case Direction.Up: return this.UpColor;
                case Direction.Down: return this.DownColor;
                case Direction.Left: return this.LeftColor;
                case Direction.Right: return this.RightColor;
                default : return MyColor.None;
            }
        }
    }
}
