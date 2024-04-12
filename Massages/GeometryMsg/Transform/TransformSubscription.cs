using System.Collections;
using ROS2;
using std_msgs.msg;
using UnityEngine;

public class TransformSubscription : MonoBehaviour
{
    public string NodeName = "unity_ros2_node";
    public string TopicName = "/transform"; // Change this to your desired image topic
    public string FrameId = "unity"; // Change this to your desired image topic
    public float fps = 10;
    private ISubscription<geometry_msgs.msg.Pose2D> subscription;
    private ROS2UnityComponent ros2Unity;
    private ROS2Node ros2Node;
    private CameraCapturer capturer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize ROS connection
        ros2Unity = GetComponent<ROS2UnityComponent>();

        ros2Node = ros2Unity.CreateOrGetNode(NodeName);
        subscription = ros2Node.CreateSubscription<geometry_msgs.msg.Pose2D>(TopicName, CallBack);
    }
    void CallBack(geometry_msgs.msg.Pose2D msg)
    {
        transform.position = new Vector3((float)msg.Y, -(float)msg.X, 0);
        transform.localEulerAngles = new(0, (float)msg.Theta, 0);
    }
}
