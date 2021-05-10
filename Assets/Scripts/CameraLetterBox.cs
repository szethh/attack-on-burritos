 using System;
 using UnityEngine;
 
public class CameraLetterBox : MonoBehaviour
{
     public Camera backgroundCam;
     public Camera mainCam;
     public float targetAspectRatio = 1f;
     public static CameraLetterBox Singleton;
    public Rect r;
     
     
     private void Awake()
     {
         //DontDestroyOnLoad(gameObject);
         //DontDestroyOnLoad(backgroundCam.gameObject);
         Singleton = this;
     }
     
     private void Start()
     {
         mainCam = GetComponent<Camera>();

         if (backgroundCam == null)
         {
             backgroundCam = new GameObject("BackgroundCam").AddComponent<Camera>();
         }
         backgroundCam.depth = mainCam.depth - 1;
     }
     
     private void Update()
     {
         float w = Screen.width;
         float h = Screen.height;
         var a = w / h;
         
         if (a > targetAspectRatio)
         {
             var tw = h * targetAspectRatio;
             var o = (w - tw) * 0.5f;
             r = new Rect(o,0,tw,h);
         }
         else
         {
             var th = w / targetAspectRatio;
             var o = (h - th) * 0.5f;
             r = new Rect(0, o, w, th);
         }
         mainCam.pixelRect = r;
     }
 }