using System;
using ROS2;
using UnityEngine;

public class OdenmetrySubscriber : MonoBehaviour
{
    public string NodeName = "unity_ros2_node";
    public string TopicName = "/sentry/transform/show"; // Change this to your desired image topic
    public bool IsGlobal = false;
    Vector3 begin = new();
    private ISubscription<nav_msgs.msg.Odometry> subscription;
    private ROS2UnityComponent ros2Unity;
    private ROS2Node ros2Node;
    private CameraCapturer capturer;
    public Vector3 Begin => begin;
    Vector3 v = new();
    double r = new();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize ROS connection
        ros2Unity = GetComponent<ROS2UnityComponent>();

        if (!IsGlobal)
            begin = transform.position;

        ros2Node = ros2Unity.CreateOrGetNode(NodeName);
        subscription = ros2Node.CreateSubscription<nav_msgs.msg.Odometry>(TopicName, CallBack);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = v + begin;
        transform.localEulerAngles = new(0, (float)r, 0);
    }
    void CallBack(nav_msgs.msg.Odometry msg)
    {
        v = new Vector3(-(float)msg.Pose.Pose.Position.Y, 0, -(float)msg.Pose.Pose.Position.X);

        // yaw (z-axis rotation)
        var q = msg.Pose.Pose.Orientation;
        double siny_cosp = 2 * (q.W * q.Z + q.X * q.Y);
        double cosy_cosp = 1 - 2 * (q.Y * q.Y + q.Z * q.Z);
        r = Math.Atan2(siny_cosp, cosy_cosp) * Mathf.Rad2Deg;
        // transform.localEulerAngles = new(0, (float)msg.Theta, 0);
    }
}
