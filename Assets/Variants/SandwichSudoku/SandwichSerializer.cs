using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandwichSerializer : ISerializer {

    public SerializedObject serializedObject;

    [System.Serializable]
    public class SerializedObject {
        public BoardSerializer.SerializedBox[] boxes;
    }

    public override void Initialise() {
        serializedObject = new SerializedObject();
        serializedObject.boxes = new BoardSerializer.SerializedBox[0];
    }

    public override string Serialize(Sudoku s) {
        serializedObject = new SerializedObject();
        int amount = 0;
        for (int i=0; i<s.settings.numHorizontal; i++) {
            if (s.GetBox(i, BoxController.topBox).answer != "") amount++;
            if (s.GetBox(i, BoxController.botBox).answer != "") amount++;
        }
        for (int j=0; j<s.settings.numVertical; j++) {
            if (s.GetBox(BoxController.topBox, j).answer != "") amount++;
            if (s.GetBox(BoxController.botBox, j).answer != "") amount++;
        }
        serializedObject.boxes = new BoardSerializer.SerializedBox[amount];
        amount = 0;
        for (int i=0; i<s.settings.numHorizontal; i++) {
            if (s.GetBox(i, BoxController.topBox).answer != "") serializedObject.boxes[amount++] = s.GetBox(i, BoxController.topBox);
            if (s.GetBox(i, BoxController.botBox).answer != "") serializedObject.boxes[amount++] = s.GetBox(i, BoxController.botBox);
        }
        for (int j=0; j<s.settings.numVertical; j++) {
            if (s.GetBox(BoxController.topBox, j).answer != "") serializedObject.boxes[amount++] = s.GetBox(BoxController.topBox, j);
            if (s.GetBox(BoxController.botBox, j).answer != "") serializedObject.boxes[amount++] = s.GetBox(BoxController.botBox, j);
        }
        serializationString = JsonUtility.ToJson(serializedObject);
        return serializationString;
    }

    public override void Deserialize() {
        serializedObject = JsonUtility.FromJson<SerializedObject>(serializationString);
    }

    public override void ApplyToBoard(VisualBoardController vbc) {
        foreach (BoardSerializer.SerializedBox bc in serializedObject.boxes) {
            vbc.SetFull(bc.posx, bc.posy, bc.answer, false);
        }
    }

}
