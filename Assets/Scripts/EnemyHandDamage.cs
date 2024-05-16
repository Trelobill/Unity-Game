using UnityEngine;
using Cinemachine;
public class EnemyHandDamage : MonoBehaviour
{
    AttributesManager atm;
    CinemachineImpulseSource impulse;
    float lastHitTime;

    private void Awake()
    {
        impulse = transform.GetComponent<CinemachineImpulseSource>();
        //παμε στο root ωστε να παρουμε το attributesManager γιατι το script ειναι στο χερι του ζομπι
        atm = transform.root.GetComponent<AttributesManager>();
    }

    //αν κανει trigger στον παιχτη του κανει ζημια και δημιουργει ενα impulse στην καμερα
    private void OnTriggerEnter(Collider other)
    {
        //εχουμε timer 0.5sec για να μην κανει πολλαπλα χτυπηματα μεσα σε μια κινηση
        if (other.tag == "Player" && Time.time - lastHitTime >= 0.5f)
        {
            lastHitTime = Time.time;
            impulse.GenerateImpulse(1);
            atm.DealDamage(other.gameObject, this.tag, Vector3.zero);
        }
    }
}
