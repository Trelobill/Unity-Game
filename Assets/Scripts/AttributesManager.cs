using System.Collections;
using Ilumisoft.RadarSystem;
using StarterAssets;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class AttributesManager : MonoBehaviour
{
    [SerializeField] float health = 100;
    [SerializeField] float fistAttack = 5;
    [SerializeField] float MeleeAttack = 20;
    [SerializeField] float pistolAttack = 70;
    [SerializeField] float rifleAttack = 100;
    [SerializeField] float critFistAttack = 1.3f;
    [SerializeField] float critMeleeAttack = 1.5f;
    [SerializeField] float critPistolAttack = 1.8f;
    [SerializeField] float critRifleAttack = 1.8f;
    [SerializeField] float critChance = 0.3f;
    [SerializeField] GameObject healthBarUI;
    [SerializeField] Image StaminaBarUI;
    HealthSystemForDummies healthSystem;
    [SerializeField] int fistHitsToDizz = 4;
    [SerializeField] int axeHitsToDizz = 4;
    [SerializeField] int pistolHitsToDizz = 5;
    [SerializeField] int rifleHitsToDizz = 5;
    [SerializeField] int currentFistHits, currentAxeHits, currentPistolHits, currentRifleHits;
    [SerializeField] int FistHitsTimer = 10, AxeHitsTimer = 8, PistolHitsTimer = 3, RifleHitsTimer = 1;
    [SerializeField] float firstFistHitTime = 0, firstAxeHitTime = 0, firstPistolHitTime = 0, firstRifleHitTime = 0;
    [SerializeField] float timeDizzedZombie = 0, resetPeriodHitTime = 10f;
    [SerializeField] bool canDizz = true;
    [SerializeField] StarterAssetsInputs _input;
    [SerializeField] PlayerInput _playerInput;
    [SerializeField] GameObject gameOver;
    [SerializeField] Text gameOverText;
    ZombieSounds sounds;
    Transform player;
    Animator animator;
    NavMeshAgent agent;
    float chaseDistance = 8.0f;
    float attackDistance = 1.1f;
    bool isDeadZombie = false, isDeadPlayer = false;
    private void Awake()
    {
        if (this.tag == "Enemy")
        {
            TryGetComponent<NavMeshAgent>(out agent);
            TryGetComponent<Animator>(out animator);
            TryGetComponent<ZombieSounds>(out sounds);
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    public bool CheckPlayerDead()
    {
        return isDeadPlayer;
    }

    public void Heal(int addHealth)
    {
        health += addHealth;
        if (health > 100) health = 100;
    }

    //οταν γινεται dizzy σταματαω τον agent 
    public void StartDizzy()
    {
        agent.isStopped = true;
        ResetAllAnimatorBools();
    }

    //οταν βγει απο το dizzy βλεπει την αποσταση απο τον παιχτη και δραει αναλογα
    public void ExitDizzy()
    {
        animator.SetBool("isChasing", false);
        agent.isStopped = false;
        float distance = Vector3.Distance(player.position, animator.transform.position);
        if (distance <= attackDistance)
        {
            animator.SetBool("isAttacking", true);
        }
        else if (distance > attackDistance && distance <= chaseDistance)
        {
            animator.SetBool("isChasing", true);
        }
        else
        {
            animator.SetBool("isPatrolling", true);
        }
        animator.SetBool("Dizzy", false);
    }

    public void StartDizzyAnimation()
    {
        ResetAllAnimatorBools();
        animator.SetBool("Dizzy", true);
    }
    void ResetAllAnimatorBools()
    {
        AnimatorControllerParameter[] parameters = animator.parameters;

        foreach (AnimatorControllerParameter parameter in parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Bool)
            {
                animator.SetBool(parameter.name, false);
            }
        }
    }

    void Start()
    {
        //αλλαζω τις τιμες στο healthbar των εχθρων
        if (this.tag == "Enemy")
        {
            TryGetComponent<HealthSystemForDummies>(out healthSystem);
            healthSystem.CurrentHealth = (int)health;
            healthSystem.MaximumHealth = (int)health;
        }
    }

    void Update()
    {
        if (this.tag == "Enemy" && health > 0)
        {
            //αν περασει ο χρονος που τρωει dizzy καθε εχθρος, το κανω reset
            if (!canDizz && Time.time - timeDizzedZombie >= resetPeriodHitTime)
            {
                canDizz = true;
                //εφοσον μπορω να κανω παλι dizz, ξεκιναω τα hits απο το 0
                currentFistHits = currentAxeHits = currentPistolHits = currentRifleHits = 0;
                //και γεμιζω την StaminaBar
                StaminaBarUI.fillAmount = 1;
            }
            //αν περασει ο χρονος που εχω για να κανω dizz, κανει reset την StaminaBar
            if (((Time.time - firstFistHitTime > FistHitsTimer && currentFistHits > 0)
                || (Time.time - firstAxeHitTime > AxeHitsTimer && currentAxeHits > 0)
                || (Time.time - firstPistolHitTime > PistolHitsTimer && currentPistolHits > 0)
                || (Time.time - firstRifleHitTime > RifleHitsTimer && currentRifleHits > 0))
                && canDizz)
            {
                StaminaBarUI.fillAmount = 1;
            }
        }
    }

    //τρεχει πριν το start οποτε αν αλλαξει η ζωη με multiplier θα αλλαχτει εκει και το healthbar
    public void BuffZombies(float multiplier)
    {
        fistAttack *= multiplier;
        health *= multiplier;
    }

    public void TakeDamage(int amount, bool isCrit, string weaponTag, Vector3 contactPoint, GameObject target = null)
    {
        health -= amount;
        Vector3 randomness = new(Random.Range(0f, 0.25f), Random.Range(0f, 0.25f), Random.Range(0f, 0.25f));

        if (health <= 0)
        {
            //αν πεθανει εχθρος
            if (target.tag == "Enemy" && !isDeadZombie)
            {
                SpawnManager.current.KillOneEnemy();
                sounds.OnDie();
                GetComponent<Ragdoll>().EnableRagdoll(contactPoint);
                isDeadZombie = true;
                Destroy(healthSystem);
                Destroy(healthBarUI);
                Destroy(this.GetComponent<Locatable>());
                return;
            }
            //αν πεθανει ο παιχτης
            else if (target.tag == "Player" && !isDeadPlayer)
            {
                GetComponent<Ragdoll>().EnableRagdoll(contactPoint);
                isDeadPlayer = true;
                StartCoroutine(StopGameAfterDie());
            }
        }
        //αλλιως στους εχθρους βγαζει ποσο ζημια δεχτηκε
        else if (target.tag == "Enemy" && !isDeadZombie)
        {
            CheckForDizz(weaponTag);
            healthSystem.AddToCurrentHealth(-amount);
            sounds.OnDamaged();
            if (isCrit)
            {
                DamagePopUpGenerator.current.CreatePopUp(transform.position + randomness, amount.ToString(), Color.red);
            }
            else
            {
                DamagePopUpGenerator.current.CreatePopUp(transform.position + randomness, amount.ToString(), Color.yellow);
            }
        }
    }

    //τρεχει αν πεθανει ο παιχτης
    IEnumerator StopGameAfterDie()
    {
        _playerInput.actions.Disable();
        _input.DisableActiveInputs();
        GetComponent<ThirdPersonController>().enabled = false;
        yield return new WaitForSeconds(4f);
        InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInDynamicUpdate;
        _input.SetCursorState(false);
        gameOver.SetActive(true);
        int waveNumber = SpawnManager.current.GetCurrentWave();
        //αν εκανα καλυτερο ρεκορ απο το προηγουμενο, το αποθηκευει
        if (waveNumber > PlayerPrefs.GetInt("BestWave"))
        {
            PlayerPrefs.SetInt("BestWave", waveNumber);
        }
        gameOverText.text = $"You made it to <color=red>Wave {waveNumber}</color> !";
        yield return new WaitForSeconds(6f);
        Time.timeScale = 0.0f;
    }

    void CheckForDizz(string weaponTag)
    {
        switch (weaponTag)
        {
            case "LeftFist":
            case "RightFist":
                //αν ειναι το πρωτο hit στο διαστημα που εχω να χτυπησω, 
                // κανω το χρονο του πρωτου χτυπηματος ισο με τον πραγματικο χρονο
                if (currentFistHits == 0)
                {
                    firstFistHitTime = Time.time;
                }
                //στο πρωτο hit μπαινει παντα στην if
                //αν περασει ο χρονος που εχω για μπουνιες(10 δευτερολεπτα) τοτε θα
                //μπει στην else και θα ξαναρχισει το timer και θα αυξησει τα hit κατα 1
                if (Time.time - firstFistHitTime <= FistHitsTimer)
                {
                    currentFistHits++;
                }
                else
                {
                    firstFistHitTime = Time.time;
                    currentFistHits = 1;
                }
                break;
            case "MeleeWeapons":
                if (currentAxeHits == 0)
                {
                    firstAxeHitTime = Time.time;
                }
                if (Time.time - firstAxeHitTime <= AxeHitsTimer)
                {
                    currentAxeHits++;
                }
                else
                {
                    firstAxeHitTime = Time.time;
                    currentAxeHits = 1;
                }
                break;
            case "Pistol":
                if (currentPistolHits == 0)
                {
                    firstPistolHitTime = Time.time;
                }
                if (Time.time - firstPistolHitTime <= PistolHitsTimer)
                {
                    currentPistolHits++;
                }
                else
                {
                    firstPistolHitTime = Time.time;
                    currentPistolHits = 1;
                }
                break;
            case "Rifle":
                if (currentRifleHits == 0)
                {
                    firstRifleHitTime = Time.time;
                }
                if (Time.time - firstRifleHitTime <= RifleHitsTimer)
                {
                    currentRifleHits++;
                }
                else
                {
                    firstRifleHitTime = Time.time;
                    currentRifleHits = 1;
                }
                break;
        }
        if (canDizz)
        {
            UpdateDizzBar(weaponTag);
            //τσεκαρω αν εφτασε το απαραιτητα χτυπηματα
            if (currentFistHits >= fistHitsToDizz
            || currentAxeHits >= axeHitsToDizz
            || currentPistolHits >= pistolHitsToDizz
            || currentRifleHits >= rifleHitsToDizz)
            {
                canDizz = false;
                StartDizzyAnimation();
                //κανω το timer για το reset ισο με τον πραγματικο χρονο ωστε να 
                // μπορω να κανω dizz μετα απο τον χρονο που χρειαζεται ο εχθρος
                timeDizzedZombie = Time.time;
            }
        }
    }

    //Ανανεωνει την staminaBar του εχθρου αναλογα το οπλο που δεχτηκε ζημια
    void UpdateDizzBar(string weaponTag)
    {
        if (!isDeadZombie)
        {
            switch (weaponTag)
            {
                case "LeftFist":
                case "RightFist":
                    float fillPercentage = 1f - ((float)currentFistHits / fistHitsToDizz);
                    StartCoroutine(DecreaseFillAmount(fillPercentage));
                    break;
                case "MeleeWeapons":
                    float fillPercentage1 = 1f - ((float)currentAxeHits / axeHitsToDizz);
                    StartCoroutine(DecreaseFillAmount(fillPercentage1));
                    break;
                case "Pistol":
                    float fillPercentage2 = 1f - ((float)currentPistolHits / pistolHitsToDizz);
                    StartCoroutine(DecreaseFillAmount(fillPercentage2));
                    break;
                case "Rifle":
                    float fillPercentage3 = 1f - ((float)currentRifleHits / rifleHitsToDizz);
                    StartCoroutine(DecreaseFillAmount(fillPercentage3));
                    break;
            }
        }
    }

    //χρησιμοποιω IEnumerator για ομαλη αλλαγη της μπαρας
    IEnumerator DecreaseFillAmount(float targetFill)
    {
        float currentFill = StaminaBarUI.fillAmount;
        float elapsedTime = 0f;

        while (elapsedTime < 0.2f && !isDeadZombie)
        {
            float fillAmount = Mathf.Lerp(currentFill, targetFill, elapsedTime / 0.2f);
            StaminaBarUI.fillAmount = fillAmount;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        if (!isDeadZombie)
            StaminaBarUI.fillAmount = targetFill;
    }


    public void DealDamage(GameObject target, string tag, Vector3 contactPoint)
    {
        //απλα κραταω απο το string μονο το κομματι που λεει ποιο μερος του σωματος δεχτηκε ζημια ωστε να εφαρμοσω αναλογη ζημια
        string prefix = "mixamorig:";
        string bodyPart = target.name;

        if (bodyPart.StartsWith(prefix))
        {
            bodyPart = bodyPart[prefix.Length..];
        }

        //παιρνω τον συντελεστη αναλογα το σημειο σωματος που χτυπησε
        float multiplier = GetMultiplier(bodyPart);
        target = target.transform.root.gameObject;
        var atm = target.GetComponent<AttributesManager>();
        if (atm != null)
        {
            float totalFistDamage = fistAttack;
            float totalMeleeDamage = MeleeAttack;
            float totalPistolDamage = pistolAttack;
            float totalRifleDamage = rifleAttack;
            bool crit = CheckCrit();
            //Αν ειναι crit attack προσαρμοζω το damage
            if (crit)
            {
                totalFistDamage *= critFistAttack;
                totalMeleeDamage *= critMeleeAttack;
                totalPistolDamage *= critPistolAttack;
                totalRifleDamage *= critRifleAttack;
            }
            switch (tag)
            {
                case "LeftFist":
                    atm.TakeDamage((int)(totalFistDamage * multiplier), crit, tag, contactPoint, target);
                    break;
                case "RightFist":
                    atm.TakeDamage((int)(totalFistDamage * multiplier), crit, tag, contactPoint, target);
                    break;
                case "MeleeWeapons":
                    atm.TakeDamage((int)(totalMeleeDamage * multiplier), crit, tag, contactPoint, target);
                    break;
                case "Pistol":
                    atm.TakeDamage((int)(totalPistolDamage * multiplier), crit, tag, contactPoint, target);
                    break;
                case "Rifle":
                    atm.TakeDamage((int)(totalRifleDamage * multiplier), crit, tag, contactPoint, target);
                    break;
            }
        }
    }

    public float GetPlayerHealth()
    {
        return health;
    }

    //απλη συναρτηση που επιστρεφει bool αν εκανε καιρια ζημια ή οχι
    private bool CheckCrit()
    {
        if (Random.Range(0f, 1f) < critChance)
            return true;
        else
            return false;
    }

    //αναλογα το σημειο σωματος, δεχεται διαφορετικη ζημια
    private float GetMultiplier(string bodyPart)
    {
        float multiplier = 1.0f;
        if (bodyPart.Contains("Head"))
        {
            return 1.8f;
        }
        else if (bodyPart.Contains("Leg"))
        {
            return 0.8f;
        }
        else if (bodyPart.Contains("Arm"))
        {
            return 0.7f;
        }
        else if (bodyPart.Contains("Spine"))
        {
            return 1.2f;
        }
        else if (bodyPart.Contains("Hips"))
        {
            return 1.3f;
        }
        else
        {
            return multiplier;
        }
    }
}
