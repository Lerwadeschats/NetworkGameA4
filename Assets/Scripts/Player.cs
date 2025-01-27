using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    private Vector2 _velocity;
    private Vector2 _position;
    private string _name;
    private Color _color;
    private Stats _stats;

    public struct Stats
    {
        public float attackValue;
        public float hpValue;
        public float maxHpValue;
        public float speed;
    }


    public Player(string name, Color color, Vector2 position)
    {
        _name = name;
        _color = color;
        _position = position;
        _stats = NewStats();
    }

    public void SetNewPlayerInfos(string name, Color color, Vector2 position)
    {
        _name = name;
        _color = color;
        _position = position;
        _stats = NewStats();
    }

    private Stats NewStats()
    {
        Stats stats = new Stats();
        stats.attackValue = 10f;
        stats.hpValue = 100f;
        stats.maxHpValue = 100f;
        stats.speed = 100f;
        return stats;
    }

    public string GetName()
    {
        return _name;
    }

    public Color GetColor()
    {
        return _color;
    }
    public Vector2 GetPosition()
    {
        return _position;
    }

    public Vector2 GetVelocity()
    {
        return _velocity;
    }

    public Stats GetStats()
    {
        return _stats;
    }

    private void UpdateVelocity(Vector2 velocity)
    {
        _velocity = velocity;
    }

    private void UpdatePosition(Vector2 position)
    {
        _position = position;
    }

    private void UpdateStats(Stats stats)
    {
        this._stats.attackValue = stats.attackValue;
        this._stats.hpValue = stats.hpValue;
        this._stats.maxHpValue = stats.maxHpValue;
    }

    
}
