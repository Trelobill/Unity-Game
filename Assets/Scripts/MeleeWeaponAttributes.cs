using UnityEngine;

//κλαση για μπουνιες και melee οπλα
public class MeleeWeaponAttributes : MonoBehaviour
{
    public AttributesManager atm;
    [SerializeField] GameObject bloodEffect;
    float lastHitTime;
    AudioSource audioSource;
    [SerializeField] AudioClip leftFistClip, rightFistClip, axeClip;

    private void Awake()
    {
        TryGetComponent<AudioSource>(out audioSource);
    }
    private void OnTriggerStay(Collider other)
    {
        //αν χτυπησει body part εχθρων
        if (other.gameObject.layer == 8)
        {
            //εδω βαζω μια μικρη καθυστερηση 0.5 δευτερολεπτα ωστε σε καθε χτυπημα με μπουνιες ή melee weapons
            //να μην κανει damage για καθε collider που βρισκει αλλα μονο στον πρωτο
            Physics.Raycast(transform.position, transform.forward, out RaycastHit hit);
            if (Time.time - lastHitTime >= 0.5f)
            {
                lastHitTime = Time.time;
                //βρισκω την κατευθυνση που πρεπει να παρει το bloodEffect
                Vector3 directionToPlayer = (atm.transform.position - other.transform.position).normalized;
                Vector3 centerContactPoint = other.bounds.center;
                Quaternion rotation = Quaternion.LookRotation(directionToPlayer);
                rotation.eulerAngles = new Vector3(0, rotation.eulerAngles.y, rotation.eulerAngles.z);

                Animator enemyAnim = other.GetComponentInParent<Animator>();
                enemyAnim.SetBool("isPatrolling", false);
                enemyAnim.SetBool("isChasing", true);
                //παιζει το animation του damage
                enemyAnim.Play("damageZombie");
                //κανει damage αναλογα το οπλο και παιζει ο ηχος του οπλοι(μπουνιες ή τσεκουρι)
                atm.DealDamage(other.gameObject, this.tag, hit.point);
                PlaySoundForThisTag(this.tag);
                GameObject blood = Instantiate(bloodEffect, centerContactPoint, rotation);
                blood.transform.SetParent(other.gameObject.transform);
                //καταστρεφω το bloodeffect μετα απο 2 δευτερολεπτα
                Destroy(blood, 2);
            }
        }
    }

    void PlaySoundForThisTag(string tag)
    {
        switch (tag)
        {
            case "RightFist":
                audioSource.PlayOneShot(rightFistClip);
                break;
            case "LeftFist":
                audioSource.PlayOneShot(leftFistClip);
                break;
            case "MeleeWeapons":
                audioSource.PlayOneShot(axeClip);
                break;
        }
    }
}

