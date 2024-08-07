using TMPro;
using UnityEngine;

/// <summary>
/// Keeps track of a dot's movement and position.
/// </summary>
public class MoveDots : MonoBehaviour
{
    /// <summary>
    /// The Y-position of the dot (which track it's on).
    /// </summary>
    public Position Position = Position.High;

    /// <summary>
    /// The index of this dot in the list.
    /// </summary>
    [HideInInspector]
    public int DotIndex = 0;

    /// <summary>
    /// The position where dots in the highest track spawn.
    /// </summary>
    [SerializeField]
    private Vector2 highPosition;

    /// <summary>
    /// The position where dots in the second-highest track spawn.
    /// </summary>
    [SerializeField]
    private Vector2 upperPosition;

    /// <summary>
    /// The position where dots in the middle track spawn.
    /// </summary>
    [SerializeField]
    private Vector2 middlePosition;

    /// <summary>
    /// The position where dots in the second-lowest track spawn.
    /// </summary>
    [SerializeField]
    private Vector2 lowerPosition;

    /// <summary>
    /// The position where dots in the lowest track spawn.
    /// </summary>
    [SerializeField]
    private Vector2 lowPosition;

    /// <summary>
    /// The starting opacity for dots (when not over the line).
    /// </summary>
    [SerializeField]
    private float startingAlpha = 0.7f;

    [SerializeField]
    private float rotationSpeed = 100f;

    [SerializeField]
    private ParticleSystem comboFX;

    private float speed;
    private TextMeshProUGUI indexText;
    private float lineXPosition;
    private bool hasFailed = false;
    private SpriteRenderer spriteRenderer;
    private Color color;
    private bool hasBeenOverLine = false;
    private DotManager dotManager;

    private void Awake()
    {
        if (!GameManager.Instance.LeftHandMode)
        {
            highPosition.x *= -1f;
            upperPosition.x *= -1f;
            middlePosition.x *= -1f;
            lowerPosition.x *= -1f;
            lowPosition.x *= -1f;
        }

        ResetDot();
        indexText = GetComponentInChildren<TextMeshProUGUI>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        color = spriteRenderer.color;
        dotManager = GameObject.FindGameObjectWithTag(Constants.Tag_DotManager).GetComponent<DotManager>();
    }

    private void Start()
    {
        lineXPosition = GameManager.Instance.LinePosition.x;
    }

    private void Update()
    {
        // Move across the screen
        float newX = GameManager.Instance.LeftHandMode ? 
            transform.position.x - (speed * Time.deltaTime) : 
            transform.position.x + (speed * Time.deltaTime);

        transform.position = new Vector2(newX, transform.position.y);

        // Rotate slowly
        //Vector3 v = new Vector3();
        //v.z = transform.localRotation.z + 1;
        //Quaternion q = Quaternion.Euler(v);
        //transform.localRotation = q;

        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime, Space.World);

        if (((GameManager.Instance.LeftHandMode) && (transform.position.x < lineXPosition - Constants.LineXZone)) 
            || ((!GameManager.Instance.LeftHandMode) && (transform.position.x > lineXPosition + Constants.LineXZone)))
        {
            if (!hasFailed)
            {
                dotManager.SetFailing(true);
                dotManager.SetCombo(0);
                hasFailed = true;
                SetAlpha(startingAlpha);
            }
        }
        else if (!hasBeenOverLine)
        {
            if ((transform.position.x > lineXPosition - Constants.LineXZone) && (transform.position.x < lineXPosition + Constants.LineXZone))
            {
                hasBeenOverLine = true;
                SetAlpha(1f);
            }
        }


        //check combo to add glow effect
        if(dotManager.Combo >= 15 && comboFX.gameObject.activeSelf == false)
        {
            AddComboFX();
        }
        else if(dotManager.Combo < 15 && comboFX.gameObject.activeSelf == true)
        {
            RemoveComboFX();
        }
        
    }

    private void AddComboFX()
    {
        comboFX.gameObject.SetActive(true);
    }
    private void RemoveComboFX()
    {
        comboFX.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(Constants.Tag_GarbageCollector))
        {
            if (DotIndex >= dotManager.DotsToUse.Count)
            {
                dotManager.ShowScoreText();
            }

            RemoveDot();
        }
    }

    private void OnEnable()
    {
        hasBeenOverLine = false;
        DotIndex = dotManager.DotIndex;
        speed = dotManager.DotSpeed;
        hasFailed = false;

        if (dotManager.ShowDotIndex)
        {
            indexText.SetText((DotIndex - 1).ToString());
        }

        SetAlpha(startingAlpha);

        // Set random rotation
        Vector3 v = new Vector3();
        v.z = Random.Range(0f, 360f);
        Quaternion q = Quaternion.Euler(v);
        transform.rotation = q;
    }

    /// <summary>
    /// Move the dot to its starting position.
    /// </summary>
    public void ResetDot()
    {
        switch (Position)
        {
            case Position.High:
                transform.position = highPosition;
                break;

            case Position.Upper:
                transform.position = upperPosition;
                break;

            case Position.Middle:
                transform.position = middlePosition;
                break;

            case Position.Lower:
                transform.position = lowerPosition;
                break;

            case Position.Low:
                transform.position = lowPosition;
                break;
        }
    }

    /// <summary>
    /// Remove the dot from the screen and add it to the pool.
    /// </summary>
    public void RemoveDot()
    {
        ObjectPool.Instance.AddToPool(gameObject);
        dotManager.CurrentDots.Remove(this);
    }

    private void SetAlpha(float a)
    {
        spriteRenderer.color = new Color(color.r, color.g, color.b, a);
    }
}
