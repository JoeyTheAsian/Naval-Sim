using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ShipAIMovement : MonoBehaviour
{
    public Vector3 netForces;
    public float gravity;
    public float mass;
    public float seaLevel;
    public float friction;
    public float acceleration;
    public float maxTurnSpeed;
    public float topSpeed;
    public bool leader;
    public Material matRed;
    public Material matWhite;
    public Transform target;
    public GameObject gameManager;
    bool reverse;
    float turnSpeed;
    public CharacterController controller;
    public GameObject nj;
    GameObject t = new GameObject();
    // Use this for initialization
    void Start()
    {
        netForces = new Vector3(0f, 0f, 0f);
        gravity = 9.81f;
        leader = false;
        gameManager = GameObject.Find("GameManager");
        RandomizePos();
        nj = GameObject.Find("New Jersey");
        t = new GameObject();
    }

    // Update is called once per frame
    void Update()
    {
        //only follows the player as a leader within 800 meters radius of the player
        if (Vector3.Distance(transform.position, nj.transform.position) <= 800f && nj.GetComponent<ShipMovement>().leader == true)
        {
            t.transform.position = nj.transform.position - nj.GetComponent<ShipMovement>().netForces.normalized * 100f;
            target = t.transform;
        }
        else
        {
            target = null;
            float direction = Mathf.Acos(new Vector3(netForces.x, 0f, netForces.z).normalized.x);
            if (netForces.x == 0f && netForces.z == 0f)
            {
                direction = transform.rotation.y;
            }
            if (target == null)
            {
                ToNearest();
            }
            if (target != transform)
            {
                if (Vector3.Distance(transform.position, target.position) > 750f)
                {
                    ToNearest();
                }
                int index = GameObject.Find("GameManager").GetComponent<GameManager>().waypoints.IndexOf(target.gameObject);
                GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();
                if (index >= gm.waypoints.Count - 1)
                {
                    if (Vector3.Distance(transform.position, gm.waypoints[0].transform.position) < Vector3.Distance(transform.position, target.position) && Vector3.Distance(gm.waypoints[0].transform.position, gm.waypoints[gm.waypoints.Count-1].transform.position) < 750f)
                    {
                        target = gm.waypoints[0].transform;
                    }
                }
                else
                {
                    if (Vector3.Distance(transform.position, gm.waypoints[index + 1].transform.position) < Vector3.Distance(transform.position, target.position))
                    {
                        target = gm.waypoints[index + 1].transform;
                    }
                }
                /*List<GameObject> ships = GameObject.Find("GameManager").GetComponent<GameManager>().ships;
                foreach (GameObject ship in ships)
                {
                    if (Vector3.Distance(transform.position, ship.transform.position) < 400f)
                    {
                        target = ship.transform;
                    }
                }*/
                //if you're within the bounding circle, stop moving
                if (Vector3.Distance(target.position, transform.position) <= GameObject.Find("GameManager").GetComponent<GameManager>().wayPointRadius)
                {
                    if (index >= gm.waypoints.Count - 1)
                    {
                        target = gm.waypoints[0].transform;
                    }
                    else
                    {
                        target = gm.waypoints[index + 1].transform;
                    }
                }
            }
        }
        //stopping distance
        float stoppingDistance = Mathf.Pow((new Vector3(netForces.x, 0f, netForces.z).magnitude) / mass, 2f) / (2f * -acceleration * .45f);
        if (Vector3.Distance(transform.position, target.position) > stoppingDistance)
        {
            ApplyForces((target.position - transform.position).normalized * mass * acceleration * Time.deltaTime);
        }
        else
        {
            ApplyForces(-(target.position - transform.position).normalized * mass * acceleration * Time.deltaTime);
        }
        foreach(GameObject ship in GameObject.Find("GameManager").GetComponent<GameManager>().ships)
        {
            if(Vector3.Distance(transform.position, ship.transform.position) < 150f)
            {
                Vector3 diff = ship.transform.position - transform.position;
                diff.y = 0f;
                ApplyForces(-diff.normalized * netForces.magnitude * Time.deltaTime * Vector3.Distance(transform.position, nj.transform.position) / 300f);
            }
        }
        if (Vector3.Distance(transform.position, nj.transform.position) < 180f)
        {
            Vector3 diff = nj.transform.position - transform.position;
            diff.y = 0f;
            ApplyForces(-diff.normalized * netForces.magnitude * Time.deltaTime * Vector3.Distance(transform.position, nj.transform.position)/180f);
        }
        /*float newDirection = Mathf.Acos(new Vector3(netForces.x, 0f, netForces.z).normalized.x);
        float diff = newDirection - direction;
        bool anti = false;
        if (diff > 180f)
        {
            diff = 360f - diff;
        }
        else if (diff < -180f)
        {
            diff = 360f + diff;
        }
        if (diff < 0)
        {
            anti = true;
        }
        else
        {
            anti = false;
        }
        direction += diff;*/
        if (target == transform)
        {
            netForces *= .97f;
            ToNearest();
        }
        if (turnSpeed > 0f)
        {
            turnSpeed -= turnSpeed * .5f * Time.deltaTime;
            if (turnSpeed < 0f)
            {
                turnSpeed = 0f;
            }
        }

        //speed limiter
        netForces = Quaternion.Euler(0f, turnSpeed * Time.deltaTime, 0f) * netForces;
        if (netForces.magnitude > mass * topSpeed)
        {
            netForces = netForces.normalized * mass * topSpeed;
        }

        //friction of water
        if (netForces.magnitude > 0f)
        {
            netForces.x -= netForces.x * friction * Time.deltaTime;
            netForces.z -= netForces.z * friction * Time.deltaTime;
        }
        //gravity
        netForces += new Vector3(0f, -gravity * mass * Time.deltaTime, 0f);
        //buoyancy
        float height = transform.position.y - seaLevel;
        if (height <= 2f)
        {
            netForces += new Vector3(0f, gravity * mass * Time.deltaTime + -2f * height * mass * Time.deltaTime, 0f);
        }
        if (netForces.x < .0001f && netForces.x > -.0001f)
        {
            netForces.x = 0f;
        }
        if (netForces.y < .0001f && netForces.y > -.0001f)
        {
            netForces.y = 0f;
        }
        if (netForces.z < .0001f && netForces.z > -.0001f)
        {
            netForces.z = 0f;
        }
        /*float speed = Vector3.Dot(transform.forward, netForces);
        if (speed > 0)
        {
            transform.rotation = Quaternion.LookRotation(new Vector3(netForces.x, 0f, netForces.z));
        }
        else
        {
            transform.rotation = Quaternion.LookRotation(new Vector3(-netForces.x, 0f, -netForces.z));
        }
        */
        transform.rotation = Quaternion.LookRotation(new Vector3(netForces.x, 0f, netForces.z));

        transform.Rotate(-height * 40f * Time.deltaTime, 0f, 0f);
        transform.Rotate(0f, 0f, turnSpeed / maxTurnSpeed * 360f * Time.deltaTime);
        transform.position += netForces / mass * Time.deltaTime;
    }
    void ApplyForces(Vector3 force)
    {
        netForces += force;
    }
    public void RandomizePos()
    {
        gameObject.transform.position = new Vector3(Random.Range(10f, GameObject.Find("Terrain").GetComponent<Terrain>().terrainData.size.x - 10f), 200.5f, Random.Range(10f, GameObject.Find("Terrain").GetComponent<Terrain>().terrainData.size.z - 10f));
        gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 0f));
    }
    public void ToNearest()
    {
        target = gameManager.GetComponent<GameManager>().GetNearestWayPoint(transform);
    }
    void OnRenderObject()
    {
        if (GameObject.Find("GameManager").GetComponent<GameManager>().debugLines)
        {
            //velocity
            GL.PushMatrix();
            matRed.SetPass(0);
            GL.Begin(GL.LINES);
            GL.Vertex(gameObject.transform.position);
            GL.Vertex(gameObject.transform.position + new Vector3(netForces.x, 0f, netForces.z).normalized * 300.0f);
            GL.End();

            matWhite.SetPass(0);
            GL.Begin(GL.LINES);
            GL.Vertex(gameObject.transform.position);
            GL.Vertex(target.position);
            GL.End();

            GL.PopMatrix();
        }
    }
}
