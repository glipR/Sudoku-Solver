using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SumUtility {

    public static int maxPerms = 1000;

    public static bool RestrictSum(BoardSolver boardSolver, List<(int x, int y)> points, int sum) {
        if (points.Count == 0) return false;
        uint perms = 1;
        List<List<uint> > options = new List<List<uint>>();
        foreach (var p in points) {
            options.Add(boardSolver.GetOptions(p.x, p.y));
            perms *= (uint)options[options.Count-1].Count;
        }
        if (perms < maxPerms) {
            Dictionary<(int x, int y), uint> new_options = new Dictionary<(int x, int y), uint>();
            List<int> indicies = new List<int>();
            foreach (var p in points) {
                new_options[(p.x, p.y)] = 0;
                indicies.Add(0);
            }
            // Try every permutation.
            while (true) {
                int s = 0;
                for (int i=0; i<options.Count; i++) {
                    s += (int)options[i][indicies[i]];
                }
                if (s == sum) for (int i=0; i<options.Count; i++) {
                    if ((new_options[(points[i].x, points[i].y)] & (1u << indicies[i])) == 0)
                        new_options[(points[i].x, points[i].y)] += (1u << indicies[i]);
                }
                bool breaking = false;
                indicies[indicies.Count-1]++;
                for (int i=indicies.Count-1; i>=0; i--) if (indicies[i] == options[i].Count) {
                    indicies[i] = 0;
                    if (i == 0) {
                        breaking = true;
                    } else {
                        indicies[i-1]++;
                    }
                }
                if (breaking) break;
            }
            bool changed = false;
            for (int i=0; i<options.Count; i++) {
                var o = new_options[(points[i].x, points[i].y)];
                for (int j=0; j<options[i].Count; j++) if ((o & (1u << j)) == 0) {
                    changed = true;
                    boardSolver.EnsureNotPossible(points[i].x, points[i].y, options[i][j]);
                }
            }
            return changed;
        }
        return false;
    }

}
