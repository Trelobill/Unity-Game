using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.Animations.Rigging;
using System.Collections.Generic;

public class ThirdPersonShooterController : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] float normalSensitivity, aimSensitivity;
    [SerializeField] LayerMask aimColliderLayerMask = new();
    [SerializeField] Transform hitPointWorld;
    [SerializeField] Transform spawnBulletPositionPistol, spawnBulletPositionRifle;
    [SerializeField] GameObject crosshair;
    [SerializeField] LineRenderer bulletTrail;
    [SerializeField] GameObject[] decals;
    [SerializeField] GameObject obstacleDecal;
    [SerializeField] Animator pistolAnimator, rifleAnimator;
    [SerializeField] Rig aimPistolRig, riflePistolRig;
    [SerializeField] ParticleSystem muzzleFlashPistol, muzzleFlashRifle;
    [SerializeField] ParticleSystem hitEffect;
    [SerializeField] GameObject emptyPistolMagazine, fullPistolMagazine, parentEmptyPistolMagazine, parentFullPistolMagazine;
    [SerializeField] GameObject emptyRifleMagazine, fullRifleMagazine, parentEmptyRifleMagazine, parentFullRifleMagazine;
    GameObject fullMagazinePistol, emptyMagazineRifle, fullMagazineRifle, actualMagazineRifle;
    int zombiesLayer;
    StarterAssetsInputs starterAssetsInputs;
    ThirdPersonController thirdPersonController;
    AttributesManager atm;
    Animator animator;
    [SerializeField] GameObject bloodEffect;
    CinemachineImpulseSource impulse;
    RaycastHit cameraRaycastHit, pistolRaycastHit, rifleRaycastHit;
    Ray cameraRay, pistolRay, rifleRay;
    bool canReload = true;
    float aimPistolRigWeight, aimRifleRigWeight;
    [SerializeField] bool canSpawnDecal = false, canAim = true;
    float smoothFactor = 5f;
    float pistolFireRate = 0.1f, rifleFireRate = 0.0833f, nextFire = 0.0f;
    private void Awake()
    {
        atm = GetComponent<AttributesManager>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        thirdPersonController = GetComponent<ThirdPersonController>();
        animator = GetComponent<Animator>();
        impulse = transform.GetComponent<CinemachineImpulseSource>();
        zombiesLayer = LayerMask.GetMask("Zombie");
    }
    private void Update()
    {
        //οριζουμε τα βαρη για τα rigs αναλογα τις τιμες, χρησιμοποιω lerp για να γινει smooth
        aimPistolRig.weight = Mathf.Lerp(aimPistolRig.weight, aimPistolRigWeight, Time.deltaTime * 20f);
        riflePistolRig.weight = Mathf.Lerp(riflePistolRig.weight, aimRifleRigWeight, Time.deltaTime * 20f);
        Vector3 mouseWorldPosition = Vector3.zero;
        Vector2 screenCenterPoint = new(Screen.width / 2f, Screen.height / 2f);
        //ray που δειχνει απο το κεντρο της οθονης στο σημειο που χτυπησε
        cameraRay = Camera.main.ScreenPointToRay(screenCenterPoint);
        //αν χτυπησει κατι
        if (Physics.Raycast(cameraRay, out cameraRaycastHit, 999f, aimColliderLayerMask))
        {
            //παιρνω το σημειο που χτυπησε σε vector3
            mouseWorldPosition = cameraRaycastHit.point;
            //αφου εχω παρει το σημειο σε world position, τωρα δημιουργω αλλο raycast που ξεκιναει απο το πιστολακι και δειχνει εκει που χτυπησε το προηγουμενο raycast
            pistolRay = new Ray(spawnBulletPositionPistol.position, (cameraRaycastHit.point - spawnBulletPositionPistol.position).normalized);
            //αντιστοιχα για το rifle
            rifleRay = new Ray(spawnBulletPositionRifle.position, (cameraRaycastHit.point - spawnBulletPositionRifle.position).normalized);
        }
        //αν δεν σημαδευει καποιο collider απλα βαραει οπου βλεπει σε 1000 μοναδες μακρια
        else
        {
            hitPointWorld.position = cameraRay.GetPoint(1000);
            mouseWorldPosition = cameraRay.GetPoint(1000);
            //αφου εχω παρει το σημειο σε world position, τωρα δημιουργω αλλο raycast που ξεκιναει απο το πιστολακι και δειχνει εκει που χτυπησε το προηγουμενο raycast
            pistolRay = new Ray(spawnBulletPositionPistol.position, (cameraRaycastHit.point - spawnBulletPositionPistol.position).normalized);
            //αντιστοιχα για το rifle
            rifleRay = new Ray(spawnBulletPositionRifle.position, (cameraRaycastHit.point - spawnBulletPositionRifle.position).normalized);
        }
        //αν το raycast απο το πιστολακι χτυπησει κατι
        if (Physics.Raycast(pistolRay, out pistolRaycastHit, 999f, aimColliderLayerMask))
        {
            hitPointWorld.position = Vector3.Lerp(hitPointWorld.position, pistolRaycastHit.point, Time.deltaTime * smoothFactor);
            //παιρνω το σημειο που χτυπησε
            mouseWorldPosition = pistolRaycastHit.point;
            //κραταω αν χτυπησε το περιβαλλον για μετα ωστε να ξερω αν θα κανω spawn το decal ή οχι
            if (pistolRaycastHit.transform.tag == "Environment")
            {
                canSpawnDecal = true;
            }
            else
            {
                canSpawnDecal = false;
            }
        }
        //αν το raycast απο το rifle χτυπησει κατι
        if (Physics.Raycast(rifleRay, out rifleRaycastHit, 999f, aimColliderLayerMask))
        {
            hitPointWorld.position = Vector3.Lerp(hitPointWorld.position, rifleRaycastHit.point, Time.deltaTime * smoothFactor);
            //παιρνω το σημειο που χτυπησε
            mouseWorldPosition = rifleRaycastHit.point;
            if (rifleRaycastHit.transform.tag == "Environment")
            {
                canSpawnDecal = true;
            }
            else
            {
                canSpawnDecal = false;
            }
        }

        //αν βαρεσουμε με μπουνιες ή τσεκουρι, κραταμε κατι απο τα 2, ειναι grounded και μπορει να βαρεσει
        if (starterAssetsInputs.meleeAttack && (WeaponManager.current.CheckWeaponState("Melee") || WeaponManager.current.CheckWeaponState("Fists")) && animator.GetBool("Grounded") && thirdPersonController.CanAttack())
        {
            animator.SetTrigger("Attack");
            starterAssetsInputs.meleeAttack = false;
        }
        //αν κανουμε aim με πιστολακι, ειναι grounded και μπορει να σημαδεψει
        if (starterAssetsInputs.aim && animator.GetBool("Grounded") && WeaponManager.current.CheckWeaponState("Pistol") && canAim)
        {
            //αλλαζουμε το βαρος για το rig απο το πιστολακι
            aimPistolRigWeight = 1f;
            //ενεργοποιω το UI του crosshair
            crosshair.SetActive(true);
            //αλλαζω την καμερα στην aim
            aimVirtualCamera.gameObject.SetActive(true);
            //αλλαζω και το sensitivity
            thirdPersonController.SetSensitivity(aimSensitivity);
            //απενεργοποιω προσωρινο το rotate μεσω το thirdPersonController και το χειριζομαι παρακατω
            thirdPersonController.SetRotateOnMove(false);
            //στο layer που ειναι το animation του pistol aim, αυξανω το βαρος smooth σε 1 ωστε να παιζει το animation του
            animator.SetLayerWeight(2, Mathf.Lerp(animator.GetLayerWeight(2), 1f, Time.deltaTime * 10f));
            CheckIfObstacle(pistolRaycastHit);

            //ο χαρακτηρας στριβει smooth οπου στοχευει
            mouseWorldPosition.y = transform.position.y;
            Vector3 aimDirection = (mouseWorldPosition - transform.position).normalized;
            transform.forward = Vector3.Lerp(aimDirection, transform.forward, Time.deltaTime * smoothFactor);
        }
        //αντιστοιχα αν κανουμε aim με το rifle
        else if (starterAssetsInputs.aim && animator.GetBool("Grounded") && WeaponManager.current.CheckWeaponState("Rifle") && canAim)
        {
            aimRifleRigWeight = 1f;
            crosshair.SetActive(true);
            aimVirtualCamera.gameObject.SetActive(true);
            thirdPersonController.SetSensitivity(aimSensitivity);
            thirdPersonController.SetRotateOnMove(false);
            animator.SetLayerWeight(3, Mathf.Lerp(animator.GetLayerWeight(3), 1f, Time.deltaTime * 10f));
            CheckIfObstacle(rifleRaycastHit);

            //ο χαρακτηρας στριβει οπου στοχευει
            mouseWorldPosition.y = transform.position.y;
            Vector3 aimDirection = (mouseWorldPosition - transform.position).normalized;
            transform.forward = Vector3.Lerp(aimDirection, transform.forward, Time.deltaTime * smoothFactor);
        }
        //αν δεν κανουμε aim
        else
        {
            obstacleDecal.SetActive(false);
            aimPistolRigWeight = 0f;
            aimRifleRigWeight = 0f;
            crosshair.SetActive(false);
            aimVirtualCamera.gameObject.SetActive(false);
            thirdPersonController.SetSensitivity(normalSensitivity);
            thirdPersonController.SetRotateOnMove(true);
            animator.SetLayerWeight(2, Mathf.Lerp(animator.GetLayerWeight(2), 0f, Time.deltaTime * 10f));
            animator.SetLayerWeight(3, Mathf.Lerp(animator.GetLayerWeight(3), 0f, Time.deltaTime * 10f));
        }

        //αν δεν στοχευουμε και πυροβολησουμε κανουμε reset το shoot ωστε να μην μεινει true
        if (!starterAssetsInputs.aim && starterAssetsInputs.shoot && (WeaponManager.current.CheckWeaponState("Pistol") || (WeaponManager.current.CheckWeaponState("Rifle"))))
        {
            starterAssetsInputs.shoot = false;
            obstacleDecal.SetActive(false);
        }
        //αν στοχευουμε και πυροβολησουμε
        if (starterAssetsInputs.aim && starterAssetsInputs.shoot && animator.GetBool("Grounded") && canAim)
        {
            //αν πυροβολησω με πιστολακι και εχει περασει το διαστημα που εχω ως περιορισμο
            if (WeaponManager.current.CheckWeaponState("Pistol") && Time.time > nextFire)
            {
                //αν εχει σφαιρες
                if (PistolHandler.current.HasAmmo())
                {
                    //ελκυει τους εχθρους σε αποσταση ιση με 200
                    AttractZombiesWithSound(200);
                    //καταναλωνει μια σφαιρα
                    PistolHandler.current.ConsumeAmmo();
                    //δημιουργει ενα impulse στην καμερα
                    impulse.GenerateImpulse(1);
                    //οριζει το χρονο για το επομενο δυνατο shoot
                    nextFire = Time.time + pistolFireRate;
                    //παιζει το animation του pistol shoot
                    pistolAnimator.SetTrigger("Shoot");
                    //δημιουργει το bullettrail της σφαιρας
                    SpawnBulletTrail(pistolRaycastHit.point, spawnBulletPositionPistol);
                    //παιζει το particle system του muzzle flash
                    muzzleFlashPistol.Play();
                    //αναλυεται παρακατω
                    SpawnDecal(pistolRaycastHit, "Pistol");
                    //αρχικοποιουμε το shoot
                    starterAssetsInputs.shoot = false;
                }
                //αν δεν εχει σφαιρες απλα παιζει τον ηχο για αδειο οπλο
                else
                {
                    PistolHandler.current.PlayEmptySound();
                    starterAssetsInputs.shoot = false;
                }
            }
            //αν βαρεσω πιο γρηγορα απο οτι πρεπει δεν κανει τιποτα
            else if (WeaponManager.current.CheckWeaponState("Pistol") && Time.time < nextFire)
            {
                starterAssetsInputs.shoot = false;
            }
            //αν πυροβολησω με αυτοματο αντιστοιχο με πιστολακι
            if (WeaponManager.current.CheckWeaponState("Rifle") && Time.time > nextFire)
            {
                if (RifleHandler.current.HasAmmo())
                {
                    AttractZombiesWithSound(400);
                    RifleHandler.current.ConsumeAmmo();
                    impulse.GenerateImpulse(1);
                    nextFire = Time.time + rifleFireRate;
                    rifleAnimator.SetTrigger("Shoot");
                    SpawnBulletTrail(rifleRaycastHit.point, spawnBulletPositionRifle);
                    muzzleFlashRifle.Play();
                    SpawnDecal(rifleRaycastHit, "Rifle");
                }
                else
                {
                    RifleHandler.current.PlayEmptySound();
                    starterAssetsInputs.shoot = false;
                }
            }
        }
        //αν δεν μπορουμε να πυροβολησουμε κανουμε reset το shoot που εχει αποθηκευτει
        else
        {
            starterAssetsInputs.shoot = false;
        }

        //αν πατησουμε για reload και μπορουμε να κανουμε(δηλαδη δεν ειμαστε σε καποιο transition οπλου ή ειμαστε στον αερα)
        if (starterAssetsInputs.reload && animator.GetBool("Grounded") && canReload)
        {
            //αν εχω πιστολακι και μπορω να κανω reload(δηλαδη εχω σφαιρες μεσα)
            if (WeaponManager.current.CheckWeaponState("Pistol") && PistolHandler.current.CanReload())
            {
                //κανω reload
                PistolHandler.current.ReloadAmmo();
                //παιζω τον ηχο του reload
                PistolHandler.current.PlayReloadSound();
                //παιζω το animation του reload
                animator.Play("reloadPistol");
                //αν ημουν σε aim το βγαζω και αρχικοποιω το reload
                starterAssetsInputs.aim = false;
                starterAssetsInputs.reload = false;
            }
            //αντιστοιχα για το rifle
            else if (WeaponManager.current.CheckWeaponState("Rifle") && RifleHandler.current.CanReload())
            {
                RifleHandler.current.ReloadAmmo();
                RifleHandler.current.PlayReloadSound();
                animator.Play("reloadRifle");
                starterAssetsInputs.aim = false;
                starterAssetsInputs.reload = false;
            }
            else
            {
                starterAssetsInputs.reload = false;
            }
        }
    }

    private void AttractZombiesWithSound(int zombiesHearSound)
    {
        //βρισκω ολους τους εχθρους μεσω overlapsphere για καλυτερο optimize
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, zombiesHearSound, zombiesLayer);
        List<GameObject> enemiesToHit = new();

        for (int i = 0; i < enemiesInRange.Length; i++)
        {
            enemiesToHit.Add(enemiesInRange[i].gameObject);
        }
        //για καθε εχθρο που βρηκαμε, παιζει το animation του hearSound και ξεκιναει να κυνηγαει τον παιχτη
        foreach (GameObject enemy in enemiesToHit)
        {
            enemy.GetComponent<Animator>().SetBool("HearSounds", true);
        }
    }

    //επειδη εχουμε TPS controller μπορει το raycast της καμερας να χτυπαει σε 
    //διαφορετικο σημειο απο οτι του οπλου οποτε βαζουμε ενα decal "X" πανω στο εμποδιο
    private void CheckIfObstacle(RaycastHit hit)
    {
        if (!hit.collider) return;
        //μονο αν ειναι πανω σε περιβαλλον, κανουμε aim και το object που χτυπαει η καμερα ειναι διαφορετικο απο του οπλου
        if (starterAssetsInputs.aim && cameraRaycastHit.transform.name != hit.transform.name && hit.transform.CompareTag("Environment"))
        {
            obstacleDecal.transform.position = hit.point;
            obstacleDecal.transform.forward = -hit.normal;
            obstacleDecal.SetActive(true);
        }
        else
        {
            obstacleDecal.SetActive(false);
        }
    }

    private void SpawnDecal(RaycastHit hitPoint, string weaponName)
    {
        var hitGameobject = hitPoint.transform.gameObject;
        Vector3 directionToPlayer = (atm.transform.position - hitPoint.transform.position).normalized;
        Vector3 centerContactPoint = hitPoint.collider.bounds.center;
        Quaternion rotation = Quaternion.LookRotation(directionToPlayer);
        rotation.eulerAngles = new Vector3(0, rotation.eulerAngles.y, rotation.eulerAngles.z);
        if (hitGameobject.layer == 8)
        {
            Animator enemyAnim = hitGameobject.GetComponentInParent<Animator>();
            enemyAnim.SetBool("isPatrolling", false);
            enemyAnim.SetBool("isChasing", true);
            enemyAnim.Play("damageZombie");
            atm.DealDamage(hitGameobject, weaponName, hitPoint.point);
            GameObject blood = Instantiate(bloodEffect, centerContactPoint, rotation);
            blood.transform.SetParent(hitGameobject.transform);
            Destroy(blood, 2);
        }
        // αν εκανε κατι hit
        if (canSpawnDecal)
        {
            //παιρνω ενα random decal
            int randomDecal = (int)Random.Range(0f, decals.Length);
            //το δημιουργω στην θεση που εκανε hit το raycast
            GameObject decal = Instantiate(decals[randomDecal], hitPoint.point, Quaternion.identity);
            ParticleSystem hitEffectParticle = Instantiate(hitEffect, hitPoint.point, Quaternion.identity);
            //το βαζω με κατευθυνση αντιθετη του normal του raycast
            decal.transform.forward = -hitPoint.normal;
            hitEffectParticle.transform.forward = hitPoint.normal;
            hitEffectParticle.Emit(1);
            //το καταστρεφω 3 δευτερολεπτα μετα
            Destroy(decal, 3f);
            Destroy(hitEffectParticle.gameObject, hitEffectParticle.main.duration);
        }
    }

    //οι παρακατω συναρτησεις ειναι για χρηση animation event
    private void DisableAim()
    {
        canAim = false;
    }
    private void EnableAim()
    {
        canAim = true;
    }


    private void DropPistolMagazine()
    {
        GameObject emptyMagazinePistol = Instantiate(emptyPistolMagazine, parentEmptyPistolMagazine.transform);
        emptyMagazinePistol.transform.SetParent(null);
        Destroy(emptyMagazinePistol, 6.0f);
    }
    private void PutPistolMagazine()
    {
        fullMagazinePistol = Instantiate(fullPistolMagazine, parentFullPistolMagazine.transform);
    }
    private void DestroyPistolMagazine()
    {
        Destroy(fullMagazinePistol);
    }
    private void CatchRifleMagazine()
    {
        actualMagazineRifle = GameObject.FindGameObjectWithTag("RifleMagazine");
        actualMagazineRifle.SetActive(false);
        emptyMagazineRifle = Instantiate(emptyRifleMagazine, parentEmptyRifleMagazine.transform);
    }
    private void DropRifleMagazine()
    {
        Rigidbody rbRifleMagazine = emptyMagazineRifle.GetComponent<Rigidbody>();
        rbRifleMagazine.useGravity = true;
        rbRifleMagazine.transform.SetParent(null);

        rbRifleMagazine.AddForce(rbRifleMagazine.transform.TransformDirection(0.1f, -1, 0) * 2f, ForceMode.Impulse);
        Destroy(emptyMagazineRifle, 6.0f);
    }
    private void PutRifleMagazine()
    {
        fullMagazineRifle = Instantiate(fullRifleMagazine, parentFullRifleMagazine.transform);
    }
    private void DestroyRifleMagazine()
    {
        Destroy(fullMagazineRifle);
        actualMagazineRifle.SetActive(true);
    }

    public void enableReload()
    {
        canReload = true;
    }
    public void disableReload()
    {
        canReload = false;
    }
    private void SpawnBulletTrail(Vector3 hitPoint, Transform bulletStartPoint)
    {
        GameObject bulletTrailEffect = Instantiate(bulletTrail.gameObject, bulletStartPoint.position, Quaternion.identity);
        LineRenderer lineR = bulletTrailEffect.GetComponent<LineRenderer>();
        lineR.SetPosition(0, bulletStartPoint.position);
        lineR.SetPosition(1, hitPoint);
        Destroy(bulletTrailEffect, 1f);
    }
}
