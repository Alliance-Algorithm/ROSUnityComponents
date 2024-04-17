using System;
using System.Collections;
using System.Collections.Generic;
using ROS2;
using std_msgs.msg;
using UnityEngine;

[RequireComponent(typeof(ROS2UnityComponent))]
public class LidarPointCloud2 : MonoBehaviour
{
    public string NodeName = "unity_ros2_node";
    public string TopicName = "/sentry/transform/publish"; // Change this to your desired image topic
    public string FrameId = "unity"; // Change this to your desired image topic
    public float fps = 10;
    private IPublisher<sensor_msgs.msg.PointCloud2> publisher;
    private ROS2UnityComponent ros2Unity;
    private ROS2Node ros2Node;
    public Lidar lidar;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize ROS connection
        ros2Unity = GetComponent<ROS2UnityComponent>();

        ros2Node = ros2Unity.CreateOrGetNode(NodeName);
        publisher = ros2Node.CreatePublisher<sensor_msgs.msg.PointCloud2>(TopicName);
        _time = 1 / fps;
        time = _time;
    }

    float _time;
    float time;
    void Update()
    {
        Publisher();
    }
    void Publisher()
    {
        _time += Time.deltaTime;
        if (time > _time)
            return;
        _time = 0;
        if (!ros2Unity.Ok())
            return;
        sensor_msgs.msg.PointCloud2 msg = new();
        var v = lidar.Point;
        List<byte> bytes = new();
        foreach (var i in v)
        {
            bytes.AddRange(BitConverter.GetBytes(i.z));
            bytes.AddRange(BitConverter.GetBytes(-i.x));
            bytes.AddRange(BitConverter.GetBytes(i.y));
        }
        msg.Fields = new sensor_msgs.msg.PointField[] {
                new (){ Name = "x", Datatype = 7, Count = 1, Offset = 0 },
                new () { Name = "y", Datatype = 7, Count = 1, Offset = 4 } ,
                new () { Name = "z", Datatype = 7, Count = 1, Offset = 8 } };
        msg.Data = bytes.ToArray();
        msg.Header = new Header()
        {
            Frame_id = FrameId
        };
        msg.Is_bigendian = false;
        msg.Point_step = 12;
        msg.Is_dense = true;
        msg.Row_step = (uint)bytes.Count;
        msg.Height = 1;
        msg.Width = (uint)v.Count;
        publisher.Publish(msg);
        msg.Dispose();

    }
}
