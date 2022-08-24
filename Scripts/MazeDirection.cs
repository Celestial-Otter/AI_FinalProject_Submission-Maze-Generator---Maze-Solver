//Hunter Chu and Edward Cao
//100701653 and 100697845
//March 28th, 2022

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MazeDirection //enums to keep track of what is facing which direction
{
    NORTH,
    EAST,
    SOUTH,
    WEST
}

public static class MazeDirections
{
    public const int Count = 4;
    private static IntVector2[] vectors =
{
    new IntVector2(0, 1),
    new IntVector2(1, 0),
    new IntVector2(0, -1),
    new IntVector2(-1, 0)
};

    private static MazeDirection[] opposites =
    {
        MazeDirection.SOUTH,
        MazeDirection.WEST,
        MazeDirection.NORTH,
        MazeDirection.EAST
    };

    private static Quaternion[] rotations =
    {
        Quaternion.identity,
        Quaternion.Euler(0f, 90f, 0f),
        Quaternion.Euler(0f, 180, 0f),
        Quaternion.Euler(0f, 270f, 0f)
    };

    public static Quaternion ToRotation (this MazeDirection direction) //converts direction into rotation
    {
        return rotations[(int)direction];
    }

    public static MazeDirection GetOpposite (this MazeDirection direction) //gets the opposite direction
    {
        return opposites[(int)direction];
    }
    public static MazeDirection RandomValue
    {
        get
        {
            return (MazeDirection)Random.Range(0, Count);
        }
    }


    public static IntVector2 ToIntVector2(this MazeDirection direction) //convert direction into an IntVector2
    {
        return vectors[(int)direction];
    }
}

