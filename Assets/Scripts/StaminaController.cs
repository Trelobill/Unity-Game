using StarterAssets;
using UnityEngine;
using UnityEngine.UI;

//κλαση για το stamina του παιχτη
public class StaminaController : MonoBehaviour
{
    ThirdPersonController controller;
    StarterAssetsInputs _input;
    public float playerStamina = 100.0f;
    public bool hasRegenerated = true;
    public bool weAreSprinting = false;
    [SerializeField] float maxStamina = 100.0f;
    [SerializeField] float jumpCost = 20.0f;
    [SerializeField] float staminaDrain = 0.5f;
    [SerializeField] float staminaRegen = 0.5f;
    [SerializeField] float slowedRunSpeed = 3.0f;
    [SerializeField] float normalRunSpeed = 5.335f;
    [SerializeField] Image staminaProgressUI = null;
    [SerializeField] CanvasGroup sliderCanvasGroup = null;

    void Awake()
    {
        controller = GetComponent<ThirdPersonController>();
        _input = GetComponent<StarterAssetsInputs>();
    }


    private void Update()
    {
        //αν δεν τρεχουμε
        if (!weAreSprinting)
        {
            //και δεν εχουμε max stamina
            if (playerStamina <= maxStamina)
            {
                //τοτε γεμιζει η μπαρα με το rate που του εχω δωσει
                playerStamina = Mathf.Clamp(playerStamina + staminaRegen * Time.deltaTime, 0f, maxStamina);
                //ανανεωνω το stamina bar
                UpdateStamina(1);
                //αν γινει max stamina τοτε αλλαζω την ταχυτητα πισω σε normal
                if (playerStamina >= maxStamina)
                {
                    controller.SetRunSpeed(normalRunSpeed);
                    sliderCanvasGroup.alpha = 0;
                    hasRegenerated = true;
                }
            }
        }
    }

    public void Sprinting()
    {
        //αν τρεχω και δεν μου ειχε τελειωσει η stamina νωριτερα   
        if (hasRegenerated)
        {
            //μειωνω την stamina με το rate που εχω δωσει
            weAreSprinting = true;
            playerStamina -= staminaDrain * Time.deltaTime;
            UpdateStamina(1);

            //και αν τελειωσει τοτε οριζουμε την ταχυτητα του παιχτη σε χαμηλοτερη μεχρι να γεμισει παλι η μπαρα και να γινει φυσιολογικη
            if (playerStamina <= 0)
            {
                hasRegenerated = false;
                controller.SetRunSpeed(slowedRunSpeed);
                if (!_input.sprint)
                    sliderCanvasGroup.alpha = 0;
            }
        }
    }

    //αν πηδηξει ο παιχτης χαλαει επισης stamina με τιμη που εχω δωσει
    public void StaminaJump()
    {
        if (playerStamina >= jumpCost)
        {
            playerStamina -= jumpCost;
            controller.PlayerJump();
            UpdateStamina(1);
            _input.jump = false;
        }
        else
        {
            _input.jump = false;
        }
    }

    //η συναρτηση αυτη απλα ανανεωνει την μπαρα στο UI
    public void UpdateStamina(int value)
    {
        staminaProgressUI.fillAmount = playerStamina / maxStamina;
        if (value == 0)
        {
            sliderCanvasGroup.alpha = 0;
        }
        else
        {
            sliderCanvasGroup.alpha = 1;
        }
    }
}
