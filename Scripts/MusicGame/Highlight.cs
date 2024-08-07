using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Highlight : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private float linePosX;
    bool touchActive = false;
    bool mouseActive = false;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        transform.position = new Vector3(
            GameManager.Instance.LinePosition.x, 
            transform.position.y, 
            transform.position.z);

        spriteRenderer.enabled = false;
        linePosX = GameManager.Instance.LinePosition.x;
    }

    private void Update()
    {
        Vector2 touchPosition = new Vector2();
       if (Input.touchCount > 0)
        {
            touchActive = true;
            touchPosition = Input.touches[0].position;
        }
       else if(Input.mousePresent)
        {
            mouseActive = true;
            touchPosition = Input.mousePosition;
        }
        if (touchActive || mouseActive)
        {
            Vector3 screenPos = new Vector3(touchPosition.x, touchPosition.y, 10);
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
            float hightlightPosX = 0f; 

            if (GameManager.Instance.LeftHandMode)
                hightlightPosX = linePosX - (Constants.LineXZone + .5f);
            else
                hightlightPosX = linePosX - (Constants.LineXZone - .5f);

            if (worldPos.x > (linePosX - (Constants.LineXZone))
                && worldPos.x < (linePosX + (Constants.LineXZone))
                && !GameManager.Instance.Paused)
            {
                transform.position = new Vector3((hightlightPosX), worldPos.y, worldPos.z);
                spriteRenderer.enabled = true;
            }
            else
            {
                spriteRenderer.enabled = false;
            }
        }
        else
        {
            spriteRenderer.enabled = false;
        }
    }
}
