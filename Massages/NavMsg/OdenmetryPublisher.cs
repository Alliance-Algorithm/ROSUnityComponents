using System.Collections;
using ROS2;
using std_msgs.msg;
using UnityEngine;

public class OdenmetryPublisher : MonoBehaviour
{
    public string NodeName = "unity_ros2_node";
    public string TopicName = "/sentry/transform/publish"; // Change this to your desired image topic
    public string FrameId = "unity"; // Change this to your desired image topic
    public float fps = 10;
    private IPublisher<nav_msgs.msg.Odometry> publisher;
    private ROS2UnityComponent ros2Unity;
    private ROS2Node ros2Node;
    public bool IsGlobal = false;
    Vector3 begin = new();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize ROS connection
        ros2Unity = GetComponent<ROS2UnityComponent>();
        if (!IsGlobal)
            begin = transform.position;

        ros2Node = ros2Unity.CreateOrGetNode(NodeName);
        publisher = ros2Node.CreatePublisher<nav_msgs.msg.Odometry>(TopicName);

        StartCoroutine(Publisher());
    }

    // Update is called once per frame
    void Update()
    {

    }
    IEnumerator Publisher()
    {
        while (true)
        {
            yield return new WaitForSeconds(1 / fps);
            if (!ros2Unity.Ok())
                continue;
            nav_msgs.msg.Odometry msg = new();
            var v = transform.position - begin;
            msg = new()
            {
                Header = new() { Frame_id = FrameId },
                Pose = new()
                {
                    Pose = new()
                    {
                        Position = new() { X = -v.z, Y = -v.x, Z = v.y },
                        Orientation = new() { X = -transform.rotation.z, Y = -transform.rotation.x, Z = transform.rotation.y, W = transform.rotation.w }
                    }
                }
            };

            publisher.Publish(msg);
        }
    }
}
