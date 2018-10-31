﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Linq;

namespace MyPuzzle
{
    public class PuzzleConfig
    {
        public int Row { get; private set; }
        public int Col { get; private set; }
        public Dictionary<MyColor, List<int>> TagNums { get; private set; }
        public List<string[]> Blocks { get; private set; }

        public PuzzleConfig(int row, int col)
        {
            this.Row = row;
            this.Col = col;
            TagNums = new Dictionary<MyColor, List<int>>();
            Blocks = new List<string[]>();
        }

        public PuzzleConfig(string config)
        {
            // 获取config信息
            /* 配置中的信息为一串"|"分割的字符串，含义依次是
             * 行数和列数，用","隔开
             * 颜色及该颜色在每一行和每一列上的数值（没有则填-1），用","隔开.可以有多组
             * 预配置方块信息，6个元素一组，依次是所在行，所在列，上下左右四个方向的颜色，没有颜色填"n"，用","隔开，可以有多组
             * 第一个元素是数字还是字符来区分是颜色配置还是预配置方块
             */
            string[] strs = config.Split('|');

            string[] temp = strs[0].Split(',');
            System.Diagnostics.Debug.Assert(temp.Length == 2);

            this.Row = int.Parse(temp[0]);
            this.Col = int.Parse(temp[1]);

            TagNums = new Dictionary<MyColor, List<int>>();
            Blocks = new List<string[]>();

            for (int i = 1; i < strs.Length; i++)
            {
                temp = strs[i].Split(',');

                if (temp[0][0] >= 'a' && temp[0][0] <= 'z')
                {
                    // 颜色信息
                    System.Diagnostics.Debug.Assert(temp.Length == this.Row + this.Col + 1);

                    MyColor color = Utils.StringToMyColor(temp[0]);
                    List<int> nums = new List<int>();
                    for (int j = 1; j < temp.Length; j++)
                        nums.Add(int.Parse(temp[j]));
                    
                    TagNums.Add(color, nums);
                }
                else
                {
                    // 预配置信息
                    System.Diagnostics.Debug.Assert(temp.Length == 6);
                    this.Blocks.Add(temp);
                }
            }
        }
    }
}