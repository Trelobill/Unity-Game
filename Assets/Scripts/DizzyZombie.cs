using UnityEngine;
using UnityEngine.AI;

public class DizzyZombie : MonoBehaviour
{
    Transform player;
    Animator animator;
    NavMeshAgent agent;
    float chaseDistance = 8.0f;
    float attackDistance = 1.1f;
    public static DizzyZombie instance;

    private void Awake()
    {
        instance = this;
        TryGetComponent<NavMeshAgent>(out agent);
        TryGetComponent<Animator>(out animator);
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    public void StartDizzy()
    {
        agent.isStopped = true;
        ResetAllAnimatorBools();
    }

    public void ExitDizzy()
    {
        animator.SetBool("isChasing", false);
        agent.isStopped = false;
        float distance = Vector3.Distance(player.position, animator.transform.position);
        if (distance <= attackDistance)
        {
            animator.SetBool("isAttacking", true);
        }
        else if (distance > attackDistance && distance <= chaseDistance)
        {
            animator.SetBool("isChasing", true);
        }
        else
        {
            animator.SetBool("isPatrolling", true);
        }
        animator.SetBool("Dizzy", false);
    }

    public void StartDizzyAnimation()
    {
        ResetAllAnimatorBools();
        animator.SetBool("Dizzy", true);
    }
    void ResetAllAnimatorBools()
    {
        AnimatorControllerParameter[] parameters = animator.parameters;

        foreach (AnimatorControllerParameter parameter in parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Bool)
            {
                animator.SetBool(parameter.name, false);
            }
        }
    }
}
