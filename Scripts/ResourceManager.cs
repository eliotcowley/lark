using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// The resource manager handles the player's resources in the game.
/// </summary>
public class ResourceManager : Singleton<ResourceManager>
{
    bool initialized = false;
    public double mindResource = 0;
    public double bodyResource = 0;
    public double spiritResource = 0;
    public int DaysLeft = Constants.Default_Days;
    public double timeLeftInDay = Constants.Default_Time_In_Day;
    private int transactions = 0;
    ResourceDisplay mindPtText = null;
    ResourceDisplay bodyPtText = null;
    ResourceDisplay soulPtText = null;
    GameClock displayClock = null;

    public MiniGameResource CurrentMinigameResource;

    public void InitializeResourceManager()
    {
        if (!initialized)
            InitializeResource(0,0,0);
    }

    void InitializeResource(double mindStart, double bodyStart, double spiritStart)
    {
        InitializeResource(mindStart, bodyStart, spiritStart, Constants.Default_Days);
    }
    
    void InitializeResource(double mindStart, double bodyStart, double spiritStart, int days)
    {
        initialized = true;

        mindResource = mindStart;
        bodyResource = bodyStart;
        spiritResource = spiritStart;
        DaysLeft = days;
        timeLeftInDay = Constants.Default_Time_In_Day;
        transactions = 0;
    }

    void ResourceTransaction(MiniGameResource resource, double multiplier = 1)
    {
        mindResource += resource.MindValue*multiplier;
        bodyResource += resource.BodyValue*multiplier;
        spiritResource += resource.SpiritValue*multiplier;
        timeLeftInDay -= resource.TimeCost;
        ++transactions;
    }

    public void PassTime(int minutes)
    {
        timeLeftInDay -= minutes;
        UpdateResourceDisplays();
    }

    public int NextDay()
    {
        timeLeftInDay = Constants.Default_Time_In_Day;
        return --DaysLeft;
    }

    public void SetMinigameResource(MiniGameResource resource)
    {
        CurrentMinigameResource = resource;
    }

    public void ApplyMinigameResource(double multiplier)
    {
        ResourceTransaction(CurrentMinigameResource, multiplier);
        UpdateResourceDisplays();
    }

    private T GetComponentFromObjectCollection<T>(T orig, GameObject[] objectCollection, string name)
    {
        if (orig == null)
        {
            GameObject obj = Array.Find(objectCollection, s => string.Compare(s.name, name) == 0);
            if (obj != null)
            {
                return obj.GetComponent<T>();
            } else
            {
                Debug.LogError("Error: " + name + " not found in scene.");
            }
        }

        return orig;
    }

    private void TryUpdateResourceDisplay(ResourceDisplay display, double updateValue)
    {
        if (display != null)
        {
            display.UpdateResource(updateValue);
        } else
        {
            Debug.LogError("Display not found.");
        }
    }

    public void UpdateResourceDisplays()
    {
        var resourceCountObjects = GameObject.FindGameObjectsWithTag(Constants.Tag_ResourceCount);

        if (resourceCountObjects == null || resourceCountObjects.Length == 0)
        {
            Debug.LogError("No Points counters on screen");
            return;
        }

        displayClock = GetComponentFromObjectCollection(displayClock, resourceCountObjects, Constants.Name_ClockDisplay);
        mindPtText = GetComponentFromObjectCollection(mindPtText, resourceCountObjects, Constants.Name_MindDisplay);
        bodyPtText = GetComponentFromObjectCollection(bodyPtText, resourceCountObjects, Constants.Name_BodyDisplay);
        soulPtText = GetComponentFromObjectCollection(soulPtText, resourceCountObjects, Constants.Name_SoulDisplay);

        TryUpdateResourceDisplay(displayClock, timeLeftInDay);
        TryUpdateResourceDisplay(mindPtText, mindResource);
        TryUpdateResourceDisplay(bodyPtText, bodyResource);
        TryUpdateResourceDisplay(soulPtText, spiritResource);
    }

    private void OnSceneLoaded(Scene _scene, LoadSceneMode _loadSceneMode)
    {
        mindPtText = bodyPtText = soulPtText = null;
        displayClock = null;
        UpdateResourceDisplays();
    }

    // Start is called before the first frame update
    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        UpdateResourceDisplays();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
