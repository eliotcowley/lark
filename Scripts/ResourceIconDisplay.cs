using UnityEngine;
using UnityEngine.UI;

public class ResourceIconDisplay : ResourceDisplay
{
    double curValue = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void UpdateResource(double newValue)
    {
        gameObject.GetComponentInChildren<Text>().text = newValue.ToString();

        if (newValue != curValue)
        {
            gameObject.GetComponentInChildren<UIParticleSystem>().Play();
        }
        curValue = newValue;
    }
}
