using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.XR.ARFoundation;

public class ShareScreenShot : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuCanvas;
    private ARPointCloudManager aRPointCloudManager;
    // Start is called before the first frame update
    void Start()
    {
        aRPointCloudManager = FindObjectOfType<ARPointCloudManager>();        
    }

    public void TakeScreenShot()
    {
        TurnOnOffARContents();
        StartCoroutine(TakeScreenShotAndShare());
    }

    private void TurnOnOffARContents()
    {
        var points = aRPointCloudManager.trackables;
        foreach(var point in points)
        {
            point.gameObject.SetActive(!point.gameObject.activeSelf);
        }
        mainMenuCanvas.SetActive(!mainMenuCanvas.activeSelf);
    }

    private IEnumerator TakeScreenShotAndShare()
    {
        yield return new WaitForEndOfFrame();

        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();

        string filePath = Path.Combine(Application.temporaryCachePath, "Compartida img.png");
        File.WriteAllBytes(filePath, ss.EncodeToPNG());

        Destroy(ss);

        new NativeShare().AddFile(filePath)
            .SetSubject("EL SUJETO ESTA AQUI").SetText("Hola mira mi aplicacion de AR, disfruta esta experiencia!")
            .SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
            .Share();
        TurnOnOffARContents();
    }
}
