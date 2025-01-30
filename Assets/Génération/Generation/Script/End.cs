using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class End : MonoBehaviour
{
    public bool end = false;
    public uint winnerIndex;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            end = true;
            winnerIndex = collision.gameObject.GetComponent<Player>().index;
            Server.Instance.SendEndToAll();
        }
    }

}
