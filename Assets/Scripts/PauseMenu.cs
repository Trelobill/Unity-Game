using UnityEngine;
using UnityEngine.SceneManagement;
using StarterAssets;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Audio;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] SceneFader fader;
    [SerializeField] GameObject ui;
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider musicSlider, sfxSlider;
    [SerializeField] StarterAssetsInputs _input;
    [SerializeField] PlayerInput _playerInput;
    [SerializeField] GameObject instructions;
    AttributesManager atm;

    private void Start()
    {
        //προσθετω τα events για τα sliders
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        atm = _input.GetComponent<AttributesManager>();
    }
    void Update()
    {
        //bug-fix το instructions.activeSelf το βαζω ωστε αν κλεισει τα instructions με escape να μην ανοιξει μαζι και το pause menu
        if (Input.GetKeyDown(KeyCode.Escape) && !instructions.activeSelf)
        {
            //αν ο παιχτης ειναι νεκρος δεν μπορω να παω στο pauseMenu
            if (!atm.CheckPlayerDead())
                Pause();
        }
    }

    public void Pause()
    {
        //αλλαζει το state του pauseMenu αντιθετα απο οτι ειναι
        ui.SetActive(!ui.activeSelf);
        if (ui.activeSelf)
        {
            //αν ειναι ενεργο τοτε εμφανιζω παλι τον κερσορα, απενεργοποιω προσωρινα τα inputs και σταματαω τον χρονο του παιχνιδιου
            InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInDynamicUpdate;
            _input.SetCursorState(false);
            _playerInput.actions.Disable();
            _input.DisableActiveInputs();
            Time.timeScale = 0.0f;
        }
        //αλλιως κανω το αναποδο
        else
        {
            InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInFixedUpdate;
            _input.SetCursorState(true);
            Time.timeScale = 1f;
            _playerInput.actions.Enable();
            _input.enabled = true;
        }
    }

    //για το button Retry
    public void Retry()
    {
        Time.timeScale = 1f;
        ui.SetActive(!ui.activeSelf);
        fader.FadeToRetryGame(SceneManager.GetActiveScene().name);
        _input.SetCursorState(true);
    }

    //για το button Menu
    public void Menu()
    {
        Time.timeScale = 1f;
        if (!atm.CheckPlayerDead())
            ui.SetActive(!ui.activeSelf);
        fader.FadeToMenu("MainMenu");
    }

    //αλλαζω το volume στο audioMixer αναλογα τα sliders
    public void SetMusicVolume(float value)
    {
        audioMixer.SetFloat("MusicVolume", value);
    }
    public void SetSFXVolume(float value)
    {
        audioMixer.SetFloat("SFXVolume", value);
        audioMixer.SetFloat("TerrainVolume", value);
    }
}
