using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sudoku {

    [System.NonSerialized]
    public List<Variant> variants;
    public string[] variant_strings;
    [System.Serializable]
    public class SudokuSettings {
        // Lines / Grids
        public int numHorizontalThicks;
        public int numVerticalThicks;
        public int numHorizontalThins;
        public int numVerticalThins;

        public int numHorizontal { get { return (numHorizontalThicks - 1) * (numHorizontalThins + 1); } }
        public int numVertical { get { return (numVerticalThicks - 1) * (numHorizontalThins + 1); } }
        // Entry types
        public int numEntryTypes;
        // Extra information
        public bool lineNumbers = false;
    }

    public SudokuSettings settings;
    public BoardSerializer.SerializedBox[] boxes;
    public BoardSerializer.SerializedBox[] lineBoxes;

    public static Sudoku BasicSudoku() {
        Sudoku s = new Sudoku();
        s.variant_strings = new string[1];
        s.variant_strings[0] = "Base";
        return s;
    }

    public void Initialise() {
        // Add variants by string
        variants = new List<Variant>();
        foreach (string s in variant_strings)
            variants.Add(VariantList.GetVariant(s));
        // Update settings
        settings = new SudokuSettings();
        foreach (Variant v in variants) {
            v.settings.UpdateSettings(settings);
        }
    }

    public void LoadBoxes() {
        boxes = new BoardSerializer.SerializedBox[settings.numHorizontal * settings.numVertical];
        int lineSize = Mathf.Max(settings.numHorizontal, settings.numVertical);
        lineBoxes = new BoardSerializer.SerializedBox[4*lineSize];
        for (int i=0; i<settings.numHorizontal; i++) for (int j=0; j<settings.numVertical; j++)
            boxes[i*settings.numVertical+j] = new BoardSerializer.SerializedBox(VisualBoardController.instance.boxes[i, j]);
        if (settings.lineNumbers) {
            for (int i=0; i<2; i++) for (int j=0; j<2; j++) {
                if (i == 0) for (int k=0; k<settings.numVertical; k++) {
                    lineBoxes[2*i*lineSize + j*lineSize + k] = new BoardSerializer.SerializedBox(VisualBoardController.instance.lineBoxes[i, j, k]);
                } else for (int k=0; k<settings.numHorizontal; k++) {
                    lineBoxes[2*i*lineSize + j*lineSize + k] = new BoardSerializer.SerializedBox(VisualBoardController.instance.lineBoxes[i, j, k]);
                }
            }
        }
    }

    public void SetBoxAnswer(int i, int j, string ans) {
        int lineSize = Mathf.Max(settings.numHorizontal, settings.numVertical);
        if (i == BoxController.topBox || i == BoxController.botBox) {
            lineBoxes[2*lineSize + lineSize * (i == BoxController.topBox ? 1 : 0) + j].answer = ans;
            return;
        }
        if (j == BoxController.topBox || j == BoxController.botBox) {
            lineBoxes[lineSize * (j == BoxController.topBox ? 1 : 0) + i].answer = ans;
            return;
        }
        boxes[i*settings.numVertical + j].answer = ans;
    }

    public int MapEntry(string s) {
        for (int i=variants.Count-1; i>=0; i--) {
            if (variants[i].solver.definesState)
                return variants[i].solver.transferState(s);
        }
        return 0;
    }

    public string MapEntry(int t) {
        for (int i=variants.Count-1; i>=0; i--) {
            if (variants[i].solver.definesState)
                return variants[i].solver.transferState(t);
        }
        return "";
    }

    public Variant GetVariant(string s) {
        for (int i=0; i<variant_strings.Length; i++) if (variant_strings[i] == s) {
            return variants[i];
        }
        Debug.Log("Variant " + s + " not found");
        return null;
    }

}
