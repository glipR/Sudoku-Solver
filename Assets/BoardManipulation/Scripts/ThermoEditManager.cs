using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThermoEditManager : IEditManager {


    public (int posx, int posy) selected;
    public bool selecting;

    public override void Initialise() {
        selected = (-1, -1);
        selecting = false;
    }

    public override void ModifyListeners() {
        BoxSelectionManager.instance.SetSelected.RemoveAllListeners();
        BoxSelectionManager.instance.SetSelected.AddListener(SetSelected);
        BoxSelectionManager.instance.ToggleSelected.RemoveAllListeners();
        BoxSelectionManager.instance.ToggleSelected.AddListener(TryConnectingAndRemove);
        BoxSelectionManager.instance.EnsureSelected.RemoveAllListeners();
        BoxSelectionManager.instance.EnsureSelected.AddListener(TryConnecting);
        BoxSelectionManager.instance.HandleKeyStrokes.RemoveAllListeners();
    }

    public void SetSelected(BoxController bc) {
        if (selecting) {
            VisualBoardController.instance.ResetColor(selected.posx, selected.posy, false);
        }
        selecting = true;
        selected = bc.position;
        VisualBoardController.instance.SetHighlight(selected.posx, selected.posy, BoxSelectionManager.instance.highlightColor);
    }

    public void TryConnectingAndRemove(BoxController bc) {
        if (!selecting) return;
        if (selected.posx == bc.position.x && selected.posy == bc.position.y) {
            VisualBoardController.instance.ResetColor(selected.posx, selected.posy, false);
            selected = (-1, -1);
            selecting = false;
        } else {
            TryConnecting(bc);
        }
    }

    public void TryConnecting(BoxController bc) {
        if (!selecting) return;
        int dist = Mathf.Abs(selected.posx - bc.position.x) + Mathf.Abs(selected.posy - bc.position.y);
        if (dist == 1) {
            var ts = VisualBoardController.instance.sudoku.GetVariant(VariantList.VariantType.Thermo.ToString()).serializer as ThermoSerializer;
            var dep = ts.serializedObject.thermo.dependencies;
            // Check if outgoing connection already exists - remove if so.
            // Otherwise add an incoming connection
            int found = -1;
            for (int i=0; i<dep.Length; i++) {
                if (dep[i].X1 == selected.posx && dep[i].Y1 == selected.posy && dep[i].X2 == bc.position.x && dep[i].Y2 == bc.position.y) {
                    found = i;
                }
            }
            if (found == -1) {
                var newDep = new ThermoSerializer.ThermoRules.BoxPair[dep.Length+1];
                for (int i=0; i<dep.Length; i++) newDep[i] = dep[i];
                newDep[dep.Length] = new ThermoSerializer.ThermoRules.BoxPair(bc.position.x, bc.position.y, selected.posx, selected.posy);
                ts.serializedObject.thermo.dependencies = newDep;
            } else {
                var newDep = new ThermoSerializer.ThermoRules.BoxPair[dep.Length-1];
                for (int i=0; i<found; i++) newDep[i] = dep[i];
                for (int i=found+1; i<dep.Length; i++) newDep[i-1] = dep[i];
                ts.serializedObject.thermo.dependencies = newDep;
            }
            // Reset view.
            VisualBoardController.instance.RemoveUnderlays();
            ts.ApplyToBoard(VisualBoardController.instance);
            VisualBoardController.instance.sudoku.SetSerializer(VariantList.VariantType.Thermo.ToString(), ts);
            VisualBoardController.instance.ResetColor(selected.posx, selected.posy, false);
            selected = bc.position;
            VisualBoardController.instance.SetHighlight(selected.posx, selected.posy, BoxSelectionManager.instance.highlightColor);
        }
    }

}
