using System;
using ROS2;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;

public class OdenmetrySubscriber : MonoBehaviour
{
    public string NodeName = "unity_ros2_node";
    public string TopicName = "/sentry/transform/show"; // Change this to your desired image topic
    public bool IsGlobal = false;
    Vector3 begin = new();
    ROSConnection ros;
    private CameraCapturer capturer;
    public Vector3 Begin => begin;
    Vector3 position = new();
    double yaw = new();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize ROS connection
        ros = ROSConnection.GetOrCreateInstance();

        if (!IsGlobal)
            begin = transform.position;

        ros.Subscribe<RosMessageTypes.Nav.OdometryMsg>(TopicName, CallBack);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = position + begin;
        transform.localEulerAngles = new(0, (float)yaw, 0);
    }
    void CallBack(RosMessageTypes.Nav.OdometryMsg msg)
    {
        position = new Vector3((float)msg.pose.pose.position.x, 0, -(float)msg.pose.pose.position.y);

        // yaw (z-axis rotation)
        var q = msg.pose.pose.orientation;
        double siny_cosp = 2 * (q.w * q.z + q.x * q.y);
        double cosy_cosp = 1 - 2 * (q.y * q.y + q.z * q.z);
        yaw = Math.Atan2(siny_cosp, cosy_cosp) * Mathf.Rad2Deg;
        // transform.localEulerAngles = new(0, (float)msg.Theta, 0);
    }
}
