using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSerializer : ISerializer {

    public override void Initialise() {}
    public override string Serialize(Sudoku s) { return ""; }
    public override void Deserialize() {}
    public override void ApplyToBoard(VisualBoardController vbc) {}

}
