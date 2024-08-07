using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpeningMessage : MonoBehaviour
{
    public GameObject BlockingPlane;
    public GameObject OpeningMsg;
    public GameObject[] ObjectsToToggle;

    // Start is called before the first frame update
    void Start()
    {
        var text = OpeningMsg.GetComponentInChildren<Text>();

        // This is only necessary for debugging from existing games.  This IF will not run in full game
        if(GameManager.Instance.PreferredGameStrings == null)
        {
            GameManager.Instance.PreferredGameStrings = GameStringLoader.LoadStrings(GameLanguage.English);
        }
        text.text = GameManager.Instance.PreferredGameStrings.portalOpen[GameManager.Instance.ActivePortal];

        StartCoroutine(DismissMessage());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator DismissMessage()
    {
        yield return new WaitForSeconds(3);

        foreach (GameObject go in ObjectsToToggle)
        {
            go.SetActive(true);
        }

        OpeningMsg.SetActive(false);
        BlockingPlane.SetActive(false);
    }
}
