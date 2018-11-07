using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Linq;

namespace MyPuzzle
{
    public class EditPuzzle : Puzzle
    {
        public EditPuzzle(string config) : base(config)
        {
            this.IsEditMode = true;
        }

        public void AddRow()
        {
            this.Config.Row++;

            Dictionary<MyColor, List<int>> newTagNums = new Dictionary<MyColor, List<int>>();
            foreach (var kvp in this.Config.TagNums)
            {
                List<int> nums = kvp.Value;
                nums.Insert(this.Config.Row - 1, -1);
                newTagNums.Add(kvp.Key, nums);
            }
            this.Config.TagNums = newTagNums;

            Cube[,] newCubes = new Cube[this.Config.Row, this.Config.Col];
            for (int r = 0; r < this.Config.Row; r++)
            {
                for (int c = 0; c < this.Config.Col; c++)
                {
                    if (r < this.Cubes.GetLength(0))
                        newCubes[r, c] = this.Cubes[r, c];
                    else
                        newCubes[r, c] = new Cube();
                }
            }
            this.Cubes = newCubes;
        }

        public void AddCol()
        {
            this.Config.Col++;

            Dictionary<MyColor, List<int>> newTagNums = new Dictionary<MyColor, List<int>>();
            foreach (var kvp in this.Config.TagNums)
            {
                List<int> nums = kvp.Value;
                nums.Add(-1);
                newTagNums.Add(kvp.Key, nums);
            }
            this.Config.TagNums = newTagNums;

            Cube[,] newCubes = new Cube[this.Config.Row, this.Config.Col];
            for (int r = 0; r < this.Config.Row; r++)
            {
                for (int c = 0; c < this.Config.Col; c++)
                {
                    if (c < this.Cubes.GetLength(1))
                        newCubes[r, c] = this.Cubes[r, c];
                    else
                        newCubes[r, c] = new Cube();
                }
            }
            this.Cubes = newCubes;
        }

        public void DelRow()
        {
            this.Config.Row--;

            Dictionary<MyColor, List<int>> newTagNums = new Dictionary<MyColor, List<int>>();
            foreach (var kvp in this.Config.TagNums)
            {
                List<int> nums = kvp.Value;
                nums.RemoveAt(this.Config.Row);
                newTagNums.Add(kvp.Key, nums);
            }
            this.Config.TagNums = newTagNums;

            Cube[,] newCubes = new Cube[this.Config.Row, this.Config.Col];
            for (int r = 0; r < this.Config.Row; r++)
            {
                for (int c = 0; c < this.Config.Col; c++)
                    newCubes[r, c] = this.Cubes[r, c];
            }
            this.Cubes = newCubes;
        }

        public void DelCol()
        {
            this.Config.Col--;

            Dictionary<MyColor, List<int>> newTagNums = new Dictionary<MyColor, List<int>>();
            foreach (var kvp in this.Config.TagNums)
            {
                List<int> nums = kvp.Value;
                nums.RemoveAt(nums.Count - 1);
                newTagNums.Add(kvp.Key, nums);
            }
            this.Config.TagNums = newTagNums;

            Cube[,] newCubes = new Cube[this.Config.Row, this.Config.Col];
            for (int r = 0; r < this.Config.Row; r++)
            {
                for (int c = 0; c < this.Config.Col; c++)
                    newCubes[r, c] = this.Cubes[r, c];
            }
            this.Cubes = newCubes;
        }

        public void AddColor(MyColor color)
        {
            if (this.Config.TagNums.ContainsKey(color))
                return;

            List<int> nums = new List<int>();

            for (int i = 0; i < this.Config.Row + this.Config.Col; i++)
                nums.Add(-1);

            this.Config.TagNums.Add(color, nums);
        }

        public void DelColor(MyColor color)
        {
            if (this.Config.TagNums.ContainsKey(color))
                this.Config.TagNums.Remove(color);
        }

        public void SetNum(int index, int num, MyColor color)
        {
            if (! this.Config.TagNums.ContainsKey(color))
                return;

            if (index >= this.Config.TagNums[color].Count)
                return;

            this.Config.TagNums[color][index] = num;
        }

        public void DelNum(int index, MyColor color)
        {
            SetNum(index, -1, color);
        }

        public void SetBlock(int r, int c, bool isBlock)
        {
            this.Cubes[r, c].SetBlockState(isBlock);
        }

        public void Clear()
        {
            Dictionary<MyColor, List<int>> newTagNums = new Dictionary<MyColor, List<int>>();
            foreach (var kvp in this.Config.TagNums)
            {
                List<int> nums = new List<int>();
                for (int i = 0; i < kvp.Value.Count; i++)
                    nums[i] = -1;
                newTagNums.Add(kvp.Key, nums);
            }
            this.Config.TagNums = newTagNums;

            for (int r = 0; r < this.Config.Row; r++)
            {
                for (int c = 0; c < this.Config.Col; c++)
                    this.Cubes[r, c].Reset();
            }
        }

        public void Save(string difficulty, int index)
        {
            // 保存前先把block信息刷新（改动的时候同步刷有些麻烦，最后统一刷快一些也方便一些）
            this.Config.Blocks.Clear();

            for (int r = 0; r < this.Config.Row; r++)
            {
                for (int c = 0; c < this.Config.Col; c++)
                {
                    Cube cube = this.Cubes[r, c];
                    if (! cube.IsBlock)
                        continue;

                    string[] str = new string[] { r.ToString(), c.ToString(), Utils.ColorToString(cube.UpColor), Utils.ColorToString(cube.DownColor),
                                                                              Utils.ColorToString(cube.LeftColor), Utils.ColorToString(cube.RightColor), };

                    this.Config.Blocks.Add(str);
                }
            }

            this.Config.SaveConfig(difficulty, index);
        }
    }
}