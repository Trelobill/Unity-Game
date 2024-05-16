using UnityEngine;

public class BulletSound : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] AudioClip bulletDrop;
    private void Awake()
    {
        TryGetComponent<AudioSource>(out audioSource);
    }

    private void OnCollisionEnter(Collision other) {
        if(other.transform.CompareTag("Environment"))
        {
            audioSource.PlayOneShot(bulletDrop);
        }
    }
}
