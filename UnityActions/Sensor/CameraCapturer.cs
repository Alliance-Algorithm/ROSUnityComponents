using System.Collections;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class CameraCapturer : MonoBehaviour
{
    public Camera cameraToCapture;
    public int width = 1920;
    public int height = 1080;
    public Texture2D texture { get; private set; }
    private void Start()
    {
        StartCoroutine(CaptureAndSaveScreenshot());
    }
    private IEnumerator CaptureAndSaveScreenshot()
    {
        while (true)
        {
            // 创建RenderTexture
            RenderTexture rt = new RenderTexture(width, height, 24);
            cameraToCapture.targetTexture = rt;

            // 渲染摄像机的视图到RenderTexture
            cameraToCapture.Render();

            // 激活RenderTexture，以便能够读取像素信息
            RenderTexture.active = rt;

            DestroyImmediate(texture);

            // 创建Texture2D，其大小与RenderTexture相同
            texture = new Texture2D(width, height, TextureFormat.RGB24, false);

            // 将RenderTexture的像素信息读取到Texture2D
            texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);

            // 应用更改
            texture.Apply();

            // 重置摄像机的targetTexture和激活的RenderTexture
            cameraToCapture.targetTexture = null;
            RenderTexture.active = null;

            // 销毁RenderTexture
            Destroy(rt);

            yield return new WaitForEndOfFrame();
        }
    }
}