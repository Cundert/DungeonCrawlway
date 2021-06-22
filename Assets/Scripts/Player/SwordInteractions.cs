using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SwordInteractions : MonoBehaviour
{
    public bool canHit = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (canHit && other.gameObject.tag == "Enemy")
        {
            other.gameObject.GetComponent<Character>().TakeDamage(GameController.instance.player.GetComponent<Player>().attack, GetComponent<Collider2D>().bounds.center);
        }
    }
}
