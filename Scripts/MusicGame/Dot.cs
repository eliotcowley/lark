using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A dot object, to be serialized into JSON.
/// </summary>
[System.Serializable]
public struct Dot
{
    public Dot(int beatNum, Position position, int index)
    {
        this.BeatNum = beatNum;
        this.Position = position;
        this.Index = index;
    }

    /// <summary>
    /// The number of beats since the last dot.
    /// </summary>
    public int BeatNum;

    /// <summary>
    /// The position on the screen that this dot appears.
    /// </summary>
    public Position Position;

    /// <summary>
    /// The index of this beat.
    /// </summary>
    public int Index;
}
