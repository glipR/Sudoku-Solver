using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThermoSolver : ISolverSettings {

    public override void Initialise(BoardSerializer bs) {}

    public override bool definesState { get { return false; } }
    public override int numEntries { get { return 9; } }
    public override int transferState(string s) { return 0; }
    public override string transferState(int t) { return ""; }

    public override void PropogateChange(int i, int j, BoardSolver bs) {}
    public override bool RestrictGrid(BoardSolver bs) {
        var ss = bs.sudoku.GetVariant(VariantList.VariantType.Thermos.ToString()).serializer as ThermosSerializer;
        bool changed = false;
        foreach (var box in ss.serializedObject.boxes) {
            var points = new List<(int x, int y)>();
            if (box.posx == BoxController.topBox || box.posx == BoxController.botBox) {
                int oneIndex = -1;
                int oneCounts = 0;
                int nineIndex = -1;
                int nineCounts = 0;
                for (int i=0; i<bs.sudoku.settings.numHorizontal; i++) {
                    if (bs.Allows(box.posy, i, 1)) { oneCounts++; oneIndex = i; }
                    if (bs.Allows(box.posy, i, 9)) { nineCounts++; nineIndex = i; }
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
                for (int j=0; j<bs.sudoku.settings.numHorizontal; j++) {
                    if (bs.Allows(j, box.posx, 1u)) { oneCounts++; oneIndex = j; }
                    if (bs.Allows(j, box.posx, 9u)) { nineCounts++; nineIndex = j; }
                }
                if (oneCounts == 1 && nineCounts == 1) {
                    for (int j=Mathf.Min(oneIndex, nineIndex)+1; j<Mathf.Max(oneIndex, nineIndex); j++) points.Add((j, box.posx));
                }
            }
            changed |= SumUtility.RestrictSum(bs, points, int.Parse(box.answer));
        }
        return changed;
    }

    public override List<BoardNotifications.BoardError> GetErrors(BoardSolver bs) {
        List<BoardNotifications.BoardError> errors = new List<BoardNotifications.BoardError>();
        var ss = bs.sudoku.GetVariant(VariantList.VariantType.Sandwich.ToString()).serializer as SandwichSerializer;
        foreach (var box in ss.serializedObject.boxes) {
            if (box.posx == BoxController.topBox || box.posx == BoxController.botBox) {
                int oneIndex = -1;
                int oneCounts = 0;
                int nineIndex = -1;
                int nineCounts = 0;
                for (int i=0; i<bs.sudoku.settings.numHorizontal; i++) {
                    if (bs.Equals(box.posy, i, 1)) { oneCounts++; oneIndex = i; }
                    if (bs.Equals(box.posy, i, 9)) { nineCounts++; nineIndex = i; }
                }
                if (oneCounts == 1 && nineCounts == 1) {
                    List<(int x, int y)> points = new List<(int x, int y)>();
                    bool bad = false;
                    for (int i=Mathf.Min(oneIndex, nineIndex)+1; i<Mathf.Max(oneIndex, nineIndex); i++) {
                        points.Add((box.posy, i));
                        if (!bs.Solved(box.posy, i)) bad = true;
                    }
                    if (!bad) {
                        int s = 0;
                        foreach (var v in points) {
                            s += (int)bs.GetValue(v.x, v.y);
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
                for (int j=0; j<bs.sudoku.settings.numHorizontal; j++) {
                    if (bs.Equals(j, box.posx, 1u)) { oneCounts++; oneIndex = j; }
                    if (bs.Equals(j, box.posx, 9u)) { nineCounts++; nineIndex = j; }
                }
                if (oneCounts == 1 && nineCounts == 1) {
                    List<(int x, int y)> points = new List<(int x, int y)>();
                    bool bad = false;
                    for (int j=Mathf.Min(oneIndex, nineIndex)+1; j<Mathf.Max(oneIndex, nineIndex); j++) {
                        points.Add((j, box.posx));
                        if (!bs.Solved(j, box.posx)) bad = true;
                    }
                    if (!bad) {
                        int s = 0;
                        foreach (var v in points) {
                            s += (int)bs.GetValue(v.x, v.y);
                        }
                        if (s != int.Parse(box.answer)) errors.Add(new BoardNotifications.BoardError(BoardNotifications.ErrorType.SUM_INVALID, points, "These boxes must sum to " + box.answer));
                    }
                }
            }
        }
        return errors;
    }

}
