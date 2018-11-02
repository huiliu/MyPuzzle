using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyPuzzle;

public class Bootstrap
    : MonoBehaviour
{
    [SerializeField] private Transform LevelMenuNode;
    [SerializeField] private Transform QuizMenuNode;
    [SerializeField] private Button StartGameButton;
    [SerializeField] private Button ResetButton;
    [SerializeField] private ResultComponent ResultComponent;
    [SerializeField] private GameObject TopMenu;

    public static Bootstrap Instance { get; private set; }

    private GameObjectRef refs;
    private void Awake()
    {
        Instance = this;

        this.refs = this.GetComponent<GameObjectRef>();

        this.LevelMenuNode.gameObject.SetActive(false);
        this.QuizMenuNode.gameObject.SetActive(false);
        this.ResetButton.onClick.AddListener(() => { PuzzleComponent.Instance.Reset(); });
        this.ResultComponent.OnNext = () =>
        {
            ++this.CurrentQuizID;
            this.ResultComponent.gameObject.SetActive(false);
            this.ResetButton.gameObject.SetActive(true);
            PuzzleComponent.Instance.StartGame(this.CurrentLevel, this.CurrentQuizID);
        };

        this.InitLevelMenu();

        this.UndoCommandMgr = new UndoCommandMgr();
        this.startGameCommand = new StartGameCommand(this);
        this.selectLevelCommand = new SelectLevelCommand(this);
        this.selectQuizCommand = new SelectQuizCommand(this);
    }

    public UndoCommandMgr UndoCommandMgr { get; private set; }
    private StartGameCommand startGameCommand;
    private SelectLevelCommand selectLevelCommand;
    private SelectQuizCommand selectQuizCommand;

    public string CurrentLevel { get; private set; }
    private void InitLevelMenu()
    {
        var template = refs["LevelButton"];
        var levels = Config.GetDiffcultTypes();
        foreach(var l in levels)
        {
            var go = Instantiate(template);
            go.transform.SetParent(this.LevelMenuNode);
            go.GetComponentInChildren<Text>().text = this.GetLevelDesc(l);
            go.GetComponent<Button>().onClick.AddListener(() =>
            {
                this.CurrentLevel = l;
                this.InitQuizMenu();

                //this.ShowOrHideLevelQuiz(false);
                this.UndoCommandMgr.Execute(this.selectLevelCommand);
            });
        }
    }

    public int CurrentQuizID { get; private set; }
    private List<GameObject> quizMenus = new List<GameObject>();
    private void InitQuizMenu()
    {
        this.CleanQuizMenu();

        var template = refs["QuizButton"];
        var quizs = Config.GetQuizsNumByDifficulty(this.CurrentLevel);
        var count = quizs.Count;
        for(var i=0; i < count; ++i)
        {
            var go = Instantiate(template);
            go.transform.SetParent(this.QuizMenuNode);
            go.GetComponentInChildren<Text>().text = this.GetQuizDesc(i);

            this.quizMenus.Add(go);
            var id = int.Parse(quizs[i]);
            go.GetComponent<Button>().onClick.AddListener(() =>
            {
                this.CurrentQuizID = id;
                //this.QuizMenuNode.gameObject.SetActive(false);
                //this.ResetButton.gameObject.SetActive(true);
                //PuzzleComponent.Instance.StartGame(this.CurrentLevel, id);
                this.UndoCommandMgr.Execute(this.selectQuizCommand);
                PuzzleComponent.Instance.OnWinEvent = this.HandleOnWin;
            });
        }
    }

    private void CleanQuizMenu()
    {
        foreach (var go in this.quizMenus)
            Destroy(go);

        this.quizMenus.Clear();
    }

    private string GetLevelDesc(string l)
    {
        return l;
    }

    private string GetQuizDesc(int i)
    {
        return i.ToString();
    }

    public void Enter()
    {
        //this.StartGameButton.gameObject.SetActive(false);
        //this.ShowOrHideLevelQuiz(true);
        this.UndoCommandMgr.Execute(this.startGameCommand);
    }

    private void HandleOnWin()
    {
        this.ResetButton.gameObject.SetActive(false);
        this.ResultComponent.gameObject.SetActive(true);
    }

    private class StartGameCommand
        : IUndoableCommand
    {
        private Bootstrap Bootstrap;
        public StartGameCommand(Bootstrap bootstrap)
        {
            this.Bootstrap = bootstrap;
        }

        public void Execute()
        {
            this.Bootstrap.StartGameButton.gameObject.SetActive(false);
            this.Bootstrap.LevelMenuNode.gameObject.SetActive(true);
        }

        public void Undo()
        {
            this.Bootstrap.LevelMenuNode.gameObject.SetActive(false);
            this.Bootstrap.StartGameButton.gameObject.SetActive(true);
            this.Bootstrap.ResultComponent.gameObject.SetActive(false);
        }
    }

    private class SelectLevelCommand
        : IUndoableCommand
    {
        private Bootstrap Bootstrap;
        public SelectLevelCommand(Bootstrap bootstrap)
        {
            this.Bootstrap = bootstrap;
        }

        public void Execute()
        {
            this.Bootstrap.LevelMenuNode.gameObject.SetActive(false);
            this.Bootstrap.QuizMenuNode.gameObject.SetActive(true);
        }

        public void Undo()
        {
            this.Bootstrap.LevelMenuNode.gameObject.SetActive(true);
            this.Bootstrap.QuizMenuNode.gameObject.SetActive(false);
            this.Bootstrap.ResultComponent.gameObject.SetActive(false);
        }
    }

    private class SelectQuizCommand
        : IUndoableCommand
    {
        private Bootstrap Bootstrap;
        public SelectQuizCommand(Bootstrap bootstrap)
        {
            this.Bootstrap = bootstrap;
        }

        public void Execute()
        {
            this.Bootstrap.QuizMenuNode.gameObject.SetActive(false);
            this.Bootstrap.ResultComponent.gameObject.SetActive(false);
            this.Bootstrap.ResetButton.gameObject.SetActive(true);
            PuzzleComponent.Instance.StartGame(this.Bootstrap.CurrentLevel, this.Bootstrap.CurrentQuizID);
        }

        public void Undo()
        {
            this.Bootstrap.QuizMenuNode.gameObject.SetActive(true);
            this.Bootstrap.ResetButton.gameObject.SetActive(false);
            this.Bootstrap.ResultComponent.gameObject.SetActive(false);
            PuzzleComponent.Instance.Clear();
        }
    }
}
