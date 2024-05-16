using UnityEngine;

//κλαση που εχουν ολα οσα μπορω να πιασω ωστε να τα ξεχωριζω
public class ItemGrabbable : MonoBehaviour
{
    Outline outlineComponent;

    private void Awake()
    {
        outlineComponent = GetComponent<Outline>();
        outlineComponent.enabled = false;
    }
    public void OverlayShow()
    {
        outlineComponent.enabled = true;
    }
    public void OverlayHide()
    {
        outlineComponent.enabled = false;
    }
}
