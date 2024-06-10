using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ROS2;
using std_msgs.msg;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(ROS2UnityComponent))]
public class PathPublisher : MonoBehaviour
{
    public string NodeName = "unity_ros2_node";
    public string TopicName = "/sentry/nav/global/path"; // Change this to your desired image topic\
    public string FrameId = "unity"; // Change this to your desired image topic
    private IPublisher<nav_msgs.msg.Path> publisher;
    private ROS2UnityComponent ros2Unity;
    private ROS2Node ros2Node;
    public float fps = 10;
    public NavMeshUpdater navUpdater;
    private Vector3 begin;

    public std_msgs.msg.Header header;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        begin = transform.position;
        // Initialize ROS connection
        ros2Unity = GetComponent<ROS2UnityComponent>();

        ros2Node = ros2Unity.CreateOrGetNode(NodeName);
        publisher = ros2Node.CreatePublisher<nav_msgs.msg.Path>(TopicName);

        StartCoroutine(Publisher());
    }
    IEnumerator Publisher()
    {
        header = new Header() { Frame_id = FrameId };
        var trans = GetComponent<OdenmetrySubscriber>();
        while (true)
        {
            yield return new WaitForSeconds(1 / fps);
            nav_msgs.msg.Path msg = new();
            List<geometry_msgs.msg.PoseStamped> poses = new();
            foreach (var c in navUpdater.Path.corners)
            {
                var d = c - begin;
                // Debug.Log(begin);
                poses.Add(new() { Pose = new() { Position = new() { X = d.z, Y = -d.x } }, Header = header });
            }
            msg.Poses = poses.ToArray();
            msg.Header = header;
            publisher.Publish(msg);
        }
    }
}
