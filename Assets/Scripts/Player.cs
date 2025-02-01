using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField]
    private Vector2 _velocity;
    private Vector2 _position;
    private string _name;
    private Color _color;
    private PlayerStats _stats;

    private PlayerInputs _inputs;

    public uint index = 255;


    public PlayerMovements _playerMovements;
    public PlayerAttacks _playerAttacks;
    public PlayerDamageManager _playerDamages;
    
    public struct PlayerStats
    {
        public float attackValue;
        public float hpValue;
        public float maxHpValue;
        public float speed;

        public PlayerStats(float atk, float hp, float maxHp, float spd)
        {
            attackValue = atk;
            hpValue = hp;
            maxHpValue = maxHp;
            speed = spd;
        }
    }

    public Vector2 Velocity
    {
        get { return _velocity; }
        set { _velocity = value; }
    }

    public Vector2 Position
    {
        get { return _position; }
        set { _position = value; }
    }

    public Color Color
    {
        get { return _color; }
        set { _color = value; }
    }

    public string Name
    {
        get { return _name; }
        set 
        { 
            _name = value;
            _nickName.text = value;
        }
    }

    public PlayerStats Stats
    {
        get { return _stats; }
        set 
        {
            if (_stats.hpValue > value.hpValue)
            {
                _playerDamages.GetHurtEffects();
            }
            
            _stats = value; 

            if(_stats.hpValue <= 0)
            {
                Respawn();
            }
        }
    }

    public PlayerInputs Inputs
    {
        get { return _inputs; }
        set { _inputs = value; }
    }

    [SerializeField]
    TextMeshProUGUI _nickName;

    public Animator animator;
    
    private void Start()
    {
        SetNewPlayerInfos();
    }

    public void SetNewPlayerInfos()
    {
        _position = gameObject.transform.position;
        _stats = new PlayerStats(10f, 100f, 100f, 10f);
        _inputs = new PlayerInputs();
    }

    void Respawn()
    {
        gameObject.transform.position = GameManager.instance.GetSpawnPosition();
        SetNewPlayerInfos();
    }
}




public class PlayerInputs
{
    public bool moveRight;
    public bool moveLeft;
    public bool jump;
    public bool attack;
    public bool block;
    public bool dash;

    

}