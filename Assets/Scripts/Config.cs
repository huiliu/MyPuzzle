using UnityEngine;

using System;
using System.Collections.Specialized;

namespace MyPuzzle
{
    public class Config
    {
        static ConfigIni.ConfigIni configIni = new ConfigIni.ConfigIni(Application.streamingAssetsPath + "/Config/quiz.ini");

        public static StringCollection GetDiffcultTypes()
        {
            StringCollection difficulties = new StringCollection();
            configIni.ReadSections(difficulties);
            return difficulties;
        }

        public static StringCollection GetQuizsByDifficulty(string difficulty)
        {
            StringCollection quiz = new StringCollection();
            configIni.ReadKeys(difficulty, quiz);
            return quiz;
        }

        public static string GetQuiz(string difficulty, int index)
        {
            return configIni.ReadString(difficulty, index.ToString(), "");
        }

        public static void WriteQuiz(string difficulty, int index, string str)
        {
            configIni.WriteString(difficulty, index.ToString(), str);
        }
    }
}
