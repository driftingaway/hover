using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public Camera firstPersonCamera;
    public Camera UICamera;
    public GameObject player;
    public bool isUIEnabled = false;
    public TMP_InputField terminalInput;
    public GameObject HUD;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Mouse0))
        {
            ToggleUI();
        }
    }

    void ToggleUI()
    {
        isUIEnabled = !isUIEnabled;

        firstPersonCamera.gameObject.SetActive(!isUIEnabled);
        player.SetActive(!isUIEnabled);
        UICamera.gameObject.SetActive(isUIEnabled);
        HUD.SetActive(!isUIEnabled);

        if(!isUIEnabled)
        {
            // Unfocus input field
            terminalInput.DeactivateInputField();
        }
        else
        {
            // Focus input field
            terminalInput.ActivateInputField();
            terminalInput.Select();
        }
    }
}