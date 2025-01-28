using System;
using System.Collections.Generic;
using System.Linq;
//using System.Numerics;
using ENet6;
using Protocols;
using TMPro;
using UnityEngine;
using static Protocols.Protocole;
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

                        SendSeedToClient(eNetEvent.Peer);
                        CreateNewPlayer(eNetEvent.Peer);
                        print($"Peer # {eNetEvent.Peer.ID} connected! \n Sended Seed To Client");
                        break;

                    // Un joueur s'est déconnecté
                    case EventType.Disconnect:
                        print($"Peer # {eNetEvent.Peer.ID} disconnected!");
                        break;

                    // On a reçu des données d'un joueur
                    case EventType.Receive:
                        {
                            print($"Peer # {eNetEvent.Peer.ID} sent a data of ({eNetEvent.Packet.Length} bytes)");
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
        List<byte> data = bytes.ToList();
        int offset = 0;
        Protocole.Opcode opcode = (Protocole.Opcode)Protocole.Deserialize_Uint8(data, ref offset);

        switch (opcode)
        {
            case Opcode.C_PlayerName:
                {
                    PlayerNamePacket playerNameInfo = PlayerNamePacket.Deserialize(data, offset);

                    Debug.Log("Player " + playerNameInfo.name + " get index(peer) :" + peer);


                    PlayerClient playerFromIndex = players.Find(player => player.player.index == peer);


                    //envoi de l'index
                    GameDataPacket gameDataPacket = new()
                    {
                        playerIndex = peer
                    };


                    List<byte> dataIndex = new List<byte>();
                    gameDataPacket.Serialize(ref dataIndex);

                    Packet packet = default;
                    packet.Create(data.ToArray(), PacketFlags.Reliable);
                    playerFromIndex.peer.Send(0, ref packet);

                    Debug.Log("Sent index to player #" + gameDataPacket.playerIndex);
                }
                break;
            case Opcode.C_PlayerInputs:
                {
                    PlayerInputsPacket inputsPackets = PlayerInputsPacket.Deserialize(data, offset);
                    //do inputeri
                }
                break;
        }
    }

    private void SendSeedToClient(Peer peer)
    {
        List<byte> data = new List<byte>();
        WorldInitPacket info = new() { seed = seed };
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



    //Tick function
    private void Tick(ref ServerData servData)
    {


        //Send PlayerPos
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
