using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMach : StateMach
{
    [Space(15)]
    [HorizontalLine(4, EColor.Gray)]
    [Header("Attributs")]
    public GameObject _playerToFollow;
    [ShowNonSerializedField] private float _speed = 0;
    [ShowNonSerializedField] private bool _isGoingLeft = true;
    [ShowNonSerializedField] private bool _canMove = true;

    private Rigidbody2D _rb;

    public float Speed { get => _speed; set => _speed = value; }
    public bool IsGoingLeft { get => _isGoingLeft; set => _isGoingLeft = value; }
    public bool CanMove { get => _canMove; set => _canMove = value; }
    public Rigidbody2D Rb { get => _rb; set => _rb = value; }

    public override void Start()
    {
        base.Start();
        _rb = GetComponent<Rigidbody2D>();
    }
}
