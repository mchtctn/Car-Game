using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycast : MonoBehaviour
{
    public float sensorLength = 10f;
    public Vector3 frontSensorPosition = new Vector3(0, 0.7f, 2.5f);
    public float frontSideSensorPosition = 0.75f;
    public float frontSensorAngle = 30f;
    public float frontSensorHitDistance = 0;
    public float leftSensorHitDistance = 0;
    public float rightSensorHitDistance = 0;

    private void FixedUpdate()
    {
        RaycastHit hit;
        Vector3 sensorStartPos = transform.position;
        sensorStartPos += transform.forward * frontSensorPosition.z;
        sensorStartPos += transform.up * frontSensorPosition.y;

        //front right angle sensor
        sensorStartPos += transform.right * frontSideSensorPosition;
        if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength))
        {
            //Debug.Log("Right sensor:" + Vector3.Distance(hit.point, sensorStartPos));
            rightSensorHitDistance = Vector3.Distance(hit.point, sensorStartPos);
            Debug.DrawLine(sensorStartPos, hit.point, Color.red);
        }
        else{
            rightSensorHitDistance = sensorLength;
            //Debug.Log("Right sensor:" + sensorLength);
        }

        //front left angle sensor
        sensorStartPos -= 2 * transform.right * frontSideSensorPosition;
        if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(-frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength))
        {
            //Debug.Log("Left sensor:" + Vector3.Distance(hit.point,sensorStartPos));
            leftSensorHitDistance = Vector3.Distance(hit.point, sensorStartPos);
            Debug.DrawLine(sensorStartPos, hit.point, Color.red);
        }
        else
        {
            leftSensorHitDistance = sensorLength;
            //Debug.Log("Left sensor:" + sensorLength);
        }

        //front center sensor
        sensorStartPos += transform.right * frontSideSensorPosition;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            //Debug.Log("Front sensor:" + Vector3.Distance(hit.point,sensorStartPos));
            frontSensorHitDistance = Vector3.Distance(hit.point, sensorStartPos);
            Debug.DrawLine(sensorStartPos, hit.point, Color.red);
        }
        else
        {
            frontSensorHitDistance = sensorLength;
            //Debug.Log("Front sensor:" + sensorLength);
        }

    }
}
