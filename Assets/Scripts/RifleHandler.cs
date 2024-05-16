using UnityEngine;
using TMPro;


//ιδια κλαση οπως του PistolHandler αλλα για το rifle
public class RifleHandler : MonoBehaviour
{
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] GameObject shellRemover;
    Rigidbody rb;
    [SerializeField] TMP_Text ammo_text;
    AudioSource audioSource;
    [SerializeField] AudioClip rifleShoot, rifleShootEmpty, rifleReload;
    public int rifleMaxMagazineAmmo = 30;
    public int rifleTotalAmmo = 150;
    public int currentRifleAmmo;
    public bool canReload;
    public static RifleHandler current;
    private void Awake()
    {
        current = this;
        canReload = false;
        TryGetComponent<AudioSource>(out audioSource);
    }
    public bool HasAmmo()
    {
        return currentRifleAmmo > 0;
    }

    public void ConsumeAmmo()
    {
        if (currentRifleAmmo > 0)
        {
            audioSource.PlayOneShot(rifleShoot);
            currentRifleAmmo--;
        }
        if (rifleTotalAmmo > 0)
        {
            canReload = true;
        }
        UpdateText(true);
    }

    public void ReloadAmmo()
    {
        int missingRounds = rifleMaxMagazineAmmo - currentRifleAmmo;

        if (rifleTotalAmmo >= missingRounds)
        {
            currentRifleAmmo = rifleMaxMagazineAmmo;
            rifleTotalAmmo -= missingRounds;
            canReload = false;
        }
        else if (rifleTotalAmmo < missingRounds && rifleTotalAmmo > 0)
        {
            currentRifleAmmo += rifleTotalAmmo;
            rifleTotalAmmo = 0;
            canReload = false;
        }
        else if (rifleTotalAmmo == 0)
        {
            PlayEmptySound();
            canReload = false;
        }
        UpdateText(true);
    }
    public void PlayEmptySound()
    {
        audioSource.PlayOneShot(rifleShootEmpty);
    }
    public void PlayReloadSound()
    {
        audioSource.PlayOneShot(rifleReload);
    }
    public void RefillAmmo()
    {
        rifleTotalAmmo = 150;
        canReload = true;
        UpdateText(true);
    }
    public bool CanReload()
    {
        return canReload;
    }
    public void SetCurrentAmmo(int currentAmmo)
    {
        currentRifleAmmo = currentAmmo;
    }
    public void SetTotalAmmo(int totalAmmo)
    {
        rifleTotalAmmo = totalAmmo;
    }
    public void UpdateText(bool haveRifle)
    {
        canReload = currentRifleAmmo < rifleMaxMagazineAmmo && rifleTotalAmmo > 0;
        if (haveRifle)
        {
            ammo_text.text = currentRifleAmmo.ToString() + " / " + rifleTotalAmmo.ToString();
        }
    }

    public void SpawnNewBullet()
    {
        GameObject newBullet = Instantiate(bulletPrefab, shellRemover.transform.position, shellRemover.transform.rotation);
        rb = newBullet.GetComponent<Rigidbody>();
    }
    public void ExplodeBullet()
    {
        float randomValue = Random.Range(-0.5f, 0.5f);
        rb.AddForce(rb.transform.TransformDirection(-1 + randomValue, 1 + randomValue, randomValue), ForceMode.Impulse);
        rb.useGravity = true;
        Destroy(rb.gameObject, 5f);
    }
}
