using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
    public Material matRed;
    public Material matWhite;
    public int NumShips = 10;
    public bool debugLines = false;
	public List<GameObject> ships = new List<GameObject>();
    public List<GameObject> waypoints = new List<GameObject>();
    public GameObject ship1;
    public GameObject ship2;
    public Terrain myTerrain;
    public GameObject waypoint;
    public Terrain terrain;
    public float wayPointRadius;

    public GameObject hordeLeader;

	// Use this for initialization
	void Start () {
		for (int i= 0; i < NumShips; i++) {
            GameObject g = Instantiate(ship1);
            ships.Add(g);
		}
        Vector3 avgPos = new Vector3(0f, 0f, 0f);
        foreach (GameObject g in ships)
        {
            avgPos += g.transform.position;
        }
        avgPos /= ships.Count;
        hordeLeader = null;
        foreach (GameObject g in ships)
        {
            if (hordeLeader == null)
            {
                hordeLeader = g;
            }
            else
            {
                if (Vector3.Distance(hordeLeader.transform.position, avgPos) > Vector3.Distance(g.transform.position, avgPos))
                {
                    hordeLeader = g;
                }
            }
        }
        hordeLeader.GetComponent<ShipAIMovement>().leader = true;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (terrain.GetComponent<Collider>().Raycast(ray, out hit, Mathf.Infinity))
            {
                GameObject wp = Instantiate(waypoint);
                wp.transform.position = hit.point + new Vector3(0f, 50f, 0f);
                waypoints.Add(wp);
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            debugLines = !debugLines;
        }
    }
	bool IsColliding(GameObject obj1, GameObject obj2){
        return true;
	}
    public void AddZombie(Transform t)
    {
        GameObject g = Instantiate(ship1);
        g.transform.position = t.position;
        ships.Add(g);
    }
    public Transform GetNearestWayPoint(Transform p)
    {
        try {
            GameObject nearest = null;
            foreach(GameObject g in waypoints)
            {
                if(nearest == null)
                {
                    nearest = g;
                }else if(Vector3.Distance(nearest.transform.position, p.position) > Vector3.Distance(g.transform.position, p.position))
                {
                    nearest = g;
                }
            }
            return nearest.transform;
        }catch(Exception) {
            return p;
        }
    }
}
