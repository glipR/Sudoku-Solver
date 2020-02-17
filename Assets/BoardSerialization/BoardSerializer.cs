using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSerializer {

    [System.Serializable]
    public class SerializedBox {

        public bool given;
        public bool visible;
        public string answer;
        public int posx;
        public int posy;

        public SerializedBox(BoxController bc) {
            given = bc.given;
            answer = bc.currentVisibleFull;
            posx = bc.position.x;
            posy = bc.position.y;
            visible = bc.visible;
        }

        public void DeserializeToBox(ref BoxController bc) {
            bc.position = (posx, posy);
            VisualBoardController.instance.Clear(bc.position.x, bc.position.y, false);
            bc.given = given;
            bc.visible = visible;
            VisualBoardController.instance.SetFull(bc.position.x, bc.position.y, answer, false);
        }
    }

    [System.Serializable]
    public class SerializedBoard {
        public Sudoku sudoku;
        public SerializedBox[] boxes;
        public string[] variantSerializations;

        public SerializedBoard(VisualBoardController vbc) {
            sudoku = vbc.sudoku;
            boxes = new SerializedBox[sudoku.settings.numHorizontal * sudoku.settings.numVertical];
            for (int i=0; i<sudoku.settings.numHorizontal; i++) for (int j=0; j<sudoku.settings.numVertical; j++) {
                boxes[i*sudoku.settings.numVertical+j] = new SerializedBox(vbc.boxes[i,j]);
            }
            variantSerializations = new string[sudoku.variants.Count];
            for (int i=0; i<sudoku.variants.Count; i++) {
                sudoku.variants[i].serializer.Serialize(vbc);
                variantSerializations[i] = sudoku.variants[i].serializer.serializiationString;
            }
        }

        public void DeserializeToBoard(VisualBoardController vbc) {
            vbc.sudoku = sudoku;
            vbc.sudoku.Initialise();
            for (int i=0; i<sudoku.settings.numHorizontal; i++) for (int j=0; j<sudoku.settings.numVertical; j++) {
                boxes[i*sudoku.settings.numVertical + j].DeserializeToBox(ref vbc.boxes[i,j]);
            }
            for (int i=0; i<sudoku.variants.Count; i++) {
                sudoku.variants[i].serializer.serializiationString = variantSerializations[i];
                sudoku.variants[i].serializer.DeserializeToBoard(vbc);
            }
        }
    }

}
