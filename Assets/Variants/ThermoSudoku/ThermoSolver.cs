﻿using System.Collections;
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
        var ss = bs.sudoku.GetVariant(VariantList.VariantType.Thermo.ToString()).serializer as ThermoSerializer;
        return false;
    }

    public override List<BoardNotifications.BoardError> GetErrors(BoardSolver bs) {
        List<BoardNotifications.BoardError> errors = new List<BoardNotifications.BoardError>();
        return errors;
    }

}