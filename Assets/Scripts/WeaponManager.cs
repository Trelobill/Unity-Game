using UnityEngine;
using StarterAssets;

//ολη η λογικη των οπλων
public class WeaponManager : MonoBehaviour
{
    StarterAssetsInputs SAinputs;
    [SerializeField] GameObject meleeWeapon;
    [SerializeField] GameObject pistol;
    [SerializeField] GameObject rifle;
    [SerializeField] GameObject rightFist;
    [SerializeField] GameObject leftFist;
    [SerializeField] GameObject visual_melee_weapon;
    [SerializeField] GameObject visual_pistol;
    [SerializeField] GameObject visual_rifle;
    [SerializeField] bool hasFists = true;
    [SerializeField] bool hasMeleeWeapon = false;
    [SerializeField] bool hasPistol = false;
    [SerializeField] bool hasRifle = false;
    [SerializeField] bool canChangeWeapon = true;
    [SerializeField] bool canThrowWeapon = true;
    [SerializeField] bool carryMeleeWeapon = false;
    [SerializeField] bool carryPistol = false;
    [SerializeField] bool carryRifle = false;
    [SerializeField] GameObject[] bulletsText;
    [SerializeField] GameObject[] imageWeapons;
    Animator anim;
    public static WeaponManager current;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        SAinputs = GetComponent<StarterAssetsInputs>();
        current = this;
    }
    private void Update()
    {
        //αν θελω να βαλω μπουνιες και μπορω να αλλαξω οπλο τοτε παιζω το αναλογο animation
        //αναλογως και στα αλλα 3
        if (SAinputs.fistsInputOnly() && !CheckWeaponState("Fists") && canChangeWeapon)
        {
            if (hasMeleeWeapon)
            {
                anim.Play("letMeleeWeaponOverShoulder");
            }
            if (hasPistol)
            {
                anim.Play("letPistolInPocket");
            }
            if (hasRifle)
            {
                anim.Play("letRifleInPocket");
            }
            ChangeTo("Fists");
            SetImageTo(0);
        }
        //αν θελω να βαλω melee weapon
        if (SAinputs.meleeInputOnly() && !CheckWeaponState("Melee") && CheckCarryWeaponState("Melee") && canChangeWeapon)
        {
            if (hasFists)
            {
                anim.Play("catchMeleeWeaponOverShoulder");
            }
            if (hasPistol)
            {
                anim.Play("letPistolForMeleeWeapon");
            }
            if (hasRifle)
            {
                anim.Play("letRifleForMeleeWeapon");
            }
            ChangeTo("Melee");
            SetImageTo(1);
        }
        //αν θελω να βαλω πιστολι
        if (SAinputs.pistolInputOnly() && !CheckWeaponState("Pistol") && CheckCarryWeaponState("Pistol") && canChangeWeapon)
        {
            if (hasFists)
            {
                anim.Play("catchPistolFromPocket");
            }
            if (hasMeleeWeapon)
            {
                anim.Play("letMeleeWeaponForPistol");
            }
            if (hasRifle)
            {
                anim.Play("letRifleForPistol");
            }
            ChangeTo("Pistol");
            SetImageTo(2);
        }
        //αν θελω να βαλω αυτοματο οπλο
        if (SAinputs.rifleInputOnly() && !CheckWeaponState("Rifle") && CheckCarryWeaponState("Rifle") && canChangeWeapon)
        {
            if (hasFists)
            {
                anim.Play("catchRifleFromPocket");
            }
            if (hasMeleeWeapon)
            {
                anim.Play("letMeleeWeaponForRifle");
            }
            if (hasPistol)
            {
                anim.Play("letPistolForRifle");
            }
            ChangeTo("Rifle");
            SetImageTo(3);
        }
    }

    //αναλογα τι οπλο εβαλα ανανεωνω το UI
    public void SetImageTo(int weapon)
    {
        foreach (GameObject weaponImage in imageWeapons)
        {
            weaponImage.SetActive(false);
        }
        foreach (GameObject bulletText in bulletsText)
        {
            bulletText.SetActive(false);
        }

        switch (weapon)
        {
            case 0:
                imageWeapons[0].SetActive(true);
                break;
            case 1:
                imageWeapons[1].SetActive(true);
                break;
            case 2:
                imageWeapons[2].SetActive(true);
                bulletsText[0].SetActive(true);
                break;
            case 3:
                imageWeapons[3].SetActive(true);
                bulletsText[1].SetActive(true);
                break;
        }
    }

    //Αλλαζει το state του οπλου που εχω για να ξερω τι κραταει
    public void ChangeTo(string weapon)
    {
        hasFists = false;
        hasMeleeWeapon = false;
        hasPistol = false;
        hasRifle = false;
        switch (weapon)
        {
            case "Fists":
                hasFists = true;
                break;
            case "Melee":
                hasMeleeWeapon = true;
                break;
            case "Pistol":
                hasPistol = true;
                break;
            case "Rifle":
                hasRifle = true;
                break;
        }
    }
    //Γυρναει true-false αναλογα αν κραταει η οχι το οπλου που περασα στο ορισμα
    public bool CheckWeaponState(string weapon)
    {
        return weapon switch
        {
            "Fists" => hasFists,
            "Melee" => hasMeleeWeapon,
            "Pistol" => hasPistol,
            "Rifle" => hasRifle,
            _ => false,
        };
    }

    //συναρτηση που βλεπω αν εχω πανω στον παιχτη καποιο οπλο
    public bool CheckCarryWeaponState(string weapon)
    {
        return weapon switch
        {
            "Melee" => carryMeleeWeapon,
            "Pistol" => carryPistol,
            "Rifle" => carryRifle,
            _ => false,
        };
    }

    //αν παρω καποιο οπλο τοτε το κραταω σαν state για να μην μπορω να ξαναπαρω το ιδιο αν δεν πεταξω το παλιο
    public void EnableCarryWeapon(string weapon)
    {
        switch (weapon)
        {
            case "Melee":
                carryMeleeWeapon = true;
                break;
            case "Pistol":
                carryPistol = true;
                break;
            case "Rifle":
                carryRifle = true;
                break;
            default:
                break;
        }
    }

    public void DisableCarryWeapon(string weapon)
    {
        switch (weapon)
        {
            case "Melee":
                carryMeleeWeapon = false;
                break;
            case "Pistol":
                carryPistol = false;
                break;
            case "Rifle":
                carryRifle = false;
                break;
            default:
                break;
        }
    }

    //χρησιμοποιειται για το animator, να ξερω ποτε μπορω να αλλαξω οπλο
    public void CanChangeWeapon(bool weaponState)
    {
        canChangeWeapon = weaponState;
    }

    //χρησιμοποιειται για το animator, να ξερω ποτε μπορω να πεταξω οπλο
    public void CanThrowWeapon(bool state)
    {
        canThrowWeapon = state;
    }

    public bool GetThrowWeaponState()
    {
        return canThrowWeapon;
    }

    //ενεργοποιει-απενεργοποιει τα colider στα οπλα
    public void EnableWeaponCollider(int isEnable)
    {
        if (meleeWeapon != null)
        {
            var col = meleeWeapon.GetComponent<Collider>();
            if (col != null)
            {
                if (isEnable == 1)
                {
                    col.enabled = true;
                }
                else
                {
                    col.enabled = false;
                }
            }
        }
    }

    //αντιστοιχα για δεξια και αριστερη μπουνια
    public void EnableRightFistCollider(int isEnable)
    {
        if (rightFist != null)
        {
            var col = rightFist.GetComponent<Collider>();
            if (col != null)
            {
                if (isEnable == 1)
                {
                    col.enabled = true;
                }
                else
                {
                    col.enabled = false;
                }
            }
        }
    }
    public void EnableLeftFistCollider(int isEnable)
    {
        if (leftFist != null)
        {
            var col = leftFist.GetComponent<Collider>();
            if (col != null)
            {
                if (isEnable == 1)
                {
                    col.enabled = true;
                }
                else
                {
                    col.enabled = false;
                }
            }
        }
    }


    //οι παρακατω συναρτησεις ειναι ολες για χρηση των animation event και χρησιμοποιουνται για το visualize των οπλων
    public void LetMeleeWeaponOverShoulder()
    {
        if (CheckCarryWeaponState("Melee"))
        {
            meleeWeapon.SetActive(false);
            visual_melee_weapon.SetActive(true);
        }
        if (CheckCarryWeaponState("Pistol"))
        {
            pistol.SetActive(false);
            visual_pistol.SetActive(true);
        }
        if (CheckCarryWeaponState("Rifle"))
        {
            rifle.SetActive(false);
            visual_rifle.SetActive(true);
        }
    }
    public void CatchMeleeWeaponOverShoulder()
    {
        if (CheckCarryWeaponState("Melee"))
        {
            meleeWeapon.SetActive(true);
            visual_melee_weapon.SetActive(false);
        }
        if (CheckCarryWeaponState("Pistol"))
        {
            pistol.SetActive(false);
            visual_pistol.SetActive(true);
        }
        if (CheckCarryWeaponState("Rifle"))
        {
            rifle.SetActive(false);
            visual_rifle.SetActive(true);
        }
    }
    public void LetPistolInPocket()
    {
        if (CheckCarryWeaponState("Melee"))
        {
            meleeWeapon.SetActive(false);
            visual_melee_weapon.SetActive(true);
        }
        if (CheckCarryWeaponState("Pistol"))
        {
            pistol.SetActive(false);
            visual_pistol.SetActive(true);
        }
        if (CheckCarryWeaponState("Rifle"))
        {
            rifle.SetActive(false);
            visual_rifle.SetActive(true);
        }
    }
    public void CatchPistolFromPocket()
    {
        if (CheckCarryWeaponState("Melee"))
        {
            meleeWeapon.SetActive(false);
            visual_melee_weapon.SetActive(true);
        }
        if (CheckCarryWeaponState("Pistol"))
        {
            pistol.SetActive(true);
            visual_pistol.SetActive(false);
        }
        if (CheckCarryWeaponState("Rifle"))
        {
            rifle.SetActive(false);
            visual_rifle.SetActive(true);
        }
    }
    public void LetRifleInPocket()
    {
        if (CheckCarryWeaponState("Melee"))
        {
            meleeWeapon.SetActive(false);
            visual_melee_weapon.SetActive(true);
        }
        if (CheckCarryWeaponState("Pistol"))
        {
            pistol.SetActive(false);
            visual_pistol.SetActive(true);
        }
        if (CheckCarryWeaponState("Rifle"))
        {
            rifle.SetActive(false);
            visual_rifle.SetActive(true);
        }
    }
    public void CatchRifleFromPocket()
    {
        if (CheckCarryWeaponState("Melee"))
        {
            meleeWeapon.SetActive(false);
            visual_melee_weapon.SetActive(true);
        }
        if (CheckCarryWeaponState("Pistol"))
        {
            pistol.SetActive(false);
            visual_pistol.SetActive(true);
        }
        if (CheckCarryWeaponState("Rifle"))
        {
            rifle.SetActive(true);
            visual_rifle.SetActive(false);
        }
    }
}
