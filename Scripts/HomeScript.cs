using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeScript : MonoBehaviour
{
    float timer = 0;
    public float secsPerInGameMin = 15;

    // Start is called before the first frame update
    void Start()
    {
        ResourceManager.Instance.InitializeResourceManager();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > secsPerInGameMin)
        {
            ResourceManager.Instance.PassTime(1);
            timer = timer%secsPerInGameMin;
        }
    }
}
