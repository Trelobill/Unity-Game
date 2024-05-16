using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class Instructions : MonoBehaviour
{
    [SerializeField] StarterAssetsInputs _input;
    [SerializeField] PlayerInput _playerInput;
    [SerializeField] SceneFader fader;
    [SerializeField] SpawnManager spawnManager;
    private void Start()
    {
        DisableAfterDelay();
    }


    //χρησιμοποιω lateUpdate ωστε να τρεξει πρωτα το pause, να μην ανοιξει 
    //εφοσον εχω ανοιχτα τα instructions και μετα να κλεισει τα instructions
    //το κανω αυτο για να μην ανοιγει το pause οταν κλεινω τα instructions με το escape
    void LateUpdate()
    {
        if (Input.anyKeyDown)
        {
            CloseInstructions();
        }
    }

    void CloseInstructions()
    {
        InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInFixedUpdate;
        //εξαφανιζω τον κερσορα
        _input.SetCursorState(true);
        //κανω τον χρονο 1 οποτε ξεκινανε ολα παλι να τρεχουνε
        Time.timeScale = 1f;
        fader.FadeInGameOrMenu();
        _playerInput.actions.Enable();
        gameObject.SetActive(false);
        spawnManager.StartFadeStartingText();
    }


    void DisableAfterDelay()
    {
        InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInDynamicUpdate;
        _input.SetCursorState(false);
        Time.timeScale = 0.0f;
        _playerInput.actions.Disable();
    }
}
