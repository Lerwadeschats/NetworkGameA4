using System;
using System.Collections.Generic;
using System.Linq;
//using System.Numerics;
using ENet6;
using Protocols;
using TMPro;
using UnityEngine;
using static Protocols.Protocole;
using static Protocols.Protocole.PlayerPositionPacket;
using Event = ENet6.Event;
using EventType = ENet6.EventType;
using Random = UnityEngine.Random;

public class Server : MonoBehaviour
{
    public TextMeshProUGUI textLogger;
    class ServerData
    {
        public ENet6.Host host;
    };

    private ServerData _serverData = new ServerData();
    private UInt32 nextTick = ENet6.Library.Time;

    private ulong seed;

    private List<PlayerClient> players = new List<PlayerClient>();

    [SerializeField]
    private List<Enemy> enemies = new List<Enemy>();

    struct PlayerClient
    {
        public Peer peer;
        public Player player;

    }

    private void Start()
    {
        // Initialisation d'enet
        if (!ENet6.Library.Initialize())
        {
            throw new Exception("failed to initialize enet");
        }

        // On planifie la libération d'enet à la fin de la fonction

        ENet6.Address address = ENet6.Address.BuildAny(AddressType.IPv6);

        address.Port = 25565;

        _serverData.host = new Host();
        _serverData.host.Create(AddressType.Any, address, 10, 10, 0, 0);
        if (!_serverData.host.IsSet)
        {
            throw new Exception("Failed to create ENet host (is port free?)");
        }

        print("Server is Runnin' on port : " + address.Port);

        seed = (ulong)Random.Range(0, 999999);

        StartCoroutine(SceneMerger.instance.CreateAndMergeSceneServerSide(seed));
    }

    // Update is called once per frame
    void Update()
    {
        // On récupère le temps actuel
        UInt32 now = ENet6.Library.Time;

        // On gère les événements ENet
        while (_serverData.host.Service(1, out Event eNetEvent) > 0)
        {
            do
            {
                switch (eNetEvent.Type)
                {
                    // Un nouveau joueur s'est connecté
                    case EventType.Connect:
                        ResetEnemiesList();
                        SendSeedToClient(eNetEvent.Peer);
                        CreateNewPlayer(eNetEvent.Peer);
                        print($"Peer # {eNetEvent.Peer.ID} connected! \n Sended Seed To Client");
                        break;

                    // Un joueur s'est déconnecté
                    case EventType.Disconnect:

                        PlayerClient playerToRemove = players.Find(p => p.peer.ID == eNetEvent.Peer.ID);

                        PlayerDisconnectPacket playerDisconnectPacket = new PlayerDisconnectPacket();
                        playerDisconnectPacket.index = (byte)playerToRemove.player.index;
                        
                        foreach (PlayerClient player in players)
                        {
                            if (player.peer.IsSet && player.player.Name != "")
                            {
                                List<byte> playerToDCData = new List<byte>();
                                playerDisconnectPacket.Serialize(ref playerToDCData);

                                Packet packet = default;
                                packet.Create(playerToDCData.ToArray(), PacketFlags.Reliable);

                                player.peer.Send(0, ref packet);
                            }

                        }

                        players.Remove(playerToRemove);

                        Destroy(playerToRemove.player.gameObject);
                        ListPlayersPacket playerListPacket = new();
                        playerListPacket.playersData = new List<ListPlayersPacket.PlayerData>();
                        foreach (PlayerClient player in players)
	{
                            if (player.peer.IsSet && player.player.Name != "") 
                            {

                                ListPlayersPacket.PlayerData packetPlayer = new()
                                {
                                    playerName = player.player.Name, 
                                    playerIndex = (byte)player.player.index, 
                                    //playerColor = player.player.Color
                                };

                                playerListPacket.playersData.Add(packetPlayer);
                            }
                        }
                        foreach (PlayerClient player in players)
						{
                            if (player.peer.IsSet && player.player.Name != "")
                            {
                                List<byte> dataIndex = new List<byte>();
                                playerListPacket.Serialize(ref dataIndex);

                                Packet packet = default;
                                packet.Create(dataIndex.ToArray(), PacketFlags.Reliable);

                                player.peer.Send(0, ref packet);
                            }
                                
                        }
                        print($"Peer # {eNetEvent.Peer.ID} disconnected!");
                        break;

                    // On a reçu des données d'un joueur
                    case EventType.Receive:
                        {
                            //print($"Peer # {eNetEvent.Peer.ID} sent a data of ({eNetEvent.Packet.Length} bytes)");
                            byte[] bytes = new byte[eNetEvent.Packet.Length];
                            eNetEvent.Packet.CopyTo(bytes);
                            HandleFromClient(eNetEvent.Peer.ID, bytes);

                            eNetEvent.Packet.Dispose();
                        }
                        break;
                }
            }
            while (_serverData.host.Service(1, out eNetEvent) > 0);
        }

        // Serveur or Physique ?
        if (now >= nextTick)
        {
            Tick(ref _serverData);
            //TOMIDIFYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYyy
            nextTick += 1;
        }
    }

    private void HandleFromClient(uint peer, byte[] bytes)
    {

        PlayerClient playerFromIndex = players.Find(player => player.player.index == peer); // on trouve le joueur de l'index
        List<byte> data = bytes.ToList();
        int offset = 0;
        Protocole.Opcode opcode = (Protocole.Opcode)Protocole.Deserialize_Uint8(data, ref offset);

        switch (opcode)
        {
            case Opcode.C_PlayerName:
                {
                    PlayerNamePacket playerNameInfo = PlayerNamePacket.Deserialize(data, offset);

                    Debug.Log("Player " + playerNameInfo.name + " get index(peer) :" + peer);
                    //envoi de l'index
                    GameDataPacket gameDataPacket = new()
                    {
                        playerIndex = peer
                    };
                    playerFromIndex.player.Name = playerNameInfo.name;

                    List<byte> dataIndex = new List<byte>();
                    gameDataPacket.Serialize(ref dataIndex);

                    Packet packet = default;
                    packet.Create(dataIndex.ToArray(), PacketFlags.Reliable);
                    playerFromIndex.peer.Send(0, ref packet);

                    Debug.Log("Sent index '" + gameDataPacket.playerIndex + "' to player : " + playerFromIndex.player.index);



                    //On envoie la liste des joueurs
                    ListPlayersPacket playerListPacket = new();

                    playerListPacket.playersData = new List<ListPlayersPacket.PlayerData>();

                    foreach (PlayerClient player in players)
                    {
                        if (player.peer.IsSet && player.player.Name != "")
                        {


                            ListPlayersPacket.PlayerData packetPlayer = new()
                            {
                                playerName = player.player.Name,
                                
                                playerIndex = (byte)player.player.index,
                                //playerColor = player.player.Color
                            };


                            playerListPacket.playersData.Add(packetPlayer);
                        }
                    }

                    foreach (PlayerClient playerClient in players)
                    {
                        if (playerClient.peer.IsSet && playerClient.player.Name != "")
                        {
                            List<byte> dataPlayers = new List<byte>();
                            playerListPacket.Serialize(ref dataPlayers);

                            Packet packetPlayers = default;
                                
                            packetPlayers.Create(dataPlayers.ToArray(), PacketFlags.Reliable);

                            playerClient.peer.Send(0, ref packetPlayers);
                        }

                    }
                    break;
                }
                
            case Opcode.C_PlayerInputs:
                {
                    PlayerInputsPacket inputsPackets = PlayerInputsPacket.Deserialize(data, offset);

                    // PlayerInputs inputs = inputsPackets.inputs;
                    PlayerInputs inputs = inputsPackets.inputs;

                    playerFromIndex.player.Inputs = inputs;
                    break;
                }
                
        }
    }

    private void SendSeedToClient(Peer peer)
    {
        List<byte> data = new List<byte>();
        WorldInitPacket info = new() { seed = seed };
        info.allEnemies = new List<WorldInitPacket.EnemyData>();
        for (int i = 0; i < enemies.Count; i++)
        {
            WorldInitPacket.EnemyData newEnemy = new WorldInitPacket.EnemyData();
            newEnemy.index = (byte)enemies[i].index;
            newEnemy.position = enemies[i].transform.position;
            info.allEnemies.Add(newEnemy);
        }
        
        info.Serialize(ref data);
        Debug.Log((ulong)seed);
        Packet packet = default;
        packet.Create(data.ToArray(), PacketFlags.Reliable);

        peer.Send(0, ref packet);
    }

    private void CreateNewPlayer(Peer peer)
    {
        Player player = Instantiate(GameManager.instance.PlayerPrefab, GameManager.instance.Lobby.transform.position, Quaternion.identity).GetComponent<Player>();

        player.index = peer.ID;

        player.Color = new Color(Random.Range(0, 255), Random.Range(0, 255), Random.Range(0, 255));

        PlayerClient clientPlayer = new PlayerClient
        {
            peer = peer,
            player = player,
        };

        players.Add(clientPlayer);
    }

    private void ResetEnemiesList()
    {
        enemies.Clear();
        Enemy[] allEnemies = FindObjectsOfType<Enemy>();
        for (int i = 0; i < allEnemies.Length; i++)
        {
            allEnemies[i].index = i;
        }
        enemies.AddRange(allEnemies);
    }

    private void UpdateEnemies()
    {
        
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i] == null)
            {
                enemies.Remove(enemies[i]);
                i--;
            }
            
        }
    }

    private void SendActiveEnemies()
    {
        
        ActiveEnemiesDataPacket enemiesDataPacket = new ActiveEnemiesDataPacket();
        enemiesDataPacket.enemyData = new List<ActiveEnemiesDataPacket.EnemyData>();
        if (enemies.Count > 0)
        {
            foreach (Enemy enemy in enemies)
            {
                /*if (enemy.IsActive())
                {
                    ActiveEnemiesDataPacket.EnemyData activeEnemy = new ActiveEnemiesDataPacket.EnemyData();
                    activeEnemy.enemyIndex = enemy.index;
                    activeEnemy.position = enemy.transform.position;
                    activeEnemy.velocity = enemy.GetVelocity();
                    activeEnemy.enemyHp = enemy.HpValue;
                    enemiesDataPacket.enemyData.Add(activeEnemy);
                }*/

                ActiveEnemiesDataPacket.EnemyData activeEnemy = new ActiveEnemiesDataPacket.EnemyData();
                activeEnemy.enemyIndex = enemy.index;
                activeEnemy.position = enemy.transform.position;
                activeEnemy.velocity = enemy.GetVelocity();
                activeEnemy.enemyHp = enemy.HpValue;
                enemiesDataPacket.enemyData.Add(activeEnemy);
            }
        }


        List<byte> data = new List<byte>();
        enemiesDataPacket.Serialize(ref data);
        Packet packet = default;
        packet.Create(data.ToArray(), PacketFlags.Reliable);

        foreach (PlayerClient clientData in players)
        {
            clientData.peer.Send(0, ref packet);
        }
    }

    //Tick function
    private void Tick(ref ServerData servData)
    {
        UpdateEnemies();
        SendPositionStatesPlayer();
        foreach (PlayerClient clientPlayer in players)
        {
            clientPlayer.player._playerMovements.UpdatePhysics();
        }

        SendActiveEnemies();
        GetPlayerAttacks();

        //Send PlayerPos
    }

    void SendPositionStatesPlayer()
    {
        PlayerPositionPacket posPacket = new PlayerPositionPacket();
        posPacket.players = new List<PlayerPositionPacket.PlayerData>();
        foreach (PlayerClient clientData in players)
        {
            if (clientData.peer.IsSet && clientData.player != null)
            {
                PlayerPositionPacket.PlayerData packetPlayer = new PlayerPositionPacket.PlayerData();
                packetPlayer.playerIndex = (byte)clientData.player.index;
                
                packetPlayer.position = clientData.player.Position;
                packetPlayer.velocity = clientData.player.Velocity;
                packetPlayer.inputs = clientData.player.Inputs;
                posPacket.players.Add(packetPlayer);
            }
        }

        List<byte> data = new List<byte>();
        posPacket.Serialize(ref data);
        Packet packet = default;
        packet.Create(data.ToArray(), PacketFlags.Reliable);

        foreach (PlayerClient clientPlayer in players)
        {
            clientPlayer.peer.Send(0, ref packet);
        }
    }

    void GetPlayerAttacks()
    {
        foreach(PlayerClient client in players)
        {
            if (client.player.Inputs.attack)
            {
                for (int i = 0; i < client.player._playerAttacks.GetHittedEntities().Count; i++)
                {
                    client.player._playerAttacks.GetHittedEntities()[i].OnBeingAttacked(client.player.Stats.attackValue);
                }
            }
        }
        
    }
    private void OnApplicationQuit()
    {
        ENet6.Library.Deinitialize();
        _serverData.host.Dispose();
    }



    string stack = "";

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (stack.Length > 2000) ClearConsole();
        logString = logString.Replace("UnityEngine", "||");
        logString = logString.Split("||")[0];
        textLogger.text = "> " + logString + "\n" + stack;
        stack = logString + "\n" + stack;
    }

    public void DebugRandom()
    {
        Debug.Log("aaaaaa UwU aaaaaa");
    }
    public void ClearConsole()
    {
        stack = "";
        Debug.Log("\nConsole Cleared");

    }
}
