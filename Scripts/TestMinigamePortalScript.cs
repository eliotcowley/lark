using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMinigamePortalScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Welcome to the minigame. To exist, please rate the player's score and press A, B, or C.");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            ResourceManager.Instance.ApplyMinigameResource(1);
            GameManager.Instance.LoadScene(ScenesEnum.EndScene);
        } else if (Input.GetKeyDown(KeyCode.B))
        {
            ResourceManager.Instance.ApplyMinigameResource(.85);
            GameManager.Instance.LoadScene(ScenesEnum.EndScene);
        } else if (Input.GetKeyDown(KeyCode.C))
        {
            ResourceManager.Instance.ApplyMinigameResource(.75);
            GameManager.Instance.LoadScene(ScenesEnum.EndScene);
        }
    }
}
