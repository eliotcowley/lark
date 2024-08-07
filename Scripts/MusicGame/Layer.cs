using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Layer
{
    public List<GestureType> PossibleGestures;
    public List<int> GestureBeats;
    public int SuccessesToAdvance;

    public Layer(List<GestureType> _possibleGestures, List<int> _gestureBeats, int _successesToAdvance)
    {
        PossibleGestures = _possibleGestures;
        GestureBeats = _gestureBeats;
        SuccessesToAdvance = _successesToAdvance;
    }
}
