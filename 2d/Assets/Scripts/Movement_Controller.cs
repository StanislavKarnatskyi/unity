using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement_Controller : MonoBehaviour
{
    Rigidbody2D _playerRB;

    [Header("Horizontal movement")]
    [Range(0, 1)]
    [SerializeField] private float _crouchSpeedReduce;
    [SerializeField] float _speed;
    bool _faceRight = true;

    [Header("Jumping")]
    [SerializeField] float _jumpForce;
    [SerializeField] Transform _groundCheck;
    [SerializeField] float _radius;
    [SerializeField] LayerMask _whatIsGround;
    [SerializeField] bool _airControl;
    bool _grounded;

    [Header("Crawling")]
    [SerializeField] Collider2D _headCollider;
    [SerializeField] Transform _cellCheck;
    bool _canStand;


    void Start()
    {
        _playerRB = GetComponent<Rigidbody2D>();
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(_groundCheck.position, _radius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_cellCheck.position, _radius);
    }

    void Flip()
    {
        _faceRight = !_faceRight;
        transform.Rotate(0, 180, 0);
    }

    public void Move(float move, bool jump, bool crouch)
    {
        #region Movement

        Vector3 fakeFriction = _playerRB.velocity;
        fakeFriction.y = _playerRB.velocity.y;
        fakeFriction.z = 0.0f;
        fakeFriction.x *= 0.5f;

        if (_grounded)
        {
            _playerRB.velocity = fakeFriction;
        }

        float speedModificator = crouch ? _crouchSpeedReduce : 1;

        if (move != 0 && (_grounded || _airControl))
            _playerRB.velocity = new Vector2(_speed * speedModificator * move, _playerRB.velocity.y);

        if (move > 0 && !_faceRight)
        {
            Flip();
        }
        else if (move < 0 && _faceRight)
        {
            Flip();
        }
        #endregion

        #region Jumping

        _grounded = Physics2D.OverlapCircle(_groundCheck.position, _radius, _whatIsGround);

        if (jump && _grounded)
        {
            _playerRB.AddForce(Vector2.up * _jumpForce);
            jump = false;
        }
        #endregion

        #region Crawling
        _canStand = !Physics2D.OverlapCircle(_cellCheck.position, _radius, _whatIsGround);

        if (crouch)
        {
            _headCollider.enabled = false;
        }
        else if (!crouch && _canStand)
        {
            _headCollider.enabled = true;
        }
        #endregion
    }
}
