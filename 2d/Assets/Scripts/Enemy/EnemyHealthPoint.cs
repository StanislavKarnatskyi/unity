using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthPoint : MonoBehaviour
{
    [SerializeField] private int enemyHP;
    //[SerializeField] protected Slider _hpSlider;
    private int currentHP;

    private Material matBlink;
    private Material matDefault;
    private SpriteRenderer spriteRender;

    void Start()
    {

        spriteRender = GetComponent<SpriteRenderer>();

        matBlink = Resources.Load("EnemyBlink", typeof(Material)) as Material;
        matDefault = spriteRender.material;

        enemyHP = currentHP;
        //_hpSlider.maxValue = _maxHp;
        //_hpSlider.value = _maxHp;
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Bullet"))
        {
            Destroy(collision.gameObject);
            enemyHP--;

            spriteRender.material = matBlink;

            if(enemyHP <= 0)
            {
                OnDeath();
            }
        }
    }

    public virtual void OnDeath()
    {
        Destroy(gameObject);
    }
}
