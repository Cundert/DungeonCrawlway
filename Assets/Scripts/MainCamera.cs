using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{

    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameController.instance.player;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.instance.player != null)
        {
            transform.position = GameController.instance.player.transform.position + new Vector3(GameController.instance.player.GetComponent<SpriteRenderer>().bounds.size.x / 2, 0, -20);
            /*if (player.GetComponent<Player>().GetFacingLeft())
                transform.position -= new Vector3(player.GetComponent<SpriteRenderer>().bounds.size.x, 0, 0);*/
        }
    }
}
