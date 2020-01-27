using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandwichSolver : ISolverSettings {

    public override void Initialise(BoardSerializer bs) {}

    public override bool definesState { get { return false; } }
    public override int numEntries { get { return 9; } }
    public override int transferState(string s) { return 0; }
    public override string transferState(int t) { return ""; }

    public override void PropogateChange(int i, int j, BoardSolver bs) {}
    public override bool RestrictGrid(BoardSolver bs) {
        var ss = bs.sudoku.GetVariant(VariantList.VariantType.Sandwich.ToString()).serializer as SandwichSerializer;
        bool changed = false;
        foreach (var box in ss.serializedObject.boxes) {
            var points = new List<(int x, int y)>();
            if (box.posx == BoxController.topBox || box.posx == BoxController.botBox) {
                int oneIndex = -1;
                int oneCounts = 0;
                int nineIndex = -1;
                int nineCounts = 0;
                for (int i=0; i<bs.sudoku.settings.numHorizontal; i++) {
                    if (bs.Allows(box.posy, i, 1)) { oneCounts++; oneIndex = i; }
                    if (bs.Allows(box.posy, i, 9)) { nineCounts++; nineIndex = i; }
                }
                if (oneCounts == 1 && nineCounts == 1) {
                    for (int i=Mathf.Min(oneIndex, nineIndex)+1; i<Mathf.Max(oneIndex, nineIndex); i++) points.Add((box.posy, i));
                }
            }
            else if (box.posy == BoxController.topBox || box.posy == BoxController.botBox) {
                int oneIndex = -1;
                int oneCounts = 0;
                int nineIndex = -1;
                int nineCounts = 0;
                for (int j=0; j<bs.sudoku.settings.numHorizontal; j++) {
                    if (bs.Allows(j, box.posx, 1u)) { oneCounts++; oneIndex = j; }
                    if (bs.Allows(j, box.posx, 9u)) { nineCounts++; nineIndex = j; }
                }
                if (oneCounts == 1 && nineCounts == 1) {
                    for (int j=Mathf.Min(oneIndex, nineIndex)+1; j<Mathf.Max(oneIndex, nineIndex); j++) points.Add((j, box.posx));
                }
            }
            changed |= SumUtility.RestrictSum(bs, points, int.Parse(box.answer));
        }
        return changed;
    }

}
