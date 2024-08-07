using UnityEngine;

/// <summary>
/// The script controlling the line.
/// </summary>
public class LineScript : MonoBehaviour
{
    /// <summary>
    /// The X-position of the line when playing in right-hand mode.
    /// </summary>
    [SerializeField]
    private float rightHandX = 7f;

    /// <summary>
    /// The yes/no dialog game object.
    /// </summary>
    [SerializeField]
    private GameObject yesNoDialog;

    private void Awake()
    {
        if (!GameManager.Instance.LeftHandMode)
        {
            transform.position = new Vector3(rightHandX, transform.position.y, transform.position.z);

            transform.localScale = new Vector3(
                -transform.localScale.x, 
                transform.localScale.y, 
                transform.localScale.z);
        }

        GameManager.Instance.LinePosition = transform.position;
    }

    /// <summary>
    /// Switch the handedness mode.
    /// </summary>
    public void SwitchHandedness()
    {
        yesNoDialog.SetActive(false);
        GameManager.Instance.LeftHandMode = !GameManager.Instance.LeftHandMode;
        DotManager.Instance.TogglePause();
        GameManager.Instance.RestartLevel();
    }

    /// <summary>
    /// Confirm whether to switch the handedness mode.
    /// </summary>
    public void ConfirmSwitchHandedness()
    {
        DotManager.Instance.HidePauseMenu();
        yesNoDialog.SetActive(true);
    }

    /// <summary>
    /// Cancel switching the handedness mode.
    /// </summary>
    public void CancelSwitchHandedness()
    {
        yesNoDialog.SetActive(false);
        DotManager.Instance.TogglePause();
    }
}
