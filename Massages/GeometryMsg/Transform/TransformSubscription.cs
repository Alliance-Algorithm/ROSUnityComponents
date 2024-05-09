using System.Collections;
using ROS2;
using std_msgs.msg;
using UnityEngine;

[RequireComponent(typeof(ROS2UnityComponent))]
public class TransformSubscription : MonoBehaviour
{
    public string NodeName = "unity_ros2_node";
    public string TopicName = "/sentry/transform/show"; // Change this to your desired image topic
    public bool IsGlobal = false;
    private ISubscription<geometry_msgs.msg.Pose> subscription;
    private ROS2UnityComponent ros2Unity;
    private ROS2Node ros2Node;
    private CameraCapturer capturer;
    Vector3 begin = new();
    Vector3 v = new();
    Quaternion r = new();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize ROS connection
        ros2Unity = GetComponent<ROS2UnityComponent>();

        if (!IsGlobal)
            begin = transform.position;

        ros2Node = ros2Unity.CreateOrGetNode(NodeName);
        subscription = ros2Node.CreateSubscription<geometry_msgs.msg.Pose>(TopicName, CallBack);

    }
    void CallBack(geometry_msgs.msg.Pose msg)
    {
        v = new Vector3((float)msg.Position.X, -(float)msg.Position.Y, (float)msg.Position.Z) + begin;
        r = new Quaternion((float)msg.Orientation.X, (float)msg.Orientation.Y, -(float)msg.Orientation.Z, (float)msg.Orientation.W) * Quaternion.Euler(180, 0, 0);
        // r = new Quaternion((float)msg.Orientation.X, (float)msg.Orientation.Y, -(float)msg.Orientation.Z, (float)msg.Orientation.W) * Quaternion.Euler(180, 0, 0);
        // transform.localEulerAngles = new(0, (float)msg.Theta, 0);

    }
    void Update()
    {
        transform.position = v / 1000;
        transform.localRotation = r;
    }
}
