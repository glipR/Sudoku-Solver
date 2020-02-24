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
    public override BoardSolver.SolveResult RestrictGrid(BoardSolver bs) {
        BoardSolver.SolveResult r = BoardSolver.SolveResult.Unchanged;
        var ts = bs.final.sudoku.GetVariant(VariantList.VariantType.Thermo.ToString()).serializer as ThermoSerializer;
        for (int i=0; i<bs.final.sudoku.settings.numHorizontal; i++) for (int j=0; j<bs.final.sudoku.settings.numVertical; j++) {
            for (int k=0; k<4; k++) {
                if (ts.incoming[i, j, k]) {
                    r = bs.Combine(r, bs.EnsureLarger(i, j, i + ThermoSerializer.directions[k, 0], j + ThermoSerializer.directions[k, 1]));
                }
                if (ts.outgoing[i, j, k]) {
                    r = bs.Combine(r, bs.EnsureLarger(i + ThermoSerializer.directions[k, 0], j + ThermoSerializer.directions[k, 1], i, j));
                }
            }
        }
        return r;

    }

    public override List<BoardNotifications.BoardError> GetErrors(BoardSolver bs) {
        List<BoardNotifications.BoardError> errors = new List<BoardNotifications.BoardError>();
        var ts = bs.final.sudoku.GetVariant(VariantList.VariantType.Thermo.ToString()).serializer as ThermoSerializer;
        for (int i=0; i<bs.final.sudoku.settings.numHorizontal; i++) for (int j=0; j<bs.final.sudoku.settings.numVertical; j++) {
            for (int k=0; k<4; k++) {
                (int x, int y) newPos = (i + ThermoSerializer.directions[k, 0], j + ThermoSerializer.directions[k, 1]);
                if (bs.GetValue(i, j) != "" && bs.GetValue(newPos.x, newPos.y) != "") {
                    int res1, res2;
                    if (!int.TryParse(bs.GetValue(i, j), out res1)) {
                        List<(int x, int y)> boxes = new List<(int x, int y)>();
                        boxes.Add((i, j));
                        errors.Add(new BoardNotifications.BoardError(
                            BoardNotifications.ErrorType.SELECTION_INVALID,
                            boxes,
                            "This box must be numeric."
                        ));
                    } else if (!int.TryParse(bs.GetValue(newPos.x, newPos.y), out res2)) {
                        List<(int x, int y)> boxes = new List<(int x, int y)>();
                        boxes.Add((newPos.x, newPos.y));
                        errors.Add(new BoardNotifications.BoardError(
                            BoardNotifications.ErrorType.SELECTION_INVALID,
                            boxes,
                            "This box must be numeric."
                        ));
                    } else if (ts.incoming[i, j, k] && res1 <= res2) {
                        List<(int x, int y)> affected = new List<(int x, int y)>();
                        affected.Add((i, j));
                        affected.Add((newPos.x, newPos.y));
                        errors.Add(new BoardNotifications.BoardError(
                            BoardNotifications.ErrorType.SELECTION_INVALID,
                            affected,
                            "Thermo rules require that numbers increase along lines, meaning we require " + bs.GetValue(i, j) + " > " + bs.GetValue(newPos.x, newPos.y)
                        ));
                    }
                }
            }
        }
        return errors;
    }

}
