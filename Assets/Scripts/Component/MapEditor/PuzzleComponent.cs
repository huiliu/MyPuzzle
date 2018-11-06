using UnityEngine;
using UnityEngine.UI;
using MyPuzzle;
using System.Collections.Generic;

namespace MapEditor
{
    public class PuzzleComponent
        : MonoBehaviour
    {
        [SerializeField] private Button IncRowButton;
        [SerializeField] private Button DecRowButton;
        [SerializeField] private Button IncColButton;
        [SerializeField] private Button DecColButton;
        [SerializeField] private PaletteComponent PaletteComponent;
        [SerializeField] private Transform PuzzleNode;
        [SerializeField] private Dropdown DifficultDropdown;
        [SerializeField] private InputField LevelInputField;

        public static PuzzleComponent Instance { get; private set; }

        public MyColor CurrentColor { get { return this.PaletteComponent.CurrentColor; } }
        public EditPuzzle Puzzle { get; private set; }
        private RectTransform thisRect;
        private RectTransform RectTransform;
        private static int CubeWidth = 36;
        private static int CubeHeight = 36;
        private void Awake()
        {
            Instance = this;

            this.thisRect = this.GetComponent<RectTransform>();
            this.RectTransform = this.PuzzleNode.gameObject.GetComponent<RectTransform>();
            this.InitPuzzle("");

            this.IncRowButton.onClick.AddListener(() => { this.Puzzle.AddRow(); this.RefreshPuzzle(); });
            this.DecRowButton.onClick.AddListener(() => { this.Puzzle.DelRow(); this.RefreshPuzzle(); });
            this.IncColButton.onClick.AddListener(() => { this.Puzzle.AddCol(); this.RefreshPuzzle(); });
            this.DecColButton.onClick.AddListener(() => { this.Puzzle.DelCol(); this.RefreshPuzzle(); });
        }

        private void InitPuzzle(string config)
        {
            this.Puzzle = new EditPuzzle(config);
            if (string.IsNullOrEmpty(config))
            {
                this.Puzzle.AddRow();
                this.Puzzle.AddRow();
                this.Puzzle.AddCol();
                this.Puzzle.AddCol();

                this.PaletteComponent.InitPalette(new List<MyColor>());
                this.RefreshPuzzle();
            }
            else
            {
                this.PaletteComponent.InitPalette(new List<MyColor>(this.Puzzle.Config.TagNums.Keys));
            }

        }

        public void Reset()
        {
            this.InitPuzzle("");
            this.RefreshPuzzle();
        }

        public void Save()
        {
            var i = this.DifficultDropdown.value;
            var difficult = this.DifficultDropdown.options[i].text;

            var level = int.Parse(this.LevelInputField.text);

            this.Puzzle.Save(difficult, level);
        }

        public void CheckResult()
        {
            var r = this.Puzzle.CheckResult();
            Debug.Log(string.Format("Result: {0}", r));
        }

        public void AddColor(MyColor myColor)
        {
            this.Puzzle.AddColor(myColor);
        }

        public void RemoveColor(MyColor myColor)
        {
            this.Puzzle.DelColor(myColor);
        }

        private Vector2 sizeDelta = Vector2.zero;
        private List<GameObject> cubes = new List<GameObject>();
        private void RefreshPuzzle()
        {
            sizeDelta.x = CubeWidth * this.Puzzle.Config.Col;
            sizeDelta.y = CubeHeight * this.Puzzle.Config.Row;
            this.RectTransform.sizeDelta = sizeDelta;
            this.thisRect.sizeDelta = sizeDelta;

            foreach (var go in this.cubes)
                ObjectPool.Instance.Return("Cube", go);
            this.cubes.Clear();

            var totoal = 0;
            for (int r = 0; r < this.Puzzle.Config.Row; r++)
            {
                for (int c = 0; c < this.Puzzle.Config.Col; c++)
                {
                    var go = ObjectPool.Instance.Take("Cube");
                    go.name = string.Format("cube({0},{1})", r, c);
                    go.transform.SetParent(this.PuzzleNode);
                    go.SetActive(true);

                    Debug.Log(string.Format("Create Cube: {0}", go.name));
                    var CubeComponent = go.GetComponent<CubeComponent>();
                    CubeComponent.Setup(this.Puzzle, r, c);
                    CubeComponent.OnDraw = this.DrawLine;
                    CubeComponent.OnDown = this.HandleOnDown;
                    CubeComponent.OnUp = this.HandleOnUp;

                    this.cubes.Add(go);

                    ++totoal;
                }
            }
        }

        private bool winFlag = false;
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
                //this.OnWin();
                //this.winFlag = true;
            }
        }

    }
}
