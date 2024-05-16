using UnityEngine;

public class EnableDisableHandCollider : MonoBehaviour
{
    [SerializeField] Collider handCollider;

    private void Awake() {
        EnableHandCollider(0);
    }

    //για χρηση animation event, ενεργοποιει-απενεργοποιει το collider του χεριου
    public void EnableHandCollider(int isEnable)
    {
        if (handCollider != null)
        {
                if (isEnable == 1)
                {
                    handCollider.enabled = true;
                }
                else
                {
                    handCollider.enabled = false;
                }
        }
    }
}
