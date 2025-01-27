using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float hpValue;
    public float maxHpValue;



    public void Death()
    {
        Destroy(gameObject);
    }

}
