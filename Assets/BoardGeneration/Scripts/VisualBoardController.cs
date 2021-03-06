﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class VisualBoardController : MonoBehaviour {

    public enum InteractionState {
        VIEWING,
        EDITING,
        PLAYING
    }

    public InteractionState interactionState = InteractionState.VIEWING;

    public static VisualBoardController instance;

    [SerializeField]
    private GameObject ThickLine = null;
    [SerializeField]
    private GameObject ThinLine = null;
    [SerializeField]
    private GameObject BoxObject = null;

    // Visual Constants
    [SerializeField]
    private float lineNumberMargin = 0.1f;
    [SerializeField]
    private float noLineNumberMargin = 0.02f;
    [SerializeField]
    private float thickLineSize = 0.02f;
    [SerializeField]
    private float thinLineSize = 0.01f;
    private float margin;

    private Vector2 backdropDimensions;
    private Vector2 lineDimensions;

    // Cached display properties
    private Vector2 beginningVerticalPos;
    private Vector2 beginningHorizontalPos;
    private Vector2 largeLineLengths;
    private Vector2 smallLineLengths;
    private float thickWidthH;
    private float thickWidthV;
    private float thinWidthH;
    private float thinWidthV;
    private GameObject backdrop;
    private GameObject boxCanvas;
    private List<GameObject> generatedObjects = new List<GameObject>();

    // Publically accessible boxes.
    public BoxController[,] boxes;
    public BoxController[,,] lineBoxes;
    public BoardSolver solver = new BoardSolver();
    public Sudoku sudoku = Sudoku.BasicSudoku();
    public string boardName;

    public static bool ready = false;

    private void Start() {
        instance = this;
        sudoku = Sudoku.BasicSudoku();
        Initialise();
        ready = true;
    }

    public void Initialise() {
        foreach (GameObject g in generatedObjects) Destroy(g);
        generatedObjects.Clear();
        sudoku.Initialise();
        GenerateDisplayConstants();
        GenerateBorders();
        GenerateBoxes();
    }

    private void GenerateDisplayConstants() {
        boxCanvas = transform.Find("BoxCanvas").gameObject;
        backdrop = transform.Find("BackdropCanvas/WhiteBackdrop").gameObject;
        backdropDimensions = GetComponent<RectTransform>().sizeDelta;
        lineDimensions = backdropDimensions;
        if (sudoku.settings.lineNumbers) margin = lineNumberMargin;
        else margin = noLineNumberMargin;
        lineDimensions.x -= 2 * margin * backdropDimensions.x;
        lineDimensions.y -= 2 * margin * backdropDimensions.y;
        thickWidthH = lineDimensions.x * thickLineSize;
        thickWidthV = lineDimensions.y * thickLineSize;
        thinWidthH = lineDimensions.x * thinLineSize;
        thinWidthV = lineDimensions.y * thinLineSize;
        var rt = backdrop.GetComponent<RectTransform>();
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, backdropDimensions.x);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, backdropDimensions.y);
        beginningVerticalPos = new Vector2(0, -thickWidthV / 2f - margin * backdropDimensions.y);
        beginningHorizontalPos = new Vector2(-backdropDimensions.x / 2f + thickWidthH / 2f + margin * backdropDimensions.x, -backdropDimensions.y / 2f);
        largeLineLengths = new Vector2((lineDimensions.x - thickWidthH) / (sudoku.settings.numHorizontalThicks - 1f), (lineDimensions.y - thickWidthV) / (sudoku.settings.numVerticalThicks - 1f));
        smallLineLengths = new Vector2((largeLineLengths.x - thickWidthH + thinWidthH) / (sudoku.settings.numHorizontalThins + 1f), (largeLineLengths.y - thickWidthV + thinWidthV) / (sudoku.settings.numVerticalThins + 1f));
    }

    private void GenerateBorders() {
        // Create the thick lines, and then each thin line.
        for (int thickIndex=0; thickIndex<sudoku.settings.numVerticalThicks; thickIndex++) {
            RectTransform top_line = Instantiate(ThickLine, boxCanvas.transform).GetComponent<RectTransform>();
            generatedObjects.Add(top_line.gameObject);
            top_line.gameObject.name = "VerticalThick " + thickIndex;
            top_line.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, lineDimensions.x);
            top_line.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, thickWidthV);
            top_line.anchoredPosition = beginningVerticalPos + new Vector2(0, -largeLineLengths.y * thickIndex);
            if (thickIndex != sudoku.settings.numVerticalThicks-1)
            for (int thinIndex=1; thinIndex<=sudoku.settings.numVerticalThins; thinIndex++) {
                RectTransform thin_line = Instantiate(ThinLine, boxCanvas.transform).GetComponent<RectTransform>();
                generatedObjects.Add(thin_line.gameObject);
                thin_line.gameObject.name = "VerticalThin " + thickIndex + " " + thinIndex;
                thin_line.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, lineDimensions.x);
                thin_line.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, thinWidthV);
                thin_line.anchoredPosition = top_line.anchoredPosition - new Vector2(0, (thickWidthV - thinWidthV) / 2f + smallLineLengths.y * thinIndex);
            }
        }
        for (int thickIndex=0; thickIndex<sudoku.settings.numHorizontalThicks; thickIndex++) {
            RectTransform left_line = Instantiate(ThickLine, boxCanvas.transform).GetComponent<RectTransform>();
            generatedObjects.Add(left_line.gameObject);
            left_line.gameObject.name = "HorizontalThick " + thickIndex;
            left_line.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, lineDimensions.y);
            left_line.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, thickWidthH);
            left_line.anchoredPosition = beginningHorizontalPos + new Vector2(largeLineLengths.x * thickIndex , 0f);
            left_line.rotation = Quaternion.AngleAxis(90f, Vector3.forward);
            if (thickIndex != sudoku.settings.numHorizontalThicks-1)
            for (int thinIndex=1; thinIndex<=sudoku.settings.numHorizontalThins; thinIndex++) {
                RectTransform thin_line = Instantiate(ThinLine, boxCanvas.transform).GetComponent<RectTransform>();
                generatedObjects.Add(thin_line.gameObject);
                thin_line.gameObject.name = "HorizontalThin " + thickIndex + " " + thinIndex;
                thin_line.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, lineDimensions.y);
                thin_line.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, thinWidthH);
                thin_line.rotation = Quaternion.AngleAxis(90f, Vector3.forward);
                thin_line.anchoredPosition = left_line.anchoredPosition + new Vector2((thickWidthH - thinWidthH) / 2f + smallLineLengths.x * thinIndex, 0);
            }
        }

    }

    private void GenerateBoxes() {
        boxes = new BoxController[sudoku.settings.numHorizontal, sudoku.settings.numVertical];
        if (sudoku.settings.lineNumbers) {
            lineBoxes = new BoxController[2, 2, Mathf.Max(sudoku.settings.numHorizontal, sudoku.settings.numVertical)];
            AddRowsAndCols();
        }
        for (int thickH=0; thickH < sudoku.settings.numHorizontalThicks-1; thickH++) {
            for (int thickV=0; thickV < sudoku.settings.numVerticalThicks-1; thickV++) {
                for (int thinH=0; thinH < sudoku.settings.numHorizontalThins+1; thinH++) {
                    for (int thinV=0; thinV < sudoku.settings.numVerticalThins+1; thinV++) {
                        RectTransform box = Instantiate(BoxObject, boxCanvas.transform).GetComponent<RectTransform>();
                        generatedObjects.Add(box.gameObject);
                        box.anchoredPosition = new Vector2(
                            thickWidthH + (smallLineLengths.x - thinWidthH) / 2f + largeLineLengths.x * thickH + smallLineLengths.x * thinH + margin * backdropDimensions.x,
                            -thickWidthV - (smallLineLengths.y - thinWidthV) / 2f - largeLineLengths.y * thickV - smallLineLengths.y * thinV - margin * backdropDimensions.y
                        );
                        box.gameObject.GetComponent<BoxController>().SetSize(new Vector2(smallLineLengths.x - thinWidthH, smallLineLengths.y - thinWidthV));
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

    public void AddRowsAndCols() {
        for (int i=0; i<sudoku.settings.numHorizontal; i++) {
            AddColNumber(i, "", true);
            AddColNumber(i, "", false);
        }
        for (int i=0; i<sudoku.settings.numVertical; i++) {
            AddRowNumber(i, "", true);
            AddRowNumber(i, "", false);
        }
    }

    public void AddRowNumber(int i, string result, bool top) {
        RectTransform box = Instantiate(BoxObject, boxCanvas.transform).GetComponent<RectTransform>();
        generatedObjects.Add(box.gameObject);
        float newXLength = smallLineLengths.x - thinWidthV;
        float newYLength = (lineDimensions.y - 2*thickWidthV) / (float)sudoku.settings.numVertical;
        box.anchoredPosition = new Vector2(
            (top ? margin * backdropDimensions.x - newXLength/2f : backdropDimensions.x - margin * backdropDimensions.x + newXLength/2f),
            -margin * backdropDimensions.y - thickWidthV - newYLength/2f - newYLength * i
        );
        box.gameObject.name = "RowNum " + i;
        var bc = box.gameObject.GetComponent<BoxController>();
        bc.SetSize(new Vector2(newXLength, newYLength));
        bc.SetFull(result, false);
        bc.position = (i, top ? BoxController.topBox : BoxController.botBox);
        lineBoxes[0, top ? 1 : 0, i] = bc;
    }

    public void AddColNumber(int j, string result, bool top) {
        RectTransform box = Instantiate(BoxObject, boxCanvas.transform).GetComponent<RectTransform>();
        generatedObjects.Add(box.gameObject);
        float newXLength = (lineDimensions.x - 2*thickWidthH) / (float)sudoku.settings.numHorizontal;
        float newYLength = smallLineLengths.y - thinWidthV;
        box.anchoredPosition = new Vector2(
            thickWidthH + margin * backdropDimensions.x + newXLength/2f + newXLength * j,
            (top ? -margin * backdropDimensions.y + newYLength/2f : -backdropDimensions.y + margin * backdropDimensions.y - newYLength/2f)
        );
        box.gameObject.name = "ColNum " + j;
        var bc = box.gameObject.GetComponent<BoxController>();
        bc.SetSize(new Vector2(newXLength, newYLength));
        bc.SetFull(result, false);
        bc.position = (top ? BoxController.topBox : BoxController.botBox, j);
        lineBoxes[1, top ? 1 : 0, j] = bc;
    }

    // This will later be handled by a separate selection panel, but for now it's fine.
    public void SaveBoard(string filename) {
        var obj = new BoardSerializer.SerializedBoard(sudoku);
        filename = "Testing/" + filename;
        StreamWriter sr = File.CreateText(filename);
        sr.WriteLine(JsonUtility.ToJson(obj));
        sr.Close();
    }
    public void SaveBoard() { SaveBoard(boardName + ".json"); }

    public void LoadBoard(string filename) {
        filename = "Testing/" + filename;
        var subs = filename.Split('.');
        var newSubs = new string[subs.Length - 1];
        for (int i=0; i<newSubs.Length; i++) newSubs[i] = subs[i];
        this.boardName = string.Join(".", newSubs);
        StreamReader sr = File.OpenText(filename);
        BoardSerializer.SerializedBoard obj = JsonUtility.FromJson<BoardSerializer.SerializedBoard>(sr.ReadToEnd());
        sudoku = obj.Deserialized();
        ResetView();
    }
    public void LoadBoard() { LoadBoard("board.json"); }

    public void SolveBoard() {
        solver.Initialise(sudoku);
        solver.settings.bruteForce = false;
        var result = solver.Solve();
        Debug.Log(result);
        for (int i=0; i<sudoku.settings.numHorizontal; i++) for (int j=0; j<sudoku.settings.numVertical; j++) if (solver.GetValue(i, j) != ""){
            SetFull(i, j, solver.GetValue(i, j).ToString(), false);
        } else {
            boxes[i, j].Clear(false);
            foreach (string x in solver.final.possible_values[i, j]) {
                boxes[i, j].ToggleCentre(x, false);
            }
        }
    }

    public void GenerateErrors() {
        var errors = solver.CollectErrors(sudoku);
        foreach (var m in errors) {
            Debug.Log(m.displayMessage);
        }
    }

    public void GetHint() {
        // TODO: Add hint to new solver.
        /*(int x, int y) hint = solver.GetBoxHint(sudoku);
        if (hint.x == -1) return;
        boxes[hint.x, hint.y].StartFlash(new Color(1, 0, 1));*/
    }

    public void ResetView() {
        Initialise();
        sudoku.ApplyToBoard(this);
    }

    // Viewmodel changes.
    public IEnumerator SetView(string sceneName) {
        var topDim = transform.parent.GetComponent<RectTransform>().sizeDelta;
        var rt = GetComponent<RectTransform>();
        var obj = GameObject.Find("SudokuHolder");
        if (obj == null) {
            rt.sizeDelta = new Vector2(0, 0);
        } else {
            float min;
            while (true) {
                Vector2 sizes = obj.GetComponent<RectTransform>().sizeDelta;
                min = Mathf.Min(sizes.x, sizes.y);
                if (min == 0) yield return new WaitForSeconds(0.01f);
                else break;
            }
            rt.sizeDelta = new Vector2(min, min);
            this.transform.position = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().WorldToScreenPoint(obj.transform.position);
            this.transform.parent.GetComponent<Canvas>().worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }
        ResetView();
    }

    public void RemoveUnderlays() {
        for (int i=0; i<sudoku.settings.numHorizontal; i++) {
            for (int j=0; j<sudoku.settings.numVertical; j++) {
                GetBox(i, j).RemoveUnderlays();
            }
        }
    }

    // Accessing boxes by index
    public BoxController GetBox(int i, int j) {
        if (i == BoxController.topBox || i == BoxController.botBox) {
            return lineBoxes[1, i == BoxController.topBox ? 1 : 0, j];
        }
        if (j == BoxController.topBox || j == BoxController.botBox) {
            return lineBoxes[0, j == BoxController.topBox ? 1 : 0, i];
        }
        return boxes[i, j];
    }

    public void SetFull(int i, int j, string s, bool fromUI) {
        var box = GetBox(i, j);
        if (interactionState == InteractionState.VIEWING && fromUI) return;
        if (box.given && interactionState == InteractionState.PLAYING && fromUI) return;
        if (interactionState == InteractionState.EDITING) {
            box.given = true;
            sudoku.GetBox(i, j).given = true;
        }
        box.SetFull(s, fromUI);
        sudoku.SetBoxAnswer(i, j, s);
    }

    public void ToggleCorner(int i, int j, string s, bool fromUI) {
        var box = GetBox(i, j);
        box.ToggleCorner(s, fromUI);
    }

    public void ToggleCentre(int i, int j, string s, bool fromUI) {
        var box = GetBox(i, j);
        box.ToggleCentre(s, fromUI);
    }

    public void SetColor(int i, int j, Color c, bool fromUI) {
        var box = GetBox(i, j);
        box.SetColor(c, fromUI);
    }

    public void SetHighlight(int i, int j, Color c) {
        var box = GetBox(i, j);
        box.SetHighlight(c);
    }

    public void ResetColor(int i, int j, bool fromUI) {
        var box = GetBox(i, j);
        box.SetColor(box.currentColor, fromUI);
    }

    public void Clear(int i, int j, bool fromUI) {
        var box = GetBox(i, j);
        if (interactionState == InteractionState.VIEWING && fromUI) return;
        if (interactionState == InteractionState.PLAYING && fromUI && box.given) return;
        if (interactionState == InteractionState.EDITING) {
            box.given = false;
            sudoku.GetBox(i, j).given = false;
        }
        box.Clear(fromUI);
        sudoku.SetBoxAnswer(i, j, "");
    }

    public void Hide(Transform t) {
        if (t.childCount > 0) {
            foreach (Transform child in t) {
                Hide(child);
            }
        }
        var r = t.gameObject.GetComponent<Image>();
        if (r != null) r.enabled = false;
    }

    public void Show(Transform t) {
        if (t.childCount > 0) {
            foreach (Transform child in t) {
                Show(child);
            }
        }
        var r = t.gameObject.GetComponent<Image>();
        if (r != null) r.enabled = true;
    }

}
