using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleText : MonoBehaviour
{
    [SerializeField]
    private GameObject startButton;

    public void SetStartButtonActive()
    {
        startButton.SetActive(true);
    }
}
