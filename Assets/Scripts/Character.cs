using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Character : MonoBehaviour
{
	public int maxHp = 100;
	public int hp;
	public int attack;
	public float invulnerabilityTime;
    public GameObject drop = null;
	protected bool dashing = false;
	[SerializeField]
	protected float speed;
	protected int defense;
	protected float invulnerabilityCooldown = 0.0f;
	protected Animator animator;
	protected bool alive = true;

	[SerializeField]
    protected float knockback_k = 0;
    private Vector2 force = Vector2.zero;
    private Vector2 wallcenter, wallsize;

	// Start is called before the first frame update
	virtual protected void Start()
    {
        SetWallCollidingHitbox(GetComponent<Collider2D>().bounds.center, GetComponent<Collider2D>().bounds.size);
        animator = GetComponent<Animator>();
		if (GameController.instance.GetFirstInstantiation()) hp=maxHp;
	}

    protected void SetWallCollidingHitbox(Vector2 center, Vector2 size) {
        wallcenter = center;
        wallsize = size;
    }

    protected bool WallColliding(Vector2 offset) {
        Vector2 c = transform.position;
        for(float i = 0.5f / offset.magnitude; i < 1; i += 0.5f / offset.magnitude) {
            Vector2 center2 = c + wallcenter + i * offset;

            Vector2Int p12 = new Vector2Int( (int) (center2.x - wallsize.x / 2), (int) (center2.y - wallsize.y / 2) );
            Vector2Int p22 = new Vector2Int( (int) (center2.x - wallsize.x / 2), (int) (center2.y + wallsize.y / 2) );
            Vector2Int p32 = new Vector2Int( (int) (center2.x + wallsize.x / 2), (int) (center2.y - wallsize.y / 2) );
            Vector2Int p42 = new Vector2Int( (int) (center2.x + wallsize.x / 2), (int) (center2.y + wallsize.y / 2) );

            if (GameController.instance.rg.TheresWall(p12)) return true;
            if (GameController.instance.rg.TheresWall(p22)) return true;
            if (GameController.instance.rg.TheresWall(p32)) return true;
            if (GameController.instance.rg.TheresWall(p42)) return true;
        }
        Vector2 center = c + wallcenter + offset;

        Vector2Int p1 = new Vector2Int((int)(center.x - wallsize.x / 2), (int)(center.y - wallsize.y / 2));
        Vector2Int p2 = new Vector2Int((int)(center.x - wallsize.x / 2), (int)(center.y + wallsize.y / 2));
        Vector2Int p3 = new Vector2Int((int)(center.x + wallsize.x / 2), (int)(center.y - wallsize.y / 2));
        Vector2Int p4 = new Vector2Int((int)(center.x + wallsize.x / 2), (int)(center.y + wallsize.y / 2));

        if (GameController.instance.rg.TheresWall(p1)) return true;
        if (GameController.instance.rg.TheresWall(p2)) return true;
        if (GameController.instance.rg.TheresWall(p3)) return true;
        if (GameController.instance.rg.TheresWall(p4)) return true;
        return false;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
		if (alive) {
			if (invulnerabilityCooldown<invulnerabilityTime) {
				invulnerabilityCooldown+=Time.deltaTime;
				if (invulnerabilityCooldown>invulnerabilityTime) invulnerabilityCooldown=invulnerabilityTime;
			}
		}
	}

	public void TakeDamage(int amount, Vector2 from) {
		if (!dashing && invulnerabilityTime==invulnerabilityCooldown) {
			invulnerabilityCooldown=0.0f;
			int damage = amount-defense;
			if (damage<=0) damage=0;
			else animator.SetTrigger("hit");
			hp-=damage;


            Vector2 direction = ((Vector2)transform.position - from).normalized;

            force = direction * knockback_k;

			if (hp<=0) {
                if (drop != null) Instantiate(drop, transform.position, Quaternion.identity);
                alive =false;
				if (transform.tag=="Enemy") {
					animator.SetTrigger("die");
					Destroy(GetComponent<Collider2D>());
				} else {
                    SceneManager.LoadScene(3);
					Destroy(gameObject);
				}
			}

		}
	}

    private void FixedUpdate()
    {
		if (alive) {
			Vector2 tmp = force*Time.deltaTime;
			if(!WallColliding(tmp)) transform.position+=new Vector3(tmp.x, tmp.y, 0);
			force*=0.9f;
		}
    }
    public bool isDashing() {
		return dashing;
	}

}
