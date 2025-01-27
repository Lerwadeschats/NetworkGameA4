using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitTypeNameSpace;

public class Room : MonoBehaviour
{
    public BoxCollider2D _boxCollider;
    public Exit[] _exits;
    public Exit[] GetExits()
    {
        Exit[] n = new Exit[transform.childCount];
        n = GetComponentsInChildren<Exit>();
        return n;
    }
}
