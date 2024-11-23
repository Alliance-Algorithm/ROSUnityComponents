using System.Collections;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;

namespace ROS2
{
    [RequireComponent(typeof(CameraCapturer))]
    public class RosImageSubscriber : MonoBehaviour
    {
        public string TopicName = "/camera/image"; // Change this to your desired image topic
        public string FrameId = "unity"; // Change this to your desired image topic

        public bool ImageTransport = true;

        public float fps = 45;


        ROSConnection ros;
        private CameraCapturer capturer;
        byte[] data;

        private void Start()
        {
            // Initialize ROS connection
            ros = ROSConnection.GetOrCreateInstance();
            ros.RegisterPublisher<RosMessageTypes.Sensor.ImageMsg>(TopicName);
            capturer = GetComponent<CameraCapturer>();

            data = new byte[capturer.width * capturer.height * 3];
            StartCoroutine(Publisher());
        }

        IEnumerator Publisher()
        {
            while (true)
            {
                yield return new WaitForSeconds(1 / fps);
                if (!ImageTransport)
                    continue;
                if (capturer.texture == null)
                    continue;
                // 创建Image消息
                RosMessageTypes.Std.HeaderMsg header = new()
                {
                    stamp = new(),
                    frame_id = FrameId
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
                RosMessageTypes.Sensor.ImageMsg msg = new()
                {
                    header = header,
                    data = data,
                    encoding = "rgb8",
                    step = (uint)(3 * capturer.width),
                    width = (uint)capturer.width,
                    height = (uint)capturer.height
                };
                ros.Publish(TopicName, msg);
            }
        }


    }
}