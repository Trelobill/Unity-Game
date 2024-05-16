using UnityEngine;

public class Water : MonoBehaviour
{
     [SerializeField] GameObject WaterEffect;


    //αν η καμερα μπει μεσα στο νερο ενεργοποιει το postProcessing effect που εχω δημιουργησει για το νερο
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            WaterEffect.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            WaterEffect.SetActive(false);
        }
    }
}
