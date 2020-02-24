using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandwichSolver : ISolverSettings {

    public override void Initialise(BoardSerializer bs) {}

    public override bool definesState { get { return false; } }
    public override int numEntries { get { return 9; } }
    public override int transferState(string s) { return 0; }
    public override string transferState(int t) { return ""; }

    public override void PropogateChange(int i, int j, BoardSolver bs) {}
    public override BoardSolver.SolveResult RestrictGrid(BoardSolver bs) {
        var ss = bs.final.sudoku.GetVariant(VariantList.VariantType.Sandwich.ToString()).serializer as SandwichSerializer;
        bool changed = false;
        foreach (var box in ss.serializedObject.boxes) {
            var points = new List<(int x, int y)>();
            if (box.posx == BoxController.topBox || box.posx == BoxController.botBox) {
                int oneIndex = -1;
                int oneCounts = 0;
                int nineIndex = -1;
                int nineCounts = 0;
                for (int i=0; i<bs.final.sudoku.settings.numHorizontal; i++) {
                    if (bs.Allows(box.posy, i, 1.ToString())) { oneCounts++; oneIndex = i; }
                    if (bs.Allows(box.posy, i, 9.ToString())) { nineCounts++; nineIndex = i; }
                }
                if (oneCounts == 1 && nineCounts == 1) {
                    for (int i=Mathf.Min(oneIndex, nineIndex)+1; i<Mathf.Max(oneIndex, nineIndex); i++) points.Add((box.posy, i));
                }
            }
            else if (box.posy == BoxController.topBox || box.posy == BoxController.botBox) {
                int oneIndex = -1;
                int oneCounts = 0;
                int nineIndex = -1;
                int nineCounts = 0;
                for (int j=0; j<bs.final.sudoku.settings.numHorizontal; j++) {
                    if (bs.Allows(j, box.posx, 1.ToString())) { oneCounts++; oneIndex = j; }
                    if (bs.Allows(j, box.posx, 9.ToString())) { nineCounts++; nineIndex = j; }
                }
                if (oneCounts == 1 && nineCounts == 1) {
                    for (int j=Mathf.Min(oneIndex, nineIndex)+1; j<Mathf.Max(oneIndex, nineIndex); j++) points.Add((j, box.posx));
                }
            } else continue;
            var r = SumUtility.RestrictSum(bs, points, int.Parse(box.answer));
            if (r == BoardSolver.SolveResult.Impossible) return r;
            if (r == BoardSolver.SolveResult.Solved) changed = true;
        }
        if (changed) return BoardSolver.SolveResult.Solved;
        return BoardSolver.SolveResult.Unchanged;
    }

    public override List<BoardNotifications.BoardError> GetErrors(BoardSolver bs) {
        List<BoardNotifications.BoardError> errors = new List<BoardNotifications.BoardError>();
        var ss = bs.final.sudoku.GetVariant(VariantList.VariantType.Sandwich.ToString()).serializer as SandwichSerializer;
        foreach (var box in ss.serializedObject.boxes) {
            if (box.posx == BoxController.topBox || box.posx == BoxController.botBox) {
                int oneIndex = -1;
                int oneCounts = 0;
                int nineIndex = -1;
                int nineCounts = 0;
                for (int i=0; i<bs.final.sudoku.settings.numHorizontal; i++) {
                    if (bs.GetValue(box.posy, i) == "1") { oneCounts++; oneIndex = i; }
                    if (bs.GetValue(box.posy, i) == "9") { nineCounts++; nineIndex = i; }
                }
                if (oneCounts == 1 && nineCounts == 1) {
                    List<(int x, int y)> points = new List<(int x, int y)>();
                    bool bad = false;
                    for (int i=Mathf.Min(oneIndex, nineIndex)+1; i<Mathf.Max(oneIndex, nineIndex); i++) {
                        points.Add((box.posy, i));
                        if (bs.GetValue(box.posy, i) == "") bad = true;
                    }
                    if (!bad) {
                        int s = 0;
                        foreach (var v in points) {
                            int res;
                            if (int.TryParse(bs.GetValue(v.x, v.y), out res)) {
                                s += res;
                            } else {
                                List<(int x, int y)> p = new List<(int x, int y)>();
                                p.Add(v);
                                errors.Add(new BoardNotifications.BoardError(BoardNotifications.ErrorType.SELECTION_INVALID, p, "This box must be numeric."));
                            }
                        }
                        if (s != int.Parse(box.answer)) errors.Add(new BoardNotifications.BoardError(BoardNotifications.ErrorType.SUM_INVALID, points, "These boxes must sum to " + box.answer));
                    }
                }
            }
            else if (box.posy == BoxController.topBox || box.posy == BoxController.botBox) {
                int oneIndex = -1;
                int oneCounts = 0;
                int nineIndex = -1;
                int nineCounts = 0;
                for (int j=0; j<bs.final.sudoku.settings.numHorizontal; j++) {
                    if (bs.GetValue(j, box.posx) == "1") { oneCounts++; oneIndex = j; }
                    if (bs.GetValue(j, box.posx) == "9") { nineCounts++; nineIndex = j; }
                }
                if (oneCounts == 1 && nineCounts == 1) {
                    List<(int x, int y)> points = new List<(int x, int y)>();
                    bool bad = false;
                    for (int j=Mathf.Min(oneIndex, nineIndex)+1; j<Mathf.Max(oneIndex, nineIndex); j++) {
                        points.Add((j, box.posx));
                        if (bs.GetValue(j, box.posx) == "") bad = true;
                    }
                    if (!bad) {
                        int s = 0;
                        foreach (var v in points) {
                            int res;
                            if (int.TryParse(bs.GetValue(v.x, v.y), out res)) {
                                s += res;
                            } else {
                                List<(int x, int y)> p = new List<(int x, int y)>();
                                p.Add(v);
                                errors.Add(new BoardNotifications.BoardError(BoardNotifications.ErrorType.SELECTION_INVALID, p, "This box must be numeric."));
                            }
                        }
                        if (s != int.Parse(box.answer)) errors.Add(new BoardNotifications.BoardError(BoardNotifications.ErrorType.SUM_INVALID, points, "These boxes must sum to " + box.answer));
                    }
                }
            }
        }
        return errors;
    }

}
