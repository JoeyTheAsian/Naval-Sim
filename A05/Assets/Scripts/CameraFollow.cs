using UnityEngine;
using System.Collections;
public class CameraFollow: MonoBehaviour {
    public Transform target;
    public float distance = 3.0f;
    public float height = 1.50f;
    public float heightDamping = 2.0f;
    public float positionDamping =2.0f;
    public float rotationDamping = 2.0f;
    bool isAerial = false;
    // Update is called once per frame
    void Update(){
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isAerial = !isAerial;
            if (isAerial)
            {
                target = GameObject.Find("AerialView").transform;
            }
            else
            {
                target = GameObject.Find("New Jersey").transform;
            }
        }
        // Early exit if there’s no target
        if (!target)
            return;
        float wantedHeight = target.position.y + height;
        float currentHeight = transform.position.y;
        // Damp the height
        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);
        // Set the position of the camera 
        Vector3 wantedPosition = target.position - target.forward * distance;
        transform.position = Vector3.Lerp(transform.position, wantedPosition, Time.deltaTime * positionDamping);
        // Adjust the height of the camera
        transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);
        // Set the forward to rotate with time
        transform.forward = Vector3.Lerp(transform.forward, new Vector3(target.position.x - transform.position.x, 0f, target.position.z - transform.position.z).normalized, Time.deltaTime * rotationDamping);
        if (!isAerial)
        {
            transform.Rotate(new Vector3(transform.rotation.x, 0f, transform.rotation.z));
            transform.LookAt(GameObject.FindWithTag("Player").transform.position);
        }
        else
        {
            transform.LookAt(GameObject.Find("WaterTile").transform.position);
            transform.rotation = Quaternion.Euler(new Vector3(90f,transform.rotation.y, transform.rotation.z));
        }

    }
}