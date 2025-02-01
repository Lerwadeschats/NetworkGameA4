using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collider2pieds : MonoBehaviour
{
    [SerializeField]
    PlayerMovements _movements;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            _movements.IsOnGround = true;
        }
       
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            _movements.IsOnGround = false;
        }
        
    }
}
