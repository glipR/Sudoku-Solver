using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSolver {

    public class SolverSettings {
        public uint numEntryTypes;
        public uint boxHeight;
        public uint boxWidth;
        public uint gridHeight;
        public uint gridWidth;
    }

    public SolverSettings settings = new SolverSettings();

    bool[,] solved;
    uint[,] options;
    uint[,] final_state;

    public void InitializeSolver(uint[, ] begin_state) {
        settings.boxHeight = 3;
        settings.boxWidth = 3;
        settings.gridHeight = 9;
        settings.gridWidth = 9;
        settings.numEntryTypes = 9;
        solved = new bool[settings.gridWidth, settings.gridHeight];
        options = new uint[settings.gridWidth, settings.gridHeight];
        final_state = new uint[settings.gridWidth, settings.gridHeight];


        for (int i=0; i<settings.gridWidth; i++) for (int j=0; j<settings.gridHeight; j++) {
            if (begin_state[i, j] == 0) { // Unknown
                options[i, j] = (1u << (int)settings.numEntryTypes) - 1;
                solved[i, j] = false;
                final_state[i, j] = 0;
            } else { // Given
                final_state[i, j] = begin_state[i, j];
                options[i, j] = (1u << (int)(final_state[i, j] - 1));
                solved[i, j] = true;
            }
        }
        for (int i=0; i<settings.gridWidth; i++) for (int j=0; j<settings.gridHeight; j++) if (solved[i, j]) PropogateSolve(i, j);
    }

    public uint[,] Solve(uint[,] begin_state) {
        InitializeSolver(begin_state);
        if (!ApplyRules()) {
            Debug.Log("Impossible to complete!");
        }
        return final_state;
    }

    public bool ApplyRules() {
        bool changed = false;
        while (true) {
            changed = false;
            for (int i=0; i<settings.gridWidth; i++) for (int j=0; j<settings.gridHeight; j++) {
                if (!solved[i, j]) {
                    if (options[i, j] == 0) return false;
                    if ((options[i, j] & (options[i, j]-1)) == 0) {
                        changed = true;
                        int v;
                        for (v=0; v<settings.numEntryTypes; v++) {
                            if ((options[i, j] & (1u << (v))) != 0) break;
                        }
                        final_state[i, j] = (uint)(v + 1);
                        solved[i, j] = true;
                        PropogateSolve(i, j);
                    }
                }
            }
            if (changed) continue;
            // Check availability for each number in each row, column, box.
            for (int v=1; v<=settings.numEntryTypes; v++) {
                for (int i=0; i<settings.gridWidth; i++) {
                    int v_located = 0;
                    for (int j=0; j<settings.gridHeight; j++) {
                        if ((options[i, j] & (1u << (v-1))) != 0) v_located ++;
                    }
                    if (v_located == 1) {
                        for (int j=0; j<settings.gridHeight; j++) {
                            if ((options[i, j] & (1u << (v-1))) != 0 && !solved[i, j]) {
                                changed = true;
                                final_state[i, j] = (uint)v;
                                solved[i, j] = true;
                                options[i, j] = (1u << (v-1));
                                PropogateSolve(i, j);
                            }
                        }
                    }
                }
                for (int j=0; j<settings.gridHeight; j++) {
                    int v_located = 0;
                    for (int i=0; i<settings.gridWidth; i++) {
                        if ((options[i, j] & (1u << (v-1))) != 0) v_located ++;
                    }
                    if (v_located == 1) {
                        for (int i=0; i<settings.gridWidth; i++) {
                            if ((options[i, j] & (1u << (v-1))) != 0 && !solved[i, j]) {
                                changed = true;
                                final_state[i, j] = (uint)v;
                                solved[i, j] = true;
                                options[i, j] = (1u << (v-1));
                                PropogateSolve(i, j);
                            }
                        }
                    }
                }
                for (int a=0; a<settings.gridWidth; a+=(int)settings.boxWidth) {
                    for (int b=0; b<settings.gridHeight; b+=(int)settings.boxHeight) {
                        // Check for box (a, b)
                        int v_located = 0;
                        for (int x=0; x<settings.boxWidth; x++) for (int y=0; y<settings.boxHeight; y++) if ((options[a+x, b+y] & (1u << (v-1))) != 0) v_located++;
                        if (v_located == 1) {
                            for (int x=0; x<settings.boxWidth; x++) for (int y=0; y<settings.boxHeight; y++) if ((options[a+x, b+y] & (1u << (v-1))) != 0 && !solved[a+x, b+y]) {
                                changed = true;
                                final_state[a+x, b+y] = (uint)v;
                                solved[a+x, b+y] = true;
                                options[a+x, b+y] = (1u << (v-1));
                                PropogateSolve(a+x, b+y);
                            }
                        }
                    }
                }
            }
            if (!changed) break;
        }
        return true;
    }

    public void PropogateSolve(int i, int j) {
        if (!solved[i, j]) return;
        for (int k=0; k<settings.gridWidth; k++) if (k != i) EnsureNotPossible(k, j, final_state[i, j]);
        for (int k=0; k<settings.gridHeight; k++) if (k != j) EnsureNotPossible(i, k, final_state[i, j]);
        // Locate the box we are in
        int boxX = (int)((i / settings.boxWidth) * settings.boxWidth);
        int boxY = (int)((j / settings.boxHeight) * settings.boxHeight);
        for (int a=0; a<settings.boxWidth; a++) for (int b=0; b<settings.boxHeight; b++) {
            if ((a + boxX != i) || (b + boxY != j)) EnsureNotPossible(a + boxX, b + boxY, final_state[i, j]);
        }
    }

    private void EnsureNotPossible(int i, int j, uint x) {
        if ((options[i, j] & (1u << (int)(x-1))) != 0) options[i, j] -= 1u << (int)(x-1);
    }

}
