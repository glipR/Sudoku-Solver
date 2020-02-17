using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Barracuda;
using System.IO;

public class DigitPredictor : MonoBehaviour {

    public RenderTexture digitField = null;
    private Texture2D texture;
    public NNModel modelSource;
    private Model loadedModel;
    private IWorker worker;

    private void Awake() {
        texture = new Texture2D(256, 256);
    }

    public float[] GetPoints(int mipLevel) {
        // Returns a grayscale normalized image for use in the detection.
        Rect rectReadTexture = new Rect(0, 0, 256, 256);
        RenderTexture.active = digitField;
        texture.ReadPixels(rectReadTexture, 0, 0);
        texture.Apply();

        RenderTexture.active = null;
        Color[] colors = texture.GetPixels(mipLevel);
        float[] brightness = new float[colors.Length];
        for (int i=0; i<brightness.Length; i++) brightness[i] = 1 - colors[i].grayscale;
        return brightness;
    }

    public float[] ReSize(float[] previousPoints, int prevW, int prevH, int newW, int newH) {
        // This should be done after correct mip levels.
        // Assuming newW < prevW, newH < prevH, we need to cull outside.
        float[] newPoints = new float[newW * newH];
        int startW = (prevW - newW) / 2;
        int startH = (prevH - newH) / 2;
        for (int i=0; i<newW; i++) for (int j=0; j<newH; j++) {
            newPoints[i*newH + j] = previousPoints[(i+startW)*prevH + (j+startH)];
        }
        return newPoints;
    }

    public float[] RePosition(float[] points) {
        // Due to how textures are indexed, we need to rotate the image and flip.
        float[] newPoints = new float[28*28];
        for (int i=0; i<28; i++) {
            for (int j=0; j<28; j++) {
                newPoints[i*28+j] = points[(27-i)*28+j];
            }
        }
        return newPoints;
    }

    public void Predict() {
        loadedModel = ModelLoader.Load(modelSource);
        worker = BarracudaWorkerFactory.CreateWorker(BarracudaWorkerFactory.Type.ComputePrecompiled, loadedModel);
        float[] points = GetPoints(3);
        float[] newPoints = RePosition(ReSize(points, 32, 32, 28, 28));
        var t = new Tensor(1, 28 * 28, newPoints);
        worker.ExecuteAndWaitForCompletion(t);
        var result = worker.PeekOutput();
        var floats = result.data.Download(10);
        float maxVal = 0;
        int maxIndex = -1;
        for (int i=0; i<floats.Length; i++) {
            if (floats[i] > maxVal) {
                maxVal = floats[i];
                maxIndex = i;
            }
        }
        Debug.Log(maxVal + " occurs at " + maxIndex);
        DrawHandler.instance.Clear();
        List<string> inputNames = loadedModel.inputs.ConvertAll(new System.Converter<Model.Input, string>(StringMap));
        List<string> outputNames = loadedModel.outputs;
        Debug.Log(inputNames.Count + " " + inputNames[0]);
        Debug.Log(outputNames.Count + " " + outputNames[0]);
        result.Dispose();
        worker.Dispose();
    }

    private static string StringMap(Model.Input inp) {
        string s = "";
        for (int i=0; i<s.Length; i++) {
            s = s + s[i] + " ";
        }
        return s;
    }

}
