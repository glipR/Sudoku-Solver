using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandwichSerializer : ISerializer {

    public SerializedObject serializedObject;

    [System.Serializable]
    public class SerializedObject {
        public BoardSerializer.SerializedBox[] boxes;
    }

    public override void Serialize(VisualBoardController vbc) {
        serializedObject = new SerializedObject();
        serializedObject.boxes = new BoardSerializer.SerializedBox[vbc.lineBoxes.Count];
        for (int i=0; i<vbc.lineBoxes.Count; i++) serializedObject.boxes[i] = new BoardSerializer.SerializedBox(vbc.lineBoxes[i]);
        serializiationString = JsonUtility.ToJson(serializedObject);
    }

    public override void DeserializeToBoard(VisualBoardController vbc) {
        serializedObject = JsonUtility.FromJson<SerializedObject>(serializiationString);
        foreach (BoardSerializer.SerializedBox bc in serializedObject.boxes) {
            if (bc.posx == BoxController.topBox) vbc.AddColNumber(bc.posy, bc.answer, true);
            else if (bc.posx == BoxController.botBox) vbc.AddColNumber(bc.posy, bc.answer, false);
            else if (bc.posy == BoxController.topBox) vbc.AddRowNumber(bc.posx, bc.answer, true);
            else if (bc.posy == BoxController.botBox) vbc.AddRowNumber(bc.posx, bc.answer, false);
        }
    }

}
