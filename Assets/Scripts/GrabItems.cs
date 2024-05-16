using UnityEngine;
using StarterAssets;
using UnityEngine.InputSystem;
using TMPro;

public class GrabItems : MonoBehaviour
{
    StarterAssetsInputs starterAssetsInputs;
    PlayerInput playerInput;
    [SerializeField] ItemGrabbable moveToObject, selectedGameobject, justForHideGameobject = null;
    [SerializeField] Transform leftHand, handIKTarget;
    [SerializeField] GameObject WeaponsSlot;
    [SerializeField] GameObject[] weapons;
    [SerializeField] GameObject[] weaponsModel;
    [SerializeField] GameObject[] visualWeapons;
    [SerializeField] float boxSize = 0.25f;
    [SerializeField] float grabDistance = 0.6f, grabFarDistance = 4f;
    GameObject pistolAmmoUI, rifleAmmoUI;
    TextMeshProUGUI pistolAmmoTextUI, rifleAmmoTextUI;
    Animator anim;
    string moveToObjectTag;
    float distanceCharacterToItem;
    float rotationSpeed = 8f;
    Quaternion targetRotation;
    RaycastHit hit, moveToHit;
    [SerializeField] float SprintSpeed = 5.335f;
    ThirdPersonController controller;
    public bool moveForAmmo = false;
    [SerializeField] bool canLookOtherItem = true, canPick = true;
    [SerializeField] bool isRotatingWhenClose = false, isRotatingWhenCloseEnough = false;
    bool isClose = false, isCloseEnough = false;
    public static GrabItems current;
    void Awake()
    {
        current = this;
        TryGetComponent<ThirdPersonController>(out controller);
        TryGetComponent<StarterAssetsInputs>(out starterAssetsInputs);
        TryGetComponent<PlayerInput>(out playerInput);
        TryGetComponent<Animator>(out anim);
    }
    
    void Update()
    {
        //αν κανω aim ή δεν μπορω να πιασω κατι τοτε κρυβω το overlay και κανω return
        if (starterAssetsInputs.aim || !canPick)
        {
            if (selectedGameobject)
            {
                selectedGameobject.OverlayHide();
                if (selectedGameobject.CompareTag("Pistol") || selectedGameobject.CompareTag("Rifle"))
                {
                    selectedGameobject.transform.Find("Canvas").gameObject.SetActive(false);
                }
            }
            return;
        }



        //παιρνω το κεντρο της οθονης
        Vector2 screenCenterPoint = new(Screen.width / 2f, Screen.height / 2f);
        Ray cameraRay = Camera.main.ScreenPointToRay(screenCenterPoint);
        //χρησιμοποιω boxCast για να εχει μεγαλυτερο ευρος για να παιρνεις τα αντικειμενα με μεγαλυτερη ευκολια
        if (Physics.BoxCast(cameraRay.origin, new Vector3(boxSize, boxSize, boxSize) / 2, cameraRay.direction, out hit, Quaternion.identity, 50f))
        {

            //αν κοιταει κατι που μπορει να κανει grab το περναει στο selectedGameObject
            //η συνθηκη canLookOtherItem ειναι για bug-fix στο οποιο οταν ειχα πατησει να πιασει κατι και οσο πηγαινε κοιτουσα σε
            //αλλο grabbable item, πηγαινε στο καινουριο αντι αυτο που ειχα πατησει
            if (canLookOtherItem && hit.collider.TryGetComponent<ItemGrabbable>(out selectedGameobject))
            {
                distanceCharacterToItem = Vector3.Distance(transform.position, selectedGameobject.transform.position);
            }
            else
            {
                selectedGameobject = null;
            }
            //αν κοιταω grabbable item, ειμαι κοντα στο να το πιασω, δεν κουνιεται και δεν το εχω ηδη
            if (selectedGameobject && selectedGameobject.GetComponent<Rigidbody>().velocity == new Vector3(0, 0, 0)
             && distanceCharacterToItem < grabDistance && !CheckIfAlreadyHave(selectedGameobject.tag))
            {
                moveToObject = selectedGameobject;
                moveToObjectTag = moveToObject.tag;
                isClose = true;
                isCloseEnough = false;
                //αν ειχα προηγουμενο hightlighted, το βγαζω
                if (justForHideGameobject && justForHideGameobject != selectedGameobject)
                {
                    justForHideGameobject.OverlayHide();
                    if (justForHideGameobject.CompareTag("Pistol") || justForHideGameobject.CompareTag("Rifle"))
                    {
                        justForHideGameobject.transform.Find("Canvas").gameObject.SetActive(false);
                    }
                }
                if (selectedGameobject.CompareTag("Pistol"))
                {
                    selectedGameobject.OverlayShow();
                    //το UI για τις σφαιρες πρεπει να κοιταει παντα πανω οτι rotation και να εχει το οπλο
                    pistolAmmoUI = selectedGameobject.transform.Find("Canvas").gameObject;
                    var centerPoint = selectedGameobject.transform.Find("Sketchfab_model");
                    Vector3 desiredPosition = centerPoint.position + Vector3.up * 0.6f;
                    pistolAmmoUI.transform.position = desiredPosition;
                    pistolAmmoUI.transform.forward = Camera.main.transform.forward;
                    pistolAmmoUI.SetActive(true);
                    pistolAmmoTextUI = pistolAmmoUI.transform.Find("Ammo").GetComponent<TextMeshProUGUI>();
                    int pistolCurrentAmmo = selectedGameobject.GetComponent<AmmoInside>().GetPistolCurrentAmmo();
                    int pistolTotalAmmo = selectedGameobject.GetComponent<AmmoInside>().GetPistolTotalAmmo();
                    if (pistolCurrentAmmo == 0 && pistolTotalAmmo == 0)
                    {
                        pistolAmmoTextUI.color = Color.red;
                    }
                    else
                    {
                        pistolAmmoTextUI.color = Color.green;
                    }
                    pistolAmmoTextUI.text = pistolCurrentAmmo.ToString() + " / " + pistolTotalAmmo.ToString();
                }
                else if (selectedGameobject.CompareTag("Rifle"))
                {
                    selectedGameobject.OverlayShow();
                    rifleAmmoUI = selectedGameobject.transform.Find("Canvas").gameObject;
                    var centerPoint = selectedGameobject.transform.Find("Forend");
                    Vector3 desiredPosition = centerPoint.position + Vector3.up * 0.6f;
                    rifleAmmoUI.transform.position = desiredPosition;
                    rifleAmmoUI.transform.forward = Camera.main.transform.forward;
                    rifleAmmoUI.SetActive(true);
                    rifleAmmoTextUI = rifleAmmoUI.transform.Find("Ammo").GetComponent<TextMeshProUGUI>();
                    int rifleCurrentAmmo = selectedGameobject.GetComponent<AmmoInside>().GetRifleCurrentAmmo();
                    int rifleTotalAmmo = selectedGameobject.GetComponent<AmmoInside>().GetRifleTotalAmmo();
                    if (rifleCurrentAmmo == 0 && rifleTotalAmmo == 0)
                    {
                        rifleAmmoTextUI.color = Color.red;
                    }
                    else
                    {
                        rifleAmmoTextUI.color = Color.green;
                    }
                    rifleAmmoTextUI.text = rifleCurrentAmmo.ToString() + " / " + rifleTotalAmmo.ToString();
                }
                else
                {
                    selectedGameobject.OverlayShow();
                }
                justForHideGameobject = selectedGameobject;
            }
            //αν κοιταω grabbable item και ειμαι λιγο πιο μακρια στο να το πιασω
            else if (selectedGameobject && selectedGameobject.GetComponent<Rigidbody>().velocity == new Vector3(0, 0, 0) && distanceCharacterToItem < grabDistance + grabFarDistance && !CheckIfAlreadyHave(selectedGameobject.tag))
            {
                moveToObject = selectedGameobject;
                moveToObjectTag = moveToObject.tag;
                isClose = false;
                isCloseEnough = true;
                //αν ειχα προηγουμενο hightlighted, το βγαζω
                if (justForHideGameobject && justForHideGameobject != selectedGameobject)
                {
                    justForHideGameobject.OverlayHide();
                    if (justForHideGameobject.CompareTag("Pistol") || justForHideGameobject.CompareTag("Rifle"))
                    {
                        justForHideGameobject.transform.Find("Canvas").gameObject.SetActive(false);
                    }
                }
                if (selectedGameobject.CompareTag("Pistol"))
                {
                    selectedGameobject.OverlayShow();
                    pistolAmmoUI = selectedGameobject.transform.Find("Canvas").gameObject;
                    var centerPoint = selectedGameobject.transform.Find("Sketchfab_model");
                    Vector3 desiredPosition = centerPoint.position + Vector3.up * 0.6f;
                    pistolAmmoUI.transform.position = desiredPosition;
                    pistolAmmoUI.transform.forward = Camera.main.transform.forward;
                    pistolAmmoUI.SetActive(true);
                    pistolAmmoTextUI = pistolAmmoUI.transform.Find("Ammo").GetComponent<TextMeshProUGUI>();
                    int pistolCurrentAmmo = selectedGameobject.GetComponent<AmmoInside>().GetPistolCurrentAmmo();
                    int pistolTotalAmmo = selectedGameobject.GetComponent<AmmoInside>().GetPistolTotalAmmo();
                    if (pistolCurrentAmmo == 0 && pistolTotalAmmo == 0)
                    {
                        pistolAmmoTextUI.color = Color.red;
                    }
                    else
                    {
                        pistolAmmoTextUI.color = Color.green;
                    }
                    pistolAmmoTextUI.text = pistolCurrentAmmo.ToString() + " / " + pistolTotalAmmo.ToString();
                }
                else if (selectedGameobject.CompareTag("Rifle"))
                {
                    selectedGameobject.OverlayShow();
                    rifleAmmoUI = selectedGameobject.transform.Find("Canvas").gameObject;
                    var centerPoint = selectedGameobject.transform.Find("Forend");
                    Vector3 desiredPosition = centerPoint.position + Vector3.up * 0.6f;
                    rifleAmmoUI.transform.position = desiredPosition;
                    rifleAmmoUI.transform.forward = Camera.main.transform.forward;
                    rifleAmmoUI.SetActive(true);
                    rifleAmmoTextUI = rifleAmmoUI.transform.Find("Ammo").GetComponent<TextMeshProUGUI>();
                    int rifleCurrentAmmo = selectedGameobject.GetComponent<AmmoInside>().GetRifleCurrentAmmo();
                    int rifleTotalAmmo = selectedGameobject.GetComponent<AmmoInside>().GetRifleTotalAmmo();
                    if (rifleCurrentAmmo == 0 && rifleTotalAmmo == 0)
                    {
                        rifleAmmoTextUI.color = Color.red;
                    }
                    else
                    {
                        rifleAmmoTextUI.color = Color.green;
                    }
                    rifleAmmoTextUI.text = rifleCurrentAmmo.ToString() + " / " + rifleTotalAmmo.ToString();
                }
                else
                {
                    selectedGameobject.OverlayShow();
                }
                justForHideGameobject = selectedGameobject;
            }
            //αν ειμαι μακρια απο grabbable item
            else
            {
                isClose = false;
                isCloseEnough = false;
                if (justForHideGameobject)
                {
                    justForHideGameobject.OverlayHide();
                    if (justForHideGameobject.CompareTag("Pistol") || justForHideGameobject.CompareTag("Rifle"))
                    {
                        justForHideGameobject.transform.Find("Canvas").gameObject.SetActive(false);
                    }
                    justForHideGameobject = null;
                }
            }

            //αν πατησω να το πιασει
            if (starterAssetsInputs.grabItem && controller.Grounded && selectedGameobject 
            && !isRotatingWhenClose && !isRotatingWhenCloseEnough 
            && (!CheckIfAlreadyHave(selectedGameobject.tag)) && (isClose || isCloseEnough))
            {
                //αν πατησω να πιασει σφαιρες ΄ή οπλο τοτε δεν θα εξαφανιστει
                switch (moveToObjectTag)
                {
                    case "Ammo":
                        SpawnManager.current.shouldDestroyAmmo = false;
                        break;
                    case "MeleeWeapons":
                    case "Pistol":
                    case "Rifle":
                        SpawnManager.current.shouldDestroyWeapon = false;
                        break;
                }

                //σταματαει να κινειται ο παιχτης
                starterAssetsInputs.sprint = false;
                starterAssetsInputs.move = new(0, 0);
                moveToHit = hit;
                canLookOtherItem = false;
                starterAssetsInputs.grabItem = false;
                //αν ειναι κοντα βρισκω το rotation 
                if (isClose)
                {
                    if (selectedGameobject != null)
                    {
                        targetRotation = Quaternion.LookRotation(selectedGameobject.transform.position - transform.position);
                        isRotatingWhenClose = true;
                        //για λιγο μπορω μονο να γυρναω το ποντικι
                        DisableAllActionsExceptLook();
                    }
                }
                //αν ειναι στα ορια που φτανει βρισκω το rotation 
                else if (isCloseEnough)
                {
                    targetRotation = Quaternion.LookRotation(moveToObject.transform.position - transform.position);
                    targetRotation.x = 0;
                    targetRotation.z = 0;
                    isRotatingWhenCloseEnough = true;
                    DisableAllActionsExceptLook();
                }
            }
            if (isRotatingWhenClose)
            {
                //στριβω τον χαρακτηρα προς το αντικειμενο
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                Vector3 currentEulerAngles = transform.rotation.eulerAngles;
                Vector3 targetEulerAngles = targetRotation.eulerAngles;
                //και οταν πλεον το κοιταζει
                if (Mathf.Abs(currentEulerAngles.y - targetEulerAngles.y) < 0.5f)
                {
                    isRotatingWhenClose = false;
                    //παιζει το animation του grab
                    anim.SetTrigger("GrabItem");
                }
            }
            else if (isRotatingWhenCloseEnough)
            {
                moveForAmmo = true;
                //στριβω τον χαρακτηρα προς το αντικειμενο
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                Vector3 currentEulerAngles = transform.rotation.eulerAngles;
                Vector3 targetEulerAngles = targetRotation.eulerAngles;
                //και οταν πλεον το κοιταζει
                if (Mathf.Abs(currentEulerAngles.y - targetEulerAngles.y) < 0.5f)
                {
                    //ανεβαζω το speed value στο animator ωστε να "τρεχει" προς την κατευθυνση
                    anim.SetFloat("Speed", SprintSpeed);
                    //ομαλα μετακινω τον χαρακτηρα προς το αντικειμενο μεχρι να ερθει αρκετα κοντα και να το πιασει
                    Vector3 newPosition = Vector3.Lerp(transform.position, moveToObject.transform.position, SprintSpeed * Time.deltaTime);
                    transform.position = newPosition;
                    if (Vector3.Distance(transform.position, moveToObject.transform.position) < grabDistance)
                    {
                        isRotatingWhenCloseEnough = false;
                        anim.SetTrigger("GrabItem");
                        moveForAmmo = false;
                    }
                }

            }
        }
    }

    private bool CheckIfAlreadyHave(string tag)
    {
        switch (tag)
        {
            case "MeleeWeapons":
                if (WeaponManager.current.CheckCarryWeaponState("Melee"))
                {
                    return true;
                }
                break;

            case "Pistol":
                if (WeaponManager.current.CheckCarryWeaponState("Pistol"))
                {
                    return true;
                }
                break;

            case "Rifle":
                if (WeaponManager.current.CheckCarryWeaponState("Rifle"))
                {
                    return true;
                }
                break;
        }
        return false;
    }

    private void SetHandIkTarget()
    {
        //τοποθετω το target που θα παει το χερι πανω στο αντικειμενο
        Vector3 center = moveToHit.collider.bounds.center;
        Vector3 topCenterPoint = center + (transform.up * (moveToHit.collider.bounds.size.y * 0.5f));
        handIKTarget.position = topCenterPoint;
    }

    private void EnableLookOtherItem()
    {
        canLookOtherItem = true;
    }
    private void JobForItemGrabbed()
    {
        switch (moveToObjectTag)
        {
            case "MeleeWeapons":
                WeaponManager.current.EnableCarryWeapon("Melee");
                visualWeapons[0].SetActive(true);
                moveToHit.transform.SetParent(WeaponsSlot.transform);
                moveToHit.transform.gameObject.SetActive(false);
                break;
            case "Pistol":
                WeaponManager.current.EnableCarryWeapon("Pistol");
                visualWeapons[1].SetActive(true);
                moveToHit.transform.SetParent(WeaponsSlot.transform);
                weapons[0].SetActive(true);
                PistolHandler.current.SetCurrentAmmo(moveToHit.transform.GetComponent<AmmoInside>().GetPistolCurrentAmmo());
                PistolHandler.current.SetTotalAmmo(moveToHit.transform.GetComponent<AmmoInside>().GetPistolTotalAmmo());
                PistolHandler.current.UpdateText(true);
                moveToHit.transform.gameObject.SetActive(false);
                weapons[0].SetActive(false);
                break;
            case "Rifle":
                WeaponManager.current.EnableCarryWeapon("Rifle");
                visualWeapons[2].SetActive(true);
                moveToHit.transform.SetParent(WeaponsSlot.transform);
                weapons[1].SetActive(true);
                RifleHandler.current.SetCurrentAmmo(moveToHit.transform.GetComponent<AmmoInside>().GetRifleCurrentAmmo());
                RifleHandler.current.SetTotalAmmo(moveToHit.transform.GetComponent<AmmoInside>().GetRifleTotalAmmo());
                RifleHandler.current.UpdateText(true);
                moveToHit.transform.gameObject.SetActive(false);
                weapons[1].SetActive(false);
                break;
            case "Ammo":
                if (WeaponManager.current.CheckCarryWeaponState("Pistol") && WeaponManager.current.CheckCarryWeaponState("Rifle"))
                {
                    Destroy(moveToObject.gameObject);
                    EnableWeapons();
                    PistolHandler.current.RefillAmmo();
                    RifleHandler.current.RefillAmmo();
                    DisableWeapons();
                    break;
                }
                else if (WeaponManager.current.CheckCarryWeaponState("Pistol") && !WeaponManager.current.CheckCarryWeaponState("Rifle"))
                {
                    Destroy(moveToObject.gameObject);
                    EnableWeapons();
                    PistolHandler.current.RefillAmmo();
                    DisableWeapons();
                    break;
                }
                else if (!WeaponManager.current.CheckCarryWeaponState("Pistol") && WeaponManager.current.CheckCarryWeaponState("Rifle"))
                {
                    EnableWeapons();
                    RifleHandler.current.RefillAmmo();
                    DisableWeapons();
                    break;
                }
                else
                {
                    Destroy(moveToObject.gameObject);
                    break;
                }
        }
        moveToObject = null;
    }
    private void DisableAllActionsExceptLook()
    {
        var allActions = playerInput.actions;
        foreach (var action in allActions)
        {
            if (action.name != "Look")
            {
                action.Disable();
            }
        }
    }

    private void EnableInputs()
    {
        playerInput.actions.Enable();
    }

    private void EnableWeapons()
    {
        if (WeaponManager.current.CheckWeaponState("Pistol"))
        {
            weapons[1].SetActive(true);
            weaponsModel[1].SetActive(false);
        }
        else if (WeaponManager.current.CheckWeaponState("Rifle"))
        {
            weapons[0].SetActive(true);
            weaponsModel[0].SetActive(false);
        }
        else
        {
            weapons[0].SetActive(true);
            weaponsModel[0].SetActive(false);
            weapons[1].SetActive(true);
            weaponsModel[1].SetActive(false);
        }
    }

    private void DisableWeapons()
    {
        //αν κραταγα ενα απο τα 2 οπλα δεν το απενεργοποιω αλλιως απενεργοποιω και τα 2
        if (WeaponManager.current.CheckWeaponState("Pistol"))
        {
            weaponsModel[1].SetActive(true);
            weapons[1].SetActive(false);
        }
        else if (WeaponManager.current.CheckWeaponState("Rifle"))
        {
            weaponsModel[0].SetActive(true);
            weapons[0].SetActive(false);
        }
        else
        {
            weaponsModel[0].SetActive(true);
            weaponsModel[1].SetActive(true);
            weapons[0].SetActive(false);
            weapons[1].SetActive(false);
        }
    }
    private void DisablePickWhileReload()
    {
        canPick = false;
    }
    private void EnablePickWhileReload()
    {
        canPick = true;
    }
}
