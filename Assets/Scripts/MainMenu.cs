using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject settingsMenu;
    [SerializeField] SceneFader fader;
    private void Start()
    {
        fader.FadeInGameOrMenu();
    }
    //μεθοδος για το κουμπι play
    public void PlayGame()
    {
        fader.FadeToGame("Game");
    }
    //μεθοδος για το κουμπι settings
    public void SettingsOpen()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }
    public void SettingsClose()
    {
        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
    }
    //μεθοδος για το κουμπι quit
    public void QuitGame()
    {
        Application.Quit();
    }

}
