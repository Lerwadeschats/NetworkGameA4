using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
        _speed = 1f;
    }

    public float HpValue
    {
        get { return _hpValue; }
        set 
        {
            _hpValue = value;
            if( _hpValue <= 0)
            {
                Death();
            }
        }
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

    float _baseScaleX = 1;

    public int index = -1;

    [SerializeField]
    private bool _isActive;

    public bool IsActive()
    {
        return _isActive;
    }

    public Vector2 GetVelocity() { return _rb.velocity; }
    public void SetVelocity(Vector2 newVelocity) { _rb.velocity = newVelocity; }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        NewEnemySetup();
        _baseScaleX = gameObject.transform.localScale.x;
    }

    private void Update()
    {
        //Il va courser le joueur
        Player target = GetTarget();

        float dirMovement = 0;

        if(target != null)
        {
            _isActive = true;
            Vector2 directionVector = target.gameObject.transform.position - gameObject.transform.position;
            if (directionVector.x <= 0)
                dirMovement = -1;
            else
                dirMovement = 1;
        }
        else
        {
            _isActive = false;
        }
        if(dirMovement != 0)
        {
            gameObject.transform.localScale = new Vector3(dirMovement * _baseScaleX, gameObject.transform.localScale.y, gameObject.transform.localScale.z);
        }
        
        Vector2 newVelocity = new Vector2(dirMovement * _speed, _rb.velocity.y);
        _rb.velocity = newVelocity;

    }

    public void Death()
    {
        DestroyImmediate(gameObject);
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
