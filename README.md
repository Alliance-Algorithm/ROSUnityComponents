# ROS UNITY Components

## 环境
- Unity: 2022.x - 6.x
- Ros：any

使用unity官方套件，效率比较低，有高效率版本的，但是需要本机ros环境，俺不喜欢

## 安装
1. 安装到你的ubuntu端并运行 https://github.com/Unity-Technologies/ROS-TCP-Endpoint
2. 安装到你的Unity项目中 https://github.com/Unity-Technologies/ROS-TCP-Connector
3. 找到你项目的Assert文件夹
4. git pull https://github.com/Alliance-Algorithm/ROSUnityComponents
5. （也许）愉快的使用

## 文件架构
```
├─Massages
│  ├─ControlMsg
│  ├─GeometryMsg
│  │  └─Transform
│  ├─NavMsg
│  └─SensorMsgs
│      ├─CompossedImage
│      ├─OccupyGrid
│      └─PointCloud
└─UnityActions
    ├─Controller
    ├─ESDFBuilder
    ├─Filter
    ├─Helper
    ├─MeshToPcd
    ├─NavFunction
    └─Sensor
```
### Massages
 发布到ROS的消息都在这里面
### Unity Action
 接受并作出行动的部分

## 交流方式
  qq 1780284652
 
