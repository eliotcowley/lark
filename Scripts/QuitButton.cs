using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitButton : MonoBehaviour
{
    public Button m_YourFirstButton;
    // Start is called before the first frame update
    void Start()
    {
        m_YourFirstButton.onClick.AddListener(TaskOnClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onClick(){
        // Save game data
        // TODO
        // Close game
        Debug.Log ("Application Closing");
        //Application.Quit ();    // TODO uncomment this
    }

    void TaskOnClick()
    {
        //Output this to console when Button1 or Button3 is clicked
        Debug.Log("You have clicked the button!");
    }
}
