using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

	public static GameController instance = null;

	[SerializeField]
	private GameObject hpBar;
	public GameObject player, playerPrefab, keyUI;
    public roomGenerator rg;

    private bool first_instantiation = true;

    private int floor_count = 0;

    private int r, s; // powerups and debuffs

    public int p_hp_index = 0;
    public int p_dex_index = 0;
    public int p_sp_index = 0;
    public int e_dens_index = 0;
    public int e_hp_index = 0;
    public int e_str_index = 0;

    private int hp = 100;

    public int GetHP()
    {
        return hp;
    }

    public void SetHP(int hp)
    {
        this.hp = hp;
    }

    public void SetRS(int r, int s)
    {
        this.r = r;
        this.s = s;
    }

    public bool GetFirstInstantiation()
    {
        return first_instantiation;
    }

    public void instantiatePlayer(Vector3 position) {
        hpBar = GameObject.Find("Bar");
        player = Instantiate(playerPrefab, position, Quaternion.identity);
        floor_count++;
        if (first_instantiation) { 
            first_instantiation = false;
        }
        else
        {
            if (r == 1) p_hp_index++;
            else if (r == 2) p_dex_index++;
            else p_sp_index++;
            if (s == 1) e_dens_index++;
            else if (s == 2) e_hp_index++;
            else e_str_index++;
            
            player.GetComponent<Player>().hp = hp + r == 1 ? 10 : 0;
            player.GetComponent<Player>().maxHp = 100 + 10 * p_hp_index;
			player.GetComponent<Player>().attack=25+15*(p_dex_index > 0 ? 1 : 0)+5*Mathf.Max(0, p_dex_index-1);
			player.GetComponent<Player>().SetSpeed(5 + 0.4f * p_sp_index);
        }
    }

	public void PlayAudio(int index) {
		AudioSource[] list = GetComponents<AudioSource>();
		list[index].Play();
	}

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.Quit();
		}
		if (hpBar!=null) {
			if (player==null) {
				hpBar.GetComponent<Image>().fillAmount=0;
			} else {
				hpBar.GetComponent<Image>().fillAmount=(float)player.GetComponent<Player>().hp/(float)player.GetComponent<Player>().maxHp;
			}
		}
    }

    public int GetFloorCount()
    {
        return floor_count;
    }

    public void Reset()
    {
        p_hp_index = 0;
        p_dex_index = 0;
        p_sp_index = 0;
        e_dens_index = 0;
        e_hp_index = 0;
        e_str_index = 0;
        hp = 100;
        floor_count = 0;
        first_instantiation = true;
		GetComponent<AudioSource>().Stop();
    }
}
