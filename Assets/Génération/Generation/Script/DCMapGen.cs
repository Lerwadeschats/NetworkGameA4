using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class DCMapGen : MonoBehaviour
{
    public static DCMapGen instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else Destroy(this);

    }

    public int _nbMaxOfRoom = 30;
    public int _nbMinOfRoom = 25;

    public int _nbOfRoomBranch = 4;
    public int _branchNumber = 2;
    public int _chanceToBranch = 50;
    public List<GameObject> _rooms = new List<GameObject>();

    Cam _camera;

    ulong seed = 0;

    bool _hasMainFinish = false;

    public List<Room> _branches = new List<Room>();


    System.Random _rand;

    List<Exit> _focusToGenerate;

    public bool DebugSeed = false;

    //C DEGUELASSE BOUERK 
    public List<Collider2D> collider2Ds = new List<Collider2D>();

    public GameObject fin;

    private void Start()
    {
        GameManager.instance.Lobby = gameObject.transform.GetChild(0).gameObject.GetComponent<Room>();
        //Regenerate(45);
    }
    public void Regenerate(ulong newSeed)
    {
        collider2Ds.Clear();
        _focusToGenerate = new List<Exit>();
        _camera = Camera.main.GetComponent<Cam>();

        seed = newSeed;
        _rand = new System.Random((int)newSeed);

        _focusToGenerate.Clear();
        //_camera.Targets.Clear();
        //_camera.Targets.Add(transform.GetChild(0).gameObject);
        for (int i = transform.childCount - 1; i > 0; i--) { Destroy(transform.GetChild(i).gameObject); }
        int LorR = _rand.Next(0, 2);
        transform.GetChild(0).GetChild(LorR).GetComponent<Exit>().UseExit();
        GenerateBranch(true, transform.GetChild(0).GetChild(LorR).GetComponent<Exit>(), Color.red);
    }

    private void GenerateBranch(bool isMain, Exit focus, Color endCol)
    {
        Room room = new();
        Physics2D.simulationMode = SimulationMode2D.Script;
        int numberOfRooms = isMain ? _nbMaxOfRoom : _nbOfRoomBranch;
        int numberOfbranchCreated = 0;
        Transform parent = isMain ? focus.transform.parent.parent : focus.transform.parent;
        for (int i = 1; i <= numberOfRooms; i++)
        {
            List<GameObject> possibleRoom = _rooms;
        RetryRoom:
            if (possibleRoom.Count == 0)
            {
                //print("OutOfRooms " + isMain);
                if (isMain)
                {
                    if (transform.childCount < _nbMinOfRoom) Regenerate(seed);
                    return;
                }
                break;
            }

            int roomSeed = _rand.Next(0, possibleRoom.Count);
            GameObject go = Instantiate(possibleRoom[roomSeed]);
            GameObject goCopy = possibleRoom[roomSeed];

            room = go.GetComponent<Room>();
            List<Exit> roomExits = room.GetExits().ToList();
            List<Exit> matchingExits = roomExits.Where(ex => ex._type == focus._compatibleType).ToList();

            if (matchingExits.Count == 0) // No exits Match
            {
                DestroyImmediate(go);
                possibleRoom = possibleRoom.Where(ro => ro != goCopy).ToList();
                goto RetryRoom;
            }
            int exitSeed = _rand.Next(0, matchingExits.Count);
            Exit exit = matchingExits[exitSeed];
            //go.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.white, endCol, isMain? (float)i / _nbMaxOfRoom : (float)i / _nbOfRoomBranch);
            exit.UseExit();
            go.transform.parent = parent;
            go.transform.position = focus.transform.position - exit.transform.position;
            go.name += " " + i;

            Physics2D.simulationMode = SimulationMode2D.Script;

           
            //print(Physics2D.IsTouchingLayers(room._boxCollider,3));
            //Collider2D[] contact = Physics2D.OverlapBoxAll(go.transform.position, go.transform.localScale, 0f, 3);
            //contact = contact.Where(col => col.name != go.name).ToArray();


            // Room is touching another one
            bool oui = false;
            foreach(Collider2D cont in collider2Ds)
            {
                if(cont != null )
                {
                    Physics2D.Simulate(0.01f);
                    bool aaaaa = Physics2D.IsTouching(room._boxCollider, cont);

                    print("doin' " + aaaaa);
                    if (aaaaa) oui = true;
                }
            }
            if (oui)
            {
                print("oui");
                DestroyImmediate(go);
                possibleRoom = possibleRoom.Where(ro => ro != goCopy).ToList();
                goto RetryRoom;
            
            }

            roomExits = roomExits.Where(ex => ex._type != focus._compatibleType).ToList();
        RetryExit:
            if (roomExits.Count == 0) // Room do not have any exits left
            {
                DestroyImmediate(go);
                possibleRoom = possibleRoom.Where(ro => ro != goCopy).ToList();
                goto RetryRoom;
            }
            collider2Ds.Add(room._boxCollider);

            int exitIndex = -1;
            foreach (Exit ex in roomExits) // test if exit is legal
            {
                exitIndex = roomExits.FindIndex(e => e == ex);
                if (ex.CanPlaceARoom())
                {
                    break;
                }
                else
                {
                    DestroyImmediate(ex.gameObject);
                    roomExits.RemoveAt(exitIndex);
                    goto RetryExit;
                }
            }
            
            //_camera.Targets.Add(go);
            focus = roomExits[exitIndex];
            focus.UseExit();
            roomExits.RemoveAt(exitIndex);
            int chanetobranch = _rand.Next(0, 100);
            if (roomExits.Count > 0 && isMain && chanetobranch < _chanceToBranch && numberOfbranchCreated < _branchNumber)
            {

                numberOfbranchCreated++;
                exitIndex = _rand.Next(0, roomExits.Count);
                roomExits[exitIndex].UseExit();
                //Save to Generate branch later
                _focusToGenerate.Add(roomExits[exitIndex]);
            }
        }

        if (isMain)
        {
            End eeee = Instantiate(fin, room.transform.position,Quaternion.identity,null).GetComponent<End>();
            Server.Instance.endGoal = eeee;
        }
        if (isMain) foreach (Exit e in _focusToGenerate) GenerateBranch(false, e, Color.blue);
        _hasMainFinish = isMain;
        foreach(Collider2D c in collider2Ds)
        {
            //if(c) c.transform.GetComponent<Rigidbody2D>().simulated = false;
        }
        Physics2D.simulationMode = SimulationMode2D.FixedUpdate;
    }


    private void OnGUI()
    {
        GUIStyle guistyle = new GUIStyle
        {
            fontSize = 122
        };
        if (DebugSeed)
        {
            GUILayout.Label(seed.ToString(), guistyle);
        }


    }
}
