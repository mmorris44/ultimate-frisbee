using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
using UnityEngine.UI;

public class CanvasVariables : MonoBehaviour
{
    public int maxPoints = 5;
    public bool validPlayerSetup = true;

    public TMP_Dropdown teamDropdown;
    public TMP_InputField nameField;
    public NetworkManager networkManager;

    public TMP_Dropdown agilityDropdown;
    public TMP_Dropdown speedDropdown;
    public TMP_Dropdown enduranceDropdown;
    public TMP_Dropdown handlingDropdown;
    public Text totalText;

    public GameObject ingameDisplay;
    public GameObject pregameOptions;
    public GameObject HUDBackground;
    public NetworkManagerHUD networkManagerHUD;
    public Button showHelpButton;
    public Button hideHelpButton;

    private bool prevCursorVis = true;

    void Start()
    {
        OnStatDropdownChange();
        prevCursorVis = Cursor.visible;
    }

    void Update()
    {
        // Check to see if cursor vis has changed
        if (Cursor.visible != prevCursorVis)
        {
            prevCursorVis = Cursor.visible;
            UpdateUIVisibility();
        }

        // Hide or show player options and HUD
        if (!networkManager.isNetworkActive)
        {
            pregameOptions.SetActive(true);
            ingameDisplay.SetActive(false);

            Cursor.visible = true;
        }
        else
        {
            pregameOptions.SetActive(false);
            ingameDisplay.SetActive(true);

            // Toggle mouse
            if (Input.GetKeyDown("m"))
            {
                Cursor.visible = !Cursor.visible;
            }
        }
    }

    // Update validity and text
    public void OnStatDropdownChange()
    {
        // Set text
        int totalPointsUsed = TotalPointsUsed();
        totalText.text = TotalText(totalPointsUsed);

        // Set color and validity
        if (totalPointsUsed > maxPoints)
        {
            validPlayerSetup = false;
            totalText.color = Color.red;
        }
        else if (totalPointsUsed == maxPoints)
        {
            validPlayerSetup = true;
            totalText.color = Color.green;
        }
        else if(totalPointsUsed < maxPoints)
        {
            validPlayerSetup = true;
            totalText.color = Color.black;
        }
    }

    // Toggle UI elements based on cursor showing
    private void UpdateUIVisibility()
    {
        HUDBackground.SetActive(Cursor.visible);
        networkManagerHUD.showGUI = Cursor.visible;
        showHelpButton.interactable = Cursor.visible;
        hideHelpButton.interactable = Cursor.visible;
    }

    private string TotalText(int pointsUsed)
    {
        return "= " + pointsUsed + "/" + maxPoints;
    }

    private int TotalPointsUsed()
    {
        return 8 - agilityDropdown.value - speedDropdown.value - enduranceDropdown.value - handlingDropdown.value;
    }
}
