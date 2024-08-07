using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeToggleButton : MonoBehaviour
{
    public bool active = false;
    public float maxAlpha = 1.0f;
    public float duration = 1;
    float counter = 0f;
    Image buttonImage;
    Text buttonText;

    void UpdateAlpha(float alpha)
    {
        var buttonColor = buttonImage.color;
        var textColor = buttonText.color;

        buttonImage.color = new Color(buttonColor.r, buttonColor.g, buttonColor.b, alpha);//Fade Traget Image
        buttonText.color = new Color(textColor.r, textColor.g, textColor.b, alpha);//Fade Text
    }

    // Start is called before the first frame update
    void Start()
    {
        Button button = GetComponent<Button>();
        buttonImage = button.GetComponent<Image>();
        buttonText = button.GetComponentInChildren<Text>();

        counter = active ? maxAlpha : 0.0f;
        UpdateAlpha(counter);
    }

    // Update is called once per frame
    void Update()
    {
        if (active && counter < duration)
        {
            counter += Time.deltaTime;
            counter = Mathf.Min(counter, duration);
        }
        else if (!active && counter > 0)
        {
            counter -= Time.deltaTime;
            counter = Mathf.Max(counter, 0);
        }

        float alpha = Mathf.Lerp(0, maxAlpha, counter / duration);
        UpdateAlpha(alpha);
    }

    public void ToggleFade(bool isActive)
    {
        active = isActive;
    }

    public void ToggleFade()
    {
        active = !active;
    }
}
