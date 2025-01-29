using ENet6;
using Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.XR;
using static Protocols.Protocole;
using static Protocols.Protocole.PlayerPositionPacket;

public class Client : MonoBehaviour
{

    public TMP_InputField hostIP_Field;
    public TMP_InputField playerName_Field;

    [SerializeField]
    private Transform _canvas;


    private ENet6.Host enetHost = null;
    private ENet6.Peer? serverPeer = null;

    private Player _player;

    private List<Player> _allPlayers = new List<Player>();

    private List<Enemy> allEnemies = new List<Enemy>();

    public bool Connect(string addressString)
    {
        ENet6.Address address = new ENet6.Address();
        if (!address.SetHost(ENet6.AddressType.Any, addressString))
        {
            Debug.LogError("failed to resolve \"" + addressString + "\"");
            return false;
        }

        Debug.Log("connecting to " + address.GetIP());

        address.Port = 25565;

        // On recréé l'host à la connexion pour l'avoir en IPv4 / IPv6 selon l'adresse
        if (enetHost != null)
            enetHost.Dispose();

        enetHost = new ENet6.Host();
        enetHost.Create(address.Type, 1, 0);
        serverPeer = enetHost.Connect(address, 0);

        // On laisse la connexion se faire pendant un maximum de 50 * 100ms = 5s
        for (uint i = 0; i < 50; ++i)
        {
            ENet6.Event evt = new ENet6.Event();
            if (enetHost.Service(100, out evt) > 0)
            {
                // Nous avons un événement, la connexion a soit pu s'effectuer (ENET_EVENT_TYPE_CONNECT) soit échoué (ENET_EVENT_TYPE_DISCONNECT)
                break; //< On sort de la boucle
            }
        }

        if (serverPeer.Value.State != PeerState.Connected)
        {
            Debug.LogError("connection to \"" + addressString + "\" failed");
            return false;
        }
        
        //On désactive l'UI affreuse svp
        


        _player = Instantiate(GameManager.instance.PlayerPrefab, GameManager.instance.Lobby.transform.position, Quaternion.identity).GetComponent<Player>();
        Camera.main.GetComponent<Cam>().Targets.Clear();
        Camera.main.GetComponent<Cam>().Targets.Add(_player.gameObject);
        _player.Name = playerName_Field.text;

        _canvas.gameObject.SetActive(false);
        PlayerNamePacket playerNamePacket = new()
        {
            name = _player.Name,
        };

        List<byte> data = new List<byte>();
        playerNamePacket.Serialize(ref data);

        Packet packet = default;
        packet.Create(data.ToArray(), PacketFlags.Reliable);
        serverPeer.Value.Send(0, ref packet);

        return true;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!ENet6.Library.Initialize())
            throw new Exception("Failed to initialize ENet");
            
        //Connect("localhost");
    }

    public void ConnectToWrittenIP()
    {
        if (playerName_Field.text == "") playerName_Field.text = "poule";
        Connect(hostIP_Field.text);
    }

    private void OnApplicationQuit()
    {
        ENet6.Library.Deinitialize();
    }

    // FixedUpdate est appelé à chaque Tick (réglé dans le projet)
    void FixedUpdate()
    {
        ENet6.Event evt = new ENet6.Event();
        if (enetHost != null && enetHost.Service(0, out evt) > 0)
        {
            do
            {
                switch (evt.Type)
                {
                    case ENet6.EventType.None:
                        {
                            Debug.Log("?");
                        }
                        break;

                    case ENet6.EventType.Connect:
                        {
                            Debug.Log("Connect");
                            break;
                        }

                    case ENet6.EventType.Disconnect:
                        Debug.Log("Disconnect");
                        serverPeer = null;
                        Destroy(this);
                        break;

                    case ENet6.EventType.Receive:
                        {
                            byte[] bytes = new byte[evt.Packet.Length];
                            evt.Packet.CopyTo(bytes);
                            HandleFromServer(bytes);
                            break;
                        }

                    case ENet6.EventType.Timeout:
                        Debug.Log("Timeout");
                        break;
                }
            }
            while (enetHost.CheckEvents(out evt) > 0);
        }
    }
    void HandleFromServer(byte[] bytes)
    {
        List<byte> data = bytes.ToList();
        int offset = 0;
        Opcode opcode = (Protocole.Opcode)Protocole.Deserialize_Uint8(data, ref offset);

        switch (opcode)
        {
            case Opcode.S_WorldInit:
                {
                    WorldInitPacket info = WorldInitPacket.Deserialize(data, offset);
                    Debug.Log("World Sedd : " + info.seed);

                    SceneMerger.instance.MergeScene();
                    DCMapGen.instance.Regenerate(info.seed);
                    allEnemies.AddRange(FindObjectsOfType<Enemy>());
                    break;
                }
                

            case Opcode.S_GameData:
                {
                    GameDataPacket gameData = GameDataPacket.Deserialize(data, offset);

                    _player.index = gameData.playerIndex;

                    break;
                    //Debug.Log("Received index #" +  gameData.playerIndex + " from server.");
                }
                

            case Opcode.S_PlayersPosition:
                {
                    PlayerPositionPacket playerPositionPacket = PlayerPositionPacket.Deserialize(data, offset);
                    foreach (PlayerPositionPacket.PlayerData playerPositionData in playerPositionPacket.players)
                    {

                        Player _currentPlayer = _allPlayers.Find(Player => Player.index == playerPositionData.playerIndex);
                        
                        _currentPlayer._playerMovements.UpdatePosition(playerPositionData.position);
                        _currentPlayer._playerMovements.UpdateVelocity(playerPositionData.velocity);
                        _currentPlayer.Inputs = playerPositionData.inputs;
                    }
                    break;
                }
                

            case Opcode.S_PlayerList:
                {
                    ListPlayersPacket playerListPacket = ListPlayersPacket.Deserialize(data, offset);

                    /*for (int i = 0; i < _allPlayers.Count; i++)
                    {
                        ListPlayersPacket.PlayerData playerData = playerPositionPacket.playersData.Find(playerFromIndex => playerFromIndex.playerIndex == _allPlayers[i].index);

                        
                    }*/

                    

                    foreach (ListPlayersPacket.PlayerData packetPlayer in playerListPacket.playersData)
                    {

                        Player playerData = _allPlayers.Find(player => player.index == packetPlayer.playerIndex);
                        
                        if (playerData == null)
                        {
                            if (_player.index == packetPlayer.playerIndex)
                            {
                                _allPlayers.Add(_player);
                            }
                            else
                            {

                                Player player = Instantiate(GameManager.instance.PlayerPrefab, GameManager.instance.Lobby.transform.position, Quaternion.identity).GetComponent<Player>();


                                player.Name = packetPlayer.playerName;
                                player.index = packetPlayer.playerIndex;

                                _allPlayers.Add(player);
                            }
                            
                            //TODO: update les noms des joueurs visuellement
                        }
                    }
                    break;

                }

            case Opcode.S_PlayerDisconnect:
                {

                    PlayerDisconnectPacket playerDecoPlacket = PlayerDisconnectPacket.Deserialize(data, offset);
                    Player playerToRemove = _allPlayers.Find(player => player.index == playerDecoPlacket.index);
                    _allPlayers.Remove(playerToRemove);
                    Destroy(playerToRemove);
                    break;
                }

            case Opcode.S_EnemiesActive:
                {
                    ActiveEnemiesDataPacket activeEnemies = ActiveEnemiesDataPacket.Deserialize(data, offset);
                    
                    foreach(ActiveEnemiesDataPacket.EnemyData enemyData in activeEnemies.enemyData)
                    {
                        Enemy enemyToFind = allEnemies.Find(enemy => enemy.index == enemyData.enemyIndex);
                        enemyToFind.transform.position = enemyData.position;
                        enemyToFind.SetVelocity(Vector2.zero);
                    }
                    break;
                }
                
                
        }

                
        
    }

    private void Update()
    {
        if(_player != null && _player.Inputs != null)
        {
            PlayerInputsPacket inputsPacket;
            inputsPacket.inputs = _player.Inputs;

            List<byte> data = new List<byte>();
            inputsPacket.Serialize(ref data);

            Packet packet = default;
            packet.Create(data.ToArray(), PacketFlags.Reliable);
            serverPeer.Value.Send(0, ref packet);

            // On simule la physique côté client, de la même façon que le serveur
            foreach (Player player in _allPlayers)
            {
                PlayerInputs inputs = player.Inputs;

                player._playerMovements.UpdatePhysics();
            }
        }
        
    }
}
