using UnityEngine;
using System.Linq;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class Ragdoll : MonoBehaviour
{
    GameObject player;
    [SerializeField] float forceMagnitude;
    Rigidbody[] ragdollRigidbodies;
    Collider[] colList;
    Animator animator;
    CharacterController characterController;
    NavMeshAgent navMeshAgent;
    List<Material> newMaterials;
    float smooth = 0;
    bool visible = true;
    float secondsToDesolve = 4f;
    bool dead = false;
    Rigidbody head;

    //παιρνω οτι χρειαζομαι για αργοτερα
    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        colList = transform.GetComponentsInChildren<Collider>();
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        if (!gameObject.CompareTag("Player"))
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            CreateNewMaterials();
        }
        DisableRagdoll();
    }

    private void Update()
    {
        //οταν πεθανει καποιος εχθρος κανει dissolve ομαλα αλλαζοντας την τιμη του float
        //στο material που εχουν για dissolve και αφου τελειωσει κανουμε destroy
        if (visible && dead)
        {
            smooth += Time.deltaTime;
            foreach (Material material in newMaterials)
            {
                material.SetFloat("_Dissolve", Mathf.Lerp(0, 1f, smooth / secondsToDesolve));
            }
            if (smooth > secondsToDesolve)
            {
                smooth = 0;
                visible = false;
                Destroy(gameObject);
            }
        }
    }

    private void CreateNewMaterials()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        newMaterials = new List<Material>();
        foreach (Renderer renderer in renderers)
        {
            foreach (Material material in renderer.materials)
            {
                if (!newMaterials.Contains(material))
                {
                    newMaterials.Add(material);
                }
            }
        }
    }

    //για καθε rigidbody που εχει, τα κανω kinematic ωστε να μην χρησιμοποιουν την βαρυτητα ακομα
    public void DisableRagdoll()
    {
        foreach (var rigidbody in ragdollRigidbodies)
        {
            //δεν πειραζω τα 2 collider που εχω στα χερια για τις μπουνιες
            if (rigidbody.tag == "LeftFist" || rigidbody.tag == "RightFist") continue;
            rigidbody.isKinematic = true;
            animator.enabled = true;
            characterController.enabled = true;
            if (!gameObject.CompareTag("Player"))
            {
                navMeshAgent.enabled = true;
            }
            //κραταω το rigidbody του κεφαλιου για bug-fix μετα
            if (rigidbody.name == "mixamorig:Head")
            {
                head = rigidbody;

            }
        }
    }

    //κανω ολα τα rigidbody να χρησιμοποιουν την βαρυτητα ωστε να "πεσουν" σαν ragdoll
    public void EnableRagdoll(Vector3 hitpoint)
    {
        animator.enabled = false;
        characterController.enabled = false;
        if (!gameObject.CompareTag("Player"))
        {
            navMeshAgent.enabled = false;
        }
        SetAllChildLayers(transform, "Ragdoll");
        foreach (var rigidbody in ragdollRigidbodies)
        {
            rigidbody.isKinematic = false;
            //αυτη η γραμμη ειναι για τον παιχτη, οταν πεθανει καταστρεφω τα rigidbody στα χερια ωστε να μην τα επηρεαζει
            if (rigidbody.gameObject.layer == 9 && (rigidbody.CompareTag("LeftFist") || rigidbody.CompareTag("RightFist")))
            {
                Destroy(rigidbody);
            }
        }
        foreach (var item in colList)
        {
            item.isTrigger = false;
        }
        //εχω κρατησει το hitpoint δηλαδη που εφαγε τελευταια damage ωστε μετα να κανω apply force στο σημειο που χτυπηθηκε και να πεσει προς αυτη την κατευθυνση
        Rigidbody hitRigidbody = ragdollRigidbodies.OrderBy(rigidbody => Vector3.Distance(rigidbody.position, hitpoint)).First();
        Vector3 forceDirection = this.transform.position - player.transform.position;
        forceDirection.y = 1.0f;
        forceDirection.Normalize();
        Vector3 force = forceMagnitude * forceDirection;
        hitRigidbody.AddForceAtPosition(force, hitpoint, ForceMode.Impulse);
        //δεν θελουμε να εξαφανιστει ο παιχτης
        if (!gameObject.CompareTag("Player"))
        {
            //αν ειναι εχθρος ξεκιναει το dissolve μετα απο 4 δευτερολεπτα
            StartCoroutine(DissolveAfterTime(4.0f));
            StartCoroutine(FreezeHeadFix(2.0f));
        }
    }

    //γινεται το dead = true το οποιο χρησιμοποιειται στην update
    private IEnumerator DissolveAfterTime(float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        dead = true;
    }

    //bug-fix για το κεφαλι που δεν εφαρμοζε καλα οποτε μετα απο λιγο το περιοριζουμε σε ολες τις κινησεις
    private IEnumerator FreezeHeadFix(float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        head.constraints = RigidbodyConstraints.FreezeAll;
    }

    //Η αναδρομική συνάρτηση που αλλάζει όλα τα layers του ζόμπι σε ragdoll ώστε να μην υπάρχουν πια colliders με τον παίχτη όταν πεθάνει
    void SetAllChildLayers(Transform parentTransform, string layerName)
    {
        parentTransform.gameObject.layer = LayerMask.NameToLayer(layerName);
        foreach (Transform childTransform in parentTransform)
        {
            SetAllChildLayers(childTransform, layerName);
        }
    }
}
