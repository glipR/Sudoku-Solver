using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxSelectionManager : MonoBehaviour {

    public enum SelectionVersion {
        FULL,
        CENTRE,
        CORNER,
        COLOR,
    }

    private static KeyCode[] keyCodes = { KeyCode.None, KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9 };

    public static BoxSelectionManager instance;
    [SerializeField]
    private Color highlightColor = new Color(0.5f, 1, 1, 1);

    private List<(int x, int y)> selected;
    private SelectionVersion currentSelection = SelectionVersion.FULL;

    private void Start() {
        instance = this;
        selected = new List<(int x, int y)>();
    }

    private void Update() {
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
            }
        }
        if (Input.GetKeyDown(KeyCode.Backspace)) foreach (var box in selected) VisualBoardController.instance.Clear(box.x, box.y);
    }

    public void SetSelected(BoxController select) {
        foreach (var box in selected) {
            VisualBoardController.instance.ResetColor(box.x, box.y);
        }
        selected.Clear();
        selected.Add(select.position);
        VisualBoardController.instance.SetColor(selected[0].x, selected[0].y, highlightColor, false);
    }

    public void ShiftSelected((int x, int y) shift) {
        foreach (var box in selected) {
            VisualBoardController.instance.ResetColor(shift.x, shift.y);
        }
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
            VisualBoardController.instance.SetColor(box.x, box.y, highlightColor, false);
        }
    }

    public void ToggleSelected(BoxController select) {
        int found = -1;
        for (int i=0; i<selected.Count; i++) if (selected[i] == select.position) found = i;
        if (found == -1) {
            selected.Add(select.position);
            VisualBoardController.instance.SetColor(select.position.x, select.position.y, highlightColor, false);
        } else {
            selected.RemoveAt(found);
            VisualBoardController.instance.ResetColor(select.position.x, select.position.y);
        }
    }

    public void EnsureSelected(BoxController select) {
        int found = -1;
        for (int i=0; i<selected.Count; i++) if (selected[i] == select.position) found = i;
        if (found == -1) {
            selected.Add(select.position);
            VisualBoardController.instance.SetColor(select.position.x, select.position.y, highlightColor, false);
        }
    }

}
