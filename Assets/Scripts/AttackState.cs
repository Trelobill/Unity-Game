using UnityEngine;


//συμπεριφορα επιθεσης των εχθρων
public class AttackState : StateMachineBehaviour
{
    Transform player;
    float attackDistance = 1.1f;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //δεν αλλαζω το y ωστε να γυρναει μονο αριστερα-δεξια
        Vector3 playerPosition = new(player.transform.position.x, animator.transform.position.y, player.transform.position.z);
        //κοιταει προς τον παιχτη οταν επιτιθεται
        animator.transform.LookAt(playerPosition);
        float distance = Vector3.Distance(player.position, animator.transform.position);
        //αν φυγει πιο μακρια σταματαει να επιτιθεται και τον κυνηγαει
        if (distance >= attackDistance)
        {
            animator.SetBool("isChasing", true);
            animator.SetBool("isAttacking", false);
            animator.SetBool("isAttacking1", false);
        }
        //αν ειναι ακομα κοντα ισως αλλαξει attack animation
        else
        {
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

    override public void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        //αν γινει dizzy κανω ολα τα bool false ωστε οταν "ξυπνησει" αναλογα το ποσο μακρια ειναι ο παιχτης να δρασει αναλογα
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
            animator.SetBool("isChasing", true);
        }
    }

}
