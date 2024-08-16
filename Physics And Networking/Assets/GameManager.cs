using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;

    [SyncVar]
    public List<PlayerScript> players = new List<PlayerScript>(); 

    private void Awake()
    {
        instance = this;
    }

    public void RegisterPlayer(PlayerScript player)
    {
        if(isServer)
        {
            players.Add(player);
        }
    }
}
