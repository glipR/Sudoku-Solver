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
        var ss = bs.sudoku.GetVariant(VariantList.VariantType.Thermo.ToString()).serializer as ThermoSerializer;
        for (int i=0; i<bs.sudoku.settings.numHorizontal; i++) for (int j=0; j<bs.sudoku.settings.numVertical; j++) {
            for (int k=0; k<4; k++) {
                if (ss.incoming[i, j, k]) {
                    changed |= bs.EnsureLarger(i, j, i + ThermoSerializer.directions[k, 0], j + ThermoSerializer.directions[k, 1]);
                }
                if (ss.outgoing[i, j, k]) {
                    changed |= bs.EnsureLarger(i + ThermoSerializer.directions[k, 0], j + ThermoSerializer.directions[k, 1], i, j);
                }
            }
        }
        return changed;
    }

    public override List<BoardNotifications.BoardError> GetErrors(BoardSolver bs) {
        List<BoardNotifications.BoardError> errors = new List<BoardNotifications.BoardError>();
        return errors;
    }

}
