using UnityEngine;

//κλαση για τα sound effects των button οταν τα κανω click ή hover
public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource pauseButtonsSource, gameOverButtonSource;
    [SerializeField] AudioClip hoverButton, clickButton, clickBackButton;

    public void PlayHoverSound()
    {
        if (pauseButtonsSource.gameObject.activeSelf)
            pauseButtonsSource.PlayOneShot(hoverButton);
        else
            gameOverButtonSource.PlayOneShot(hoverButton);
    }
    public void PlayClickSound()
    {
        if (pauseButtonsSource.gameObject.activeSelf)
            pauseButtonsSource.PlayOneShot(clickButton);
        else
            gameOverButtonSource.PlayOneShot(clickButton);
    }
    public void PlayClickBackSound()
    {
        if (pauseButtonsSource.gameObject.activeSelf)
            pauseButtonsSource.PlayOneShot(clickBackButton);
        else
            gameOverButtonSource.PlayOneShot(clickBackButton);
    }
}
