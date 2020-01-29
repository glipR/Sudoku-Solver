using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigitPredictor : MonoBehaviour {

    public RenderTexture digitField = null;
    private Texture2D texture;

    public float[] GetPoints(int mipLevel) {
        // Returns a grayscale normalized image for use in the detection.
        Rect rectReadTexture = new Rect(0, 0, 256, 256);
        RenderTexture.active = digitField;
        texture.ReadPixels(rectReadTexture, 0, 0);
        texture.Apply();

        RenderTexture.active = null;
        Color[] colors = texture.GetPixels(mipLevel);
        float[] brightness = new float[colors.Length];
        for (int i=0; i<brightness.Length; i++) brightness[i] = colors[i].grayscale / 255f;
        return brightness;
    }

    public float[] ReSize(float[] previousPoints, int prevW, int prevH, int newW, int newH) {
        // This should be done after correct mip levels.
        // Assuming newW < prevW, newH < prevH, we need to cull outside.
        float[] newPoints = new float[newW * newH];
        int startW = (prevW - newW) / 2;
        int startH = (prevH - newH) / 2;
        for (int i=0; i<newW; i++) for (int j=0; j<newH; j++) {
            newPoints[i + j*newW] = previousPoints[(i+startW) + (j+startH)*prevW];
        }
        return newPoints;
    }

}
