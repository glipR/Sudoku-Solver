﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
        ListController.instance.SetSelected(this);
        VisualBoardController.instance.sudoku = board.Deserialized();
        VisualBoardController.instance.ResetView();
        VisualBoardController.instance.boardName = TitleField.text;
        ListController.instance.clicked = true;
    }

    public void SetColors(Color bg, Color mainText, Color secondaryText) {
        GetComponent<Image>().color = bg;
        TitleField.color = mainText;
        TagsField.color = secondaryText;
    }

}
