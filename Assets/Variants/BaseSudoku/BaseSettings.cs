using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSettings : ISudokuSettings {

    public override void UpdateSettings(Sudoku.SudokuSettings settings) {
        settings.numEntryTypes = 9;
        settings.numHorizontalThicks = 4;
        settings.numVerticalThicks = 4;
        settings.numHorizontalThins = 2;
        settings.numVerticalThins = 2;
    }

}
