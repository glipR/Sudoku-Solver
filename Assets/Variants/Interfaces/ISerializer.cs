using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class ISerializer {

    public string serializiationString { get; set; }

    public abstract void Serialize(VisualBoardController vbc);
    public abstract void DeserializeToBoard(VisualBoardController vbc);

}
