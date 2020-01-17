﻿using System.Collections;
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

    private List<BoxController> selected;
    private SelectionVersion currentSelection = SelectionVersion.FULL;

    private void Start() {
        instance = this;
        selected = new List<BoxController>();
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
                    foreach (var box in selected) box.SetFull(i.ToString());
                if (currentSelection == SelectionVersion.CORNER)
                    foreach (var box in selected) box.ToggleCorner(i);
                if (currentSelection == SelectionVersion.CENTRE)
                    foreach (var box in selected) box.ToggleCentre(i);
            }
        }
        if (Input.GetKeyDown(KeyCode.Backspace)) foreach (var box in selected) box.Clear();
    }

    public void SetSelected(BoxController select) {
        foreach (var box in selected) {
            box.SetColor(box.currentColor, true);
        }
        selected.Clear();
        selected.Add(select);
        selected[0].SetColor(highlightColor, false);
    }

    public void ShiftSelected((int x, int y) shift) {
        foreach (var box in selected) {
            box.SetColor(box.currentColor, true);
        }
        for (int i=0; i<selected.Count; i++) {
            if (
                0 <= selected[i].position.x + shift.x &&
                selected[i].position.x + shift.x < VisualBoardController.instance.settings.numHorizontal &&
                0 <= selected[i].position.y + shift.y &&
                selected[i].position.y + shift.y < VisualBoardController.instance.settings.numVertical
            )
                selected[i] = VisualBoardController.instance.boxes[selected[i].position.x + shift.x, selected[i].position.y + shift.y];
            selected[i].SetColor(highlightColor, false);
        }
        // Check uniqueness.
        HashSet<(int x, int y)> found_pos = new HashSet<(int x, int y)>();
        for (int i=0; i<selected.Count; i++) {
            if (!found_pos.Add(selected[i].position)) {
                selected.RemoveAt(i--);
            }
        }
    }

    public void ToggleSelected(BoxController select) {
        int found = -1;
        for (int i=0; i<selected.Count; i++) if (selected[i].position == select.position) found = i;
        if (found == -1) {
            selected.Add(select);
            select.SetColor(highlightColor, false);
        } else {
            selected.RemoveAt(found);
            select.SetColor(select.currentColor, true);
        }
    }

    public void EnsureSelected(BoxController select) {
        int found = -1;
        for (int i=0; i<selected.Count; i++) if (selected[i].position == select.position) found = i;
        if (found == -1) {
            selected.Add(select);
            select.SetColor(highlightColor, false);
        }
    }

}
