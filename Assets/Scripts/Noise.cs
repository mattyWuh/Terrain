using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noise {
    public static float GaussianNoise(float mu, float sigma)
    {
        float uniform1 = Random.Range(0f,1f);
        float uniform2 = Random.Range(0f,1f);

        float gaussianRandom = (
            sigma * Mathf.Sqrt(-2*Mathf.Log(uniform1)) * Mathf.Cos(2*Mathf.PI*uniform2)
        );

        return gaussianRandom + mu;
    }

}
