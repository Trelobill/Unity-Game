using UnityEngine;

public class UIBillBoarding : MonoBehaviour
{
    //Το script αυτο υπαρχει μονο για να κοιταει ο καμβας μας παντα προς την καμερα
    Camera cam;
    private void Awake()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        transform.forward = cam.transform.forward;
    }
}
