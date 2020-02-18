using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class ISerializer {

    public string serializiationString { get; set; }

    public abstract void Initialise();
    public abstract void Serialize(Sudoku s);
    public abstract void ApplyToBoard(VisualBoardController vbc);

}
