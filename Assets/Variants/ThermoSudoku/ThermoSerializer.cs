using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThermoSerializer : ISerializer {

    public SerializedObject serializedObject;
    public bool[,,] incoming;
    public bool[,,] outgoing;

    public static int[,] directions = {
        {0, -1},
        {1, 0},
        {0, 1},
        {-1, 0}
    };

    [System.Serializable]
    public class SerializedObject {
        public ThermoRules thermo;
    }

    [System.Serializable]
    public class ThermoRules {
        [System.Serializable]
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
        incoming = new bool[vbc.sudoku.settings.numHorizontal, vbc.sudoku.settings.numVertical, 4];
        outgoing = new bool[vbc.sudoku.settings.numHorizontal, vbc.sudoku.settings.numVertical, 4];
        for (int i=0; i<vbc.sudoku.settings.numHorizontal; i++) for (int j=0; j<vbc.sudoku.settings.numVertical; j++) for (int k=0; k<4; k++) {
            incoming[i, j, k] = false;
            outgoing[i, j, k] = false;
        }
        foreach (ThermoRules.BoxPair bp in serializedObject.thermo.dependencies) {
            // Calculate direction
            for (int dir=0;dir<4;dir++) {
                if (bp.X1 + directions[dir, 0] == bp.X2 && bp.Y1 + directions[dir, 1] == bp.Y2) {
                    outgoing[bp.X2, bp.Y2, (dir+2)%4] = true;
                    incoming[bp.X1, bp.Y1, dir] = true;
                }
            }
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
                    Sprite s = Resources.Load<Sprite>("Default/Thermos/Base1Connection");
                    int rotation = 0;
                    if (outgoing[i, j, 1]) {
                        rotation = 270;
                    } else if (outgoing[i, j, 2]) {
                        rotation = 180;
                    } else if (outgoing[i, j, 3]) {
                        rotation = 90;
                    }
                    vbc.boxes[i, j].AddUnderlay(s, rotation);
                }
                else if (amountOutgoing == 4) {
                    // All directions - easy
                    Sprite s = Resources.Load<Sprite>("Default/Thermos/Base4Connections");
                    vbc.boxes[i, j].AddUnderlay(s, 0);
                }
                else if (amountOutgoing == 3) {
                    // One missing, rotate to align
                    Sprite s = Resources.Load<Sprite>("Default/Thermos/Base3Connections");
                    int rotation = 0;
                    if (!outgoing[i, j, 0]) {
                        rotation = 270;
                    } else if (!outgoing[i, j, 1]) {
                        rotation = 180;
                    } else if (!outgoing[i, j, 2]) {
                        rotation = 90;
                    }
                    vbc.boxes[i, j].AddUnderlay(s, rotation);
                }
                else if (amountOutgoing == 2) {
                    // Corner or opposite.
                    if (outgoing[i, j, 0] == outgoing[i, j, 2]) {
                        // Opposite.
                        Sprite s = Resources.Load<Sprite>("Default/Thermos/Base2ConnectionsOpposite");
                        int rotation = 0;
                        if (outgoing[i, j, 1]) rotation = 90;
                        vbc.boxes[i, j].AddUnderlay(s, rotation);
                    } else {
                        // Corner.
                        Sprite s = Resources.Load<Sprite>("Default/Thermos/Base2ConnectionsCorner");
                        int rotation;
                        if (outgoing[i, j, 0]) {
                            if (outgoing[i, j, 1]) {
                                rotation = 0;
                            } else {
                                rotation = 90;
                            }
                        } else {
                            if (outgoing[i, j, 1]) {
                                rotation = 270;
                            } else {
                                rotation = 180;
                            }
                        }
                        vbc.boxes[i, j].AddUnderlay(s, rotation);
                    }
                }
            }
            else {
                // Line / Ending.
                if (amountEither == 1) {
                    Sprite s = Resources.Load<Sprite>("Default/Thermos/Line1Connection");
                    int rotation = 0;
                    if (outgoing[i, j, 0] || incoming[i, j, 0]) {
                        rotation = 180;
                    }
                    else if (outgoing[i, j, 1] || incoming[i, j, 1]) {
                        rotation = 90;
                    }
                    else if (outgoing[i, j, 3] || incoming[i, j, 3]) {
                        rotation = 270;
                    }
                    vbc.boxes[i, j].AddUnderlay(s, rotation);
                }
                else if (amountEither == 4) {
                    // All directions - easy
                    Sprite s = Resources.Load<Sprite>("Default/Thermos/Line4Connections");
                    vbc.boxes[i, j].AddUnderlay(s, 0);
                }
                else if (amountEither == 3) {
                    // One missing, rotate to align
                    Sprite s = Resources.Load<Sprite>("Default/Thermos/Line3Connections");
                    int rotation = 0;
                    if (!(outgoing[i, j, 0] || incoming[i, j, 0])) {
                        rotation = 270;
                    }
                    else if (!(outgoing[i, j, 1] || incoming[i, j, 1])) {
                        rotation = 180;
                    }
                    else if (!(outgoing[i, j, 2] || incoming[i, j, 2])) {
                        rotation = 90;
                    }
                    vbc.boxes[i, j].AddUnderlay(s, rotation);
                }
                else if (amountEither == 2) {
                    // Corner or opposite.
                    if ((incoming[i, j, 0] || outgoing[i, j, 0]) == (incoming[i, j, 2] || outgoing[i, j, 2])) {
                        // Opposite.
                        Sprite s = Resources.Load<Sprite>("Default/Thermos/Line2ConnectionsOpposite");
                        int rotation = 0;
                        if (outgoing[i, j, 1] || incoming[i, j, 1]) {
                            rotation = 90;
                        }
                        vbc.boxes[i, j].AddUnderlay(s, rotation);
                    } else {
                        // Corner.
                        Sprite s = Resources.Load<Sprite>("Default/Thermos/Line2ConnectionsCorner");
                        int rotation = 0;
                        if (outgoing[i, j, 0] || incoming[i, j, 0]) {
                            if (outgoing[i, j, 1] || incoming[i, j, 1]) {
                                rotation = 90;
                            }
                            else {
                                rotation = 180;
                            }
                        }
                        else {
                            if (outgoing[i, j, 1] || incoming[i, j, 1]) {
                                rotation = 0;
                            }
                            else {
                                rotation = 270;
                            }
                        }
                        vbc.boxes[i, j].AddUnderlay(s, rotation);
                    }
                }
            }
        }
    }

}
