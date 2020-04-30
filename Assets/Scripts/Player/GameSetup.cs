using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum PlayerColor { Red, Blue, None}

public class GameSetup : NetworkBehaviour
{
    public TextMesh playerNameText;
    public GameObject redTeamColor, blueTeamColor;

    private UIController canvasVariables;

    [SyncVar(hook = nameof(SetPlayerName))]
    string playerName = "Player";

    [SyncVar(hook = nameof(SetPlayerColor))]
    PlayerColor playerColor = PlayerColor.None;

    void Start()
    {
        if (!isLocalPlayer) return;
        canvasVariables = GameObject.Find("/GameControl").GetComponent<UIController>();

        // Hide cursor by default
        Cursor.visible = false;

        // Execute command to set name if not blank
        if (canvasVariables.nameField.text != "")
        {
            CmdSetName(canvasVariables.nameField.text);
        }

        // Execute command to set color
        CmdSetColor((PlayerColor) canvasVariables.teamDropdown.value);
    }

    // Sync var updated
    void SetPlayerName(string oldValue, string newValue)
    {
        playerNameText.text = newValue;
    }

    // Command to set the name in the server
    [Command]
    void CmdSetName(string newPlayerName)
    {
        playerName = newPlayerName;
    }

    // Sync var updated
    void SetPlayerColor(PlayerColor oldValue, PlayerColor newValue)
    {
        redTeamColor.SetActive(false);
        blueTeamColor.SetActive(false);
        if (newValue == PlayerColor.Blue)
        {
            blueTeamColor.SetActive(true);
            playerNameText.color = Color.blue;
        }
        else if (newValue == PlayerColor.Red)
        {
            redTeamColor.SetActive(true);
            playerNameText.color = Color.red;
        }
    }

    // Command to set the color in the server
    [Command]
    void CmdSetColor(PlayerColor newPlayerColor)
    {
        playerColor = newPlayerColor;
    }
}
