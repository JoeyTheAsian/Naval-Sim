using UnityEngine;
using System.Collections;

public class ThirdPersonCamera : MonoBehaviour {
    public GameObject target;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = target.transform.position;
        transform.position += new Vector3(0f, 15f, 0f);
        Vector3 forces = target.GetComponent<ShipMovement>().netForces;
        transform.LookAt(transform.position + target.transform.forward);
	}
}
