using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movement_Controller))]
public class PC_Movement_Controller : MonoBehaviour
{
    Movement_Controller _playerMovement;

    float _move;  
    bool _jump;
    bool _crouch;

    private void Start()
    {
        _playerMovement = GetComponent<Movement_Controller>();
    }

    void Update()
    {
        _move = Input.GetAxisRaw("Horizontal");
        if (Input.GetButton("Jump"))
        {
            _jump = true;
        }

        _crouch = Input.GetKey(KeyCode.C);
    }

    private void FixedUpdate()
    {
        _playerMovement.Move(_move, _jump, _crouch);
        _jump = false;
    }
}
