using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetup : NetworkBehaviour
{
    public TextMesh playerNameText;

    private CanvasVariables canvasVariables;

    [SyncVar(hook = nameof(SetPlayerName))]
    public string playerName = "Player";

    void Start()
    {
        if (!isLocalPlayer) return;
        canvasVariables = GameObject.Find("/GameControl").GetComponent<CanvasVariables>();

        // Execute command to set name if not blank
        if (canvasVariables.nameField.text != "")
        {
            CmdSetName(canvasVariables.nameField.text);
        }

        canvasVariables.pregameOptions.SetActive(false);
    }

    // Sync var updated
    void SetPlayerName(string oldValue, string newValue)
    {
        playerNameText.text = newValue;
    }

    // Command to set the name in the server
    [Command]
    public void CmdSetName(string newPlayerName)
    {
        playerName = newPlayerName;
    }
}
