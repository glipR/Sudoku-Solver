using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BoxSelectionManager : MonoBehaviour {

    public enum SelectionVersion {
        FULL,
        CENTRE,
        CORNER,
        COLOR,
    }

    private static KeyCode[] keyCodes = { KeyCode.None, KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9 };

    public ColourPallete colourPallete;

    public static BoxSelectionManager instance;
    [SerializeField]
    public Color highlightColor = new Color(0.5f, 1, 1, 0.3f);

    private List<(int x, int y)> selected;
    private SelectionVersion currentSelection = SelectionVersion.FULL;
    public static bool ready = false;

    private void Start() {
        instance = this;
        selected = new List<(int x, int y)>();
        AddListeners();
        ready = true;
    }

    public void AddListeners() {
        SetSelected.RemoveAllListeners();
        SetSelected.AddListener(SetSelectedAction);
        ToggleSelected.RemoveAllListeners();
        ToggleSelected.AddListener(ToggleSelectedAction);
        HandleKeyStrokes.RemoveAllListeners();
        HandleKeyStrokes.AddListener(HandleKeyStrokesAction);
        EnsureSelected.RemoveAllListeners();
        EnsureSelected.AddListener(EnsureSelectedAction);
    }

    public UnityEvent HandleKeyStrokes = new UnityEvent();

    public void HandleKeyStrokesAction() {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) ShiftSelected((-1, 0));
        if (Input.GetKeyDown(KeyCode.RightArrow)) ShiftSelected((1, 0));
        if (Input.GetKeyDown(KeyCode.UpArrow)) ShiftSelected((0, -1));
        if (Input.GetKeyDown(KeyCode.DownArrow)) ShiftSelected((0, 1));

        if (Input.GetKeyDown(KeyCode.Z)) currentSelection = SelectionVersion.FULL;
        if (Input.GetKeyDown(KeyCode.X)) currentSelection = SelectionVersion.CORNER;
        if (Input.GetKeyDown(KeyCode.C)) currentSelection = SelectionVersion.CENTRE;
        if (Input.GetKeyDown(KeyCode.V)) currentSelection = SelectionVersion.COLOR;

        for (int i=1; i<keyCodes.Length; i++) {
            if (Input.GetKeyDown(keyCodes[i])) {
                if (currentSelection == SelectionVersion.FULL)
                    foreach (var box in selected) VisualBoardController.instance.SetFull(box.x, box.y, i.ToString(), true);
                if (currentSelection == SelectionVersion.CORNER)
                    foreach (var box in selected) VisualBoardController.instance.ToggleCorner(box.x, box.y, i);
                if (currentSelection == SelectionVersion.CENTRE)
                    foreach (var box in selected) VisualBoardController.instance.ToggleCentre(box.x, box.y, i);
                if (currentSelection == SelectionVersion.COLOR)
                    foreach (var box in selected) VisualBoardController.instance.SetColor(box.x, box.y, GetColor(i));
            }
        }
        if (Input.GetKeyDown(KeyCode.Backspace)) foreach (var box in selected) VisualBoardController.instance.Clear(box.x, box.y, true);
    }

    private void Update() {
        HandleKeyStrokes.Invoke();
    }

    public class BoxControllerEvent : UnityEvent<BoxController> {}
    public BoxControllerEvent SetSelected = new BoxControllerEvent();
    public void SetSelectedAction(BoxController select) {
        foreach (var box in selected) {
            VisualBoardController.instance.ResetColor(box.x, box.y);
        }
        selected.Clear();
        selected.Add(select.position);
        VisualBoardController.instance.SetHighlight(selected[0].x, selected[0].y, highlightColor);
    }

    public void ShiftSelected((int x, int y) shift) {
        foreach (var box in selected) {
            VisualBoardController.instance.ResetColor(box.x, box.y);
        }
        // This needs to be revamped in a separate method on VisualBoardController.
        for (int i=0; i<selected.Count; i++) {
            if (
                0 <= selected[i].x + shift.x &&
                selected[i].x + shift.x < VisualBoardController.instance.sudoku.settings.numHorizontal &&
                0 <= selected[i].y + shift.y &&
                selected[i].y + shift.y < VisualBoardController.instance.sudoku.settings.numVertical
            )
                selected[i] = (selected[i].x + shift.x, selected[i].y + shift.y);
        }
        // Check uniqueness.
        HashSet<(int x, int y)> found_pos = new HashSet<(int x, int y)>();
        for (int i=0; i<selected.Count; i++) {
            if (!found_pos.Add(selected[i])) {
                selected.RemoveAt(i--);
            }
        }
        foreach (var box in selected) {
            VisualBoardController.instance.SetHighlight(box.x, box.y, highlightColor);
        }
    }

    public BoxControllerEvent ToggleSelected = new BoxControllerEvent();
    public void ToggleSelectedAction(BoxController select) {
        int found = -1;
        for (int i=0; i<selected.Count; i++) if (selected[i] == select.position) found = i;
        if (found == -1) {
            selected.Add(select.position);
            VisualBoardController.instance.SetHighlight(select.position.x, select.position.y, highlightColor);
        } else {
            selected.RemoveAt(found);
            VisualBoardController.instance.ResetColor(select.position.x, select.position.y);
        }
    }

    public BoxControllerEvent EnsureSelected = new BoxControllerEvent();
    public void EnsureSelectedAction(BoxController select) {
        int found = -1;
        for (int i=0; i<selected.Count; i++) if (selected[i] == select.position) found = i;
        if (found == -1) {
            selected.Add(select.position);
            VisualBoardController.instance.SetHighlight(select.position.x, select.position.y, highlightColor);
        }
    }

    private Color GetColor(int i) {
        return colourPallete.colours[i-1];
    }

}
