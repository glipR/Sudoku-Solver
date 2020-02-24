using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSolver {

    public enum SolveResult {
        Solved,         // Sudoku is solved, final.sudoku and final.possible_values is up to date.
        Incomplete,     // Solver is inadequate to solve any further, final.sudoku and final.possible_values is up to date.
        MaybeMultiple,  // Solver has found a single solution, but can't rule out other solutions. final.sudoku and final.possible_values denotes one such solve.
        Multiple,       // Multiple solutions are possible, final.sudoku shows one such solution.
        Impossible,     // Sudoku is impossible to solve.
        Unchanged       // final and original should be exactly the same.
    }

    public class SolveState {
        public Sudoku sudoku;
        public List<string>[,] possible_values;
    }

    public SolveState original;
    public SolveState final;

    // Solving options.
    public class SolveSettings {
        public bool stopOnFirstSolve;
        public bool bruteForce;
    }

    public SolveSettings settings = new SolveSettings();

    public void Initialise(Sudoku s) {
        original = new SolveState();
        original.sudoku = s;
        original.possible_values = new List<string>[s.settings.numHorizontal, s.settings.numVertical];
        for (int i=0; i<s.settings.numHorizontal; i++) for (int j=0; j<s.settings.numVertical; j++) {
            if (s.GetBox(i, j).answer == "") {
                original.possible_values[i, j] = s.AllValues();
            }
            else {
                original.possible_values[i, j] = new List<string>();
                original.possible_values[i, j].Add(s.GetBox(i, j).answer);
            }
        }
        final = original;
    }

    public void Initialise(SolveState bs, SolveSettings ss) {
        settings = ss;
        original = bs;
        final = bs;
    }

    public SolveResult Solve() {
        for (int i=0; i<final.sudoku.settings.numHorizontal; i++) for (int j=0; j<final.sudoku.settings.numVertical; j++) {
            // Trigger the propogate for all already set vlaues.
            if (GetValue(i, j) != "") SetValue(i, j, GetValue(i, j));
        }
        SolveResult s = ApplyRules();
        if (s == SolveResult.Impossible)
            return s;
        if (s == SolveResult.Solved)
            return s;
        // In any other case, we can continue solving.
        if (!settings.bruteForce) {
            return s;
        }
        // Find the smallest possible switch box
        (int x, int y) bestPoint = (-1, -1);
        int best = final.sudoku.AllValues().Count + 1;
        for (int i=0; i<final.sudoku.settings.numHorizontal; i++) for (int j=0; j<final.sudoku.settings.numVertical; j++) {
            if ((final.possible_values[i, j].Count < best) && (final.possible_values[i, j].Count != 1)) {
                best = final.possible_values[i, j].Count;
                bestPoint = (i, j);
            }
        }
        // Split on (i, j)
        int amountSolved = 0;
        int amountIncomplete = 0;
        SolveState newFinal = new SolveState();
        foreach (var k in final.possible_values[bestPoint.x, bestPoint.y]) {
            BoardSolver bs = new BoardSolver();
            bs.Initialise(final, settings);
            var r = bs.Solve();
            if (r == SolveResult.Solved) {
                amountSolved ++;
                newFinal = bs.final;
            }
            if (r == SolveResult.Incomplete) {
                amountIncomplete ++;
                if (amountSolved == 0) newFinal = bs.final;
            }
            if (r == SolveResult.MaybeMultiple) {
                newFinal = bs.final;
                amountSolved ++;
                amountIncomplete ++;
            }
            if (r == SolveResult.Multiple) return r;
            if (r == SolveResult.Impossible) continue;
        }
        if ((amountSolved == 0) && (amountIncomplete == 0)) return SolveResult.Impossible;
        final = newFinal;
        if (amountSolved > 1) return SolveResult.Multiple;
        if (amountIncomplete == 0) {
            return SolveResult.Solved;
        }
        if (amountSolved == 0) {
            return SolveResult.Incomplete;
        }
        return SolveResult.MaybeMultiple;
    }

    public void SetValue(int i, int j, string k) {
        final.possible_values[i, j] = new List<string>();
        final.possible_values[i, j].Add(k);
        final.sudoku.SetBoxAnswer(i, j, k);
        for (int a=0; a<final.sudoku.variants.Count; a++) {
            final.sudoku.variants[a].solver.PropogateChange(i, j, this);
        }
    }

    public bool Allows(int i, int j, string v) {
        return (final.possible_values[i, j].Contains(v));
    }

    public SolveResult EnsureNotPossible(int i, int j, string k) {
        if (Allows(i, j, k)) {
            final.possible_values[i, j].Remove(k);
            if (final.possible_values[i, j].Count == 0) return SolveResult.Impossible;
            if (final.possible_values[i, j].Count == 1) SetValue(i, j, final.possible_values[i, j][0]);
            return SolveResult.Solved;
        }
        return SolveResult.Unchanged;
    }

    public SolveResult EnsureLarger(int x1, int y1, int x2, int y2) {
        SolveResult r = SolveResult.Unchanged;
        int smallest_value = -1;
        foreach (var s in final.possible_values[x2, y2]) {
            int res;
            if (int.TryParse(s, out res))
                if (smallest_value == -1 || res < smallest_value)
                    smallest_value = res;
        }
        if (smallest_value == -1) return SolveResult.Incomplete;
        foreach (var s in new List<string>(final.possible_values[x1, y1])) {
            int res;
            if (int.TryParse(s, out res))
                if (smallest_value >= res) {
                    r = Combine(r, EnsureNotPossible(x1, y1, s));
                }
        }
        // The other way.
        int largest_value = -1;
        foreach (var s in final.possible_values[x1, y1]) {
            int res;
            if (int.TryParse(s, out res))
                if (res > largest_value)
                    largest_value = res;
        }
        if (largest_value == -1) return SolveResult.Incomplete;
        foreach (var s in new List<string>(final.possible_values[x2, y2])) {
            int res;
            if (int.TryParse(s, out res))
                if (largest_value <= res) {
                    r = Combine(r, EnsureNotPossible(x2, y2, s));
                }
        }
        return r;
    }

    public SolveResult Combine(SolveResult r1, SolveResult r2) {
        if (r1 == SolveResult.Impossible || r2 == SolveResult.Impossible) return SolveResult.Impossible;
        if (r1 == SolveResult.Multiple || r2 == SolveResult.Multiple) return SolveResult.Multiple;
        if (r1 == SolveResult.Incomplete || r2 == SolveResult.Incomplete) {
            if (r1 == SolveResult.MaybeMultiple || r2 == SolveResult.MaybeMultiple) return SolveResult.MaybeMultiple;
            if (r1 == SolveResult.Solved || r2 == SolveResult.Solved) return SolveResult.MaybeMultiple;
            return SolveResult.Incomplete;
        }
        if (r1 == SolveResult.MaybeMultiple || r2 == SolveResult.MaybeMultiple) return SolveResult.MaybeMultiple;
        if (r1 == SolveResult.Solved || r2 == SolveResult.Solved) return SolveResult.Solved;
        // Only two pairs of unchanged.
        return r1;
    }

    public SolveResult ApplyRules() {
        bool changed = false;
        while (true) {
            changed = false;
            // Use variants to check box availability.
            foreach (Variant v in final.sudoku.variants) {
                SolveResult r = v.solver.RestrictGrid(this);
                if (r == SolveResult.Impossible) return r;
                if (r == SolveResult.Unchanged) continue;
                // Otherwise valid changes have been made
                changed = true;
            }
            if (!changed) break;
        }
        for (int i=0; i<final.sudoku.settings.numHorizontal; i++) for (int j=0; j<final.sudoku.settings.numVertical; j++) {
            if (final.sudoku.GetBox(i, j).answer == "") return SolveResult.Incomplete;
        }
        return SolveResult.Solved;
    }

    public string GetValue(int i, int j) {
        return final.sudoku.GetBox(i, j).answer;
    }

    public List<BoardNotifications.BoardError> CollectErrors(Sudoku state) {
        List<BoardNotifications.BoardError> all_errors = new List<BoardNotifications.BoardError>();
        Initialise(state);
        foreach (Variant v in original.sudoku.variants) {
            all_errors.AddRange(v.solver.GetErrors(this));
        }
        return all_errors;
    }
}
