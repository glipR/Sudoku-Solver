using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SumUtility {

    public static int maxPerms = 1000;

    public static BoardSolver.SolveResult RestrictSum(BoardSolver boardSolver, List<(int x, int y)> points, int sum) {
        if (points.Count == 0) return BoardSolver.SolveResult.Unchanged;
        uint perms = 1;
        List<List<string> > options = new List<List<string>>();
        foreach (var p in points) {
            options.Add(new List<string>(boardSolver.final.possible_values[p.x, p.y]));
            perms *= (uint)options[options.Count-1].Count;
        }
        if (perms < maxPerms) {
            List<string>[] new_options = new List<string>[points.Count];
            List<int> indicies = new List<int>();
            int a=0;
            foreach (var p in points) {
                new_options[a++] = new List<string>();
                indicies.Add(0);
            }
            // Try every permutation.
            while (true) {
                int s = 0;
                for (int i=0; i<options.Count; i++) {
                    s += int.Parse(options[i][indicies[i]]);
                }
                if (s == sum) for (int i=0; i<options.Count; i++) {
                    if (!new_options[i].Contains(options[i][indicies[i]]))
                        new_options[i].Add(options[i][indicies[i]]);
                }
                bool breaking = false;
                indicies[indicies.Count-1]++;
                for (int i=indicies.Count-1; i>=0; i--)
                    if (indicies[i] == options[i].Count) {
                        indicies[i] = 0;
                        if (i == 0) {
                            breaking = true;
                        } else {
                            indicies[i-1]++;
                        }
                    } else break;
                if (breaking) break;
            }
            BoardSolver.SolveResult r = BoardSolver.SolveResult.Unchanged;
            for (int i=0; i<options.Count; i++) {
                foreach (var x in options[i]) {
                    if (!new_options[i].Contains(x))
                        r = boardSolver.Combine(r, boardSolver.EnsureNotPossible(points[i].x, points[i].y, x));
                }
            }
            return r;
        }
        return BoardSolver.SolveResult.Incomplete;
    }

}
