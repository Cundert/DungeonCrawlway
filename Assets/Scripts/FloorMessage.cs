using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FloorMessage : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int floor = GameController.instance.GetFloorCount();
        string th = floor % 10 == 1 && floor != 11 ? "st" : (floor % 10 == 2 && floor != 12 ? "nd" : (floor % 10 == 3 && floor != 13 ? "rd" : "th"));
        GetComponent<UnityEngine.UI.Text>().text = "You cleared the " + floor + th + " floor!";

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {

            SceneManager.LoadScene(3);
        }

    }
}
