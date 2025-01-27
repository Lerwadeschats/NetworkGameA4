using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class DCMapGen : MonoBehaviour
{
    public int _nbOfRoom = 30;
    public int _nbOfRoomBranch = 4;
    public int _branchNumber = 2;
    public float _timeBetweenRoom = .1f;
    public int _chanceToBranch = 50;
    public List<GameObject> _rooms = new List<GameObject>();

    List<Room> _branches = new List<Room>();

    Cam _camera;

    private void Start()
    {
        _camera = Camera.main.GetComponent<Cam>();
        StartCoroutine(GenerateBranch(true , transform.GetChild(0).GetChild(Random.Range(0, 4)).GetComponent<Exit>(),Color.red,_timeBetweenRoom));
        //Physics2D.simulationMode = SimulationMode2D.FixedUpdate;

    }
    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Space)) 
        {
            StopAllCoroutines();
            _camera.Targets.Clear();
            for(int i = transform.childCount- 1; i>0; i--) { Destroy(transform.GetChild(i).gameObject); }
            _camera.Targets.Add(transform.GetChild(0).gameObject);
            StartCoroutine(GenerateBranch(true, transform.GetChild(0).GetChild(Random.Range(0, 2)).GetComponent<Exit>(), Color.red, _timeBetweenRoom));
        }

    }

    private IEnumerator GenerateBranch(bool isMain,Exit focus, Color endCol, float timeToWait)
    {
        Physics2D.simulationMode = SimulationMode2D.Script;
        int numberOfRooms = isMain? _nbOfRoom: _nbOfRoomBranch;
        int numberOfbranchCreated = 0;
        Transform parent = isMain? focus.transform.parent.parent: focus.transform.parent;
        for (int i = 1; i <= numberOfRooms; i++)
        {
            List<GameObject> possibleRoom = _rooms;
            RetryRoom:
            if(possibleRoom.Count == 0) 
            {
                print("OutOfRooms " + isMain);
                break;
            }

            int roomSeed = Random.Range(0, possibleRoom.Count);
            GameObject go = Instantiate(possibleRoom[roomSeed]);
            GameObject goCopy = possibleRoom[roomSeed];
            Room room = go.GetComponent<Room>();
            List<Exit> roomExits = room.GetExits().ToList();
            List<Exit> matchingExits = roomExits.Where(ex => ex._type == focus._compatibleType).ToList();

            if(matchingExits.Count == 0) // No exits Match
            { 
                Destroy(go);
                possibleRoom = possibleRoom.Where(ro => ro != goCopy).ToList();
                goto RetryRoom;
            }

            int exitSeed = Random.Range(0, matchingExits.Count);
            Exit exit = matchingExits[exitSeed];
            go.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.white, endCol, (float)i / _nbOfRoom);
            go.transform.parent = parent;
            go.transform.position = focus.transform.position - exit.transform.position;
            go.name += " "+ i;
            Physics2D.simulationMode = SimulationMode2D.Script;
            Physics2D.Simulate(_timeBetweenRoom);

            Collider2D[] contact = Physics2D.OverlapBoxAll(go.transform.position, go.transform.localScale, 0f);
            contact = contact.Where(col => col.name != go.name).ToArray();

            if (contact.Length > 0) // Room is touching another one
            {
                Destroy(go);
                possibleRoom = possibleRoom.Where(ro => ro != goCopy).ToList();
                goto RetryRoom; 
            }

            roomExits = roomExits.Where(ex => ex._type != focus._compatibleType).ToList();
            RetryExit:
            if(roomExits.Count == 0) // Room do not have any exits left
            {
                Destroy(go);
                possibleRoom = possibleRoom.Where(ro => ro != goCopy).ToList();
                goto RetryRoom;
            }
            int exitIndex = -1;
            foreach(Exit ex in roomExits) // test if exit is legal
            {
                exitIndex = roomExits.FindIndex(e => e == ex);
                if (ex.CanPlaceARoom())
                {
                    break;
                }
                else
                {
                    Destroy(ex.gameObject);
                    roomExits.RemoveAt(exitIndex);
                    goto RetryExit;
                }
            }
            
            _camera.Targets.Add(go);
            focus = roomExits[exitIndex];
            roomExits.RemoveAt(exitIndex);
            if (roomExits.Count > 0 && isMain && Random.Range(0f,100) < _chanceToBranch && numberOfbranchCreated <_branchNumber)
            {
                numberOfbranchCreated++;
                exitIndex = Random.Range(0, roomExits.Count);
                StartCoroutine(GenerateBranch(false,roomExits[exitIndex],Color.blue, timeToWait));
            }
            yield return new WaitForSeconds(timeToWait);
        }
        Physics2D.simulationMode = SimulationMode2D.FixedUpdate;
    }
}
