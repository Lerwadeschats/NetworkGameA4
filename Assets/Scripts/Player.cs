using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    private Vector2 _velocity;
    private Vector2 _position;
    private string _name;
    private Color _color;
    private PlayerStats _stats;

    private PlayerInputs _inputs;
    
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
        set { _name = value; }
    }

    public PlayerStats Stats
    {
        get { return _stats; }
        set { _stats = value; }
    }

    public PlayerInputs Inputs
    {
        get { return _inputs; }
        set { _inputs = value; }
    }

    public void SetNewPlayerInfos(string name, Color color, Vector2 position)
    {
        _name = name;
        _color = color;
        _position = position;
        _stats = new PlayerStats(10f, 100f, 100f, 20f);
        _inputs = new PlayerInputs();
    }

    private void Start()
    {
        SetNewPlayerInfos("Name", Color.white, Vector2.zero);
    }


    public void UpdateVelocity(Vector2 velocity)
    {
        _velocity = velocity;
    }

    public void UpdatePosition(Vector2 position)
    {
        _position = position;
    }

    public void UpdateStats(PlayerStats stats)
    {
        
        this._stats.attackValue = stats.attackValue;
        this._stats.hpValue = stats.hpValue;
        this._stats.maxHpValue = stats.maxHpValue;
        this._stats.speed = stats.speed;
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

    /*public PlayerInputs(bool moveRightInput, bool moveLeftInput, bool jumpInput, bool attackInput, bool blockInput, bool dashInput)
    {
        moveRight = moveRightInput;
        moveLeft = moveLeftInput;
        jump = jumpInput;
        attack = attackInput;
        block = blockInput;
        dash = dashInput;
    }*/
}