using UnityEngine;
using UnityEngine.UI;

public class GameClock : ResourceDisplay
{
    double minutesPastStart = 0;
    public double startingTimeInMinutes = 480;  // default is 480 or 9AM
    Text clockDisplay;

    // Start is called before the first frame update
    void Start()
    {
        UpdateClockText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetStartingTime(double newStart)
    {
        startingTimeInMinutes = newStart;
    }

    private void UpdateClockText()
    {
        if (clockDisplay == null)
            clockDisplay = gameObject.GetComponentInChildren<Text>();

        double curTime = startingTimeInMinutes + minutesPastStart;
        int hours = (int)(curTime/60); 
        int minutes = (int)(curTime%60);

        clockDisplay.text = hours.ToString("00") + ":" + minutes.ToString("00");
    }

    // We get the new value as time left in the day. So if it was 10 and you start out with 100
    // Then it is 90 minutes past the start.
    public override void UpdateResource(double newValue)
    {
        minutesPastStart = Constants.Default_Time_In_Day - newValue;
        UpdateClockText();
    }
}
