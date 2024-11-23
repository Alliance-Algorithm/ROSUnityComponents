using System;
using System.Collections;
using System.Collections.Generic;
using ROS2;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;

public class LidarPointCloud2 : MonoBehaviour
{
    public string NodeName = "unity_ros2_node";
    public string TopicName = "/sentry/transform/publish"; // Change this to your desired image topic
    public string FrameId = "unity"; // Change this to your desired image topic
    public float fps = 10;
    ROSConnection ros;
    public Lidar lidar;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize ROS connection
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<RosMessageTypes.Sensor.PointCloud2Msg>(TopicName);
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
        RosMessageTypes.Sensor.PointCloud2Msg msg = new();
        var v = lidar.Point;
        List<byte> bytes = new();
        foreach (var i in v)
        {
            bytes.AddRange(BitConverter.GetBytes(i.z));
            bytes.AddRange(BitConverter.GetBytes(-i.x));
            bytes.AddRange(BitConverter.GetBytes(i.y));
        }
        msg.fields = new RosMessageTypes.Sensor.PointFieldMsg[] {
                new (){ name = "x", datatype = 7, count = 1, offset = 0 },
                new () { name = "y", datatype = 7, count = 1, offset = 4 } ,
                new () { name = "z", datatype = 7,count = 1, offset = 8 } };
        msg.data = bytes.ToArray();
        msg.header = new()
        {
            frame_id = FrameId
        };
        msg.is_bigendian = false;
        msg.point_step = 12;
        msg.is_dense = true;
        msg.row_step = (uint)bytes.Count;
        msg.height = 1;
        msg.width = (uint)v.Count;
        ros.Publish(TopicName, msg);
    }
}
