using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GraphConversionUtils
{
    public static Tuple<int, int, float[]> LinearizeFloatMatrix(float[][] mat)
    {
        int n = mat[0].Length;
        int m = mat[1].Length;
        float[] linearizedMat = new float[n * m];

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                linearizedMat[i*j+j] = mat[i][j];
            }
        }

        return new Tuple<int, int, float[]>(mat[0].Length, mat[1].Length, linearizedMat);
    }

    public static string StrfArr(float[] a)
    {
        return string.Join(",", a);
    }

    public static void SaveGraph()
    {
        throw new NotImplementedException();
    }
}
