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
    private Color highlightColor;

    private List<BoxController> selected;
    private SelectionVersion currentSelection = SelectionVersion.FULL;

    private void Start() {
        instance = this;
        selected = new List<BoxController>();
    }

    private void Update() {
        for (int i=1; i<keyCodes.Length; i++) {
            if (Input.GetKeyDown(keyCodes[i])) {
                foreach (var box in selected) box.SetCentre(i.ToString());
            }
        }
    }

    public void SetSelected(BoxController select) {
        foreach (var box in selected) {
            box.SetColor(box.currentColor, true);
        }
        selected.Clear();
        selected.Add(select);
        selected[0].SetColor(highlightColor, false);
    }

}
