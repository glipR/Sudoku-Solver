﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ListItem : MonoBehaviour {

    public BoardSerializer.SerializedBoard board;

    [SerializeField]
    private TextMeshProUGUI TitleField;
    [SerializeField]
    private TextMeshProUGUI TagsField;

    public void SetTitle(string t) {
        TitleField.text = t;
    }

    public void SetBoard(BoardSerializer.SerializedBoard b) {
        board = b;
        TagsField.text = string.Join(" ", b.sudoku.variant_strings);
    }

    private void OnMouseDown() {
        board.DeserializeToBoard(VisualBoardController.instance);
        VisualBoardController.instance.Initialise();
        board.DeserializeToBoard(VisualBoardController.instance);
    }

}