using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomePlayerMovement : MonoBehaviour
{
    public float speed;
    public float horizontalModifier;
    public float verticalModifier;

    private Rigidbody rb;

    void Start ()
    {
        rb = GetComponent<Rigidbody>();
        Vector3? previousLocationInHome = GameManager.Instance.GetPlayerLocationTransform();

        if (previousLocationInHome != null)
        {
            Debug.Log("Using previous home location " + previousLocationInHome.ToString());
            this.transform.position = (Vector3)previousLocationInHome;
        } else
        {
            Debug.Log("Using default location.");
        }
    }

    void FixedUpdate ()
    {
        float moveHorizontal = horizontalModifier*Input.GetAxis ("Horizontal");
        float moveVertical = verticalModifier*Input.GetAxis ("Vertical");
        
        Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);

        rb.AddForce (movement * speed);
    }
}
