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

    public static List<VariantType> SuggestedVariants(string searchTerm) {
        searchTerm = searchTerm.ToLower();
        List<VariantType> values = new List<VariantType>();
        List<(VariantType var, float s)> scores = new List<(VariantType var, float s)>();
        foreach (VariantType v in System.Enum.GetValues(typeof(VariantType))) {
            // Check if searchTerm is subsequence of
            string check = v.ToString().ToLower();
            int stringIndex = 0;
            float totalScore = 0;
            int i=0;
            for (i=0; i<check.Length; i++) {
                if (stringIndex == searchTerm.Length) break;
                if (searchTerm[stringIndex] == check[i]) {
                    stringIndex++;
                    totalScore += i;
                }
            }
            if (stringIndex < searchTerm.Length) continue;
            // Ensure shorter strings are matched first.
            totalScore += searchTerm.Length / (float)check.Length;
            // Lower totalScore is good.
            scores.Add((v, totalScore));
        }
        scores.Sort((a, b) => a.s.CompareTo(b.s));
        foreach ((VariantType var, int s) r in scores) values.Add(r.var);
        return values;
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
