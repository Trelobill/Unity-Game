using UnityEngine;

public class AmmoInside : MonoBehaviour
{
    int pistolCurrentAmmo, pistolTotalAmmo;
    int rifleCurrentAmmo, rifleTotalAmmo;

    //καθε οπλο που δημιουργειται παιρνει τυχαια σφαιρες
    void Awake()
    {
        pistolCurrentAmmo = Random.Range(1, 6);
        pistolTotalAmmo = Random.Range(1, 10);
        rifleCurrentAmmo = Random.Range(5, 15);
        rifleTotalAmmo = Random.Range(1, 30);
    }

    public int GetPistolCurrentAmmo()
    {
        return pistolCurrentAmmo;
    }
    public int GetPistolTotalAmmo()
    {
        return pistolTotalAmmo;
    }
    public int GetRifleCurrentAmmo()
    {
        return rifleCurrentAmmo;
    }
    public int GetRifleTotalAmmo()
    {
        return rifleTotalAmmo;
    }
    public void SetPistolCurrentAmmo(int ammo)
    {
        pistolCurrentAmmo = ammo;
    }
    public void SetPistolTotalAmmo(int ammo)
    {
        pistolTotalAmmo = ammo;
    }
    public void SetRifleCurrentAmmo(int ammo)
    {
        rifleCurrentAmmo = ammo;
    }
    public void SetRifleTotalAmmo(int ammo)
    {
        rifleTotalAmmo = ammo;
    }
}
