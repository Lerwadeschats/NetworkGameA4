using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Enemy : MonoBehaviour
{
    private float _hpValue;
    private float _maxHpValue;
    private float _attack;
    private float _speed;

   

    public void NewEnemySetup()
    {
        _hpValue = 100f;
        _maxHpValue = 100f;
        _attack = 20f;
        _speed = 10f;
    }

    public float HpValue
    {
        get { return _hpValue; }
        set { _hpValue = value; }
    }

    public float MaxHpValue
    {
        get { return _maxHpValue; }
        set { _maxHpValue = value; }
    }

    public float Attack
    {
        get { return _attack; }
        set { _attack = value; }
    }

    public float Speed
    {
        get { return _speed; }
        set { _speed = value; }
    }

    Rigidbody2D _rb;

    [SerializeField]
    float _minDetectionRange = 10f;

    

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        NewEnemySetup();
    }

    private void Update()
    {
        //Il va courser le joueur
        Player target = GetTarget();

        float dirMovement = 0;

        if(target != null)
        {
            Vector2 directionVector = target.gameObject.transform.position - gameObject.transform.position;
            if (directionVector.x <= 0)
                dirMovement = -1;
            else
                dirMovement = 1;
        }
        if(dirMovement != 0)
        {
            gameObject.transform.localScale = new Vector3(dirMovement, gameObject.transform.localScale.y, gameObject.transform.localScale.z);
        }
        
        Vector2 newVelocity = new Vector2(dirMovement * _speed, _rb.velocity.y);
        _rb.velocity = newVelocity;

    }

    public void Death()
    {
        Destroy(gameObject);
    }

    public Player GetTarget()
    {
        Player[] players = FindObjectsOfType<Player>();
        float smallestDist = -1;
        Player nearestPlayer = null;
        foreach (Player p in players)
        {
            if(p != null)//On sait jamais
            {
                float dist = Vector2.Distance(gameObject.transform.position, p.gameObject.transform.position);
                if((smallestDist == -1 || dist < smallestDist) && dist < _minDetectionRange)
                {

                    nearestPlayer = p;
                    smallestDist = dist;
                }
            }
                
        }
        return nearestPlayer;
    }

    

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _minDetectionRange);

    }

}
