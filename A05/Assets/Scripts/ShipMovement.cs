using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShipMovement : MonoBehaviour
{
    public Vector3 netForces;
    public float gravity;
    public float mass;
    public float seaLevel;
    public float friction;
    public float acceleration;
    public float maxTurnSpeed;
    public float topSpeed;
    public Text speedo;
    bool reverse;
    float turnSpeed;
    public CharacterController controller;
    // Use this for initialization
    void Start()
    {
        netForces = new Vector3(0f, 0f, 0f);
        gravity = 9.81f;
        turnSpeed = 0.0f;
        speedo = GameObject.Find("Speed").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            ApplyForces(gameObject.transform.forward.normalized * mass * acceleration * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            ApplyForces(gameObject.transform.forward.normalized * mass * -acceleration * .45f * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.A) && new Vector3(netForces.x, 0f, netForces.z).magnitude > 0f)
        {
            if (turnSpeed > -maxTurnSpeed)
            {
                turnSpeed -= maxTurnSpeed * Time.deltaTime / 4;
                if (turnSpeed < -maxTurnSpeed)
                {
                    turnSpeed = -maxTurnSpeed;
                }
            }
        }
        else if (Input.GetKey(KeyCode.D) && new Vector3(netForces.x, 0f, netForces.z).magnitude > 0f)
        {
            if (turnSpeed < maxTurnSpeed)
            {
                turnSpeed += maxTurnSpeed * Time.deltaTime / 4;
                if (turnSpeed > maxTurnSpeed)
                {
                    turnSpeed = maxTurnSpeed;
                }
            }
        }
        else
        {
            turnSpeed -= turnSpeed * .5f * Time.deltaTime;
        }
        netForces = Quaternion.Euler(0f, turnSpeed * Time.deltaTime, 0f) * netForces;
        if(netForces.magnitude > mass * topSpeed)
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
        float speed = Vector3.Dot(transform.forward, netForces);
        if(speed > 0)
        {
            transform.rotation = Quaternion.LookRotation(new Vector3(netForces.x, 0f, netForces.z));
        }else
        {
            transform.rotation = Quaternion.LookRotation(new Vector3(-netForces.x, 0f, -netForces.z));
        }


        transform.Rotate(-height * 40f * Time.deltaTime, 0f, 0f);
        transform.Rotate(0f, 0f,-turnSpeed / maxTurnSpeed * 180f * Time.deltaTime);
        transform.position += netForces / mass * Time.deltaTime;
        speedo.text = "SPEED: " + (int)(Mathf.Abs(speed)/mass) + " KNOTS";
    }
    void ApplyForces(Vector3 force)
    {
        netForces += force;
    }
}
