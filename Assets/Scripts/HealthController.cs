using System.Collections;
using UnityEngine;
using TMPro;

public class HealthController : MonoBehaviour
{
    [SerializeField] Transform healthBarTransform;
    [SerializeField] TMP_Text health_text;
    float minScale = 1f;
    float maxScale = 0.8f;
    float healthThreshold1 = 20f;
    float healthThreshold2 = 50f;
    float animationSpeed1 = 5f;
    float animationSpeed2 = 2f;
    float currentHealth;
    bool animating = false;
    AttributesManager atm;

    private void Start()
    {
        atm = GetComponent<AttributesManager>();
    }

    void Update()
    {
        currentHealth = GetHealth();
        if (currentHealth > 1)
            health_text.text = currentHealth.ToString();
        else
            health_text.text = "0";

        if (!animating)
        {
            //αν ειναι πιο κατω απο 20 χτυπαει πολυ γρηγορα
            if (currentHealth < healthThreshold1)
            {
                StartCoroutine(ScaleAnimation(animationSpeed1));
            }
            //κατω απο 50 λιγο πιο αργα
            else if (currentHealth < healthThreshold2)
            {
                StartCoroutine(ScaleAnimation(animationSpeed2));
            }
        }
    }

    IEnumerator ScaleAnimation(float animationSpeed)
    {
        animating = true;

        while (GetHealth() < healthThreshold1 || GetHealth() < healthThreshold2)
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * animationSpeed;
                float scale = Mathf.Lerp(minScale, maxScale, t);
                healthBarTransform.localScale = new Vector3(scale, scale, scale);
                yield return null;
            }

            t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * animationSpeed;
                float scale = Mathf.Lerp(maxScale, minScale, t);
                healthBarTransform.localScale = new Vector3(scale, scale, scale);
                yield return null;
            }
        }

        animating = false;
    }

    private float GetHealth()
    {
        return atm.GetPlayerHealth();
    }
}
