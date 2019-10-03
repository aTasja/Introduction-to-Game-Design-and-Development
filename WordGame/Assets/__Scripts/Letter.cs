using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Letter : MonoBehaviour {

    [Header("Set in Inspector")]
    public float timeDuration = 0.5f;
    public string easingCuve = Easing.InOut; // Easing from Utils

    [Header("Set Dynamically")]
    public TextMesh tMesh; // The TextMesh shows the char
    public Renderer tRend; // The Renderer of 3D Text. This will
                           // determine whether the char is visible
    public bool big = false; // Big letters act a little differently
    // Linear interpolation fields
    public List<Vector3> pts = null;
    public float timeStart = -1;

    private char _c; // The char shown on this Letter
    private Renderer rend;

    void Awake()
    {
        tMesh = GetComponentInChildren<TextMesh>();
        tRend = tMesh.GetComponent<Renderer>();
        rend = GetComponent<Renderer>();
        visible = false;
    }
    
    // Property to get or set _c and the letter shown by 3D Text
    public char c
    {
        get { return (_c); }
        set
        {
            _c = value;
            tMesh.text = _c.ToString();
        }
    }
    
    // Gets or sets _c as a string
    public string str
    {
        get { return (_c.ToString()); }
        set { c = value[0]; }
    }
   
    // Enables or disables the renderer for 3D Text, which causes the char to be
    // visible or invisible respectively.
    public bool visible
    {
        get { return (tRend.enabled); }
        set { tRend.enabled = value; }
    }
    
    // Gets or sets the color of the rounded rectangle
    public Color color
    {
        get { return (rend.material.color); }
        set { rend.material.color = value; }
    }
    
    // Sets the position of the Letter's gameObject
    public Vector3 pos
    {
        set
        {
            //transform.position = value;
            // More will be added here later

            // Find a midpoint that is a random distance from the actual
            // midpoint between the current position and the value passed in
            Vector3 mid = (transform.position + value) / 2f;

            // The random distance will be within 1/4 of the magnitude of the
            // line from the actual midpoint
            float mag = (transform.position - value).magnitude;
            mid += Random.insideUnitSphere * mag * 0.25f;

            // Create a List<Vector3> of Bezier points
            pts = new List<Vector3>() { transform.position, mid, value };
            
            // If timeStart is at the default -1, then set it
            if (timeStart == -1) timeStart = Time.time;
        }
    }

    // Moves immediately to the new position
    public Vector3 posImmediate
    {
        set
        {
            transform.position = value;
        }
    }

    // Interpolation code
    void Update()
    {
        if (timeStart == -1) return;

        // Standard linear interpolation code
        float u = (Time.time - timeStart) / timeDuration;
        u = Mathf.Clamp01(u);
        float u1 = Easing.Ease(u, easingCuve);
        Vector3 v = Utils.Bezier(u1, pts);
        transform.position = v;
        
        // If the interpolation is done, set timeStart back to -1
        if (u == 1) timeStart = -1;
    }

}