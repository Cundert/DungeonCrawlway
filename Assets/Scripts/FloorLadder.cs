using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FloorLadder : MonoBehaviour
{
    public Sprite locked, unlocked;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = locked;
    }

    // Update is called once per frame
    void Update()
    {
        if(GameController.instance.keyUI.activeInHierarchy) GetComponent<SpriteRenderer>().sprite = unlocked;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameController.instance.keyUI.activeInHierarchy && collision.gameObject.tag == "Player")
        {
            Debug.Log(GameController.instance.player.GetComponent<Player>().hp);
            GameController.instance.SetHP(GameController.instance.player.GetComponent<Player>().hp);
            SceneManager.LoadScene(2);
        }
    }
}
