using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager current;
    [SerializeField] GameObject copZombie, crawlerZombie, bigZombie, bossZombie;
    [SerializeField] GameObject ammoBox, axe, pistol, rifle, health;
    [SerializeField] TextMeshProUGUI startingWaveText, waveNumberText;
    [SerializeField] Transform[] spawnPoints;
    List<Transform> availableSpawnPoints = new List<Transform>();
    int currentWave, enemiesRemaining = 0;
    float baseEnemyStrength = 1.0f;
    public bool shouldDestroyAmmo = true, shouldDestroyWeapon = true;

    private void Awake()
    {
        current = this;
    }

    private void Start()
    {
        currentWave = 1;
    }

    public int GetCurrentWave()
    {
        return currentWave;
    }

    //τρεχει με το που κλεισουν τα instructions
    public void StartFadeStartingText()
    {
        //μετα απο 4 δευτερολεπτα κανει fade το αρχικο κειμενο
        StartCoroutine(FadeText(4.0f, startingWaveText, 2.0f));
        //ξεκιναει τα waves απο το 1 που εχει αρχικοποιηθει
        Invoke("ShowWaveText", 6.0f);
    }

    IEnumerator FadeText(float delayTime, TextMeshProUGUI text, float fadeTime)
    {
        //αρχικη καθυστερηση αν θελω
        yield return new WaitForSeconds(delayTime);
        Color originalColor = text.color;
        Color targetColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        float startTime = Time.time;
        //χρονος που κανει smooth fade out
        while (Time.time < startTime + fadeTime)
        {
            float t = (Time.time - startTime) / fadeTime;
            text.color = Color.Lerp(originalColor, targetColor, t);
            yield return null;
        }
        text.color = targetColor;
        text.gameObject.SetActive(false);
    }

    void WaveHandling()
    {
        //τα πρωτα 10 κυματα ειναι fixed, μετα ειναι στανταρ που απλα δυναμωνουν οι εχθροι
        if (currentWave <= 10)
        {
            switch (currentWave)
            {
                case 1:
                    StartCoroutine(SpawnZombie(baseEnemyStrength, copZombie, 1));
                    break;
                case 2:
                    StartCoroutine(SpawnZombie(baseEnemyStrength, copZombie, 2));
                    break;
                case 3:
                    StartCoroutine(SpawnZombie(baseEnemyStrength, copZombie, 3));
                    SpawnWeapon(axe, 120f);
                    StartHealthInterval();
                    break;
                case 4:
                    StartCoroutine(SpawnZombie(baseEnemyStrength, copZombie, 6));
                    break;
                case 5:
                    StartCoroutine(SpawnZombie(baseEnemyStrength, copZombie, 2));
                    StartCoroutine(SpawnZombie(baseEnemyStrength, crawlerZombie, 2));
                    StartCoroutine(SpawnZombie(baseEnemyStrength, bossZombie, 1));
                    SpawnWeapon(pistol, 120f);
                    StartAmmoInterval();
                    break;
                case 6:
                    StartCoroutine(SpawnZombie(baseEnemyStrength, copZombie, 8));
                    StartCoroutine(SpawnZombie(baseEnemyStrength, crawlerZombie, 2));
                    break;
                case 7:
                    StartCoroutine(SpawnZombie(baseEnemyStrength, copZombie, 6));
                    StartCoroutine(SpawnZombie(baseEnemyStrength, crawlerZombie, 1));
                    StartCoroutine(SpawnZombie(baseEnemyStrength, bigZombie, 1));
                    SpawnWeapon(rifle, 120f);
                    break;
                case 8:
                    StartCoroutine(SpawnZombie(baseEnemyStrength, copZombie, 8));
                    StartCoroutine(SpawnZombie(baseEnemyStrength, crawlerZombie, 3));
                    StartCoroutine(SpawnZombie(baseEnemyStrength, bigZombie, 2));
                    break;
                case 9:
                    StartCoroutine(SpawnZombie(baseEnemyStrength, copZombie, 10));
                    StartCoroutine(SpawnZombie(baseEnemyStrength, crawlerZombie, 5));
                    StartCoroutine(SpawnZombie(baseEnemyStrength, bigZombie, 3));
                    break;
                case 10:
                    StartCoroutine(SpawnZombie(baseEnemyStrength, copZombie, 10));
                    StartCoroutine(SpawnZombie(baseEnemyStrength, crawlerZombie, 10));
                    StartCoroutine(SpawnZombie(baseEnemyStrength, bigZombie, 5));
                    StartCoroutine(SpawnZombie(baseEnemyStrength, bossZombie, 1));
                    break;
            }
        }
        else
        {
            //μετα το wave 10 βγαινουν και τα 3 ειδη εχθρων με random αριθμο(8-13) και σε καθε wave αυξανετε η δυναμη τους(ζωη και ζημια) κατα 0.1
            int numEnemies = Random.Range(8, 13);
            float enemyStrength = (float)(baseEnemyStrength + (currentWave - 10) * 0.1);
            StartCoroutine(SpawnZombie(enemyStrength, copZombie, numEnemies));
            StartCoroutine(SpawnZombie(enemyStrength, crawlerZombie, numEnemies));
            StartCoroutine(SpawnZombie(enemyStrength, bigZombie, numEnemies - 5));
            //καθε 5 wave συνεχιζει να βγαινει και ενας αρχηγος ζομπιε
            if (currentWave % 5 == 0)
            {
                StartCoroutine(SpawnZombie(enemyStrength, bossZombie, 1));
                //υπαρχει και 30% πιθανοτητα να κανει spawn πιστολακι ή 20% rifle
                if (Random.Range(0, 100) < 30)
                {
                    SpawnWeapon(pistol, 60f);
                }
                if (Random.Range(0, 100) < 20)
                {
                    SpawnWeapon(rifle, 60f);
                }
            }
        }
    }

    public void KillOneEnemy()
    {
        enemiesRemaining--;
        if (enemiesRemaining <= 0)
        {
            currentWave++;
            ShowWaveText();
        }
    }

    //καθε 160 με 200 δευτερολεπτα(μετα που καταστραφει ή παρω το αντικειμενο) 
    // θα κανει spawn ζωη που θα μενει ενεργη για 120 δευτερολεπτα ωστε να παρει ο παιχτης
    void StartHealthInterval()
    {
        float randomDelay = Random.Range(160f, 200f);
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Vector3 spawnPoint = spawnPoints[randomIndex].position;
        //απλα για να κανει spawn λιγο πιο πανω
        spawnPoint += Vector3.up;
        GameObject healthClone = Instantiate(health, spawnPoint, Quaternion.identity);
        Destroy(healthClone, 120f);

        Invoke("StartHealthInterval", randomDelay);
    }
    //καθε 160 με 200 δευτερολεπτα(μετα που καταστραφει ή παρω το αντικειμενο) θα κανει 
    // spawn σφαιρες που θα μενουν ενεργες για 120 δευτερολεπτα ωστε να παρει ο παιχτης
    void StartAmmoInterval()
    {
        shouldDestroyAmmo = true;
        float randomDelay = Random.Range(160f, 200f);
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomIndex];
        GameObject ammoClone = Instantiate(ammoBox, spawnPoint.position, Quaternion.identity);
        //αν πατησω να το πιασει τοτε δεν θα διαγραφει μετα απο λιγο χρονο γιατι πλεον θα το εχω παρει
        StartCoroutine(DestroyAmmo(ammoClone, 120f));

        Invoke("StartAmmoInterval", randomDelay);
    }

    //bug-fix. Αν πατησω να παρει σφαιρες ή οπλο και οσο πηγαινε καταστραφουν υπηρχε bug οποτε με μια 
    //απλη bool που γινεται false αν πατησω να τα παρει τοτε θα τα παρει και δεν θα τα καταστρεψει
    IEnumerator DestroyAmmo(GameObject ammoClone, float timeToDestroy)
    {
        yield return new WaitForSeconds(timeToDestroy);

        if (shouldDestroyAmmo)
        {
            Destroy(ammoClone);
        }
    }

    //χρησιμοποιω IEnumerator για να βαλω μια μικρη καθυυστερηση 0.2 δευτερολεπτα πριν καθε spawn για να μην βγαινουν ολα μαζι
    IEnumerator SpawnZombie(float multiplier, GameObject zombiePrefab, int numberOfSpawns)
    {
        enemiesRemaining += numberOfSpawns;
        availableSpawnPoints.Clear();
        availableSpawnPoints.AddRange(spawnPoints);
        for (int i = 0; i < numberOfSpawns; i++)
        {
            if (availableSpawnPoints.Count > 0)
            {
                int randomIndex = Random.Range(0, availableSpawnPoints.Count);
                Transform spawnPoint = availableSpawnPoints[randomIndex];

                GameObject clone = Instantiate(zombiePrefab, spawnPoint.position, Quaternion.identity);
                clone.GetComponent<AttributesManager>().BuffZombies(multiplier);
                availableSpawnPoints.RemoveAt(randomIndex);
            }
            else
            {
                availableSpawnPoints.AddRange(spawnPoints);
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    void SpawnWeapon(GameObject weapon, float destroyTime)
    {
        shouldDestroyWeapon = true;
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomIndex];
        GameObject weaponClone = Instantiate(weapon, spawnPoint.position, weapon.transform.rotation);
        //αν πατησω να το πιασει τοτε δεν θα διαγραφει μετα απο λιγο χρονο γιατι πλεον θα το εχω παρει
        StartCoroutine(DestroyWeapon(weaponClone, destroyTime));
    }
    IEnumerator DestroyWeapon(GameObject weapon, float timeToDestroy)
    {
        yield return new WaitForSeconds(timeToDestroy);
        if (shouldDestroyWeapon)
        {
            Destroy(weapon);
        }
    }



    //δειχνει σε ποιο κυμα ειμαστε, το κανει fade out και ξεκιναει το waveHandling
    void ShowWaveText()
    {
        //επαναφερω το alpha(transparent) value για το επομενο wave
        waveNumberText.color = new Color(waveNumberText.color.r, waveNumberText.color.g, waveNumberText.color.b, 1f);
        waveNumberText.text = $"ΚYΜΑ {currentWave}";
        waveNumberText.gameObject.SetActive(true);
        StartCoroutine(FadeText(3.0f, waveNumberText, 1.0f));
        Invoke("WaveHandling", 4.0f);
    }
}
