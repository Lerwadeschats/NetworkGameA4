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
            if(_hpValue > value)
            {
                Instantiate(DamagesParticles, this.gameObject.transform.position, Quaternion.identity);
            }
            _hpValue = value;
            if(HpValue <= 0)
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

    public bool IsActive 
    { 
        get => _isActive; 
        set => _isActive = value;
    }

    Rigidbody2D _rb;

    [SerializeField]
    float _minDetectionRange = 10f;

    [SerializeField]
    float _minAggroRange = 10f;

    float _baseScaleX = 1;

    public int index;

    public ParticleSystem DamagesParticles;

    
    private bool _isActive;

    public Animator animator;

    

    public Vector2 GetVelocity() { return _rb.velocity; }
    public void SetVelocity(Vector2 newVelocity) { _rb.velocity = newVelocity; }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        NewEnemySetup();
        _baseScaleX = gameObject.transform.localScale.x;
        index = -1;
    }

    private void Update()
    {
        //Il va courser le joueur
        Player target = GetTarget();


        float dirMovement = 0;
        Vector2 directionVector = Vector2.zero;

        if (target != null)
        {
            IsActive = true;
            directionVector = target.gameObject.transform.position - gameObject.transform.position;
            
            if (directionVector.x <= 0)
                dirMovement = -1;
            else
                dirMovement = 1;

            if(directionVector.magnitude > _minAggroRange)
            {
                directionVector = Vector2.zero;
            }
        }
        else
        {
            IsActive = false;
        }
        if(dirMovement != 0)
        {
            gameObject.transform.localScale = new Vector3(-dirMovement * _baseScaleX, gameObject.transform.localScale.y, gameObject.transform.localScale.z);
            
        }

        Vector2 newVelocity = new Vector2(directionVector.x * _speed, _rb.velocity.y);
        _rb.velocity = newVelocity;
        SetAnimationState();



    }

    public void Death()
    {
        Server.Instance.OnEnemyDeath(index);
        
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

    void SetAnimationState()
    {
        if(Mathf.Abs(_rb.velocity.x) > 0)
        {
            animator.SetInteger("AnimState", 2);
        }
        else
        {
            if (IsActive)
            {
                animator.SetInteger("AnimState", 1);
            }
            else
            {
                animator.SetInteger("AnimState", 0);

            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _minDetectionRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _minAggroRange);

    }

    

}
