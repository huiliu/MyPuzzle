using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MyPuzzle;

public partial class CubeComponent
    : MonoBehaviour
    , IPointerDownHandler
    , IPointerUpHandler
    , IPointerEnterHandler
    , IPointerExitHandler
{
    [SerializeField] private Image UpLine;
    [SerializeField] private Image RightLine;
    [SerializeField] private Image DownLine;
    [SerializeField] private Image LeftLine;
    [SerializeField] private Image Center;
    [SerializeField] private Color BlockColor;

    private Image Background;
    private RectTransform RectTransform;
    private float halfRectWidth;
    private float halfRectHeight;
    private Vector2 UpPoint;
    private Vector2 RightPoint;
    private Vector2 DownPoint;
    private Vector2 LeftPoint;
    private void Start()
    {
        this.Background = this.GetComponent<Image>();
        this.RectTransform = this.GetComponent<RectTransform>();
        this.halfRectWidth = this.RectTransform.rect.width / 2;
        this.halfRectHeight = this.RectTransform.rect.height / 2;

        this.InitPoint();
    }

    private void InitPoint()
    {
        this.UpPoint.y = this.halfRectHeight;
        this.RightPoint.x = this.halfRectWidth;
        this.DownPoint.y = -this.halfRectHeight;
        this.LeftPoint.x = -this.halfRectWidth;

        this.Center.gameObject.SetActive(false);
    }

    private Cube Cube;
    private int Row;
    private int Col;
    public void Setup(Puzzle puzzle, int r, int c)
    {
        this.Cube = puzzle.Cubes[r, c];
        this.Row = r;
        this.Col = c;

        this.Cube.IsDirty = true;
        this.InitTags(new System.Collections.Generic.List<MyColor>(puzzle.Config.TagNums.Keys));
    }

    public void Reset()
    {
        this.ResetTags();
    }

    private void Update()
    {
        if (this.Cube == null || !this.Cube.IsDirty)
            return;

        this.Cube.IsDirty = false;

        this.UpLine.color = this.Cube.UpColor.ToColor();
        this.UpLine.enabled = this.Cube.UpColor != MyColor.None;

        this.RightLine.color = this.Cube.RightColor.ToColor();
        this.RightLine.enabled = this.Cube.RightColor != MyColor.None;

        this.DownLine.color = this.Cube.DownColor.ToColor();
        this.DownLine.enabled = this.Cube.DownColor != MyColor.None;

        this.LeftLine.color = this.Cube.LeftColor.ToColor();
        this.LeftLine.enabled = this.Cube.LeftColor != MyColor.None;

        if (this.Cube.IsBlock)
        {
            this.Background.color = this.BlockColor;
        }
    }

    #region Event Handler
    public Action<int, int, Direction> OnDraw;
    public void OnPointerEnter(PointerEventData eventData)
    {
        Vector2 localPos = this.transform.InverseTransformPoint(eventData.position);
        var dir = localPos.ToDirection();

        //OnDraw.SafeInvoke(this.Row, this.Col, dir);
        Debug.Log(string.Format("Mouse Enter! {0}/{1}", this.Row, this.Col));
        //this.Center.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Vector2 localPos = this.transform.InverseTransformPoint(eventData.position);
        var dir = localPos.ToDirection();

        OnDraw.SafeInvoke(this.Row, this.Col, dir);
        Debug.Log(string.Format("Mouse Exit! {0}/{1}", this.Row, this.Col));
        this.Center.gameObject.SetActive(false);
        this.clickFlag = false;
    }

    public Action OnDown;
    public Action OnUp;
    private bool clickFlag = false;
    public void OnPointerDown(PointerEventData eventData)
    {
        this.OnDown.SafeInvoke();
        this.Center.gameObject.SetActive(true);
        Debug.Log(string.Format("Pointer Down! {0}/{1}", this.Row, this.Col));
        this.clickFlag = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        this.OnUp.SafeInvoke();
        this.Center.gameObject.SetActive(false);
        Debug.Log(string.Format("Pointer Up! {0}/{1}", this.Row, this.Col));
        if (this.clickFlag)
            this.HandleClick();
    }
    #endregion
}
