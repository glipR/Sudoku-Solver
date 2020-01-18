using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSolver : ISolverSettings {

    public override void Initialise(BoardSerializer bs) {

    }

    public override bool definesState { get { return true; } }
    public override int numEntries { get { return 9; } }
    public override int transferState(string s) {
        if (s == "") return 0;
        return int.Parse(s);
    }
    public override string transferState(int t) {
        if (t == 0) return "";
        return t.ToString();
    }

    public override void PropogateChange(int i, int j, BoardSolver bs) {
        for (int k=0; k<bs.sudoku.settings.numHorizontal; k++) if (k != i) bs.EnsureNotPossible(k, j, bs.GetValue(i, j));
        for (int k=0; k<bs.sudoku.settings.numVertical; k++) if (k != j) bs.EnsureNotPossible(i, k, bs.GetValue(i, j));
        // Locate the box we are in
        int boxX = (int)((i / (bs.sudoku.settings.numHorizontalThins + 1)) * (bs.sudoku.settings.numHorizontalThins + 1));
        int boxY = (int)((j / (bs.sudoku.settings.numVerticalThins + 1)) * (bs.sudoku.settings.numVerticalThins + 1));
        for (int a=0; a<(bs.sudoku.settings.numHorizontalThins + 1); a++) for (int b=0; b<(bs.sudoku.settings.numVerticalThins + 1); b++) {
            if ((a + boxX != i) || (b + boxY != j)) bs.EnsureNotPossible(a + boxX, b + boxY, bs.GetValue(i, j));
        }
    }
    public override bool RestrictGrid(BoardSolver bs) {
        bool changed = false;
        for (uint v=1; v<=bs.sudoku.settings.numEntryTypes; v++) {
            for (int i=0; i<bs.sudoku.settings.numHorizontal; i++) {
                int v_located = 0;
                for (int j=0; j<bs.sudoku.settings.numVertical; j++) {
                    if (bs.Allows(i, j, v)) v_located ++;
                }
                if (v_located == 1) {
                    for (int j=0; j<bs.sudoku.settings.numVertical; j++) {
                        if (bs.Allows(i, j, v) && bs.GetValue(i, j) != v) {
                            changed = true;
                            bs.SetValue(i, j, v, false);
                        }
                    }
                }
            }
            for (int j=0; j<bs.sudoku.settings.numVertical; j++) {
                int v_located = 0;
                for (int i=0; i<bs.sudoku.settings.numHorizontal; i++) {
                    if (bs.Allows(i, j, v)) v_located ++;
                }
                if (v_located == 1) {
                    for (int i=0; i<bs.sudoku.settings.numHorizontal; i++) {
                        if (bs.Allows(i, j, v) && bs.GetValue(i, j) != v) {
                            changed = true;
                            bs.SetValue(i, j, v, false);
                        }
                    }
                }
            }
            for (int a=0; a<bs.sudoku.settings.numHorizontal; a+=(int)(bs.sudoku.settings.numHorizontalThins + 1)) {
                for (int b=0; b<bs.sudoku.settings.numVertical; b+=(int)(bs.sudoku.settings.numVerticalThins + 1)) {
                    // Check for box (a, b)
                    int v_located = 0;
                    for (int x=0; x<(bs.sudoku.settings.numHorizontalThins + 1); x++) for (int y=0; y<(bs.sudoku.settings.numVerticalThins + 1); y++) if (bs.Allows(a+x, b+y, v)) v_located++;
                    if (v_located == 1) {
                        for (int x=0; x<(bs.sudoku.settings.numHorizontalThins + 1); x++) for (int y=0; y<(bs.sudoku.settings.numVerticalThins + 1); y++) if (bs.Allows(a+x, b+y, v) && bs.GetValue(a+x, b+y) != v) {
                            changed = true;
                            bs.SetValue(a+x, b+y, v, false);
                        }
                    }
                }
            }
        }
        return changed;
    }
}
