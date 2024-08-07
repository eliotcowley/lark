using UnityEngine;
using UnityEngine.AI;

public class AnimationController : MonoBehaviour
{
    private Animator anim;
    private PlayerController player;
    private SpriteRenderer sprite;
    private NavMeshAgent agent;
    private int stateHash = Animator.StringToHash("PlayerState");
    private int dirHash = Animator.StringToHash("WalkDir");

    // Initialize animation objects
    private void Start()
    {
        anim = GetComponent<Animator>();
        player = GetComponentInParent<PlayerController>();
        sprite = GetComponent<SpriteRenderer>();
        agent = GetComponentInParent<NavMeshAgent>();
    }

    // Update animation
    private void Update()
    {
        if ((agent.destination.x - agent.transform.position.x) < 0.0f && player.WalkDirection > 0)
        {
            sprite.flipX = true;
        }
        else if ((agent.destination.x - agent.transform.position.x) > 0.0f && player.WalkDirection > 0)
        {
            sprite.flipX = false;
        }    

        anim.SetInteger(dirHash, player.WalkDirection);
        anim.SetInteger(stateHash, (int)player.CurrentState);
    }
}