using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardNotifications {

    public enum ErrorType {
        // Basic Sudoku
        ROW_INVALID,
        COL_INVALID,
        BOX_INVALID,
        // Arithmetic
        SUM_INVALID
    }

    public class BoardError {
        public ErrorType type;
        public List<(int x, int y)> affectedBoxes = new List<(int x, int y)>();
        public string displayMessage;

        public BoardError(ErrorType type, List<(int x, int y)> affectedBoxes, string displayMessage) {
            this.type = type;
            this.affectedBoxes = affectedBoxes;
            this.displayMessage = displayMessage;
        }
    }
}
