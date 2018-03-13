using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffScreenIndicatorScript : MonoBehaviour {


    public Texture2D icon; //The icon. Preferably an arrow pointing upwards.
    public float iconSize = 10f;
    [HideInInspector]
    public GUIStyle gooey; //GUIStyle to make the box around the icon invisible. Public so that everything has the default stats.
    Vector3 indRange;
    float scaleRes = Screen.width / 1000; //The width of the screen divided by 500. Will make the GUI automatically
                                         //scale with varying resolutions.
    Camera cam;
    bool visible = true; //Whether or not the object is visible in the camera.

    void Start()
    {
        visible = GetComponent<SpriteRenderer>().isVisible;

        cam = Camera.main; //Don't use Camera.main in a looping method, its very slow, as Camera.main actually
                           //does a GameObject.Find for an object tagged with MainCamera.

        indRange.x = Screen.width - (Screen.width / 6);
        indRange.z = Screen.height - (Screen.height / 7);
        indRange /= 2f;

        gooey.normal.textColor = new Vector4(0, 0, 0, 0); //Makes the box around the icon invisible.
    }

    void OnGUI()
    {
        if (!visible)
        {
            Vector3 dir = transform.position - cam.transform.position;
            dir = Vector3.Normalize(dir);
            dir.z *= -1f;

            Vector3 indPos = new Vector3(indRange.x * dir.x, indRange.z * dir.z);
            indPos = new Vector3((Screen.width / 2) + indPos.x,
                              (Screen.height / 2) + indPos.z);

            Vector3 pdir = transform.position - cam.ScreenToWorldPoint(new Vector3(indPos.x, transform.position.y,
                                                                                    indPos.z));
            pdir = Vector3.Normalize(pdir);

            float angle = Mathf.Atan2(pdir.x, pdir.z) * Mathf.Rad2Deg;

            GUIUtility.RotateAroundPivot(angle, indPos); //Rotates the GUI. Only rotates GUI drawn after the rotate is called, not before.
            GUI.Box(new Rect(indPos.x, indPos.z, scaleRes * iconSize, scaleRes * iconSize), icon, gooey);
            GUIUtility.RotateAroundPivot(0, indPos); //Rotates GUI back to the default so that GUI drawn after is not rotated.
        }
    }

    void OnBecameInvisible()
    {
        visible = false;
    }
    //Turns off the indicator if object is onscreen.
    void OnBecameVisible()
    {
        visible = true;
    }
}
