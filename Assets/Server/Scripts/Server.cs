using System;
using System.Collections.Generic;
using System.Linq;
using ENet6;
using Protocols;
using TMPro;
using UnityEngine;
using Event = ENet6.Event;
using EventType = ENet6.EventType;

public class Server : MonoBehaviour
{
    public TextMeshProUGUI textLogger;
    class ServerData
    {
        public ENet6.Host host;
    };

    private ServerData _serverData = new ServerData();
    private UInt32 nextTick = ENet6.Library.Time;

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
                        print($"Peer # {eNetEvent.Peer.ID} connected!");
                        SendDataToClient(eNetEvent.Peer, "Welcome to the server!");
                        break;

                    // Un joueur s'est déconnecté
                    case EventType.Disconnect:
                        print($"Peer # {eNetEvent.Peer.ID} disconnected!");
                        break;

                    // On a reçu des données d'un joueur
                    case EventType.Receive:
                        print($"Peer # {eNetEvent.Peer.ID} sent data ({eNetEvent.Packet.Length} bytes)");
                        eNetEvent.Packet.Dispose();
                        break;
                }
            }
            while (_serverData.host.Service(1, out eNetEvent) > 0);
        }

        // On déclenche un tick si suffisamment de temps s'est écoulé
        if (now >= nextTick)
        {
            Tick(ref _serverData);
            nextTick += 1;
        }
    }

    private void SendDataToClient(Peer peer, string message)
    {
        List<byte> data = new List<byte>();
        Protocole.Serialize_str(ref data,message);

        // Create an ENet packet with the data
        Packet packet = default;
        packet.Create(data.ToArray(), PacketFlags.Reliable); // Use Reliable if you want guaranteed delivery

        // Send the packet to the client
        peer.Send(0, ref packet); // Use channel 0 (or another channel if you want)
    }


    //Tick function
    private void Tick(ref ServerData servData)
    {



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
        string toddebug = logString.Replace("UnityEngine", "||");
        toddebug = toddebug.Split("||")[0];
        textLogger.text = "> " + toddebug + "\n" + stack;
        stack = toddebug + "\n" + stack;
    }

    public void DebugRandom()
    {
        Debug.Log("aaaaaa UwU aaaaaa");
    }
    public void ClearConsole()
    {
        stack = "";
        Debug.Log("Console Cleared");

    }
}
