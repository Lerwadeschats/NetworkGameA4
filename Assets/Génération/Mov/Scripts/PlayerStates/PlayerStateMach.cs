using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class PlayerStateMach : StateMach
{
    [Space(15)]
    [HorizontalLine(4, EColor.Gray)]
    [Header("Attributs")]
    [ShowNonSerializedField] float _speed = 0;
    [ShowNonSerializedField] bool _canMove = true;
    [ShowNonSerializedField] bool _facingRight = true;

    [ShowNonSerializedField] int _jumpLeft = 0;
    [ShowNonSerializedField] int _nbJump = 2;
    [ShowNonSerializedField] int _jumpForce = 5;


    private NetworkIdentity _nwIdentity;

    private Rigidbody2D _rb;
    
    
    public float Speed { get => _speed; set => _speed = value; }
    public bool CanMove { get => _canMove; set => _canMove = value; }
    public int JumpLeft { get => _jumpLeft; set => _jumpLeft = value; }
    public bool FacingRight { get => _facingRight; set => _facingRight = value; }
    public int NbJump { get => _nbJump; set => _nbJump = value; }
    public int JumpForce { get => _jumpForce; set => _jumpForce = value; }


    public Rigidbody2D Rb { get => _rb; set => _rb = value; }
    public NetworkIdentity NwIdentity { get => _nwIdentity; set => _nwIdentity = value; }

    public override void Start()
    {
        _jumpLeft = _nbJump;
        base.Start();
        _rb = GetComponent<Rigidbody2D>();
        _nwIdentity = GetComponent<NetworkIdentity>();
    }
}
