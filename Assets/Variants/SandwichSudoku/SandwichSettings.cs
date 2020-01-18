using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandwichSettings : ISudokuSettings {

    public override void UpdateSettings(Sudoku.SudokuSettings settings) {
        settings.lineNumbers = true;
    }

}
