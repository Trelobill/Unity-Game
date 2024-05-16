using UnityEngine;
using TMPro;

public class PistolHandler : MonoBehaviour
{
    [SerializeField] GameObject bulletPrefab;
    Rigidbody rb;
    [SerializeField] TMP_Text ammo_text;
    AudioSource audioSource;
    [SerializeField] AudioClip pistolShoot, pistolShootEmpty, pistolReload;
    public int pistolMaxMagazineAmmo = 6;
    public int pistolTotalAmmo = 30;
    public int currentPistolAmmo;
    public bool canReload;
    public static PistolHandler current;
    private void Awake()
    {
        current = this;
        canReload = false;
        TryGetComponent<AudioSource>(out audioSource);
    }

    public bool HasAmmo()
    {
        return currentPistolAmmo > 0;
    }

    //αν εχω σφαιρες παιζω τον ηχο μειωνω κατα 1 και ανανεωνω το UI
    public void ConsumeAmmo()
    {
        if (currentPistolAmmo > 0)
        {
            audioSource.PlayOneShot(pistolShoot);
            currentPistolAmmo--;
        }
        if (pistolTotalAmmo > 0)
        {
            canReload = true;
        }
        UpdateText(true);
    }

    //αν εχω υπολειπομενες σφαιρες τοτε μπορω να κανω reload
    public void ReloadAmmo()
    {
        int missingRounds = pistolMaxMagazineAmmo - currentPistolAmmo;

        if (pistolTotalAmmo >= missingRounds)
        {
            currentPistolAmmo = pistolMaxMagazineAmmo;
            pistolTotalAmmo -= missingRounds;
            canReload = false;
        }
        else if (pistolTotalAmmo < missingRounds && pistolTotalAmmo > 0)
        {
            currentPistolAmmo += pistolTotalAmmo;
            pistolTotalAmmo = 0;
            canReload = false;
        }
        //αν δεν εχω για reload τοτε παιζω τον ηχο για αδειο γεμιστηρα
        else if (pistolTotalAmmo == 0)
        {
            PlayEmptySound();
            canReload = false;
        }
        UpdateText(true);
    }

    public void PlayEmptySound()
    {
        audioSource.PlayOneShot(pistolShootEmpty);
    }
    public void PlayReloadSound()
    {
        audioSource.PlayOneShot(pistolReload);
    }

    //αν παρω σφαιρες
    public void RefillAmmo()
    {
        pistolTotalAmmo = 30;
        canReload = true;
        UpdateText(true);
    }

    //χρησιμοποειται στο TPS controller για να ξερω αν μπορω να κανω reload
    public bool CanReload()
    {
        return canReload;
    }

    //χρησιμοποποιουνται οταν παιρνω σφαιρες
    public void SetCurrentAmmo(int currentAmmo)
    {
        currentPistolAmmo = currentAmmo;
    }
    public void SetTotalAmmo(int totalAmmo)
    {
        pistolTotalAmmo = totalAmmo;
    }

    //συναρτηση που ανανεωνει το UI αναλογα τις σφαιρες που εχω
    public void UpdateText(bool havePistol)
    {
        canReload = currentPistolAmmo < pistolMaxMagazineAmmo && pistolTotalAmmo > 0;
        if (havePistol)
        {
            ammo_text.text = currentPistolAmmo.ToString() + " / " + pistolTotalAmmo.ToString();
        }
    }

    //χρησιμοποιυονται για animation events ωστε να φευγουν οι καλυκες απο το οπλο οταν πυροβολαω
    public void SpawnNewBullet()
    {
        GameObject newBullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
        rb = newBullet.GetComponent<Rigidbody>();
    }
    public void ExplodeBullet()
    {
        float randomValue = Random.Range(-0.5f, 0.5f);
        rb.AddForce(rb.transform.TransformDirection(1 + randomValue, 1 + randomValue, randomValue), ForceMode.Impulse);
        rb.useGravity = true;
        Destroy(rb.gameObject, 5f);
    }
}
