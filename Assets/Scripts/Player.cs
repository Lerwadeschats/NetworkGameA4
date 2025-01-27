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

    public struct PlayerStats
    {
        public float attackValue;
        public float hpValue;
        public float maxHpValue;
        public float speed;
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


    public void SetNewPlayerInfos(string name, Color color, Vector2 position)
    {
        _name = name;
        _color = color;
        _position = position;
        _stats = NewStats();
    }

    private void Start()
    {
        SetNewPlayerInfos("Name", Color.white, Vector2.zero);
    }

    private PlayerStats NewStats()
    {
        PlayerStats stats = new PlayerStats();
        stats.attackValue = 10f;
        stats.hpValue = 100f;
        stats.maxHpValue = 100f;
        stats.speed = 20f;
        return stats;
    }


    private void UpdateVelocity(Vector2 velocity)
    {
        _velocity = velocity;
    }

    private void UpdatePosition(Vector2 position)
    {
        _position = position;
    }

    private void UpdateStats(PlayerStats stats)
    {
        this._stats.attackValue = stats.attackValue;
        this._stats.hpValue = stats.hpValue;
        this._stats.maxHpValue = stats.maxHpValue;
    }

    
}
