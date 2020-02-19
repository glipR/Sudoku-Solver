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

        public SerializedBox(int x, int y) {
            given = false;
            answer = "";
            posx = x;
            posy = y;
            visible = true;
        }

        public void DeserializeToBox(ref BoxController bc) {
            bc.position = (posx, posy);
            string a = answer;
            VisualBoardController.instance.Clear(bc.position.x, bc.position.y, false);
            bc.given = given;
            bc.visible = visible;
            VisualBoardController.instance.SetFull(bc.position.x, bc.position.y, a, false);
        }
    }

    [System.Serializable]
    public class SerializedBoard {
        public Sudoku sudoku;
        public string[] variantSerializations;

        public SerializedBoard(Sudoku s) {
            sudoku = s;
            variantSerializations = new string[sudoku.variants.Count];
            for (int i=0; i<sudoku.variants.Count; i++) {
                variantSerializations[i] = sudoku.variants[i].serializer.Serialize(s);
            }
        }

        public Sudoku Deserialized() {
            // Ensure the serializationStrings are set.
            sudoku.GenerateVariants();
            sudoku.Initialise();
            for (int i=0; i<sudoku.variants.Count; i++) {
                sudoku.variants[i].serializer.serializationString = variantSerializations[i];
                sudoku.variants[i].serializer.Deserialize();
            }
            return sudoku;
        }
    }

}
