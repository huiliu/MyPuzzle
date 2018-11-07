using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace MyPuzzle
{
    public class PuzzleConfig
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public Dictionary<MyColor, List<int>> TagNums { get; set; }
        public List<string[]> Blocks { get; set; }

        public PuzzleConfig(int row, int col)
        {
            this.Row = row;
            this.Col = col;
            this.TagNums = new Dictionary<MyColor, List<int>>();
            this.Blocks = new List<string[]>();
        }

        public PuzzleConfig(string config)
        {
            // 获取config信息
            /* 配置中的信息为一串"|"分割的字符串，含义依次是
             * 行数和列数，用","隔开
             * 颜色及该颜色在每一行和每一列上的数值（没有则填-1），用","隔开.可以有多组。为简化数据，后面的-1可以省略
             * 预配置方块信息，6个元素一组，依次是所在行，所在列，上下左右四个方向的颜色，没有颜色填"n"，用","隔开，可以有多组
             * 第一个元素是数字还是字符来区分是颜色配置还是预配置方块
             */
            string[] strs = config.Split('|');

            string[] temp = strs[0].Split(',');
            System.Diagnostics.Debug.Assert(temp.Length == 2);

            this.Row = int.Parse(temp[0]);
            this.Col = int.Parse(temp[1]);

            this.TagNums = new Dictionary<MyColor, List<int>>();
            this.Blocks = new List<string[]>();

            for (int i = 1; i < strs.Length; i++)
            {
                temp = strs[i].Split(',');

                if (temp[0][0] >= 'a' && temp[0][0] <= 'z')
                {
                    // 颜色信息
                    System.Diagnostics.Debug.Assert(temp.Length == this.Row + this.Col + 1);

                    MyColor color = Utils.StringToMyColor(temp[0]);
                    List<int> nums = new List<int>();
                    for (int j = 1; j < temp.Length && j <= this.Row + this.Col; j++)
                    {
                        int num = int.Parse(temp[j]);
                        if (num == -2)
                        {
                            // -1，-1，...，-1
                            // n个连续的-1省略为-2，,n
                            int times = int.Parse(temp[++j]);
                            for (int k =0; k < times; k++)
                                nums.Add(-1);
                        }
                        else
                            nums.Add(num);
                    }

                    // 补足后面的-1
                    while (nums.Count < this.Row + this.Col)
                        nums.Add(-1);
                    
                    this.TagNums.Add(color, nums);
                }
                else
                {
                    // 预配置信息
                    System.Diagnostics.Debug.Assert(temp.Length == 6);
                    this.Blocks.Add(temp);
                }
            }
        }

        public void SaveConfig(string difficulty)
        {
            var quizs = Config.GetQuizsByDifficulty(difficulty);
            int index = int.Parse(quizs[quizs.Count - 1]) + 1;

            Config.WriteQuiz(difficulty, index, this.ToString());
        }

        public void SaveConfig(string difficulty, int index)
        {
            Config.WriteQuiz(difficulty, index, this.ToString());
        }

        public override string ToString()
        {
            string str = string.Format("{0},{1}", this.Row, this.Col);

            foreach (var kvp in this.TagNums)
                str += string.Format("|{0}{1}", Utils.ColorToString(kvp.Key), compress(kvp.Value));

            foreach (var block in this.Blocks)
                str += string.Format("|{0}", string.Join(",", block));

            return str;
        }

        private string compress(List<int> list)
        {
            string str = string.Empty;
            // 压缩数据长度
            // 过滤掉结尾的-1，中间的连续n个-1变为 -2,n
            int startPos = -1;

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == -1 && startPos == -1)
                    startPos = i;

                if (list[i] != -1 && startPos != -1)
                {
                    int num = i - startPos;

                    if (num == 1)
                        str += ",-1";
                    else if (num == 2)
                        str += ",-1,-1";
                    else
                        str += string.Format(",-2,{0}", num);

                    startPos = -1;
                }

                if (list[i] != -1)
                    str += string.Format(",{0}", list[i]);
            }

            return str;
        }
    }
}