using TMPro;
using UnityEngine;

public class ChangeBestWaveText : MonoBehaviour
{
    //τρεχει οταν μπαινω στο μενου ωστε να ανανεωσει το best Score
    void Start()
    {
        TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();
        int waveNumber = PlayerPrefs.GetInt("BestWave");
        text.text = $"Play (Best Wave: {waveNumber})";
    }
}
