using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// The JSON tool to let you create a JSON file in-game.
/// </summary>
public class JsonTool : MonoBehaviour
{
    private DotList dotList;
    private int lastBeat = 0;
    private string filename = "out.json";
    private string filePath;

    private void Start()
    {
        dotList = new DotList
        {
            dots = new List<Dot>()
        };

        filePath = Path.Combine(Application.dataPath, "Resources", filename);
    }

    private void Update()
    {
        // Whenever you press F, get the current beat
        if (Input.GetButtonDown(Constants.Input_RecordBeat))
        {
            int beatDiff = DotManager.Instance.SubBeatNum - lastBeat;

            if ((dotList.dots.Count == 0) || (beatDiff > 0))
            {
                Dot dot = new Dot(beatDiff, Position.Middle, dotList.dots.Count);
                dotList.dots.Add(dot);
                lastBeat = DotManager.Instance.SubBeatNum;
                Debug.Log($"Beat recorded: {lastBeat}");
            }
        }

        if (Input.GetButtonDown(Constants.Input_WriteJson))
        {
            string json = JsonUtility.ToJson(dotList);
            File.WriteAllText(filePath, json);
            string relativePath = Path.Combine("Resources", filename);
            Debug.Log($"Json written: {relativePath}");
        }
    }
}
