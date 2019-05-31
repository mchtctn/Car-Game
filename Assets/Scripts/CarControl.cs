using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CarControl : MonoBehaviour
{
    private float inputX;
    private float inputY;
    private float brakeInput;
    private float steeringAngle;

    public WheelCollider frontLeft, frontRight;
    public WheelCollider rearLeft, rearRight;
    public Transform frontLeftT, frontRightT;
    public Transform rearLeftT, rearRightT;
    public float maxSteerAngle = 45.0f;
    public float motorForce = 400.0f;
    public float maxBreakForce = 2200.0f;
    public float currentSpeed = 0.0f;
    [SerializeField]
    private float topSpeed = 220.0f;
    [SerializeField]
    private Vector3 centerOfMass = Vector3.zero;
    public bool isCrashed = false;
    public float point = 0f;
    private float time = 0f;
    public GameObject startPoint;
    private float distanceAwayStartPos = 0f;
    public GameObject server;
    private int index;
    public string input = null;

    private void Start(){
        GetComponent<Rigidbody>().centerOfMass = centerOfMass;
        index = (int)this.transform.position.y / 2;
        server = GameObject.Find("Server");
    }

    private void FixedUpdate()
    {
        GetInput();
        Steer();
        Accelerate();
        UpdateWheelPoses();
        currentSpeed = 2 * 22/7 * rearLeft.radius * rearLeft.rpm * 60/1000; //formula for calculating speed in kmph
        distanceAwayStartPos = Vector3.Distance(new Vector3(this.transform.position.x, 0, this.transform.position.z), new Vector3(startPoint.transform.position.x, 0, startPoint.transform.position.z));
        if (!isCrashed)
        {
            time += Time.deltaTime;
        }
        point = distanceAwayStartPos * time;


       // Debug.Log(index + ": " + point);
       // Debug.Log(rearLeft.rpm);
       
    }

    public void GetInput()
    {
        input = server.GetComponent<Server>().clientMessage;
        if(!input.Equals(null))
            try {
                input = input.Split(' ')[index].Split(':')[1];
                Debug.Log("input: " + input);
            }
            catch
            {
                Debug.Log(input);
            }


        if (input.Equals("NN"))
        {
            inputX = 0; inputY = 0; brakeInput = 0;
        }
        else if(input.Equals("FN")){
            inputX = 0; inputY = 1; brakeInput = 0;
        }
        else if (input.Equals("FR"))
        {
            inputX = 1; inputY = 1; brakeInput = 0;
        }
        else if (input.Equals("FL"))
        {
            inputX = -1; inputY = 1; brakeInput = 0;
        }
        else if (input.Equals("BN"))
        {
            inputX = 0; inputY = 0; brakeInput = 1;
        }
        else if (input.Equals("BR"))
        {
            inputX = 1; inputY = 0; brakeInput = 1;
        }
        else if (input.Equals("BL"))
        {
            inputX = -1; inputY = 1; brakeInput = 1;
        }
        else if (input.Equals("NR"))
        {
            inputX = 1; inputY = 0; brakeInput = 0;
        }
        else if (input.Equals("NL"))
        {
            inputX = -1; inputY = 0; brakeInput = 0;
        }
    }

    private void Steer()
    {
        steeringAngle = maxSteerAngle * inputX;
        frontLeft.steerAngle = steeringAngle;
        frontRight.steerAngle = steeringAngle;
    }

    private void Accelerate()
    {

        Debug.Log(index + ": " + "inputY: " + inputY + "inputX: "+ inputX);

        if (currentSpeed < topSpeed){
            frontLeft.motorTorque = inputY * motorForce;
            frontRight.motorTorque = inputY * motorForce;
        }

        frontLeft.brakeTorque = maxBreakForce * brakeInput;
        frontRight.brakeTorque = maxBreakForce * brakeInput;
        rearLeft.brakeTorque = maxBreakForce * brakeInput;
        rearRight.brakeTorque = maxBreakForce * brakeInput;
        
    }

    private void UpdateWheelPoses()
    {
        UpdateWheelPose(frontLeft, frontLeftT);
        UpdateWheelPose(frontRight, frontRightT);
        UpdateWheelPose(rearLeft, rearLeftT);
        UpdateWheelPose(rearRight, rearRightT);
    }

    private void UpdateWheelPose(WheelCollider _collider, Transform _transform)
    {
        Vector3 _pos = _transform.position;
        Quaternion _quat = _transform.rotation;

        _collider.GetWorldPose(out _pos,out _quat);

        _transform.position = _pos;
        _transform.rotation = _quat;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Wall")
        {
            Debug.Log("Omg you crashed");
            isCrashed = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Finish")
        {
            Debug.Log("Congratulations");
            if (SceneManager.GetActiveScene().name.Equals("Lv1"))
            {
                SceneManager.LoadScene("Lv2");
            }
            else if (SceneManager.GetActiveScene().name.Equals("Lv2"))
            {
                SceneManager.LoadScene("Lv1");
            }
        }
    }
 
}
