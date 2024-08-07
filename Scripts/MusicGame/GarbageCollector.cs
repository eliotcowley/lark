using UnityEngine;

/// <summary>
/// The script controlling the garbage collector, which cleans up game objects.
/// </summary>
public class GarbageCollector : MonoBehaviour
{
    /// <summary>
    /// The position of the line when playing in right-hand mode.
    /// </summary>
    [SerializeField]
    private float rightHandPositionX;

    private void Start()
    {
        if (!GameManager.Instance.LeftHandMode)
        {
            transform.position = new Vector3(rightHandPositionX, transform.position.y, transform.position.z);
        }
    }
}
