using UnityEngine;

public class IdleState : StateMachineBehaviour
{
    float timer;
    float chaseRange = 8;
    Transform player;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer += Time.deltaTime;
        //μετα απο 3 δευτερολεπτα μπαινει σε patrol state
        if (timer > 3)
        {
            animator.SetBool("isPatrolling", true);
        }
        float distance = Vector3.Distance(player.position, animator.transform.position);
        //αν ειναι σχετικα κοντα και μπορει να παει τοτε κυνηγαει τον παιχτη
        if (distance <= chaseRange )
        {
            animator.SetBool("isPatrolling", false);
            animator.SetBool("isChasing", true);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
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
    }
}
