using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class VisualBoardController : MonoBehaviour {

    public static VisualBoardController instance;

    [SerializeField]
    private GameObject ThickLine = null;
    [SerializeField]
    private GameObject ThinLine = null;
    [SerializeField]
    private GameObject BoxObject = null;
    private Vector2 backdropDimensions;

    // Cached display properties
    private Vector2 beginningVerticalPos;
    private Vector2 beginningHorizontalPos;
    private Vector2 largeLineLengths;
    private Vector2 smallLineLengths;
    private float thickWidth;
    private float thinWidth;
    private GameObject backdrop;

    // Publically accessible boxes.
    public BoxController[,] boxes;
    public BoardSolver solver = new BoardSolver();
    public Sudoku sudoku = Sudoku.BasicSudoku();

    private void Start() {
        instance = this;
        GenerateDisplayConstants();
        GenerateBorders();
        GenerateBoxes();
        sudoku.Initialise();
    }

    private void GenerateDisplayConstants() {
        backdrop = transform.Find("WhiteBackdrop").gameObject;
        backdropDimensions = GetComponent<RectTransform>().sizeDelta;
        var rt = backdrop.GetComponent<RectTransform>();
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, backdropDimensions.x);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, backdropDimensions.y);
        ThickLine.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, backdropDimensions.x);
        ThinLine.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, backdropDimensions.x);
        thickWidth = ThickLine.GetComponent<RectTransform>().sizeDelta.y;
        thinWidth = ThinLine.GetComponent<RectTransform>().sizeDelta.y;
        beginningVerticalPos = new Vector2(0, -thickWidth / 2f);
        beginningHorizontalPos = new Vector2(-backdropDimensions.x / 2f + thickWidth / 2f, -backdropDimensions.y / 2f);
        largeLineLengths = new Vector2((backdropDimensions.x - thickWidth) / (sudoku.settings.numHorizontalThicks - 1f), (backdropDimensions.y - thickWidth) / (sudoku.settings.numVerticalThicks - 1f));
        smallLineLengths = new Vector2((largeLineLengths.x - thickWidth + thinWidth) / (sudoku.settings.numHorizontalThins + 1f), (largeLineLengths.y - thickWidth + thinWidth) / (sudoku.settings.numVerticalThins + 1f));
    }

    private void GenerateBorders() {
        // Create the thick lines, and then each thin line.
        for (int thickIndex=0; thickIndex<sudoku.settings.numVerticalThicks; thickIndex++) {
            RectTransform top_line = Instantiate(ThickLine, backdrop.transform).GetComponent<RectTransform>();
            top_line.gameObject.name = "VerticalThick " + thickIndex;
            top_line.anchoredPosition = beginningVerticalPos + new Vector2(0, -largeLineLengths.y * thickIndex);
            if (thickIndex != sudoku.settings.numVerticalThicks-1)
            for (int thinIndex=1; thinIndex<=sudoku.settings.numVerticalThins; thinIndex++) {
                RectTransform thin_line = Instantiate(ThinLine, backdrop.transform).GetComponent<RectTransform>();
                thin_line.gameObject.name = "VerticalThin " + thickIndex + " " + thinIndex;
                thin_line.anchoredPosition = top_line.anchoredPosition - new Vector2(0, (thickWidth - thinWidth) / 2f + smallLineLengths.y * thinIndex);
            }
        }
        for (int thickIndex=0; thickIndex<sudoku.settings.numHorizontalThicks; thickIndex++) {
            RectTransform left_line = Instantiate(ThickLine, backdrop.transform).GetComponent<RectTransform>();
            left_line.gameObject.name = "HorizontalThick " + thickIndex;
            left_line.anchoredPosition = beginningHorizontalPos + new Vector2(largeLineLengths.x * thickIndex , 0f);
            left_line.rotation = Quaternion.AngleAxis(90f, Vector3.forward);
            if (thickIndex != sudoku.settings.numHorizontalThicks-1)
            for (int thinIndex=1; thinIndex<=sudoku.settings.numHorizontalThins; thinIndex++) {
                RectTransform thin_line = Instantiate(ThinLine, backdrop.transform).GetComponent<RectTransform>();
                thin_line.gameObject.name = "HorizontalThin " + thickIndex + " " + thinIndex;
                thin_line.rotation = Quaternion.AngleAxis(90f, Vector3.forward);
                thin_line.anchoredPosition = left_line.anchoredPosition + new Vector2((thickWidth - thinWidth) / 2f + smallLineLengths.x * thinIndex, 0);
            }
        }

    }

    private void GenerateBoxes() {
        boxes = new BoxController[sudoku.settings.numHorizontal, sudoku.settings.numVertical];
        for (int thickH=0; thickH < sudoku.settings.numHorizontalThicks-1; thickH++) {
            for (int thickV=0; thickV < sudoku.settings.numVerticalThicks-1; thickV++) {
                for (int thinH=0; thinH < sudoku.settings.numHorizontalThins+1; thinH++) {
                    for (int thinV=0; thinV < sudoku.settings.numVerticalThins+1; thinV++) {
                        RectTransform box = Instantiate(BoxObject, backdrop.transform).GetComponent<RectTransform>();
                        box.anchoredPosition = new Vector2(
                            thickWidth + (smallLineLengths.x - thinWidth) / 2f + largeLineLengths.x * thickH + smallLineLengths.x * thinH,
                            -thickWidth - (smallLineLengths.y - thinWidth) / 2f - largeLineLengths.y * thickV - smallLineLengths.y * thinV
                        );
                        box.gameObject.GetComponent<BoxController>().SetSize(new Vector2(smallLineLengths.x - thinWidth, smallLineLengths.y - thinWidth));
                        int inx = thickH * (sudoku.settings.numHorizontalThins + 1) + thinH;
                        int iny = thickV * (sudoku.settings.numVerticalThins + 1) + thinV;
                        box.gameObject.name = "Box (" + (inx+1) + ", " + (iny+1) + ")";
                        boxes[inx, iny] = box.GetComponent<BoxController>();
                        boxes[inx, iny].position = (inx, iny);
                    }
                }
            }
        }
    }

    // This will later be handled by a separate selection panel, but for now it's fine.
    public void SaveBoard() {
        var obj = new BoardSerializer.SerializedBoard(this);
        string fileName = "Testing/board.json";
        StreamWriter sr = File.CreateText(fileName);
        sr.WriteLine(JsonUtility.ToJson(obj));
        sr.Close();
    }

    public void LoadBoard() {
        string fileName = "Testing/board.json";
        StreamReader sr = File.OpenText(fileName);
        BoardSerializer.SerializedBoard obj = JsonUtility.FromJson<BoardSerializer.SerializedBoard>(sr.ReadToEnd());
        obj.DeserializeToBoard(this);
    }

    public void SolveBoard() {
        var solved = solver.Solve(sudoku);
        for (int i=0; i<sudoku.settings.numHorizontal; i++) for (int j=0; j<sudoku.settings.numVertical; j++) if (solved[i, j] != 0){
            boxes[i, j].SetFull(solved[i, j].ToString());
        }
    }

    // Accessing boxes by index
    public void SetFull(int i, int j, string s) {
        boxes[i, j].SetFull(s);
        sudoku.boxes[i*sudoku.settings.numVertical+j].answer = s;
    }

    public void ToggleCorner(int i, int j, int s) {
        boxes[i, j].ToggleCorner(s);
    }

    public void ToggleCentre(int i, int j, int s) {
        boxes[i, j].ToggleCentre(s);
    }

    public void SetColor(int i, int j, Color c, bool save) {
        boxes[i, j].SetColor(c, save);
    }

    public void ResetColor(int i, int j) {
        boxes[i, j].SetColor(boxes[i, j].currentColor, true);
    }

    public void Clear(int i, int j) {
        boxes[i, j].Clear();
    }

}
