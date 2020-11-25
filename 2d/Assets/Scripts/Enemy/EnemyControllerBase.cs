using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]

public abstract class EnemyControllerBase : MonoBehaviour
{
    protected Rigidbody2D _enemyRB;
    protected Animator _enemyAnimator;
    protected Vector2 _startPoint;

    [Header("HP")]
    [SerializeField] protected int _maxHp;
    //[SerializeField] protected Slider _hpSlider;
    protected int _currentHp;

    [Header("State")]
    [SerializeField] private float _maxStateTime;
    [SerializeField] private float _minStateTime;
    [SerializeField] private EnemyState[] _availableState;
    [SerializeField] private DamageType _collisionDamageType;


    [Header("Move")]
    [SerializeField] private float _speed;
    [SerializeField] private float _range;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private LayerMask _whatIsGround;

    [SerializeField] protected int _collisionDamage;
    [SerializeField] protected float _collisionTimeDelay;
    private float _lastDamageTime;

    protected bool _faceRight = true;

    private float _lastStateChange;
    private float _timeToNextChange;

    
    protected EnemyState _currentState;

    #region UnityMethods
    protected virtual void Start()
    {
        _startPoint = transform.position;
        _enemyRB = GetComponent<Rigidbody2D>();
        _enemyAnimator = GetComponent<Animator>();
        _currentHp = _maxHp;
        //_hpSlider.maxValue = _maxHp;
        //_hpSlider.value = _maxHp;
    }

    protected virtual void FixedUpdate()
    {
        if (_currentState == EnemyState.Death)
            return;

        if (IsGroundEnding())
            Flip();

        if (_currentState == EnemyState.Move)
            Move();
    }

    protected virtual void Update()
    {
        if (_currentState == EnemyState.Death)
            return;

        if (Time.time - _lastStateChange > _timeToNextChange)
            GetRandomState();
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (_currentState == EnemyState.Death)
            return;
        TryToDamage(collision.collider);
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(_range * 2, 0.5f, 0));
    }
    #endregion
    #region PublicMethods
    public virtual void TakeDamage(int damage, DamageType type = DamageType.Casual, Transform player = null)
    {
        if (_currentState == EnemyState.Death)
            return;

        _currentHp -= damage;
        Debug.Log(string.Format("Enemy {0} take damage {1} and his currentHP = {2}", gameObject, damage, _currentHp));
        if (_currentHp <= 0)
        {
            _currentHp = 0;
            //_hpSlider.value = _currentHp;
            ChangeState(EnemyState.Death);
        }
       
    }

    public virtual void OnDeath()
    {
        Destroy(gameObject);
    }
    #endregion 
    #region PrivateMethods
    protected virtual void ChangeState(EnemyState state)
    {
        if (_currentState == EnemyState.Death)
            return;

        ResetState();
        _currentState = EnemyState.Idle;

        if (state != EnemyState.Idle)
            _enemyAnimator.SetBool(state.ToString(), true);
        
        _currentState = state;
        _lastStateChange = Time.time;

        switch (_currentState)
        {
            case EnemyState.Idle:
                _enemyRB.velocity = Vector2.zero;
                break;
            case EnemyState.Death:
                DisableEnemy();
                break;
        }
    }

    protected virtual void EndState()
    {
        if (_currentState == EnemyState.Death)
            OnDeath();

        ResetState();
    }

    protected virtual void ResetState()
    {
        _enemyAnimator.SetBool(EnemyState.Move.ToString(), false);
        _enemyAnimator.SetBool(EnemyState.Death.ToString(), false);
    }

    protected virtual void DisableEnemy()
    {
        _enemyRB.velocity = Vector2.zero;
        _enemyRB.bodyType = RigidbodyType2D.Static;
        GetComponent<Collider2D>().enabled = false;
    }

    protected void GetRandomState()
    {
        if (_currentState == EnemyState.Death)
            return;

        int state = Random.Range(0, _availableState.Length);

        if (_currentState == EnemyState.Idle && _availableState[state] == EnemyState.Idle)
        {
            GetRandomState();
        }

        _timeToNextChange = Random.Range(_minStateTime, _maxStateTime);
        ChangeState(_availableState[state]);
    }

     protected virtual void TryToDamage(Collider2D enemy)
    {
        if ((Time.time - _lastDamageTime) < _collisionTimeDelay)
            return;

        Player_Controller player = enemy.GetComponent<Player_Controller>();
        if (player != null)
            player.TakeDamage(_collisionDamage, _collisionDamageType, transform);
    }

    protected virtual void Move()
    {
        if (_currentState == EnemyState.Death)
            return;
        _enemyRB.velocity = transform.right * new Vector2(_speed, _enemyRB.velocity.y);
    }

    protected void Flip()
    {
        if (_currentState == EnemyState.Death)
            return;
        _faceRight = !_faceRight;
        transform.Rotate(0, 180, 0);
        //_canvas.transform.Rotate(0, 180, 0);
    }

    private bool IsGroundEnding()
    {
        return !Physics2D.OverlapPoint(_groundCheck.position, _whatIsGround);
    }
    #endregion
}

public enum EnemyState
{
    Idle, 
    Move,
    Shoot,
    Strike,
    PowerStrike,
    Hurt,
    Death,
}