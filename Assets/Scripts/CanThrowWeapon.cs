using UnityEngine;

public class CanThrowWeapon : StateMachineBehaviour
{
    //χρησιμοποιειται στο animator ωστε να καθοριζει σε καθε state αν μπορω να πεταξω το οπλο ή οχι
    [SerializeField] bool state;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        WeaponManager.current.CanThrowWeapon(state);
    }
}
