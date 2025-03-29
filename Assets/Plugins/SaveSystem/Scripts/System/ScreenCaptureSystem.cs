using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ScreenCaptureSystem : MonoBehaviour
{
    public static Camera gameScreen;
    public static Rect screenCaptureArea;


    public static Texture2D CaptureCamera()
    {
        return CaptureCamera(gameScreen, screenCaptureArea);
    }

    public static Texture2D CaptureCamera(Camera camera, Rect rect)
    {
        RenderTexture rt = new RenderTexture((int)rect.width, (int)rect.height, 0);
        camera.targetTexture = rt;
        camera.Render();

        RenderTexture.active = rt;
        Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height);
        screenShot.ReadPixels(rect, 0, 0);
        screenShot.Apply();

        camera.targetTexture = null;
        RenderTexture.active = null;
        GameObject.Destroy(rt);

        return screenShot;
    }
}
