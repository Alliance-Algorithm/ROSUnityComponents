using System.Collections;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using sensor_msgs.msg;
using std_msgs.msg;
using UnityEngine;

namespace ROS2
{
    [RequireComponent(typeof(ROS2UnityComponent))]
    [RequireComponent(typeof(CameraCapturer))]
    public class RosImageSubscriber : MonoBehaviour
    {
        public string NodeName = "unity_ros2_node";
        public string TopicName = "/camera/image"; // Change this to your desired image topic
        public string FrameId = "unity"; // Change this to your desired image topic

        public bool ImageTransport = true;

        public float fps = 45;

        private IPublisher<Image> publisher;
        private Texture2D receivedTexture;

        private ROS2UnityComponent ros2Unity;

        private ROS2Node ros2Node;
        private CameraCapturer capturer;
        byte[] data;

        private void Start()
        {
            // Initialize ROS connection
            ros2Unity = GetComponent<ROS2UnityComponent>();

            ros2Node = ros2Unity.CreateOrGetNode(NodeName);
            publisher = ros2Node.CreateSensorPublisher<Image>(TopicName);
            capturer = GetComponent<CameraCapturer>();

            data = new byte[capturer.width * capturer.height * 3];
            StartCoroutine(Publisher());
        }

        IEnumerator Publisher()
        {
            while (true)
            {
                yield return new WaitForSeconds(1 / fps);
                if (!ros2Unity.Ok())
                    continue;
                if (!ImageTransport)
                    continue;
                if (capturer.texture == null)
                    continue;
                // 创建Image消息
                Header header = new Header
                {
                    Stamp = new builtin_interfaces.msg.Time(),
                    Frame_id = FrameId
                };
                Color32[] colors = capturer.texture.GetPixels32();
                for (int i = capturer.height - 1, k = 0; i >= 0; i--)
                    for (int j = 0, l = i * capturer.width; j < capturer.width; j++)
                    {
                        data[k++] = colors[l].r;
                        data[k++] = colors[l].g;
                        data[k++] = colors[l].b;
                        l++;
                    }
                Image msg = new Image
                {
                    Header = header,
                    Data = data,
                    Encoding = "rgb8",
                    Step = (uint)(3 * capturer.width),
                    Width = (uint)capturer.width,
                    Height = (uint)capturer.height
                };
                publisher.Publish(msg);
            }
        }


    }
}