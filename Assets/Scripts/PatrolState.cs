using UnityEngine;
using UnityEngine.AI;

public class PatrolState : StateMachineBehaviour
{
    float wanderRadius = 100.0f;
    float timer;
    NavMeshAgent agent;
    Transform player;
    float chaseRange = 15.0f;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("HearSounds", false);
        agent = animator.GetComponent<NavMeshAgent>();
        agent.speed = 0.5f;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        timer = 0;
        //βαζω να παει σε ενα τυχαιο σημειο
        GetNewRandomWaypoint(animator);
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (agent.remainingDistance < 1f)
        {
            //βρισκει τυχαιο σημειο στο περιβαλλον
            GetNewRandomWaypoint(animator);
        }
        timer += Time.deltaTime;
        //μετα απο 20 δευτερολεπτα μπαινει παλι σε idle state
        if (timer > 20)
        {
            animator.SetBool("isPatrolling", false);
        }
        float distance = Vector3.Distance(player.position, animator.transform.position);
        //αν ειναι σχετικα κοντα και μπορει να παει τοτε κυνηγαει τον παιχτη
        if (distance <= chaseRange)
        {
            animator.SetBool("isPatrolling", false);
            animator.SetBool("isChasing", true);
        }
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Dizzy Idle"))
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
        else
        {
            agent.SetDestination(agent.transform.position);
        }
    }
    void GetNewRandomWaypoint(Animator animator)
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += animator.transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }
}
