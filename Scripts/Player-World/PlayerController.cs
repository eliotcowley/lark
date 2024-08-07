using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public enum PlayerActionState
    {
        Idle = 0,
        Walking = 1,
        Defeat = 2
    }

    public PlayerActionState CurrentState { get; private set; }
    public int WalkDirection { get; private set; }
    public bool IsWalkingActive;

    NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        CurrentState = PlayerActionState.Idle;
    }

    // Current configured to move where the mouse clicks
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && IsWalkingActive)
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 300))
            {
                MovePlayer(hit.point);
                Debug.Log("HIT" + agent.destination.ToString());
            }
        }
        if (WithinRange())
        {
            WalkDirection = 0;
            CurrentState = PlayerActionState.Idle;
        }
    }

    public void MovePlayer(Vector3 hit)
    {
        CurrentState = PlayerActionState.Walking;
        agent.destination = hit;
        SetWalkDirection(agent.destination - agent.transform.position);
    }

    void SetWalkDirection(Vector3 diff)
    {
        // Set to neutral value to start
        WalkDirection = 0;

        // If Horizontal is biggest, go left(2) or right(3)
        if (Mathf.Abs(diff.x) >= Mathf.Abs(diff.z))
        {
            if(diff.x > 0)
            {
                WalkDirection = 2;
            }
            else
            {
                WalkDirection = 3;
            }
        }
        // Go Up(1) or Down(4)
        else
        {
            if (diff.z > 0)
            {
                WalkDirection = 4;
            }
            else
            {
                WalkDirection = 1;
            }
        }
    }

    bool WithinRange()
    {
        float range = Constants.Default_Range_From_Object;
        
        return (Mathf.Abs(agent.destination.x - agent.transform.position.x) < range && Mathf.Abs(agent.destination.z - agent.transform.position.z) < range);
    }
}
