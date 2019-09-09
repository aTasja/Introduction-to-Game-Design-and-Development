#define LightCone_OnDrawGizmos_Collide

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof(Light) )]
public class LightCone : MonoBehaviour
{
    public int          wedges = 36;
    public float        dist = 20;
    public float        width = 2;
    public float        height = 2;
    [SerializeField]
	private Color       _color = new Color(1, 1, 1, 0.25f);
	public LayerMask    collidesWith;
    public int          gizmoWedges = 4;

    [Header("Spotlight Settings")]
    public bool         spotlightEnabled = true;
    public float        spotAnglePercent = 100;
    public float        rangePercent = 100;


    MeshFilter          mF;
    Mesh                mesh;
    Vector3[]           verts;
    int[]               tris;
    Material            mat;
	RaycastHit[]        hits;
    
    private Light       lit;
    private float       dist0, width0, sAP0, rP0;

	public Color color
	{
		get
		{
			return _color;
		}

		set
		{
			_color = value;
			UpdateLight();
		}
	}


	// Use this for initialization
    void Start()
    {
        mF = GetComponent<MeshFilter>();
        mesh = mF.mesh;

        mesh.Clear();

        MakeVerts();

        tris = new int[wedges*3];

        int triN=0;
        int vertN=1;
        for (int i = 0; i < wedges; i++)
        {
            tris[triN] = 0;
            triN++;
            if (vertN < verts.Length - 1)
            {
                tris[triN] = vertN+1;
            }
            else
            {
                tris[triN] = 1;
            }
            triN++;
            tris[triN] = vertN;
            vertN++;
            triN++;
        }
        mesh.triangles = tris;

        mat = GetComponent<Renderer>().material;

        UpdateLight();
	}


    void MakeVerts()
    {
        verts = new Vector3[wedges+1];
        verts[0] = Vector3.zero;

        Vector3 v, r;
        hits = new RaycastHit[wedges];
        for (int i = 0; i < wedges; i++)
        {
            v = Vector3.zero;
            v.z = dist;
            v.x = Mathf.Cos(i * Mathf.PI * 2 / wedges) * width;
            v.y = Mathf.Sin(i * Mathf.PI * 2 / wedges) * height;
            r = transform.rotation * v.normalized;
            if (Physics.Raycast(transform.position, r, out hits[i], v.magnitude, collidesWith))
            {
                v = v.normalized * hits[i].distance;
                
			}
            verts[i+1] = v;
        }
        mesh.vertices = verts;
    }
	
	
	// Update is called once per frame
    void Update()
    {
        MakeVerts();
        if (color != mat.color)
        {
            mat.color = color;
        }
	}


    // This draws the outline when not playing (and Selected)
    void OnDrawGizmosSelected()
    {
        if (Application.isPlaying) return;
        Vector3[] pts = new Vector3[gizmoWedges];
        Vector3 v, vDir;

        Color litColor = color;
        litColor.a = 1;
#if LightCone_OnDrawGizmos_Collide
        Color grayColor = Color.Lerp(Color.clear, color, 0.5f);
        grayColor.a = 0.5f;
#else
        Gizmos.color = litColor;
#endif

        for (int i = 0; i < gizmoWedges; i++)
        {
            v = Vector3.zero;
            v.z = dist;
            v.x = Mathf.Cos(i * Mathf.PI * 2 / gizmoWedges) * width;
            v.y = Mathf.Sin(i * Mathf.PI * 2 / gizmoWedges) * height;
            vDir = transform.rotation * v;
            pts[i] = vDir;
            pts[i] += transform.position;

#if LightCone_OnDrawGizmos_Collide
#else
            Gizmos.DrawLine(transform.position, pts[i]);
            if (i>0) {
                Gizmos.DrawLine(pts[i-1], pts[i]);
                if (i == gizmoWedges-1) {
                    Gizmos.DrawLine(pts[i], pts[0]);
                }
            }
#endif
        }

#if LightCone_OnDrawGizmos_Collide
        Vector3[] ptsColl = new Vector3[gizmoWedges];
        RaycastHit hitInfo;
        for (int i = 0; i < pts.Length; i++)
        {
            ptsColl[i] = pts[i];
            vDir = ptsColl[i] - transform.position;
            if (Physics.Raycast(transform.position, vDir, out hitInfo, vDir.magnitude, collidesWith))
            {
                // The line collided with something, so make it shorter
                ptsColl[i] = hitInfo.point;
                
            }

            // Draw the area past the collision
            Gizmos.color = grayColor;
            Gizmos.DrawLine(ptsColl[i], pts[i]);
            if (i > 0)
            {
                Gizmos.DrawLine(pts[i - 1], pts[i]);
                if (i == gizmoWedges - 1)
                {
                    Gizmos.DrawLine(pts[i], pts[0]);
                }
            }
            // Draw the collision
            Gizmos.color = litColor;
            Gizmos.DrawLine(transform.position, ptsColl[i]);
            if (i > 0)
            {
                Gizmos.DrawLine(ptsColl[i - 1], ptsColl[i]);
                if (i == gizmoWedges - 1)
                {
                    Gizmos.DrawLine(ptsColl[i], ptsColl[0]);
                }
            }
        }
#endif

    }


    private void OnDrawGizmos()
    {
        if (Application.isPlaying) return;
        UpdateLight();
    }


    void UpdateLight()
    {
        if (lit == null)
        {
            lit = GetComponent<Light>();
            lit.type = LightType.Spot;
        }

        if (dist0 != dist || width0 != width || sAP0 != spotAnglePercent || rangePercent != rP0)
        {
            lit.spotAngle = Mathf.Atan2(width, dist) * 360 / Mathf.PI * (spotAnglePercent / 100f);
            lit.range = dist * rangePercent / 100f;
            dist0 = dist;
            width0 = width;
            sAP0 = spotAnglePercent;
            rP0 = rangePercent;
        }

        lit.color = color;

        lit.enabled = spotlightEnabled;
    }


    public RaycastHit[] GetRaycastHits()
    {
		return hits;
	}
}
