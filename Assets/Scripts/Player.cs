using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : Character
{
    public int quantum;
	public GameObject slash;
	private ParticleActivate dashParticles;
	private ParticleActivate swordParticles;
    private int DISTANCE_TO_SLASH = 1;
    public float slash_duration;
    private float current_slash = 0f;
    private Transform swordTransform;
    [SerializeField]
    private float sword_velocity = 100.0f;
    [SerializeField]
    private float sword_distance = 0.58f;

    private Quaternion attack_direction;
    private Vector3 attack_direction_v3;


    private bool attacking = false;
    private bool facing_left = false;
    private bool change_dir = false;
	[SerializeField]
	private float dash_duration;
	[SerializeField]
	private float dash_speed_bonus;
	[SerializeField]
	private float post_dash_cooldown;
	private float post_dash_cooldown_timer = 0.0f;
	private float current_dash = 0.0f;
	private Vector2 dash_direction;
	public Vector2 last_movement;

	protected override void Start() {

        Vector2 centerHB = new Vector2(GetComponent<Collider2D>().bounds.center.x - transform.position.x - 0.1f, 
                                       GetComponent<Collider2D>().bounds.center.y - transform.position.y + 0.2f * GetComponent<Collider2D>().bounds.size.y);
        Vector2 sizeHB = new Vector2(1.05f * GetComponent<Collider2D>().bounds.size.x, 0.75f * GetComponent<Collider2D>().bounds.size.y);
        SetWallCollidingHitbox(centerHB, sizeHB);
        

        quantum = 10;
        swordTransform = transform.GetChild(0);
        swordParticles = swordTransform.GetComponent<ParticleActivate>();

		dashParticles=GetComponent<ParticleActivate>();
		dashParticles.duration=dash_duration;
        animator = GetComponent<Animator>();
        hp = GameController.instance.GetHP();
        //base.Start();
    }

	// Update is called once per frame
	override protected void Update()
    {

        // FLIPPING
        bool aux = facing_left;
        facing_left = GetMouseDirection().x < 0.1 * (facing_left ? 1.0 : -1.0);
        change_dir = facing_left != aux;
        Flip();


        bool moving = false;
		Vector3 movement = new Vector3(0, 0, 0);

		if (!dashing) {
			if ( (Input.GetKey("up")||Input.GetKey("w")) && (!WallColliding(new Vector2(0, speed * (attacking ? 0.5f : 1) * Time.deltaTime)))) {
				movement += new Vector3(0, speed*(attacking ? 0.5f : 1)*Time.deltaTime, 0);
				moving=true;
            }
			if ((Input.GetKey("down")||Input.GetKey("s")) && (!WallColliding(new Vector2(0, -speed * (attacking ? 0.5f : 1) * Time.deltaTime)))) {
				movement += new Vector3(0, -speed*(attacking ? 0.5f : 1)*Time.deltaTime, 0);
				moving=true;
			}
			if ((Input.GetKey("right")||Input.GetKey("d")) && (!WallColliding(new Vector2(speed * (attacking ? 0.5f : 1) * Time.deltaTime, 0)))) {
				movement += new Vector3(speed*(attacking ? 0.5f : 1)*Time.deltaTime, 0, 0);
				moving=true;
				/*change_dir=facing_left ? true : false;
				facing_left=false;
                Flip();*/
			}
			if ((Input.GetKey("left")||Input.GetKey("a")) && (!WallColliding(new Vector2(-speed * (attacking ? 0.5f : 1) * Time.deltaTime, 0)))) {
				movement += new Vector3(-speed*(attacking ? 0.5f : 1)*Time.deltaTime, 0, 0);
				moving=true;
				/*change_dir=facing_left ? false : true;
				facing_left=true;
				Flip();*/
			}
			transform.Translate(movement);
			last_movement=movement;

			animator.SetBool("moving", moving);
			if (Input.GetMouseButtonDown(0)) {
				Attack();
			} else if (post_dash_cooldown_timer<=0.0f) {
				if (Input.GetMouseButtonDown(1)||Input.GetKey("space")) {
					Dodge(movement);
				}
			} else {
				post_dash_cooldown_timer-=Time.deltaTime;
			}
		} else {
			current_dash-=Time.deltaTime;
            if (!WallColliding(new Vector2(dash_direction.x * (speed + dash_speed_bonus) * Time.deltaTime, dash_direction.y * (speed + dash_speed_bonus) * Time.deltaTime)))
			    gameObject.transform.Translate(dash_direction.x*(speed+dash_speed_bonus)*Time.deltaTime, dash_direction.y*(speed+dash_speed_bonus)*Time.deltaTime, 0);
            if (current_dash <= 0.0f)
            {
                current_dash = 0.0f;
                dashing = false;
                post_dash_cooldown_timer = post_dash_cooldown;
            }
        }


        if (attacking)
        {
            current_slash += Time.deltaTime;
            MoveSwordAttack();
            if (current_slash>slash_duration) {
                EndAttack();
			}
        }
        else
        {
            updateSwordPos();
        }


        


        base.Update();

        --quantum;
        if(quantum <= 0) {
            Vector2 goal = GameController.instance.rg.closestPoint((int) transform.position.x, (int) transform.position.y);
            Vector3 diff = new Vector3(goal.x, goal.y, 0) - transform.position;
            if(Mathf.Abs(diff.x) > 1.1 || Mathf.Abs(diff.y) > 1.1) transform.Translate(new Vector3(goal.x, goal.y, 0) - transform.position);
        }
	}



    void updateSwordPos()
    {
        Vector3 mouse = Input.mousePosition;
        mouse.z = transform.position.z - Camera.main.transform.position.z;
        mouse = Camera.main.ScreenToWorldPoint(mouse);
        mouse = mouse - GetComponent<BoxCollider2D>().bounds.center;
       

        float angle = Vector2.SignedAngle(new Vector2(facing_left ? -1.0f : 1.0f, -0.5f), mouse);
        Quaternion to = Quaternion.Euler(0.0f, 0.0f, angle);
        swordTransform.rotation = to;

        mouse.Normalize();

        swordTransform.position = GetComponent<BoxCollider2D>().bounds.center + mouse * sword_distance;
    }

    void setAtackDirection()
    {
        Vector3 mouse = Input.mousePosition;
        mouse.z = transform.position.z - Camera.main.transform.position.z;
        mouse = Camera.main.ScreenToWorldPoint(mouse);
        mouse = mouse - GetComponent<BoxCollider2D>().bounds.center;
        mouse.Normalize();
        attack_direction_v3 = mouse;

        float angle = Vector2.SignedAngle(new Vector2(facing_left ? 1.0f : -1.0f, 1.0f), mouse);   

        attack_direction = Quaternion.Euler(0.0f, 0.0f, angle);

        
    }

    void MoveSwordAttack()
    {
        float t = current_slash / slash_duration;

        swordTransform.rotation = Quaternion.Slerp(swordTransform.rotation, attack_direction, t);

        swordTransform.position = GetComponent<BoxCollider2D>().bounds.center + attack_direction_v3 * sword_distance;
    }

    void Flip()
    {
        transform.localScale = facing_left ? new Vector3(-1, 1, 1) : new Vector3(1, 1, 1);

        swordTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
       
    }

    private Vector2 GetMouseDirection()
    {
        // Calcula la pos del mouse en coords de mundo
        Vector3 mouse_pos = Input.mousePosition;
        float d =  transform.position.z - Camera.main.transform.position.z;

        mouse_pos = Camera.main.ScreenToWorldPoint(new Vector3(mouse_pos.x, mouse_pos.y, d));

        // Normaliza y multiplica por DISTANCE_TO_SLASH el vector desde el player al mouse
        Vector3 pos_to_mouse = mouse_pos - GetComponent<BoxCollider2D>().bounds.center;
        Vector2 pos_to_mouse2d = new Vector2(pos_to_mouse.x, pos_to_mouse.y);
        return pos_to_mouse2d;
    }
	private Vector2 GetMouseDirectionNormalized() {
        Vector2 data = GetMouseDirection();
        data.Normalize();
		return data;
	}

    void Attack()
    {
        if (!attacking) {
            // Vector2 pos_to_mouse2d = GetMouseDirectionNormalized()*DISTANCE_TO_SLASH;
            // GameObject s = Instantiate(slash, new Vector3(GetComponent<BoxCollider2D>().bounds.center.x + pos_to_mouse2d.x, GetComponent<BoxCollider2D>().bounds.center.y + pos_to_mouse2d.y, -1), Quaternion.Euler(Vector3.forward * Vector2.SignedAngle(new Vector2(-1, 1), pos_to_mouse2d)));
            // s.GetComponent<Slash>().attack = attack;
			// s.transform.parent=transform;
            attacking = true;
            setAtackDirection();
            swordParticles.activateForDuration();
            swordTransform.GetComponent<SwordInteractions>().canHit = true;
			GameController.instance.PlayAudio(1);
		}
	}

    public void EndAttack()
    {
        current_slash = 0.0f;
        attacking = false;
        swordTransform.GetComponent<SwordInteractions>().canHit = false; 

        //swordParticles.OffParticleSystems();
    }

    private void Dodge(Vector2 movement) {
		if (current_dash==0.0f) {
			dashing=true;
			current_dash=dash_duration;
			movement.Normalize();
			dash_direction=movement;
			last_movement=movement;
			dashParticles.activateForDuration();
		}
	}

    public bool GetFacingLeft()
    {
        return facing_left;
    }

    public float GetSpeed()
    {
        return speed;
    }
    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }
}
