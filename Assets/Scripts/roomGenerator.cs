using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class roomGenerator : MonoBehaviour
{
    public GameObject[] blocks;
    public GameObject[] ground;
    public Character[] enemies;
    public GameObject collider;
	public GameObject map;
    public GameObject exit;
    public GameObject key;

    private const int ROOM = 0, WALL = 1, PATH = 2, DEBUG1 = 3, DEBUG2 = 4;
    const int w = 50, h = 50;
    const int nrooms_min = 4, nrooms_max = 7;
    const int min_size = 7, max_size = 15;

    const float flatness = 0.004f; // flatness > 0
    const float noiseMag = 100f;
    const float minweight = 1;

    float enemyDensityLow = 0.01f, enemyDensityHigh = 0.02f;

    private float[,] weights;

    private int[,] plan;

    public bool TheresWall(Vector2Int p) {
        return plan[p.x, p.y] == WALL;
    }

    private void init_weights() {
        weights = new float[w + 2, h + 2];
        for (float i = 0; i < w + 2; ++i)
            for (float j = 0; j < h + 2; ++j)
                weights[(int)i, (int)j] = minweight + noiseMag * Mathf.PerlinNoise(i / flatness, j / flatness);
    }

    private float cost(Vector2 point) {
        if (point.x == 0 || point.x == w + 1 || point.y == 0 || point.y == h + 1) return float.PositiveInfinity;
        if (plan[(int)point.x, (int)point.y] != WALL/* || plan[(int)point.x-1, (int)point.y] != WALL || plan[(int)point.x+1, (int)point.y] != WALL
                                                     || plan[(int)point.x, (int)point.y-1] != WALL || plan[(int)point.x, (int)point.y+1] != WALL*/) {
            return float.PositiveInfinity;
        }
        return weights[(int)point.x, (int)point.y];
    }

    private bool isIn(List<Vector2> l, Vector2 v) {
        foreach (Vector2 v2 in l) {
            if (v.x == v2.x && v.y == v2.y) {
                return true;
            }
        }
        return false;
    }

    private bool dijkstra(Vector2 from, Vector2 to) {
        List<Vector3> queue = new List<Vector3>();
        List<Vector2> goals = getAllAdjacentPoints(to);

        Vector2 init = getAdjacentPoint((int)from.x, (int)from.y);

        queue.Add(new Vector3(init.x, init.y, 0));

        float[,] dist = new float[w + 2, h + 2];
        Vector2[,] father = new Vector2[w + 2, h + 2];
        bool[,] visited = new bool[w + 2, h + 2];
        for (int i = 0; i < w + 2; ++i) {
            for (int j = 0; j < h + 2; ++j) {
                dist[i, j] = float.PositiveInfinity;
                visited[i, j] = false;
            }
        }
        dist[(int)from.x, (int)from.y] = 0.0f;
        queue.Add(new Vector3(from.x, from.y, 0));

        int adsad = 0;
        while (queue.Count > 0) {
            ++adsad;
            if (adsad > w * h) return false;

            Vector3 best = new Vector3(0, 0, float.PositiveInfinity);
            foreach (Vector3 v in queue) {
                if (v.z < best.z) best = v;
            }

            Vector2 bestp = new Vector2(best.x, best.y);

            if (isIn(goals, bestp)) {
                to = bestp;
                break;
            }
            queue.Remove(best);
            visited[(int)best.x, (int)best.y] = true;
            for (int i = -1; i <= 1; i++) {
                for (int j = -1 + i * i; j < 2; j += 2) {
                    bool isGoal = isIn(goals, new Vector2(best.x + i, best.y + j));
                    bool isWall = cost(new Vector2(best.x + i, best.y + j)) != float.PositiveInfinity;
                    bool notVis = !visited[(int)best.x + i, (int)best.y + j];
                    float actDistance = dist[(int)best.x + i, (int)best.y + j];
                    float newDistance = dist[(int)best.x, (int)best.y] + cost(new Vector2(best.x + i, best.y + j));
                    if (isGoal || (isWall && notVis)) {
                        if (newDistance == float.PositiveInfinity) newDistance = dist[(int)best.x, (int)best.y] + 1;
                        if (actDistance > newDistance) {
                            dist[(int)best.x + i, (int)best.y + j] = newDistance;
                            father[(int)best.x + i, (int)best.y + j] = bestp;
                            queue.Add(new Vector3(best.x + i, best.y + j, newDistance));
                        }
                    }
                }
            }
        }

        if (dist[(int)to.x, (int)to.y] == float.PositiveInfinity) return false;
        Vector2 trail = to;
        while (trail.x != from.x || trail.y != from.y) {
            plan[(int)trail.x, (int)trail.y] = PATH;
            if (trail.x == father[(int)trail.x, (int)trail.y].x && trail.y == father[(int)trail.x, (int)trail.y].y) break;
            trail = father[(int)trail.x, (int)trail.y];
        }
        plan[(int)trail.x, (int)trail.y] = PATH;

        return true;
    }

    private void pathfinding_transform() {
        for (int i = 0; i < w + 2; ++i) {
            for (int j = 0; j < h + 2; ++j) plan[i, j]++;
        }
    }

    private void pathfinding_inverse_transform() {
        for (int i = 0; i < w + 2; ++i) {
            for (int j = 0; j < h + 2; ++j) plan[i, j] = plan[i, j] > 0 ? plan[i, j] - 1 : -plan[i, j] - 1;
        }
    }

    private List<Vector2> getAllAdjacentPoints(Vector2 p) {
        pathfinding_transform();

        List<Vector2> Stack = new List<Vector2>();
        Stack.Add(p);

        List<Vector2> ret = new List<Vector2>();
        while (Stack.Count > 0) {
            p = Stack[Stack.Count - 1];
            Stack.RemoveAt(Stack.Count - 1);
            if (plan[(int)p.x, (int)p.y] < 0) continue;
            plan[(int)p.x, (int)p.y] = -plan[(int)p.x, (int)p.y];
            if (p.x == 0 || p.x == w + 1 || p.y == 0 || p.y == h + 1) continue;
            if (plan[(int)p.x, (int)p.y] == -WALL - 1) {
                ret.Add(p);
                continue;
            }

            if (plan[(int)p.x + 1, (int)p.y] > 0)
                Stack.Add(new Vector2(p.x + 1, p.y));
            if (plan[(int)p.x - 1, (int)p.y] > 0)
                Stack.Add(new Vector2(p.x - 1, p.y));
            if (plan[(int)p.x, (int)p.y + 1] > 0)
                Stack.Add(new Vector2(p.x, p.y + 1));
            if (plan[(int)p.x, (int)p.y - 1] > 0)
                Stack.Add(new Vector2(p.x, p.y - 1));
        }

        pathfinding_inverse_transform();

        return ret;
    }

    private Vector2 getAdjacentPoint(int x, int y) {
        List<Vector2> res = getAllAdjacentPoints(new Vector2(x, y));
        int i = Random.Range(0, res.Count);
        return res[i];
    }

    private void generate() {
        init_weights();

        plan = new int[w + 2, h + 2];
        for (int i = 0; i < w + 2; ++i) {
            for (int j = 0; j < h + 2; ++j) plan[i, j] = WALL;
        }

        int nr = Random.Range(nrooms_min, nrooms_max);

        List<int[]> rooms = new List<int[]>();
        List<int[]> ogrms = new List<int[]>();

        for (int rr = 0; rr < nr; ++rr) {
            int wr = Random.Range(min_size, max_size);
            int hr = Random.Range(min_size, max_size);

            int x = Random.Range(1, w - wr);
            int y = Random.Range(1, h - hr);

            bool error = false;
            for (int i = x - 1; i < x + wr + 1 && !error; ++i) {
                for (int j = y - 1; j < y + hr + 1 && !error; ++j) {
                    if (plan[i, j] == ROOM) error = true;
                }
            }
            if (error) {
                --rr;
                continue;
            }
            rooms.Add(new int[4] { x, y, wr, hr });
            ogrms.Add(new int[4] { x, y, wr, hr });
            for (int i = x; i < x + wr && !error; ++i) {
                for (int j = y; j < y + hr && !error; ++j) {
                    plan[i, j] = ROOM;
                }
            }
        }

        int iters = 0;
        while (rooms.Count > 1) {
            if (iters == 50) break;
            ++iters;
            int i1 = Random.Range(0, rooms.Count);
            int i2 = Random.Range(0, rooms.Count - 1);
            if (i2 >= i1) i2++;
            if (!dijkstra(new Vector2(rooms[i1][0], rooms[i1][1]), new Vector2(rooms[i2][0], rooms[i2][1]))) continue;
            if (iters > Random.Range(1, 3)) rooms.RemoveAt(i2);

        }

        bool theresplayerpos = false;
        bool theresportalpos = false;
        bool keybool = false;

        int p = 0;
        int q = Random.Range(0, ogrms.Count);

        while (ogrms.Count > 0) {
            int i1 = Random.Range(0, ogrms.Count);
            int[] room = ogrms[i1];
            ogrms.RemoveAt(i1);
            if (!theresplayerpos) {
                theresplayerpos = true;
                int x = Random.Range(room[0] + 1, room[0] + room[2] - 1);
                int y = Random.Range(room[1] + 1, room[1] + room[3] - 1);
                GameController.instance.instantiatePlayer(new Vector3(x, y, 0));
                continue;
            }
            int numberlow  = (int) (enemyDensityLow  * room[2] * room[3]);
            int numberhigh = (int) (enemyDensityHigh * room[2] * room[3]);
            int number = Random.Range(numberlow, numberhigh);
            if (number < 1) number = 1;
            List<Vector2> repe = new List<Vector2>();

            if (p == q || (ogrms.Count == 0 && !theresportalpos)) {
                theresportalpos = true;
                int lowx = room[0] + 1;
                int highx = room[0] + room[2] - 1;
                int lowy = room[1] + 1;
                int highy = room[1] + room[3] - 1;
                int x = Random.Range(lowx, highx);
                int y = Random.Range(lowy, highy);
                repe.Add(new Vector2(x, y));
                Instantiate(exit, new Vector3(x, y, 0), Quaternion.identity);
            }

            int niter = 0;
            for (int i = 0; i < number; ++i) {
                ++niter;
                if (niter > 200) break;

                Character enemy = enemies[Random.Range(0, enemies.Length)];
                float lowx = room[0] + enemy.GetComponent<Collider2D>().bounds.size.x;
                float highx = room[0] + room[2] - 2; // enemy.GetComponent<Collider2D>().bounds.size.x;
                float lowy = room[1] + enemy.GetComponent<Collider2D>().bounds.size.y;
                float highy = room[1] + room[3] - 2; // enemy.GetComponent<Collider2D>().bounds.size.y;
                if(highx < lowx || highy < lowy) {
                    --i;
                    continue;
                }
                float x = Random.Range(lowx, highx);
                float y = Random.Range(lowy, highy);
                if(isIn(repe, new Vector2(x, y))) {
                    --i;
                    continue;
                }

                repe.Add(new Vector2(x, y));
                Character en = Instantiate(enemy, new Vector3(x, y, 0), Quaternion.identity);
                if (!keybool) {
                    en.drop = key;
                    keybool = true;
                }
                else en.drop = null;
            }
            p++;
        }
    }


    private void addUnitCube(float x, float y, float z) {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(x, y, z);
    }
    
    private void addWall(float x, float y, float z, GameObject o, string Layer, Quaternion rotation, Vector3 scale){
        GameObject w = Instantiate(o, new Vector3(x,y,z), Quaternion.identity);
        w.transform.rotation = rotation;
        w.transform.parent=map.transform;
        w.transform.localScale = scale;

        if (w.GetComponent<SpriteRenderer>() != null) {
            w.GetComponent<SpriteRenderer>().sortingLayerName = Layer;
            w.GetComponent<SpriteRenderer>().sortingOrder = 2;
        }
    }

    private void show_plan() {
        Quaternion xrot = Quaternion.Euler(45,0,0);
        Vector3 id = new Vector3(1, 1, 1);
        Vector3 lsc = new Vector3(1, Mathf.Sqrt(2), 1);

        for (int i = 0; i < w + 2; ++i) {
            for (int j = 0; j < h + 2; ++j) {
                if (plan[i, j] == WALL) {
                    bool noskip = true;
                    for (int x = -1; x < 2 && noskip; x++) {
                        for (int y = -1; y < 2 && noskip; y++){
                            if (x == 0 && y == 0) continue;
                            if ((i + x >= 0 && i + x < w && j + y >= 0 && j + y < h && plan[i + x, j + y] != WALL)) {
                                addWall((float)i + 0.5f, (float)j + 0.5f, 0, collider, "Colliders", Quaternion.identity, id);
                                noskip = false;
                                break;
                            }
                        }
                    }

                    if(j > 0 && plan[i, j-1] != WALL) {
                        if (i > 0 && plan[i - 1, j - 1] == WALL) addWall(i, j, -6 - h + j, blocks[5], "Units", Quaternion.identity, id);
                        else if (i < w + 1 && plan[i + 1, j - 1] == WALL) addWall(i, j, -6 - h + j, blocks[3], "Units", Quaternion.identity, id);
                        else addWall(i, j, -6 - h + j, blocks[4], "Units", Quaternion.identity, id);
                    }
                    if (j < h + 1 && plan[i, j + 1] != WALL) {
                        if (i > 1 && plan[i - 1, j + 1] == WALL) addWall(i, j, -5 - h + j, blocks[2], "Units", Quaternion.identity, id);
                        else if (i < w + 1 && plan[i + 1, j + 1] == WALL) addWall(i, j, -5 - h + j, blocks[0], "Units", Quaternion.identity, id);
                        else addWall(i, j, -5 - h + j, blocks[1], "Units", Quaternion.identity, id);
                    }
                    if (i > 0 && plan[i - 1, j] != WALL) addWall(i, j, -6 - h + j, blocks[7], "Units", xrot, lsc);
                    if (i < w + 1 && plan[i + 1, j] != WALL) addWall(i, j, -6 - h + j, blocks[6], "Units", xrot, lsc);
                } else {
                    if (i > 0 && j < h + 1 && plan[i - 1, j] == WALL && plan[i, j + 1] == WALL) addWall(i - 1, j + 1, -5 - h + j, blocks[10], "Units", Quaternion.identity, id);
                    if (i < w + 1 && j < h + 1 && plan[i + 1, j] == WALL && plan[i, j + 1] == WALL) addWall(i + 1, j + 1, -5 - h + j, blocks[11], "Units", Quaternion.identity, id);
                    if (i > 0 && j > 0 && plan[i - 1, j] == WALL && plan[i, j - 1] == WALL) addWall(i - 1, j - 1, -6 - h + j, blocks[12], "Units", Quaternion.identity, id);
                    if (i < w + 1 && j > 0 && plan[i + 1, j] == WALL && plan[i, j - 1] == WALL) addWall(i + 1, j - 1, -6 - h + j, blocks[13], "Units", Quaternion.identity, id);

                    if (j < h + 1 && plan[i, j + 1] == WALL) addWall(i, j, -5 - h + j, blocks[1], "Units", Quaternion.identity, id);
                    if (j > 0 && plan[i, j - 1] == WALL) addWall(i, j, -6 - h + j, blocks[4], "Units", Quaternion.identity, id);

                    addWall(i, j, 4, ground[(int) (7 - Mathf.Sqrt(Random.Range(0, 8*8)))], "Ground", Quaternion.identity, id);
                }
            }
        }
    }

    public Vector2 closestPoint(int x, int y) {
        List<Vector2> visitados = new List<Vector2>();
        List<Vector2> queue = new List<Vector2>();
        queue.Add(new Vector2(x, y));
        Vector2 top = new Vector2(-1, -1);
        while (queue.Count > 0) {
            top = queue[0];
            queue.RemoveAt(0);
            if (isIn(visitados, top)) continue;
            visitados.Add(top);
            if (top.x > 0 && top.x < w + 2 && top.y > 0 && top.y < h + 2 && plan[(int)top.x, (int)top.y] != WALL) break;

            for(int i = -1; i < 2; i += 2) {
                queue.Add(new Vector2(top.x + i, top.y));
                queue.Add(new Vector2(top.x, top.y + i));
            }
        }

        return top;
    }

    public int[,] getLevel() {
        generate();
        return plan;
    }

    // Start is called before the first frame update
    void Awake(){
        GameController.instance.rg = this;
        enemyDensityLow = 0.0212764314523f * Mathf.Log(GameController.instance.e_dens_index + 1.6f) * 1.5f;
        enemyDensityHigh = 2 * enemyDensityLow;
		map=new GameObject("Map");
		generate();
        show_plan();

    }

    // Update is called once per frame
    void Update(){
        
    }
}
