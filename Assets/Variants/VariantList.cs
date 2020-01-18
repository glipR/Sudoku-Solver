using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Variant {
    public ISerializer serializer;
    public ISolverSettings solver;
    public ISudokuSettings settings;
}

public static class VariantList {

    public enum VariantType {
        Base
    }

    public static Variant GetVariant(string v) {
        if (v == VariantType.Base.ToString())
            return BaseVariant();
        Debug.Log("Variant Type " + v + ", not handled, returning the base.");
        return BaseVariant();
    }

    public static Variant BaseVariant() {
        Variant v = new Variant();
        v.solver = new BaseSolver();
        v.settings = new BaseSettings();
        return v;
    }

}
