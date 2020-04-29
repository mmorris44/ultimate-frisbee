using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPresets : NetworkBehaviour
{
    // Preset values

    // Agility
    public float[] layoutSpeed = { 13f, 10f, 7f }; // Layout speed
    public float[] layoutRecovery = { 1.5f, 2f, 2.5f }; // Layout recovery time
    public float[] turnSpeed = { 100f, 80f, 60f }; // Rotation speed

    // Speed
    public float[] topSpeed = { 3.5f, 3f, 2.5f }; // Top speed
    public float[] shuffleSpeed = { 2.5f, 2f, 1.5f }; // Movement on spot

    // Handling
    public float[] pivotHorizontalReach = { 1.2f, 0.9f, 0.4f }; // Max 1.2, min 0.4
    public float[] pivotVerticalReach = { 0.5f, 0.3f, 0.1f }; // Normal height = 1.5, min height = 1, max height 2. So max vertical reach is 0.5
    public float[] reach = { 7f, 5f, 4f }; // Reach when catching

    // Endurance
    public float[] maxEnergy = { 500f, 400f, 300f }; // Max energy for sprinting and laying out
    public float[] energyRecovery = { 0.7f, 0.5f, 0.4f }; // Energy recovered per frame

    public PlayerController playerController;

    private CanvasVariables canvasVariables;

    void Start()
    {
        if (!isLocalPlayer) return;
        canvasVariables = GameObject.Find("/GameControl").GetComponent<CanvasVariables>();

        // Set to default if not valid
        if (!canvasVariables.validPlayerSetup)
        {
            canvasVariables.agilityDropdown.value = 1;
            canvasVariables.speedDropdown.value = 1;
            canvasVariables.handlingDropdown.value = 1;
            canvasVariables.enduranceDropdown.value = 1;
        }

        // Agility
        playerController.layoutSpeed = layoutSpeed[canvasVariables.agilityDropdown.value];
        playerController.layoutRecovery = layoutRecovery[canvasVariables.agilityDropdown.value];
        playerController.turnSpeed = turnSpeed[canvasVariables.agilityDropdown.value];

        // Speed
        playerController.topSpeed = topSpeed[canvasVariables.speedDropdown.value];
        playerController.shuffleSpeed = shuffleSpeed[canvasVariables.speedDropdown.value];

        // Handling
        playerController.pivotHorizontalReach = pivotHorizontalReach[canvasVariables.handlingDropdown.value];
        playerController.pivotVerticalReach = pivotVerticalReach[canvasVariables.handlingDropdown.value];
        playerController.reach = reach[canvasVariables.handlingDropdown.value];

        // Endurance
        playerController.maxEnergy = maxEnergy[canvasVariables.enduranceDropdown.value];
        playerController.energyRecovery = energyRecovery[canvasVariables.enduranceDropdown.value];
    }
}