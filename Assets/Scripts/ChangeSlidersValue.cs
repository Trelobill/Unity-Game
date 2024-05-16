using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class ChangeSlidersValue : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider musicSlider,sfxSlider;

    //οταν ενεργοποειται το script, αλλαζει τα slider αναλογα με το ποσο εχω ορισει τον ηχο
    void OnEnable()
    {
        audioMixer.GetFloat("MusicVolume", out float musicValue);
        audioMixer.GetFloat("SFXVolume", out float SFXValue);
        musicSlider.value = musicValue;
        sfxSlider.value = SFXValue;
    }
}
