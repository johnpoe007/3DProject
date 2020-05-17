using UnityEngine;
using System.Collections;
namespace HotTotem.GhostToolPro.DemoScripts
{
    public class CarController : MonoBehaviour
    {
        public GameObject WheelFrontLeft, WheelFrontRight, WheelBackLeft, WheelBackRight;
        private Transform car;
        private float currentSpeed = 0f;
        // Update is called once per frame
        void Start()
        {
            car = GetComponent<Transform>();
        }
        void Update()
        {
            if (Input.GetKey(KeyCode.A))
            {
                if (WheelFrontLeft.transform.rotation.eulerAngles.y > 345f || WheelFrontLeft.transform.rotation.eulerAngles.y <= 16f)
                {
                    WheelFrontLeft.transform.Rotate(new Vector3(0, -1f, 0));
                    WheelFrontRight.transform.Rotate(new Vector3(0, -1f, 0));
                }
                if (currentSpeed > 0.002f)
                {
                    car.Rotate(WheelBackLeft.transform.up, -10f * currentSpeed);
                }
                else if (currentSpeed < -0.002f)
                {
                    car.Rotate(WheelBackLeft.transform.up, 10f * currentSpeed);
                }
            }
            if (Input.GetKey(KeyCode.D))
            {
                if (WheelFrontLeft.transform.rotation.eulerAngles.y < 15f || WheelFrontLeft.transform.rotation.eulerAngles.y >= 345f)
                {
                    WheelFrontLeft.transform.Rotate(new Vector3(0, 1f, 0));
                    WheelFrontRight.transform.Rotate(new Vector3(0, 1f, 0));
                }
                if (currentSpeed > 0.002f)
                {
                    car.Rotate(WheelBackRight.transform.up,10f*currentSpeed);
                }
                else if (currentSpeed < -0.002f)
                {
                    car.Rotate(WheelBackRight.transform.up, -10f * currentSpeed);
                }
            }
            if (Input.GetKey(KeyCode.W))
            {
                if (currentSpeed < 0.5f)
                {
                    currentSpeed += 0.01f;
                }
            }
            else if (currentSpeed > 0f)
            {
                currentSpeed -= 0.02f;
            }
            if (Input.GetKey(KeyCode.S))
            {
                if (currentSpeed > -0.5f)
                {
                    currentSpeed -= 0.01f;
                }
            }
            else if (currentSpeed < 0f)
            {
                currentSpeed += 0.02f;

            }
            if (currentSpeed>0.01f || currentSpeed < -0.01f)
            {
                car.position += (car.forward * currentSpeed);
            }
            if(car.transform.position.x > 30f || car.transform.position.x < -30f || car.transform.position.z > 30f || car.transform.position.z < -30f)
            {
                car.transform.position = Vector3.zero;
            }
        }
    }
}