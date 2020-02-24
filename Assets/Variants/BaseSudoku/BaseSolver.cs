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
        for (int k=0; k<bs.final.sudoku.settings.numHorizontal; k++) if (k != i) bs.EnsureNotPossible(k, j, bs.GetValue(i, j));
        for (int k=0; k<bs.final.sudoku.settings.numVertical; k++) if (k != j) bs.EnsureNotPossible(i, k, bs.GetValue(i, j));
        // Locate the box we are in
        int boxX = (int)((i / (bs.final.sudoku.settings.numHorizontalThins + 1)) * (bs.final.sudoku.settings.numHorizontalThins + 1));
        int boxY = (int)((j / (bs.final.sudoku.settings.numVerticalThins + 1)) * (bs.final.sudoku.settings.numVerticalThins + 1));
        for (int a=0; a<(bs.final.sudoku.settings.numHorizontalThins + 1); a++) for (int b=0; b<(bs.final.sudoku.settings.numVerticalThins + 1); b++) {
            if ((a + boxX != i) || (b + boxY != j)) bs.EnsureNotPossible(a + boxX, b + boxY, bs.GetValue(i, j));
        }
    }
    public override BoardSolver.SolveResult RestrictGrid(BoardSolver bs) {
        bool changed = false;
        for (int v=1; v<=9; v++) {
            for (int i=0; i<bs.final.sudoku.settings.numHorizontal; i++) {
                int v_located = 0;
                for (int j=0; j<bs.final.sudoku.settings.numVertical; j++) {
                    if (bs.Allows(i, j, v.ToString())) v_located ++;
                }
                if (v_located == 0) return BoardSolver.SolveResult.Impossible;
                if (v_located == 1) {
                    for (int j=0; j<bs.final.sudoku.settings.numVertical; j++) {
                        if (bs.Allows(i, j, v.ToString()) && bs.GetValue(i, j) != v.ToString()) {
                            changed = true;
                            bs.SetValue(i, j, v.ToString());
                        }
                    }
                }
            }
            for (int j=0; j<bs.final.sudoku.settings.numVertical; j++) {
                int v_located = 0;
                for (int i=0; i<bs.final.sudoku.settings.numHorizontal; i++) {
                    if (bs.Allows(i, j, v.ToString())) v_located ++;
                }
                if (v_located == 0) return BoardSolver.SolveResult.Impossible;
                if (v_located == 1) {
                    for (int i=0; i<bs.final.sudoku.settings.numHorizontal; i++) {
                        if (bs.Allows(i, j, v.ToString()) && bs.GetValue(i, j) != v.ToString()) {
                            changed = true;
                            bs.SetValue(i, j, v.ToString());
                        }
                    }
                }
            }
            for (int a=0; a<bs.final.sudoku.settings.numHorizontal; a+=(int)(bs.final.sudoku.settings.numHorizontalThins + 1)) {
                for (int b=0; b<bs.final.sudoku.settings.numVertical; b+=(int)(bs.final.sudoku.settings.numVerticalThins + 1)) {
                    // Check for box (a, b)
                    int v_located = 0;
                    for (int x=0; x<(bs.final.sudoku.settings.numHorizontalThins + 1); x++) for (int y=0; y<(bs.final.sudoku.settings.numVerticalThins + 1); y++)
                        if (bs.Allows(a+x, b+y, v.ToString())) v_located++;
                    if (v_located == 0) return BoardSolver.SolveResult.Impossible;
                    if (v_located == 1) {
                        for (int x=0; x<(bs.final.sudoku.settings.numHorizontalThins + 1); x++) for (int y=0; y<(bs.final.sudoku.settings.numVerticalThins + 1); y++)
                            if (bs.Allows(a+x, b+y, v.ToString()) && bs.GetValue(a+x, b+y) != v.ToString()) {
                                changed = true;
                                bs.SetValue(a+x, b+y, v.ToString());
                            }
                    }
                }
            }
        }
        if (changed) return BoardSolver.SolveResult.Solved;
        return BoardSolver.SolveResult.Unchanged;
    }

    public override List<BoardNotifications.BoardError> GetErrors(BoardSolver bs) {
        List<BoardNotifications.BoardError> errors = new List<BoardNotifications.BoardError>();
        for (int v=1; v<=9; v++) {
            for (int i=0; i<bs.final.sudoku.settings.numHorizontal; i++) {
                List<(int x, int y)> boxes = new List<(int x, int y)>();
                for (int j=0; j<bs.final.sudoku.settings.numVertical; j++) {
                    if (bs.GetValue(i, j) == v.ToString()) {
                        boxes.Add((i, j));
                    }
                }
                if (boxes.Count > 1) {
                    errors.Add(new BoardNotifications.BoardError(BoardNotifications.ErrorType.COL_INVALID, boxes, "This column can only have a single " + v));
                }
            }
            for (int j=0; j<bs.final.sudoku.settings.numVertical; j++) {
                List<(int x, int y)> boxes = new List<(int x, int y)>();
                for (int i=0; i<bs.final.sudoku.settings.numHorizontal; i++) {
                    if (bs.GetValue(i, j) == v.ToString()) {
                        boxes.Add((i, j));
                    }
                }
                if (boxes.Count > 1) {
                    errors.Add(new BoardNotifications.BoardError(BoardNotifications.ErrorType.ROW_INVALID, boxes, "This row can only have a single " + v));
                }
            }
            for (int a=0; a<bs.final.sudoku.settings.numHorizontal; a+=(int)(bs.final.sudoku.settings.numHorizontalThins + 1)) {
                for (int b=0; b<bs.final.sudoku.settings.numVertical; b+=(int)(bs.final.sudoku.settings.numVerticalThins + 1)) {
                    // Check for box (a, b)
                    List<(int x, int y)> boxes = new List<(int x, int y)>();
                    for (int x=0; x<(bs.final.sudoku.settings.numHorizontalThins + 1); x++) for (int y=0; y<(bs.final.sudoku.settings.numVerticalThins + 1); y++)
                        if (bs.GetValue(a+x, b+y) == v.ToString()) {
                            boxes.Add((a+x, b+y));
                        }
                    if (boxes.Count > 1) {
                        errors.Add(new BoardNotifications.BoardError(BoardNotifications.ErrorType.BOX_INVALID, boxes, "This box can only have a single " + v));
                    }
                }
            }
        }
        return errors;
    }
}
