using UnityEngine;
using UnityEngine.AI;

//συμπεριφορα chase των εχθρων
public class ChaseState : StateMachineBehaviour
{
    NavMeshAgent agent;
    Transform player;
    float speed = 5f;
    float attackDistance = 1.0f;


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent = animator.GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent.speed = speed;
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //τρεχει προς τον παιχτη
        agent.SetDestination(player.position);
        float distance = Vector3.Distance(player.position, animator.transform.position);
        //αν ειναι κοντα στον παιχτη επιτιθεται
        if (distance <= attackDistance)
        {
            animator.SetBool("isChasing", false);

            int randomChoice = Random.Range(0, 2);
            if (randomChoice == 0)
            {
                animator.SetBool("isAttacking", true);
                animator.SetBool("isAttacking1", false);
            }
            else
            {
                animator.SetBool("isAttacking", false);
                animator.SetBool("isAttacking1", true);
            }
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
            agent.SetDestination(animator.transform.position);
        }
    }
}
