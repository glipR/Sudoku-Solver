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
        public int numHorizontalThicks;
        public int numVerticalThicks;
        public int numHorizontalThins;
        public int numVerticalThins;

        public int numHorizontal { get { return (numHorizontalThicks - 1) * (numHorizontalThins + 1); } }
        public int numVertical { get { return (numVerticalThicks - 1) * (numHorizontalThins + 1); } }
        public int numEntryTypes;
    }

    public SudokuSettings settings;
    public BoardSerializer.SerializedBox[] boxes;

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
        boxes = new BoardSerializer.SerializedBox[settings.numHorizontal * settings.numVertical];
        for (int i=0; i<settings.numHorizontal; i++) for (int j=0; j<settings.numVertical; j++) boxes[i*settings.numVertical+j] = new BoardSerializer.SerializedBox(VisualBoardController.instance.boxes[i, j]);
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

}
