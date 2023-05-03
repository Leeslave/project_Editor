using System;

public struct AlgorithmTest
{
    int DP[][][];

    public void AlgorithmTest(int n)
    {
        int Start = 1 >> n - 1;
        DP = new int[1 >> n][1 >> n][1 >> n];
    }
}
