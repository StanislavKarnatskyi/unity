using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class HorizontalMover : MonoBehaviour
{
    [SerializeField] public float speed;
    [SerializeField] public float range;
    private bool moveRight = true;
    public Transform groundCheck;
    private Animator enemyAnimator;

    private void Start()
    {
        enemyAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);

        RaycastHit2D groundCheckInfo = Physics2D.Raycast(groundCheck.position, Vector2.down, range);

        if(groundCheckInfo.collider == false)
        {
            if (moveRight == true)
            {
                transform.eulerAngles = new Vector3(0, 180, 0);
                moveRight = false;
            }
            else
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
                moveRight = true;
            }
        }

        Animate();
    }

    private void Animate()
    {
        enemyAnimator.SetFloat("Speed", Mathf.Abs(speed));
    }
}
