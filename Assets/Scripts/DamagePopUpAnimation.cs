using TMPro;
using UnityEngine;

public class DamagePopUpAnimation : MonoBehaviour
{
    public AnimationCurve opacityCurve;
    public AnimationCurve scaleCurve;
    public AnimationCurve heightCurve;
    TextMeshProUGUI tmp;
    float time = 0;
    Vector3 origin;
    private void Awake()
    {
        tmp = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        origin = transform.position;
    }

    //αλλαζει smoothly(κανει scale down) το νουμερο που δειχνει την ζημια
    private void Update()
    {
        tmp.color = new(1, 1, 1, opacityCurve.Evaluate(time));
        transform.localScale = (Vector3.one * scaleCurve.Evaluate(time)) / 2;
        transform.position = origin + new Vector3(0, 1 + heightCurve.Evaluate(time), 0);
        time += Time.deltaTime;
    }
}
