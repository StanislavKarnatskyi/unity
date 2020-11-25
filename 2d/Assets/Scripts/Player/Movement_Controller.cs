using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(AudioSource))]
[RequireComponent(typeof(Player_Controller))]
public class Movement_Controller : MonoBehaviour
{
    private Rigidbody2D _playerRB;
    private Animator _playerAnimation;
    private Player_Controller _playerController;

    public event Action<bool> OnGetHurt = delegate { };

    [Header("Horizontal movement")]
    [Range(0, 1)]
    [SerializeField] private float _crouchSpeedReduce;
    [SerializeField] float _speed;
    private bool _faceRight = true;
    private bool _canMove = true;

    [Header("Jumping")]
    [SerializeField] float _jumpForce;
    [SerializeField] Transform _groundCheck;
    [SerializeField] float _radius;
    [SerializeField] LayerMask _whatIsGround;
    [SerializeField] bool _airControl;
    bool _grounded;
    private bool _canDoubleJump;

    [Header("Crawling")]
    [SerializeField] Collider2D _headCollider;
    [SerializeField] Transform _cellCheck;
    bool _canStand;

    [Header("Casting")]
    [SerializeField] private GameObject _fireBall;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _fireBallSpeed;
    [SerializeField] private int _castCost;
    private bool _IsCasting;

    [Header("Strike")]
    [SerializeField] private Transform _strikePoint;
    [SerializeField] private int _damage;
    [SerializeField] private float _attackRange;
    [SerializeField] private LayerMask _enemies;
    private bool _isAttack;

    [Header("PowerStrike")]
    [SerializeField] private float _chargeTime;
    public float ChargeTime => _chargeTime;
    [SerializeField] private float _powerStrikeSpeed;
    [SerializeField] private Collider2D _strikeCollider;
    [SerializeField] private int _powerStrikeDamage;
    [SerializeField] private int _powerStrikeCost;
    private List<EnemyControllerBase> _damageEnemies = new List<EnemyControllerBase>();

    [Header("Audio")]
    [SerializeField] private InGameSound _runClip;
    private InGameSound _currentSound;
    private AudioSource _audioSource;

    [SerializeField] private float _pushForce;
    private float _lastHurtTime;

    void Start()
    {
        _playerRB = GetComponent<Rigidbody2D>();
        _playerAnimation = GetComponent<Animator>();
        _playerController = GetComponent<Player_Controller>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        _grounded = Physics2D.OverlapCircle(_groundCheck.position, _radius, _whatIsGround);

        if (_playerAnimation.GetBool("Hurt") && _grounded && Time.time - _lastHurtTime > 0.5f)
            EndHurt();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(_groundCheck.position, _radius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_cellCheck.position, _radius);
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(_strikePoint.position, _attackRange);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        EnemyControllerBase enemy = collision.collider.GetComponent<EnemyControllerBase>();
        if (enemy == null || _damageEnemies.Contains(enemy))
            return;

        enemy.TakeDamage(_powerStrikeDamage, DamageType.PowerStrike);
        _damageEnemies.Add(enemy);
    }

    void Flip()
    {
        _faceRight = !_faceRight;
        transform.Rotate(0, 180, 0);
    }

    public void Move(float move, bool jump, bool crouch)
    {
        if (!_canMove)
            return;

        #region Movement
        float speedModificator = _headCollider.enabled ? 1 : _crouchSpeedReduce;

        if ((_grounded || _airControl))
            _playerRB.velocity = new Vector2(_speed * move * speedModificator, _playerRB.velocity.y);

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

        if (jump)
        {
            if (_grounded)
            {
                _playerRB.velocity = new Vector2(_playerRB.velocity.x, _jumpForce);
                _canDoubleJump = true;
            }
            else if (_canDoubleJump)
            {
                _playerRB.velocity = new Vector2(_playerRB.velocity.x, _jumpForce);
                _canDoubleJump = false;
            }
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

        #region Animator
        _playerAnimation.SetFloat("Speed", Mathf.Abs(move));
        _playerAnimation.SetBool("Crouch", !_headCollider.enabled);
        if(!_grounded)
        {
            _playerAnimation.SetBool("Jump", true);
        }
        else
        {

            _playerAnimation.SetBool("Jump", false);
        }
        #endregion
        
        if (_grounded && _playerRB.velocity.x != 0 && !_audioSource.isPlaying)
            PlayAudio(_runClip);
        else if (!_grounded || _playerRB.velocity.x == 0)
            StopAudio(_runClip);
    }
    
    public void PlayAudio(InGameSound sound)
    {
        if (_currentSound != null && (_currentSound == sound || _currentSound.Priority > sound.Priority))
            return;

        _currentSound = sound;
        _audioSource.clip = _currentSound.AudioClip;
        _audioSource.loop = _currentSound.Loop;
        //_audioSource.pitch = _currentSound.Pitch;
        //_audioSource.volume = _currentSound.Volume;
        _audioSource.Play();
    }

    public void StopAudio(InGameSound sound)
    {
        if (_currentSound == null || _currentSound != sound)
            return;

        _audioSource.Stop();
        _audioSource.clip = null;
        _currentSound = null;
    }
    
    public void StartCasting()
    {
        if (_IsCasting || !_playerController.ChangeMP(-_castCost))
            return;
        _IsCasting = true;
        _playerAnimation.SetBool("Casting", true);
    }

    private void FireCast()
    {
        GameObject fireBall = Instantiate(_fireBall, _firePoint.position, Quaternion.identity);
        fireBall.GetComponent < Rigidbody2D>().velocity = transform.right * _fireBallSpeed;
        fireBall.GetComponent<SpriteRenderer>().flipX = !_faceRight;
        Destroy(fireBall, 1f);
    }

    private void EndAnimation()
    {
        _playerAnimation.SetBool("Strike", false);
        _playerAnimation.SetBool("PowerStrike", false);
        _playerAnimation.SetBool("Casting", false);
    }

    private void EndCasting()
    {
        _IsCasting = false;
        _playerAnimation.SetBool("Casting", false);
    }

    public void StartStrike(float holdTime)
    {
        if (_isAttack)
            return;
        if(holdTime >= _chargeTime)
        {
            if (!_playerController.ChangeMP(-_powerStrikeCost))
                return;
            _playerAnimation.SetBool("PowerStrike", true);
            _canMove = false;
        }
        else
        {
            _playerAnimation.SetBool("Strike", true);
        }
        _isAttack = true;
    }

    public void GetHurt(Vector2 position)
    {
        _lastHurtTime = Time.time;
        _canMove = false;
        OnGetHurt(false);
        Vector2 pushDirection = new Vector2();
        pushDirection.x = position.x > transform.position.x ? -1 : 1;
        pushDirection.y = 1;
        EndAnimation();
        _playerAnimation.SetBool("Hurt", true);
        _playerRB.AddForce(pushDirection * _pushForce, ForceMode2D.Impulse);
    }

    private void EndHurt()
    {
        _canMove = true;
        _playerAnimation.SetBool("Hurt", false);
        OnGetHurt(true);
    }

    private void StartPowerStrike()
    {
        _playerRB.velocity = transform.right * _powerStrikeSpeed;
        _strikeCollider.enabled = true;
    }

    private void DisablePowerStrike()
    {
        _playerRB.velocity = Vector2.zero;
        _strikeCollider.enabled = false;
        _damageEnemies.Clear();
    }

    private void EndPowerStrike()
    {
        _playerAnimation.SetBool("PowerStrike", false);
        _canMove = true;
        _isAttack = false;
    }


    private void Strike()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(_strikePoint.position, _attackRange, _enemies);
        for(int i =0; i<enemies.Length;i++)
        {
           EnemyControllerBase enemy = enemies[i].GetComponent<EnemyControllerBase>();
           if(enemy!=null)
             enemy.TakeDamage(_damage);
        }
    }

    public void EndStrike()
    {
        _playerAnimation.SetBool("Strike", false);
        _isAttack = false;
    }

    private void ResetPlayer()
    {
        _playerAnimation.SetBool("Strike", false);
        _playerAnimation.SetBool("PowerStrike", false);
        _playerAnimation.SetBool("Casting", false);
        _playerAnimation.SetBool("Hurt", false);
        _IsCasting = false;
        _isAttack = false;
        _canMove = false;
    }
}