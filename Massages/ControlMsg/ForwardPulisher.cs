using System.Collections;
using ROS2;
using std_msgs.msg;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(ROS2UnityComponent))]
public class ForwardPulisher : MonoBehaviour
{
    public string NodeName = "unity_ros2_node";
    public string TopicName = "/sentry/forward_dir/pub"; // Change this to your desired image topic
    public float fps = 10;
    public float SpeedMax = 0.3f;
    public float SpeedMin = 0.1f;
    public float MaxSpeedDistance = 2;

    public Transform Target;
    private IPublisher<geometry_msgs.msg.Pose2D> publisher;
    private ROS2UnityComponent ros2Unity;
    private ROS2Node ros2Node;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
            Vector3 vec = Target.position - transform.position;
            vec = new(vec.x, 0, vec.z);
            vec = (Mathf.Clamp01(vec.magnitude / MaxSpeedDistance) * (SpeedMax - SpeedMin) + SpeedMin) * vec.normalized;
            Debug.DrawLine(transform.position, vec + transform.position);
            msg = new()
            {
                X = vec.x,
                Y = vec.z,
                // Theta = -transform.localEulerAngles.y
            };

            publisher.Publish(msg);
        }
    }
}
