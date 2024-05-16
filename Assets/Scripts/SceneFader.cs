using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneFader : MonoBehaviour
{
    public Image img;
    public AnimationCurve curve;
    [SerializeField] Slider progressSlider;
    [SerializeField] TextMeshProUGUI progressText;
    [SerializeField] GameObject instructions;
    [SerializeField] GameObject instructionsWithBar;

    public void FadeInGameOrMenu()
    {
        StartCoroutine(FadeIn());
    }
    public void FadeToGame(string scene)
    {
        instructions.SetActive(true);
        //καλω την συναρτηση μετα απο 0.5 ωστε να σιγουρευτω οτι ανοιξε
        //το instructions και δεν κολλησε πριν αρχισει να κανει load
        StartCoroutine(FadeOutToGame(scene, 0.5f));
    }

    public void FadeToRetryGame(string scene)
    {
        FadeOutToRetryGame(scene);
    }
    public void FadeToMenu(string scene)
    {
        StartCoroutine(FadeOutToMenu(scene));
    }

    IEnumerator FadeIn()
    {
        float t = 1f;

        while (t > 0f)
        {
            t -= Time.deltaTime;
            float a = curve.Evaluate(t);
            img.color = new Color(0f, 0f, 0f, a);
            yield return 0;
        }
    }
    IEnumerator FadeOutToMenu(string scene)
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime;
            float a = curve.Evaluate(t);
            img.color = new Color(0f, 0f, 0f, a);
            yield return 0;
        }
        SceneManager.LoadScene(scene);
    }

    void FadeOutToRetryGame(string scene)
    {
        instructions.SetActive(false);
        instructionsWithBar.SetActive(true);
        StartCoroutine(ReLoadLevelAsync(scene));
    }
    IEnumerator FadeOutToGame(string scene,float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(LoadLevelAsync(scene));
    }


    IEnumerator ReLoadLevelAsync(string levelToLoad)
    {
        progressSlider.value = 0;
        AsyncOperation load = SceneManager.LoadSceneAsync(levelToLoad);
        load.allowSceneActivation = false;
        float progress = 0;
        while (!load.isDone)
        {
            progress = Mathf.MoveTowards(progress, load.progress, Time.deltaTime);
            progressSlider.value = progress;
            progressText.text = $"Φορτώνει.. ({Mathf.RoundToInt(progress * 100)}%)";
            if (progress >= 0.9f)
            {
                progressSlider.value = 1;
                progressText.text = "Φορτώνει.. (100%)";
                load.allowSceneActivation = true;
            }
            yield return null;
        }
        instructions.SetActive(true);
        instructionsWithBar.SetActive(false);
    }
    
    //συναρτηση που φορτωνει το loading bar αναλογα ποσο % εχει φορτωσει η σκηνη του παιχνιδιου
    IEnumerator LoadLevelAsync(string levelToLoad)
    {
        progressSlider.value = 0;
        AsyncOperation load = SceneManager.LoadSceneAsync(levelToLoad);
        load.allowSceneActivation = false;
        float progress = 0;
        while (!load.isDone)
        {
            progress = Mathf.MoveTowards(progress, load.progress, Time.deltaTime);
            progressSlider.value = progress;
            progressText.text = $"Φορτώνει.. ({Mathf.RoundToInt(progress * 100)}%)";
            if (progress >= 0.9f)
            {
                progressSlider.value = 1;
                progressText.text = "Φορτώνει.. (100%)";
                load.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
