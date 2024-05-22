using System.Collections.Generic;
using UnityEngine;

public static class MazeUtility
{
    public static KeyValuePair<byte, byte> D3PositonMatrix(Vector3 pos)
    {
        byte mazeX = (byte)Mathf.RoundToInt(4.5f - pos.z);
        byte mazeZ = (byte)Mathf.RoundToInt(pos.x + 4.5f);

        return new KeyValuePair<byte, byte>(mazeX, mazeZ);
    }

    public static Vector3 MatrixTo3DPosition(byte row, byte column)
    {
        float posX = -4.5f + column;
        float posZ = 4.5f - row;

        return new Vector3(posX, 0.5f, posZ);
    }
}
