using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one GameManger in scene.");
        }
    }

    private const string PREFIX = "Player ";
    private static Dictionary<string, PlayerManager> players = new Dictionary<string, PlayerManager>();

    public static void RegisterPlayer(string networkID, PlayerManager player)
    {
        string playerID = PREFIX + networkID;
        players.Add(playerID, player);
        player.transform.name = playerID;
    }

    public static void UnregisterPlayer(string playerID)
    {
        players.Remove(playerID);
    }

    public static PlayerManager GetPlayer(string playerID)
    {
        return players[playerID];
    }

    public static string AmmoCheckLog()
    {
        string log = "";
        foreach (KeyValuePair<string, PlayerManager> entry in players)
        {
            CharacterStates states = entry.Value.gameObject.GetComponent<CharacterStates>();

            if (states.AmmoCount < 0)
            {
                log += "Player: " + entry.Key + " . AmmoCount: " + states.AmmoCount + " \n";
            }
        }
        if (log == "")
        {
            log = "No issues.";
        }
        return log;
    }

    /* ********** *************** ********** */
    /* **********  For Debugging  ********** */
    /* ********** *************** ********** */

    //void OnGUI()
    //{
    //    GUILayout.BeginArea(new Rect(200, 200, 200, 500));
    //    GUILayout.BeginVertical();

    //    foreach(string playerID in players.Keys)
    //    {
    //        GUILayout.Label(playerID + " - " + players[playerID].transform.name);
    //    }

    //    GUILayout.EndVertical();
    //    GUILayout.EndArea();
    //}
}