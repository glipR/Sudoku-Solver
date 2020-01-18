using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class ISerializer {

    public ISerializer(VisualBoardController vbc) {}
    public abstract void DeserializeToBoard(VisualBoardController vbc);

}
