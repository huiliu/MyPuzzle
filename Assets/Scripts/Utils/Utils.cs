using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyPuzzle
{
    //  
    public enum MyColor
    {
        None = 0,
        Red = 1,
        Blue = 2,
        Green = 3,
        Yellow = 4,
    }

    public enum Direction
    {
        None = 0,
        Up = 1,
        Down = 2,
        Left = 4,
        Right = 8,
    }

    public class Pos
    {
        public int R;
        public int C;

        public Pos(int r, int c)
        {
            this.R = r;
            this.C = c;
        }

        public void Copy(Pos pos)
        {
            this.R = pos.R;
            this.C = pos.C;
        }

        public bool Equal(Pos p)
        {
            return p.R == this.R && p.C == this.C;
        }
    }

    public static class Utils
    {
        public static string ColorToString(MyColor color)
        {
            switch (color)
            {
                case MyColor.Red: return "r";
                case MyColor.Blue: return "b";
                case MyColor.Green: return "g";
                case MyColor.Yellow: return "y";
                default: return "n";
            }
        }

        public static MyColor StringToMyColor(string str)
        {
            switch (str)
            {
                case "r": return MyColor.Red;
                case "b": return MyColor.Blue;
                case "g": return MyColor.Green;
                case "y": return MyColor.Yellow;
                default: return MyColor.None;
            }
        }

        public static Color MyColorToColor(MyColor color)
        {
            switch (color)
            {
                case MyColor.Red: return Color.red;
                case MyColor.Blue: return Color.blue;
                case MyColor.Green: return Color.green;
                case MyColor.Yellow: return Color.yellow;
                default: return Color.white;
            }
        }

        public static Color ToColor(this MyColor c)
        {
            return MyColorToColor(c);
        }

        public static Vector2 ToVector2(this Vector3 v)
        {
            return v;
        }

        public static Direction ToDirection(this Vector2 dir)
        {
            if (Mathf.Abs(dir.x) < float.Epsilon &&
                Mathf.Abs(dir.y) < float.Epsilon)
                return Direction.None;

            if (Mathf.Abs(dir.x) < float.Epsilon)
                return dir.y < 0 ? Direction.Down : Direction.Up;

            if (Mathf.Abs(dir.y) < float.Epsilon)
                return dir.x < 0 ? Direction.Left : Direction.Right;

            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                return dir.x < 0 ? Direction.Left : Direction.Right;
            else if (Mathf.Abs(dir.x) < Mathf.Abs(dir.y))
                return dir.y < 0 ? Direction.Down : Direction.Up;

            return Direction.None;
        }

        public static void SetActiveEx(this GameObject go, bool flag)
        {
            if (go == null)
                return;

            if (go.activeSelf == flag)
                return;

            go.SetActive(flag);
        }

        public static Direction GetOppositeDirection(Direction direct)
        {
            switch (direct)
            {
                case Direction.Up: return Direction.Down;
                case Direction.Down: return Direction.Up;
                case Direction.Left: return Direction.Right;
                case Direction.Right: return Direction.Left;
                default:return Direction.None;
            }
        }
    }
}
