using System.Collections;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;

public class TransformSubscription : MonoBehaviour
{
    public string NodeName = "unity_ros2_node";
    public string TopicName = "/sentry/transform/show"; // Change this to your desired image topic
    public bool IsGlobal = false;

    ROSConnection ros;
    private CameraCapturer capturer;
    Vector3 begin = new();
    Vector3 v = new();
    Quaternion r = new();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize ROS connection
        ros = ROSConnection.GetOrCreateInstance();

        if (!IsGlobal)
            begin = transform.position;

        ros.Subscribe<RosMessageTypes.Geometry.PoseMsg>(TopicName, CallBack);

    }
    void CallBack(RosMessageTypes.Geometry.PoseMsg msg)
    {
        v = new Vector3((float)msg.position.x, (float)msg.position.z, -(float)msg.position.y) + begin;
        r = new Quaternion((float)msg.orientation.x, (float)msg.orientation.z, -(float)msg.orientation.y, (float)msg.orientation.w) * Quaternion.Euler(180, 0, 0);
        // r = new Quaternion((float)msg.Orientation.X, (float)msg.Orientation.Y, -(float)msg.Orientation.Z, (float)msg.Orientation.W) * Quaternion.Euler(180, 0, 0);
        // transform.localEulerAngles = new(0, (float)msg.Theta, 0);

    }
    void Update()
    {
        transform.position = v / 1000;
        transform.localRotation = r;
    }
}
