using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class CanvasVariables : MonoBehaviour
{
    public TMP_Dropdown teamDropdown;
    public TMP_InputField nameField;
    public NetworkManager networkManager;

    public GameObject ingameDisplay;
    public GameObject pregameOptions;

    void Update()
    {
        // Hide or show player options and HUD
        if (!networkManager.isNetworkActive)
        {
            pregameOptions.SetActive(true);
            ingameDisplay.SetActive(false);
        }
        else
        {
            pregameOptions.SetActive(false);
            ingameDisplay.SetActive(true);
        }
    }
}
