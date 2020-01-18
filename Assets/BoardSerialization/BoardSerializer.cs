using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSerializer {

    const string SEPARATOR = "\n";

    [System.Serializable]
    public class SerializedBox {

        public bool given;
        public string answer;
        public int posx;
        public int posy;

        public SerializedBox(BoxController bc) {
            given = bc.given;
            answer = bc.currentVisibleFull;
            posx = bc.position.x;
            posy = bc.position.y;
        }

        public void DeserializeToBox(ref BoxController bc) {
            bc.position = (posx, posy);
            bc.given = given;
            VisualBoardController.instance.Clear(bc.position.x, bc.position.y);
            VisualBoardController.instance.SetFull(bc.position.x, bc.position.y, answer);
        }
    }

    [System.Serializable]
    public class SerializedBoard {
        public Sudoku sudoku;
        public SerializedBox[] boxes;

        public SerializedBoard(VisualBoardController vbc) {
            sudoku = vbc.sudoku;
            boxes = new SerializedBox[sudoku.settings.numHorizontal * sudoku.settings.numVertical];
            for (int i=0; i<sudoku.settings.numHorizontal; i++) for (int j=0; j<sudoku.settings.numVertical; j++) {
                boxes[i*sudoku.settings.numVertical+j] = new SerializedBox(vbc.boxes[i,j]);
            }
        }

        public void DeserializeToBoard(VisualBoardController vbc) {
            vbc.sudoku = sudoku;
            vbc.sudoku.Initialise();
            for (int i=0; i<sudoku.settings.numHorizontal; i++) for (int j=0; j<sudoku.settings.numVertical; j++) {
                boxes[i*sudoku.settings.numVertical + j].DeserializeToBox(ref vbc.boxes[i,j]);
            }
        }
    }

}
