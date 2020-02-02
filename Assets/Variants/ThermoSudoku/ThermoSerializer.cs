using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThermoSerializer : ISerializer {

    public SerializedObject serializedObject;

    [System.Serializable]
    public class SerializedObject {
        public ThermoRules thermo;
    }

    [System.Serializable]
    public class ThermoRules {
        public class BoxPair {
            public int X1;
            public int Y1;
            public int X2;
            public int Y2;
        }
        // (x1, y1, x2, y2) => x1, y1 must be LARGER than x2, y2. (IE x2, y2 can be a base)
        public BoxPair[] dependencies;
    }

    public override void Serialize(VisualBoardController vbc) {
        serializedObject = new SerializedObject();
        serializiationString = JsonUtility.ToJson(serializedObject);
    }

    public override void DeserializeToBoard(VisualBoardController vbc) {
        serializedObject = JsonUtility.FromJson<SerializedObject>(serializiationString);
        // Find out what to apply to what box.
        // URDL is direction 0123.
        bool[,,] incoming = new bool[vbc.sudoku.settings.numHorizontal, vbc.sudoku.settings.numVertical, 4];
        bool[,,] outgoing = new bool[vbc.sudoku.settings.numHorizontal, vbc.sudoku.settings.numVertical, 4];
        for (int i=0; i<vbc.sudoku.settings.numHorizontal; i++) for (int j=0; j<vbc.sudoku.settings.numVertical; j++) for (int k=0; k<4; k++) {
            incoming[i, j, k] = false;
            outgoing[i, j, k] = true;
        }
        foreach (ThermoRules.BoxPair bp in serializedObject.thermo.dependencies) {
            // Calculate direction
            int dir;
            if (bp.X1 != bp.X2) {
                if (bp.X1 > bp.X2) dir = 1;
                else dir = 3;
            } else {
                if (bp.Y1 > bp.Y2) dir = 2;
                else dir = 0;
            }
            outgoing[bp.X2, bp.Y2, dir] = true;
            incoming[bp.X1, bp.X2, (dir+2)%4] = true;
        }

        for (int i=0; i<vbc.sudoku.settings.numHorizontal; i++) for (int j=0; j<vbc.sudoku.settings.numVertical; j++) {
            int amountIncoming = 0;
            int amountOutgoing = 0;
            int amountEither = 0;
            for (int k=0; k<4; k++) {
                if (incoming[i, j, k]) amountIncoming++;
                if (outgoing[i, j, k]) amountOutgoing++;
                if (incoming[i, j, k] || outgoing[i, j, k]) amountEither++;
            }
            if (amountIncoming == 0 && amountOutgoing == 0) continue;
            if (amountIncoming == 0) {
                // Base
                if (amountOutgoing == 1) {
                    // Single line - sort out direction
                } else if (amountOutgoing == 4) {
                    // All directions - easy
                } else if (amountOutgoing == 3) {
                    // One missing, rotate to align
                } else if (amountOutgoing == 2) {
                    // Corner or opposite.
                    if (outgoing[i, j, 0] == outgoing[i, j, 2]) {
                        // Opposite.
                    } else {
                        // Corner.
                    }
                }
            } else {
                // Line / Ending.
                if (amountEither == 1) {
                    // End node - sort out direction
                } else if (amountEither == 4) {
                    // All directions - easy
                } else if (amountEither == 3) {
                    // One missing, rotate to align
                } else if (amountEither == 2) {
                    // Corner or opposite.
                    if ((incoming[i, j, 0] || outgoing[i, j, 0]) == (incoming[i, j, 2] || outgoing[i, j, 2])) {
                        // Opposite.
                    } else {
                        // Corner.
                    }
                }
            }
        }
    }

}
