using System.Collections;
using ROS2;
using std_msgs.msg;
using UnityEngine;

[RequireComponent(typeof(ROS2UnityComponent))]
public class TransformSubscription2D : MonoBehaviour
{
    public string NodeName = "unity_ros2_node";
    public string TopicName = "/sentry/transform/show"; // Change this to your desired image topic
    public bool IsGlobal = false;
    private ISubscription<geometry_msgs.msg.Pose2D> subscription;
    private ROS2UnityComponent ros2Unity;
    private ROS2Node ros2Node;
    private CameraCapturer capturer;
    Vector3 begin = new();
    Vector3 v = new();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize ROS connection
        ros2Unity = GetComponent<ROS2UnityComponent>();

        if (!IsGlobal)
            begin = transform.position;

        ros2Node = ros2Unity.CreateOrGetNode(NodeName);
        subscription = ros2Node.CreateSubscription<geometry_msgs.msg.Pose2D>(TopicName, CallBack);

    }
    void CallBack(geometry_msgs.msg.Pose2D msg)
    {
        v = new Vector3((float)msg.X, 0, (float)msg.Y) + begin;
        // transform.localEulerAngles = new(0, (float)msg.Theta, 0);
    }
    void Update()
    {
        transform.position = v;
    }
}
