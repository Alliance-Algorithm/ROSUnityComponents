using System.Collections;
using ROS2;
using std_msgs.msg;
using UnityEngine;

[RequireComponent(typeof(ROS2UnityComponent))]
public class TransformPublisher : MonoBehaviour
{
    public string NodeName = "unity_ros2_node";
    public string TopicName = "/sentry/transform/publish"; // Change this to your desired image topic
    public string FrameId = "unity"; // Change this to your desired image topic
    public float fps = 10;
    private IPublisher<geometry_msgs.msg.Pose2D> publisher;
    private ROS2UnityComponent ros2Unity;
    private ROS2Node ros2Node;
    private CameraCapturer capturer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize ROS connection
        ros2Unity = GetComponent<ROS2UnityComponent>();

        ros2Node = ros2Unity.CreateOrGetNode(NodeName);
        publisher = ros2Node.CreatePublisher<geometry_msgs.msg.Pose2D>(TopicName);

        StartCoroutine(Publisher());
    }
    IEnumerator Publisher()
    {
        while (true)
        {
            yield return new WaitForSeconds(1 / fps);
            if (!ros2Unity.Ok())
                continue;
            geometry_msgs.msg.Pose2D msg = new();
            msg = new()
            {
                X = transform.position.y,
                Y = -transform.position.x,
                Theta = -transform.localEulerAngles.y
            };

            publisher.Publish(msg);
        }
    }
}
