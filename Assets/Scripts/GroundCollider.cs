using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCollider : MonoBehaviour
{
	void OnTriggerStay2D(Collider2D other) {
        if(other.gameObject.tag == "Player") {
            other.GetComponent<Player>().quantum = 10;
        }
	}
}
