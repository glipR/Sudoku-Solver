using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class ISerializer {

    public string serializationString { get; set; }

    public abstract void Initialise();
    public abstract string Serialize(Sudoku s);
    public abstract void Deserialize();
    public abstract void ApplyToBoard(VisualBoardController vbc);

}
