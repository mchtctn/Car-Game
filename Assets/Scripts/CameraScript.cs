using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public GameObject target;
    private float cameraPosY = 20;
    public GameObject server;
    public GameObject[] playerObject; 

    // Start is called before the first frame update
    void Start()
    {
        target = server.GetComponent<Server>().playerObject[0];
        transform.position = new Vector3(target.transform.position.x, cameraPosY, target.transform.position.z );
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(target.transform.position.x, cameraPosY, target.transform.position.z);
        playerObject = GameObject.FindGameObjectsWithTag("Player");
    }
}
