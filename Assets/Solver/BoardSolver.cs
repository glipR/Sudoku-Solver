using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSolver {

    public Sudoku sudoku;

    bool[,] solved;
    uint[,] options;
    uint[,] final_state;
    List<(int x, int y)> solvedCoords = new List<(int x, int y)>();

    public void InitializeSolver(Sudoku beginning_state) {
        sudoku = beginning_state;
        sudoku.settings = beginning_state.settings;
        solved = new bool[sudoku.settings.numHorizontal, sudoku.settings.numVertical];
        options = new uint[sudoku.settings.numHorizontal, sudoku.settings.numVertical];
        final_state = new uint[sudoku.settings.numHorizontal, sudoku.settings.numVertical];

        for (int i = 0; i < sudoku.settings.numHorizontal; i++) for (int j = 0; j < sudoku.settings.numVertical; j++)
            {
                if (sudoku.MapEntry(sudoku.boxes[i * sudoku.settings.numVertical + j].answer) == 0)
                { // Unknown
                    options[i, j] = (1u << (int)sudoku.settings.numEntryTypes) - 1;
                    solved[i, j] = false;
                    final_state[i, j] = 0;
                }
                else
                { // Given
                    final_state[i, j] = (uint)sudoku.MapEntry(sudoku.boxes[i * sudoku.settings.numVertical + j].answer);
                    options[i, j] = 1u << (int)(final_state[i, j] - 1);
                    solved[i, j] = true;
                }
            }
        for (int i = 0; i < sudoku.settings.numHorizontal; i++) for (int j = 0; j < sudoku.settings.numVertical; j++) if (solved[i, j]) PropogateSolve(i, j);
    }

    public uint[,] Solve(Sudoku beginning_state) {
        InitializeSolver(beginning_state);
        if (!ApplyRules(false)) {
            Debug.Log("Impossible to complete!");
        }
        return final_state;
    }

    public (int x, int y) GetBoxHint(Sudoku beginning_state) {
        InitializeSolver(beginning_state);
        if (ApplyRules(true)) {
            return solvedCoords[0];
        }
        return (-1, -1);
    }

    public bool ApplyRules(bool stopOnFirstSolve) {
        bool changed = false;
        while (true) {
            changed = false;
            // Clear all boxes with 1 option.
            for (int i = 0; i < sudoku.settings.numHorizontal; i++) for (int j = 0; j < sudoku.settings.numVertical; j++) {
                if (!solved[i, j]) {
                    if (options[i, j] == 0) {
                        Debug.Log(i + " " + j);
                        return false;
                    }
                    if ((options[i, j] & (options[i, j] - 1)) == 0) {
                        changed = true;
                        uint v;
                        for (v = 1; v <= sudoku.settings.numEntryTypes; v++) {
                            if ((options[i, j] & (1u << (int)(v - 1))) != 0) break;
                        }
                        SetValue(i, j, v, true);
                        if (stopOnFirstSolve) return true;
                    }
                }
            }
            if (changed) continue;
            // Use variants to check box availability.
            foreach (Variant v in sudoku.variants) {
                changed = v.solver.RestrictGrid(this);
            }
            if (!changed) break;
        }
        return true;
    }

    public List<BoardNotifications.BoardError> CollectErrors(Sudoku beginning_state) {
        List<BoardNotifications.BoardError> all_errors = new List<BoardNotifications.BoardError>();
        InitializeSolver(beginning_state);
        foreach (Variant v in sudoku.variants) {
            all_errors.AddRange(v.solver.GetErrors(this));
        }
        return all_errors;
    }

    public void PropogateSolve(int i, int j) {
        if (!solved[i, j]) return;
        foreach (Variant v in sudoku.variants) {
            v.solver.PropogateChange(i, j, this);
        }
    }

    public void EnsureNotPossible(int i, int j, uint v) {
        if (Allows(i, j, v)) options[i, j] -= 1u << (int)(v - 1);
    }

    public uint GetValue(int i, int j) {
        if (!solved[i, j]) return 0;
        return final_state[i, j];
    }

    public void SetValue(int i, int j, uint v, bool change) {
        options[i, j] = 1u << (int)(v - 1);
        if (change) {
            solved[i, j] = true;
            final_state[i, j] = v;
            solvedCoords.Add((i, j));
            PropogateSolve(i, j);
        }
    }

    public bool Allows(int i, int j, uint v) {
        return (options[i, j] & (1u << (int)(v - 1))) != 0;
    }

    public bool Equals(int i, int j, uint v) {
        return (solved[i, j] && final_state[i, j] == v);
    }

    public bool Solved(int i, int j) {
        return solved[i, j];
    }

    public List<uint> GetOptions(int i, int j) {
        var res = new List<uint>();
        for (uint v=1; v<=sudoku.settings.numEntryTypes; v++) {
            if (Allows(i, j, v)) res.Add(v);
        }
        return res;
    }

}
