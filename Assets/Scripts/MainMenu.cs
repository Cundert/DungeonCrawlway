using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene(1);
			if (GameController.instance!=null && !GameController.instance.GetComponent<AudioSource>().isPlaying) GameController.instance.PlayAudio(0);
		}
    }
}
