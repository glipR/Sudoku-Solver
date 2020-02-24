using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ISolverSettings {

    public abstract void Initialise(BoardSerializer bs);

    public virtual bool definesState { get { return false; } }
    public virtual int numEntries { get { return 0; } }
    public abstract int transferState(string s);
    public abstract string transferState(int t);

    public abstract void PropogateChange(int i, int j, BoardSolver bs);
    // Only returns Impossible, Solved, or Unchanged
    public abstract BoardSolver.SolveResult RestrictGrid(BoardSolver bs);
    public abstract List<BoardNotifications.BoardError> GetErrors(BoardSolver bs);

}
