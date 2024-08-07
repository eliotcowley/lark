using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// This class is a work around to a bug in the UI system that was stopping
// the buttons from being able to be clicked.
public class ImpersonateEventButtons : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
     void Update()
     {
         var pointer = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
         var raycastResults = new List<RaycastResult>();
         EventSystem.current.RaycastAll(pointer, raycastResults);
 
         if (raycastResults.Count > 0)
         {
             foreach (var raycastResult in raycastResults)
             {
                 if (EventSystem.current.IsPointerOverGameObject() && raycastResult.gameObject.name.Contains("Button"))
                 {
                    var button = raycastResult.gameObject.GetComponent<Button>();
                    if (button != null && Input.GetMouseButtonDown(0))
                    {
                        button.onClick.Invoke();
                    }
                     // Debug.Log(Input.mousePosition.x < raycastResult.gameObject.transform.position.x ? raycastResult.gameObject.name + " Left" : raycastResult.gameObject.name + " Right");
                 }
             }
         }
     }
}
