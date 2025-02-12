using ENet6;
using Protocols;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Protocols.Protocole;
using static Protocols.Protocole.PlayerPositionPacket;
using static UnityEngine.EventSystems.EventTrigger;

public class Client : MonoBehaviour
{
    public GameObject UIConnect;
    public GameObject UIWin;
    public GameObject StartGame;


    public TMP_InputField hostIP_Field;
    public TMP_InputField playerName_Field;

    public Image imagefeddo;
    public TextMeshProUGUI winnerText;

    public TextMeshProUGUI nbPlayer;


    PlayerInputs _lastInputs;

    private ENet6.Host enetHost = null;
    private ENet6.Peer? serverPeer = null;

    private Player _player;

    private List<Player> _allPlayers = new List<Player>();

    [SerializeField]
    private List<Enemy> allActiveEnemies = new List<Enemy>();
    float _phyTick = 0.016f;
    float _elapsedSinceLastTick = 0;
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
        


        _player = Instantiate(GameManager.instance.PlayerPrefab, GameManager.instance.GetSpawnPosition(), Quaternion.identity).GetComponent<Player>();
        Camera.main.GetComponent<Cam>().Targets.Clear();
        Camera.main.GetComponent<Cam>().Targets.Add(_player.gameObject);
        _player.Name = playerName_Field.text;

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
        if (Connect(hostIP_Field.text))
        {
            UIConnect.SetActive(false);
            StartGame.SetActive(true);
        }
    }

    public void StartGameButton()
    {
        StartGame.SetActive(false);

        List<Byte> data = new List<Byte>();
        StartedTheGame p;
        p.Serialize(ref data);

        Packet packet = default;

        packet.Create(data.ToArray(),PacketFlags.Reliable);

        serverPeer.Value.Send(0, ref packet);
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
                    
                    allActiveEnemies = FindObjectsOfType<Enemy>(false).ToList();
                    /*foreach(Enemy a  in allActiveEnemies)
                    {
                        Debug.Log(a.transform.parent.name);
                        if (a.transform.parent) Debug.Log("Fail");
                        else Debug.Log("Oui");
                    }*/

                    for (int i = 0; i < info.allEnemies.Count; i++)
                    {
                        if (allActiveEnemies[i] != null)
                        {
                            allActiveEnemies[i].index = info.allEnemies[i].index;
                            allActiveEnemies[i].gameObject.name = "Enemy #" + info.allEnemies[i].index;
                            allActiveEnemies[i].gameObject.transform.position = info.allEnemies[i].position;
                        }
                       
                    }

                    /*foreach (WorldInitPacket.EnemyData enemyData in info.allEnemies)
                    {
                        Enemy foundEnemy = allActiveEnemies.Find(enemy => enemy.transform.position == (Vector3)enemyData.position);
                        if(foundEnemy != null)
                        {
                            foundEnemy.index = enemyData.index;
                        }
                    }*/
                    for (int i = 0; i < allActiveEnemies.Count; i++)
                    {
                        if (allActiveEnemies[i].index == -1)
                        {
                            Destroy(allActiveEnemies[i].gameObject);
                            allActiveEnemies.Remove(allActiveEnemies[i]);
                            if (i < allActiveEnemies.Count)
                            {
                                i--;
                            }
                            //allActiveEnemies[i].transform.GetComponentInChildren<SpriteRenderer>().color = Color.black;

                        }
                    }
                    /*foreach (Enemy enemy in allActiveEnemies)
                    {
                        if(enemy.index == -1)
                        {
                            allActiveEnemies.Remove(enemy);
                            Destroy(enemy);

                        }
                    }*/

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
                        _lastInputs= playerPositionData.inputs;
                        _currentPlayer._playerMovements.IsOnGround = playerPositionData.isOnGround;
                        _currentPlayer._playerMovements.CanJump = playerPositionData.canJump;
                        _currentPlayer._playerMovements.CanDash = playerPositionData.canDash;
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
                                nbPlayer.text = _allPlayers.Count + "Players in Game";

                            }
                            else
                            {

                                Player player = Instantiate(GameManager.instance.PlayerPrefab, GameManager.instance.GetSpawnPosition(), Quaternion.identity).GetComponent<Player>();
                                player.isMain = false;

                                player.Name = packetPlayer.playerName;
                                player.index = packetPlayer.playerIndex;

                                _allPlayers.Add(player);
                                nbPlayer.text = _allPlayers.Count + "Players in Game";
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
                    nbPlayer.text = _allPlayers.Count + "Players in Game";

                    break;
                }

            case Opcode.S_EnemiesActive:
                {

                    
                    ActiveEnemiesDataPacket activeEnemies = ActiveEnemiesDataPacket.Deserialize(data, offset);

                    foreach (ActiveEnemiesDataPacket.EnemyData enemyData in activeEnemies.enemyData)
                    {
                        
                        Enemy enemyToFind = allActiveEnemies.Find(enemy => enemy.index == enemyData.enemyIndex);
                        if(enemyToFind != null)
                        {
                            
                            enemyToFind.transform.position = enemyData.position;
                            enemyToFind.SetVelocity(Vector2.zero); // pour éviter qu'ils poussent mais jsp
                            enemyToFind.HpValue = enemyData.enemyHp;
                        }
                    }

                    
                    
                    break;
                }
            case Opcode.S_EndGame:
                {
                    // faire s'afficher une merde avec le nom qu'on peut retrouver grâce à l'index en gros!
                    EndGamePacket end = EndGamePacket.Deserialize(data, offset);
                    winnerText.text = "Winning Player is : " + _allPlayers[end.winnerIndex].Name;
                    UIWin.SetActive(true);
                    StartCoroutine(CloseApp());
                    break;
                }
            case Opcode.S_EnemyDead:
                {
                    DeadEnemyPacket deathPacket = DeadEnemyPacket.Deserialize(data, offset);

                    Enemy deadEnemy = allActiveEnemies.Find(enemy => enemy.index == deathPacket.deadIndex);
                    if(deadEnemy != null)
                    {
                        allActiveEnemies.Remove(deadEnemy);
                        Destroy(deadEnemy.gameObject);
                    }
                    break;
                }

            case Opcode.S_PlayerStats:
                {
                    PlayerStatsPacket playerListPacket = PlayerStatsPacket.Deserialize(data, offset);

                    foreach (PlayerStatsPacket.PlayerStatData packetPlayer in playerListPacket.playerStatsList)
                    {

                        Player playerData = _allPlayers.Find(player => player.index == packetPlayer.playerIndex);
                        if(playerData.Stats.hpValue != packetPlayer.playerStats.hpValue)
                        {
                            Debug.Log("hpvalue: " + playerData.Stats.hpValue);
                        }

                        playerData.Stats = new Player.PlayerStats
                        {
                            attackValue = packetPlayer.playerStats.attackValue,
                            hpValue = packetPlayer.playerStats.hpValue,
                            
                            maxHpValue = packetPlayer.playerStats.maxHpValue,
                            speed = packetPlayer.playerStats.speed,
                        };

                        


                    }
                    break;
                }
            case Opcode.S_GameStarted:
                {
                    StartTheGame();
                    break;
                }
        }

                
        
    }
    void StartTheGame()
    {
        StartGame.SetActive(false);
        DCMapGen.instance?.UnlockDoors();
    }
    private IEnumerator CloseApp()
    {
        imagefeddo.color = imagefeddo.color + new Color(0.5f, 0.5f, 0.5f, 0f);
        yield return new WaitForSeconds(4);

        Application.Quit();
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
            _elapsedSinceLastTick += Time.deltaTime;
            if (_elapsedSinceLastTick > _phyTick)
            {
                // On simule la physique côté client, de la même façon que le serveur
                foreach (Player player in _allPlayers)
                {
                    PlayerInputs inputs = _lastInputs;

                    player._playerMovements.UpdatePhysics();
                }
                _elapsedSinceLastTick = 0;
            }

        }
        
    }
}
