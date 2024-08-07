using DigitalRubyShared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class GestureManager : MonoBehaviour
{
    [HideInInspector]
    public Layer m_currentLayer;

    [HideInInspector]
    public bool m_hasWon = false;

    [SerializeField]
    private TextMeshProUGUI m_gestureText;

    [SerializeField]
    private TextMeshProUGUI m_successesText;

    [SerializeField]
    private TextMeshProUGUI m_streakText;

    [SerializeField]
    private TextMeshProUGUI m_layerText;

    private TapGestureRecognizer m_tapGesture;
    private TapGestureRecognizer m_doubleTapGesture;
    private SwipeGestureRecognizer m_swipeGesture;
    private GestureType m_currentGesture = GestureType.Tap;
    private GestureType m_nextGesture = GestureType.Tap;
    private bool m_success = false;
    private string m_gestureListFileName = "JsonData" + Path.DirectorySeparatorChar + "gestureList.json";
    private List<Layer> m_layers;
    private int m_successesThisLayer = 0;
    private int m_currentStreak = 0;
    private int m_currentLayerIndex = 0;

    private void Start()
    {
        CreateDoubleTapGesture();
        CreateTapGesture();
        CreateSwipeGesture();

        m_layers = new List<Layer>()
        {
            new Layer(new List<GestureType>{GestureType.Tap}, new List<int>{9, 17, 25}, 2),
            new Layer(new List<GestureType>{GestureType.SwipeLeft, GestureType.SwipeRight}, new List<int>{9, 17, 25}, 2),
            new Layer(new List<GestureType>{GestureType.Tap, GestureType.SwipeLeft, GestureType.SwipeRight}, new List<int>{9, 17, 25}, 2)
        };

        m_currentLayer = m_layers[m_currentLayerIndex];
        UnityEngine.Random.InitState(DateTime.Now.Millisecond);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            //GameManager.Instance.LoadScene(ScenesEnum.TestLoadingScene);
        }

        m_streakText.SetText("Streak: " + m_currentStreak);
        m_successesText.SetText("Successes this layer: " + m_successesThisLayer);
        m_layerText.SetText("Layer: " + (m_currentLayerIndex + 1));
    }

    private void OnTap(GestureRecognizer _gesture)
    {
        if (_gesture.State == GestureRecognizerState.Ended)
        {
            if ((m_currentGesture == GestureType.Tap) && (!m_success))
            {
                GestureSucceeded();
            }
        }
    }

    private void OnDoubleTap(GestureRecognizer _gesture)
    {
        if (_gesture.State == GestureRecognizerState.Ended)
        {
            if ((m_currentGesture == GestureType.DoubleTap) && (!m_success))
            {
                GestureSucceeded();
            }
        }
    }

    private void OnSwipe(GestureRecognizer _gesture)
    {
        if (_gesture.State == GestureRecognizerState.Ended)
        {
            if (_gesture.DeltaX > 0)
            {
                if ((m_currentGesture == GestureType.SwipeRight) && (!m_success))
                {
                    GestureSucceeded();
                }
            }
            else
            {
                if ((m_currentGesture == GestureType.SwipeLeft) && (!m_success))
                {
                    GestureSucceeded();
                }
            }
        }
    }

    private void CreateTapGesture()
    {
        m_tapGesture = new TapGestureRecognizer();
        m_tapGesture.StateUpdated += OnTap;
        m_tapGesture.RequireGestureRecognizerToFail = m_doubleTapGesture;
        FingersScript.Instance.AddGesture(m_tapGesture);
    }

    private void CreateDoubleTapGesture()
    {
        m_doubleTapGesture = new TapGestureRecognizer();
        m_doubleTapGesture.NumberOfTapsRequired = 2;
        m_doubleTapGesture.StateUpdated += OnDoubleTap;
        FingersScript.Instance.AddGesture(m_doubleTapGesture);
    }

    private void CreateSwipeGesture()
    {
        m_swipeGesture = new SwipeGestureRecognizer();
        m_swipeGesture.Direction = SwipeGestureRecognizerDirection.Any;
        m_swipeGesture.StateUpdated += OnSwipe;
        m_swipeGesture.DirectionThreshold = 1.0f; // allow a swipe, regardless of slope
        FingersScript.Instance.AddGesture(m_swipeGesture);
    }

    public void ShowRandomGesture()
    {
        int gesture = UnityEngine.Random.Range(0, m_currentLayer.PossibleGestures.Count);
        m_nextGesture = m_currentLayer.PossibleGestures[gesture];

        switch (m_nextGesture)
        {
            case GestureType.DoubleTap:
                m_gestureText.SetText("Double tap!");
                break;

            case GestureType.SwipeLeft:
                m_gestureText.SetText("Swipe left!");
                break;

            case GestureType.SwipeRight:
                m_gestureText.SetText("Swipe right!");
                break;

            case GestureType.Tap:
                m_gestureText.SetText("Tap!");
                break;

            default:
                Debug.Log("Error in ShowRandomGesture()");
                break;
        }
    }

    public void SetGesture()
    {
        m_currentGesture = m_nextGesture;
        m_success = false;
    }

    /// <summary>
    /// Reset the successes this layer.
    /// </summary>
    public void NewBeatSet()
    {
        m_successesThisLayer = 0;
    }

    private void GestureSucceeded()
    {
        m_gestureText.SetText("Yay!");
        m_success = true;
        m_successesThisLayer++;

        if (m_successesThisLayer >= m_currentLayer.GestureBeats.Count)
        {
            m_currentStreak++;

            if (m_currentStreak >= m_currentLayer.SuccessesToAdvance)
            {
                m_currentLayerIndex++;
                m_currentStreak = 0;

                if (m_currentLayerIndex >= m_layers.Count)
                {
                    m_gestureText.SetText("You win!");
                    m_hasWon = true;
                }
                else
                {
                    m_currentLayer = m_layers[m_currentLayerIndex];
                }
            }
        }
    }

    //private string ConvertToJson(GestureEntry gestureEntry)
    //{
    //    return JsonUtility.ToJson(gestureEntry);
    //}

    //private void LoadGestureList()
    //{
    //    string filePath = Path.Combine(Application.dataPath, m_gestureListFileName);

    //    if (File.Exists(filePath))
    //    {
    //        string dataAsJson = File.ReadAllText(filePath);
    //        GestureEntry loadedData = JsonUtility.FromJson<GestureEntry>(dataAsJson);
    //    }
    //}
}
