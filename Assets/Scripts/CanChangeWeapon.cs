using UnityEngine;

public class CanChangeWeapon : StateMachineBehaviour
{
    //χρησιμοποιειται στο animator ωστε να καθοριζει σε καθε state αν μπορω να αλλαξω το οπλο ή οχι
    [SerializeField] bool state;
    //τρέχει μια φορά όταν μπαίνει στο animation που έχω προσθέσει το script
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //στο weaponManager script που είναι η λογική για τα όπλα
        // ενημερώνει αν μπορεί να αλλάξει όπλο την δεδομένη στιγμή
        WeaponManager.current.CanChangeWeapon(state);
    }
}
