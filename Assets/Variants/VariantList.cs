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
        Base,
        Sandwich,
        Thermo
    }

    public static Variant GetVariant(string v) {
        if (v == VariantType.Base.ToString())
            return BaseVariant();
        if (v == VariantType.Sandwich.ToString())
            return SandwichVariant();
        if (v == VariantType.Thermo.ToString())
            return ThermoVariant();
        Debug.Log("Variant Type " + v + ", not handled, returning the base.");
        return BaseVariant();
    }

    public static Variant BaseVariant() {
        Variant v = new Variant();
        v.solver = new BaseSolver();
        v.settings = new BaseSettings();
        v.serializer = new BaseSerializer();
        return v;
    }

    public static Variant SandwichVariant() {
        Variant v = new Variant();
        v.solver = new SandwichSolver();
        v.settings = new SandwichSettings();
        v.serializer = new SandwichSerializer();
        return v;
    }

    public static Variant ThermoVariant() {
        Variant v = new Variant();
        v.solver = new ThermoSolver();
        v.settings = new ThermoSettings();
        v.serializer = new ThermoSerializer();
        return v;
    }

}
