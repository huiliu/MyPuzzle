using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyPuzzle;

[RequireComponent(typeof(GameObjectRef))]
public class PuzzleComponent
    : MonoBehaviour
{
    [SerializeField] private int CellWidth;
    [SerializeField] private int CellHeigh;
    [SerializeField] private PaletteComponent PaletteComponent;
    [SerializeField] private RectTransform GameBoard;

    public static PuzzleComponent Instance { get; private set; }

    private GameObjectRef refs;
    void Awake()
    {
        Instance = this;
        this.refs = GetComponent<GameObjectRef>();
    }

    public MyColor CurrentColor { get { return this.PaletteComponent.CurrentColor; } }
    public Puzzle Puzzle { get; private set; }
    public void StartGame(string level, int quizID)
    {
        this.Clear();

        string config = Config.GetQuiz(level, quizID);
        this.Puzzle = new Puzzle(config);
        this.InitPuzzle();
        this.PaletteComponent.Init(new List<MyColor>(this.Puzzle.Config.TagNums.Keys), this.GameBoard.sizeDelta);

        this.winFlag = false;
    }

    public void Reset()
    {
        foreach (var c in this.cubes)
            c.Reset();

        this.Puzzle.Reset();
    }

    #region Initialize Puzzle
    private void InitPuzzle()
    {
        this.InitCube();
        this.InitLabel();
    }

    public void Clear()
    {
        foreach (var c in this.cubes)
            Destroy(c.gameObject);

        foreach (var r in this.rowLables)
            Destroy(r);

        foreach (var c in this.colLables)
            Destroy(c);

        this.cubes.Clear();
        this.rowLables.Clear();
        this.colLables.Clear();
        this.PaletteComponent.ResetPalette();
    }

    private List<CubeComponent> cubes = new List<CubeComponent>();
    private Vector2 sizeDelta = Vector2.zero;
    private void InitCube()
    {
        this.sizeDelta.x = this.Puzzle.Config.Col * 32;
        this.sizeDelta.y = this.Puzzle.Config.Row * 32;
        this.GameBoard.sizeDelta = this.sizeDelta;

        var cube = refs["Cube"];
        var datum = refs["Datum"];

        for (int r = 0; r < this.Puzzle.Config.Row; r++)
        {
            for (int c = 0; c < this.Puzzle.Config.Col; c++)
            {
                GameObject newCube = Instantiate<GameObject>(cube);
                newCube.name = string.Format("cube({0},{1})", r, c);
                newCube.transform.SetParent(datum.transform);
                newCube.SetActive(true);

                var CubeComponent = newCube.GetComponent<CubeComponent>();
                CubeComponent.Setup(this.Puzzle, r, c);
                CubeComponent.OnDraw = this.DrawLine;
                CubeComponent.OnDown = this.HandleOnDown;
                CubeComponent.OnUp = this.HandleOnUp;

                this.cubes.Add(CubeComponent);
            }
        }
    }

    private List<GameObject> rowLables = new List<GameObject>();
    private List<GameObject> colLables = new List<GameObject>();

    private void InitLabel()
    {
        var datum = refs["Datum"];
        var rowNum = refs["RowNum"];
        var colNum = refs["ColNum"];

        var xPos = -this.GameBoard.sizeDelta.x / 2 - 16;
        var yPos = this.GameBoard.sizeDelta.y / 2 + 16;

        for (int r = 0; r < this.Puzzle.Config.Row; r++)
        {
            GameObject newRowNum = new GameObject();
            newRowNum.name = string.Format("rowNum({0})", r);
            newRowNum.transform.SetParent(this.transform, false);
            //newRowNum.transform.localPosition = new Vector3(-this.CellWidth, -r * this.CellHeigh, 0);
            newRowNum.transform.localPosition = new Vector3(xPos, -r * this.CellHeigh + yPos - 32, 0);
            newRowNum.SetActive(true);

            this.rowLables.Add(newRowNum);
        }

        for (int c = 0; c < this.Puzzle.Config.Col; c++)
        {
            var newColNum = new GameObject();
            newColNum.name = string.Format("colNum({0})", c);
            newColNum.transform.SetParent(this.transform, false);
            //newColNum.transform.localPosition = new Vector3(c * this.CellWidth, 0, 0);
            newColNum.transform.localPosition = new Vector3(c * this.CellWidth + xPos + 32, yPos, 0);
            newColNum.SetActive(true);

            this.colLables.Add(newColNum);
        }

        var rowLength = this.rowLables.Count;
        var rowPos = Vector2.zero;
        var colPos = Vector2.zero;
        colPos.x = -16;
        colPos.y = -16;
        foreach (var kvp in this.Puzzle.Config.TagNums)
        {
            MyColor color = kvp.Key;
            List<int> nums = kvp.Value;

            rowPos.x -= 16;
            for (int i = 0; i < rowLength; i++)
            {
                if (nums[i] == -1)
                    continue;

                var num = Instantiate(rowNum);
                num.transform.SetParent(this.rowLables[i].transform, false);
                num.transform.localPosition = rowPos;
                Debug.Log(string.Format("i = {0} Pos: {1}/{2}", i, rowPos, num.transform.position));

                var text = num.GetComponent<Text>();
                text.text = nums[i].ToString();
                text.color = color.ToColor();

                num.SetActive(true);
            }

            colPos.y += 16;
            for (int i = 0; i < this.colLables.Count; i++)
            {
                if (nums[rowLength + i] == -1)
                    continue;

                var num = Instantiate(rowNum);
                num.transform.SetParent(this.colLables[i].transform);
                num.transform.localPosition = colPos;

                var text = num.GetComponent<Text>();
                text.color = color.ToColor();
                text.text = nums[rowLength + i].ToString();

                num.SetActive(true);
            }
        }

    }
    #endregion

    private bool DrawFlag = false;
    private void DrawLine(int r, int c, Direction direction)
    {
        if (!this.DrawFlag || this.winFlag)
            return;

        this.Puzzle.DrawLine(r, c, direction, this.PaletteComponent.CurrentColor);
    }

    private void HandleOnDown()
    {
        this.DrawFlag = true;
    }

    private void HandleOnUp()
    {
        this.DrawFlag = false;

        if (this.Puzzle.CheckResult())
        {
            this.OnWin();
        }
    }

    public Action OnWinEvent;
    private bool winFlag;
    private void OnWin()
    {
        this.winFlag = true;
        this.OnWinEvent.SafeInvoke();
    }
}
