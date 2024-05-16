using UnityEngine;
using StarterAssets;
using System;

public class ThrowItems : MonoBehaviour
{
    [SerializeField] GameObject WeaponsSlot;
    [SerializeField] GameObject[] weapons;
    [SerializeField] float dropUpwardForce, dropForwardForce;
    StarterAssetsInputs starterAssetsInputs;
    Animator anim;
    void Start()
    {
        TryGetComponent<StarterAssetsInputs>(out starterAssetsInputs);
        TryGetComponent<Animator>(out anim);
    }

    void Update()
    {
        if (!starterAssetsInputs.throwItem || WeaponManager.current.CheckWeaponState("Fists")|| !WeaponManager.current.GetThrowWeaponState())
            return;
    
        anim.Play("throwWeapon");
    }
    public void Throw()
    {   
        //αναλογα τι κραταω για να πεταξω, βρισκω το αντικειμενο και τρεχω την ThrowWeapon
        if (WeaponManager.current.CheckWeaponState("Melee"))
        {
            Transform currentWeaponToThrow = FindChildWithKeyword(WeaponsSlot.transform, "axe");
            CopyTransformAndScale(weapons[0].transform, currentWeaponToThrow.transform);
            ThrowWeapon(weapons[0], currentWeaponToThrow, "Melee");
        }
        else if (WeaponManager.current.CheckWeaponState("Pistol"))
        {
            Transform currentWeaponToThrow = FindChildWithKeyword(WeaponsSlot.transform, "colt");
            CopyTransformAndScale(weapons[1].transform, currentWeaponToThrow.transform);
            ThrowWeapon(weapons[1], currentWeaponToThrow, "Pistol");
        }
        else if (WeaponManager.current.CheckWeaponState("Rifle"))
        {
            Transform currentWeaponToThrow = FindChildWithKeyword(WeaponsSlot.transform, "rifle");
            CopyTransformAndScale(weapons[2].transform, currentWeaponToThrow.transform);
            ThrowWeapon(weapons[2], currentWeaponToThrow, "Rifle");
        }
    }

    //βρισκει το gameobject με το συγκεκριμενο string
    private Transform FindChildWithKeyword(Transform parent, string keyword)
    {
        foreach (Transform child in parent)
        {
            if (child.gameObject.name.ToLower().Contains(keyword.ToLower()))
            {
                return child;
            }
        }
        return null;
    }

    private void CopyTransformAndScale(Transform source, Transform target)
    {
        target.position = source.position;
        target.rotation = source.rotation;
        target.localScale = source.localScale;
    }

    private void ThrowWeapon(GameObject WeaponInHand, Transform VisualWeapon, String weaponType)
    {
        //κραταω το speed για να παρει το οπλο το velocity μετα που το πεταω
        float playerSpeed = anim.GetFloat("Speed");
        Rigidbody weaponRB = VisualWeapon.GetComponent<Rigidbody>();
        WeaponInHand.SetActive(false);
        VisualWeapon.gameObject.SetActive(true);
        VisualWeapon.SetParent(null);
        //αν ειναι pistol ή rifle ανανεωνω τις σφαιρες που εχει μεσα
        switch (weaponType)
        {
            case "Pistol":
                VisualWeapon.GetComponent<AmmoInside>().SetPistolCurrentAmmo(PistolHandler.current.currentPistolAmmo);
                VisualWeapon.GetComponent<AmmoInside>().SetPistolTotalAmmo(PistolHandler.current.pistolTotalAmmo);
                PistolHandler.current.UpdateText(false);
                break;
            case "Rifle":
                VisualWeapon.GetComponent<AmmoInside>().SetRifleCurrentAmmo(RifleHandler.current.currentRifleAmmo);
                VisualWeapon.GetComponent<AmmoInside>().SetRifleTotalAmmo(RifleHandler.current.rifleTotalAmmo);
                RifleHandler.current.UpdateText(false);
                break;
        }
        //κανω apply force λιγο προς τα πανω και προς τα μπροστα(επηρεαζεται και απο την ταχυτητα του παιχτη)
        weaponRB.AddForce(transform.up * dropUpwardForce, ForceMode.Impulse);
        playerSpeed = Mathf.Max(2f, playerSpeed);
        weaponRB.AddForce(transform.forward * dropForwardForce * playerSpeed, ForceMode.Impulse);
        float random = UnityEngine.Random.Range(-1f, 1f);
        weaponRB.AddTorque(new Vector3(random, random, random) * 50);
        //αλλαζει το state σε μπουνιες και ανανεωνει το UI
        WeaponManager.current.ChangeTo("Fists");
        WeaponManager.current.SetImageTo(0);
        WeaponManager.current.DisableCarryWeapon(weaponType);
        starterAssetsInputs.meleeAttack = false;
        starterAssetsInputs.throwItem = false;
    }

}
