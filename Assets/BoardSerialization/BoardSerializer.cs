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
            bc.Clear();
            bc.SetFull(answer);
            bc.given = given;
            bc.position = (posx, posy);
        }
    }

    [System.Serializable]
    public class SerializedBoard {
        public VisualBoardSettings settings;
        public SerializedBox[] boxes;

        public SerializedBoard(VisualBoardController vbc) {
            settings = vbc.settings;
            boxes = new SerializedBox[settings.numHorizontal * settings.numVertical];
            for (int i=0; i<settings.numHorizontal; i++) for (int j=0; j<settings.numVertical; j++) {
                boxes[i*settings.numVertical+j] = new SerializedBox(vbc.boxes[i,j]);
            }
        }

        public void DeserializeToBoard(VisualBoardController vbc) {
            vbc.settings = settings;
            for (int i=0; i<settings.numHorizontal; i++) for (int j=0; j<settings.numVertical; j++) {
                boxes[i*settings.numVertical + j].DeserializeToBox(ref vbc.boxes[i,j]);
            }
        }
    }

}
