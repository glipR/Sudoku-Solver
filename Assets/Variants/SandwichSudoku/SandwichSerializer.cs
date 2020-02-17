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
        int amount = 0;
        for (int i=0; i<vbc.sudoku.settings.numHorizontal; i++) {
            if (vbc.GetBox(i, BoxController.topBox).currentVisibleFull != "") amount++;
            if (vbc.GetBox(i, BoxController.botBox).currentVisibleFull != "") amount++;
        }
        for (int j=0; j<vbc.sudoku.settings.numVertical; j++) {
            if (vbc.GetBox(BoxController.topBox, j).currentVisibleFull != "") amount++;
            if (vbc.GetBox(BoxController.botBox, j).currentVisibleFull != "") amount++;
        }
        serializedObject.boxes = new BoardSerializer.SerializedBox[amount];
        amount = 0;
        for (int i=0; i<vbc.sudoku.settings.numHorizontal; i++) {
            if (vbc.GetBox(i, BoxController.topBox).currentVisibleFull != "") serializedObject.boxes[amount++] = new BoardSerializer.SerializedBox(vbc.GetBox(i, BoxController.topBox));
            if (vbc.GetBox(i, BoxController.botBox).currentVisibleFull != "") serializedObject.boxes[amount++] = new BoardSerializer.SerializedBox(vbc.GetBox(i, BoxController.botBox));
        }
        for (int j=0; j<vbc.sudoku.settings.numVertical; j++) {
            if (vbc.GetBox(BoxController.topBox, j).currentVisibleFull != "") serializedObject.boxes[amount++] = new BoardSerializer.SerializedBox(vbc.GetBox(BoxController.topBox, j));
            if (vbc.GetBox(BoxController.botBox, j).currentVisibleFull != "") serializedObject.boxes[amount++] = new BoardSerializer.SerializedBox(vbc.GetBox(BoxController.botBox, j));
        }
        serializiationString = JsonUtility.ToJson(serializedObject);
    }

    public override void DeserializeToBoard(VisualBoardController vbc) {
        serializedObject = JsonUtility.FromJson<SerializedObject>(serializiationString);
        foreach (BoardSerializer.SerializedBox bc in serializedObject.boxes) {
            vbc.SetFull(bc.posx, bc.posy, bc.answer, false);
        }
    }

}
