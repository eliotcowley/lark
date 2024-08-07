/**
 * CameraController.cs
 * 
 * This class will control all basic movements for the camera.  This
 * must be attached to the Player object in order to work.  For any items
 * that you want the camera to focus in on, you will need to set that item
 * with the tag of 'FocusItem'.
 **/
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector3 cameraOffset;
    public Camera camera;

    // FOV Vars
    private float standardFov;
    private float focusFov;
    private readonly float focusDiff = 10.0f;
    private bool isTouchingFocusItem = false;

    private float cameraEdge = 16.25f;

    // Initial config
    void Start()
    {
        this.standardFov = this.camera.fieldOfView;
        focusFov = standardFov - focusDiff;
        cameraOffset = camera.transform.position - transform.position;

        if (Camera.main.aspect >= 2.11f) // 19:9
        {
            Debug.Log("19:9");
            cameraEdge = 16.25f;
        }
        else if (Camera.main.aspect >= 2f) // 18:9
        {
            Debug.Log("18:9");
            cameraEdge = 23f;
        }
        else if (Camera.main.aspect >= 1.77f) // 16:9
        {
            Debug.Log("16:9");
            cameraEdge = 36f;
        }
        else if (Camera.main.aspect >= 1.6f) // 16:10
        {
            Debug.Log("16:10");
            cameraEdge = 47f;
        }
    }

    void Update()
    {
        // Update Camera Pos
        if (transform.position.x < -cameraEdge)
        {
            camera.transform.position = new Vector3(-cameraEdge, camera.transform.position.y, camera.transform.position.z);
        }
        else if (transform.position.x > cameraEdge)
        {
            camera.transform.position = new Vector3(cameraEdge, camera.transform.position.y, camera.transform.position.z);
        }
        else
        {
            camera.transform.position = new Vector3(transform.position.x, camera.transform.position.y, camera.transform.position.z);
        }

        // Move towards FOV points
        if (isTouchingFocusItem)
        {
            ChangeFieldOfView(focusFov, 2.0f);
        }
        else
        {
            ChangeFieldOfView(standardFov, 2.0f);
        }
        
    }

    // Use Lerp to slowly change values
    public void ChangeFieldOfView(float fov, float speed)
    {
        Camera.main.fieldOfView = Mathf.Lerp(this.camera.fieldOfView, fov, speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FocusItem"))
        {
            this.isTouchingFocusItem = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("FocusItem"))
        {
            this.isTouchingFocusItem = false;
        }
    }
}
