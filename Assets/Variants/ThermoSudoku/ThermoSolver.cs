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
        bool changed = false;
        var ts = bs.sudoku.GetVariant(VariantList.VariantType.Thermo.ToString()).serializer as ThermoSerializer;
        for (int i=0; i<bs.sudoku.settings.numHorizontal; i++) for (int j=0; j<bs.sudoku.settings.numVertical; j++) {
            for (int k=0; k<4; k++) {
                if (ts.incoming[i, j, k]) {
                    changed |= bs.EnsureLarger(i, j, i + ThermoSerializer.directions[k, 0], j + ThermoSerializer.directions[k, 1]);
                }
                if (ts.outgoing[i, j, k]) {
                    changed |= bs.EnsureLarger(i + ThermoSerializer.directions[k, 0], j + ThermoSerializer.directions[k, 1], i, j);
                }
            }
        }
        return changed;
    }

    public override List<BoardNotifications.BoardError> GetErrors(BoardSolver bs) {
        List<BoardNotifications.BoardError> errors = new List<BoardNotifications.BoardError>();
        var ts = bs.sudoku.GetVariant(VariantList.VariantType.Thermo.ToString()).serializer as ThermoSerializer;
        for (int i=0; i<bs.sudoku.settings.numHorizontal; i++) for (int j=0; j<bs.sudoku.settings.numVertical; j++) {
            for (int k=0; k<4; k++) {
                (int x, int y) newPos = (i + ThermoSerializer.directions[k, 0], j + ThermoSerializer.directions[k, 1]);
                if (bs.Solved(i, j) && bs.Solved(newPos.x, newPos.y)) {
                    if (ts.incoming[i, j, k] && (bs.GetValue(i, j) <= bs.GetValue(newPos.x, newPos.y))) {
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
