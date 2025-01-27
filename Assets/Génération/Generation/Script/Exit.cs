using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitTypeNameSpace;
public class Exit : MonoBehaviour
{
    public Vector3 _pos = Vector3.zero;
    public ExitType _type;
    public ExitType _compatibleType;

    public bool CanPlaceARoom()
    {
        Vector3 v3 = Vector3.zero;
        switch(_type)
        {
            case ExitType.Top:
                v3 = Vector3.up;
                break;
            case ExitType.Bot:
                v3 = Vector3.down;
                break;
            case ExitType.Left:
                v3 = Vector3.left;
                break;
            case ExitType.Right:
                v3 = Vector3.right;
                break;
        }
        RaycastHit2D hit = Physics2D.Raycast(transform.position, v3, .5f);
        Debug.DrawRay(transform.position, v3, Color.red, 3f);
        //if(hit) Debug.Log(transform.name + " " + hit.collider.name);
        return !hit;
    }

    private void OnValidate()
    {
        _pos = transform.position;
        switch (_type)
        {
            case ExitType.Top:
                _compatibleType = ExitType.Bot; break;
            case ExitType.Bot:
                _compatibleType = ExitType.Top; break;
            case ExitType.Right:
                _compatibleType = ExitType.Left; break;
            case ExitType.Left:
                _compatibleType = ExitType.Right; break;
        }
    }
}
namespace ExitTypeNameSpace
{
    public enum ExitType
    {
        Top,
        Bot,
        Left,
        Right,
    }
}