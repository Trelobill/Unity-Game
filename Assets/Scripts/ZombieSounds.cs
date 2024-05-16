using UnityEngine;

//κλαση για τους ηχους των εχθρων
public class ZombieSounds : MonoBehaviour
{

    AudioSource audioSource;
    [SerializeField] AudioClip zombieAngry;
    [SerializeField] AudioClip zombieAttackLight, zombieAttackStrong, zombieDie;
    [SerializeField] AudioClip[] zombieDamaged, zombiePatrolling;
    float maxDistance = 30.0f;
    float minVolume = 0f;
    float timerToPatrolSound = 0f;
    bool isPlayingPatrolSound = false;

    private void Awake()
    {
        TryGetComponent<AudioSource>(out audioSource);
    }

    private void Update()
    {
        //το patrol sound δεν πρεπει να ξαναπαιξει πριν περασουν 5 δευτερολεπτα
        if (isPlayingPatrolSound)
        {
            timerToPatrolSound += Time.deltaTime;
            if (timerToPatrolSound >= 5f)
            {
                isPlayingPatrolSound = false;
                timerToPatrolSound = 0f;
            }
        }
    }

    //οι παρακατω συναρτησεις ειναι για χρηση animation event
    //παιζουν με ενταση αναλογα την αποσταση απο τον παιχτη
    //καποια διαλεγουν τυχαια ενα clip που θα παιχτει
    private void OnPatrolling(AnimationEvent animationEvent)
    {
        if (!isPlayingPatrolSound)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                float distance = Vector3.Distance(GameObject.FindWithTag("Player").transform.position, transform.position);
                float volume = Mathf.Clamp01(1.0f - (distance / maxDistance));
                volume = Mathf.Max(volume, minVolume);
                int randomClipIndex = Random.Range(0, zombiePatrolling.Length);
                audioSource.PlayOneShot(zombiePatrolling[randomClipIndex], volume);
                isPlayingPatrolSound = true;
            }
        }
    }
    private void OnAngry(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            float distance = Vector3.Distance(GameObject.FindWithTag("Player").transform.position, transform.position);
            float volume = 1.0f - Mathf.Clamp01(distance / maxDistance);
            volume = Mathf.Max(volume, minVolume);
            audioSource.PlayOneShot(zombieAngry, volume);
        }
    }
    public void OnDamaged()
    {
        float distance = Vector3.Distance(GameObject.FindWithTag("Player").transform.position, transform.position);
        float volume = 1.0f - Mathf.Clamp01(distance / maxDistance);
        volume = Mathf.Max(volume, minVolume);
        int randomClipIndex = Random.Range(0, zombieDamaged.Length);
        audioSource.PlayOneShot(zombieDamaged[randomClipIndex], volume);
    }
    private void OnAttackLight(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f && !audioSource.isPlaying)
        {
            {
                audioSource.PlayOneShot(zombieAttackLight);
            }
        }
    }

    private void OnAttackStrong(AnimationEvent animationEvent)
    {

        if (animationEvent.animatorClipInfo.weight > 0.5f && !audioSource.isPlaying)
        {
            {
                audioSource.PlayOneShot(zombieAttackStrong);
            }
        }
    }

    public void OnDie()
    {
        float distance = Vector3.Distance(GameObject.FindWithTag("Player").transform.position, transform.position);
        float volume = 1.0f - Mathf.Clamp01(distance / maxDistance);
        volume = Mathf.Max(volume, minVolume);
        audioSource.PlayOneShot(zombieDie, volume);
    }
}
