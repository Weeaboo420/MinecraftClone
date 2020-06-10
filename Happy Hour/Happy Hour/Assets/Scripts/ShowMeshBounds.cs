using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

//[ExecuteInEditMode()]

public class ShowMeshBounds : MonoBehaviour
{
    private Color32 color = Color.green;
    private bool draw = false;

    private Vector3 v3FrontTopLeft;
    private Vector3 v3FrontTopRight;
    private Vector3 v3FrontBottomLeft;
    private Vector3 v3FrontBottomRight;
    private Vector3 v3BackTopLeft;
    private Vector3 v3BackTopRight;
    private Vector3 v3BackBottomLeft;
    private Vector3 v3BackBottomRight;

    private Material material;

    /*void Update()
    {
        //CalcPositons();
        //DrawBox();
    }*/

    public bool Draw
    {
        get
        {
            return draw;
        }

        set
        {
            draw = value;
            switch(draw)
            {
                case true:
                    material = Resources.Load<Material>("Materials/line");
                    DrawMeshBounds();
                    break;
                case false:
                    foreach(Transform child in transform)
                    {
                        Destroy(child.gameObject);
                    }
                    break;
            }
        }
    }

    private void DrawMeshBounds()
    {
        CalcPositons();
        DrawBox();
    }

    void CalcPositons()
    {
        Bounds bounds = GetComponent<MeshFilter>().mesh.bounds;

        //Bounds bounds;
        //BoxCollider bc = GetComponent<BoxCollider>();
        //if (bc != null)
        //    bounds = bc.bounds;
        //else
        //return;

        Vector3 v3Center = bounds.center;
        Vector3 v3Extents = bounds.extents;

        v3FrontTopLeft = new Vector3(v3Center.x - v3Extents.x, v3Center.y + v3Extents.y, v3Center.z - v3Extents.z);  // Front top left corner
        v3FrontTopRight = new Vector3(v3Center.x + v3Extents.x, v3Center.y + v3Extents.y, v3Center.z - v3Extents.z);  // Front top right corner
        v3FrontBottomLeft = new Vector3(v3Center.x - v3Extents.x, v3Center.y - v3Extents.y, v3Center.z - v3Extents.z);  // Front bottom left corner
        v3FrontBottomRight = new Vector3(v3Center.x + v3Extents.x, v3Center.y - v3Extents.y, v3Center.z - v3Extents.z);  // Front bottom right corner
        v3BackTopLeft = new Vector3(v3Center.x - v3Extents.x, v3Center.y + v3Extents.y, v3Center.z + v3Extents.z);  // Back top left corner
        v3BackTopRight = new Vector3(v3Center.x + v3Extents.x, v3Center.y + v3Extents.y, v3Center.z + v3Extents.z);  // Back top right corner
        v3BackBottomLeft = new Vector3(v3Center.x - v3Extents.x, v3Center.y - v3Extents.y, v3Center.z + v3Extents.z);  // Back bottom left corner
        v3BackBottomRight = new Vector3(v3Center.x + v3Extents.x, v3Center.y - v3Extents.y, v3Center.z + v3Extents.z);  // Back bottom right corner

        v3FrontTopLeft = transform.TransformPoint(v3FrontTopLeft);
        v3FrontTopRight = transform.TransformPoint(v3FrontTopRight);
        v3FrontBottomLeft = transform.TransformPoint(v3FrontBottomLeft);
        v3FrontBottomRight = transform.TransformPoint(v3FrontBottomRight);
        v3BackTopLeft = transform.TransformPoint(v3BackTopLeft);
        v3BackTopRight = transform.TransformPoint(v3BackTopRight);
        v3BackBottomLeft = transform.TransformPoint(v3BackBottomLeft);
        v3BackBottomRight = transform.TransformPoint(v3BackBottomRight);
    }

    void DrawBox()
    {

        //Destroy children if there are any
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }

#region Front Top
        GameObject FrontTop = new GameObject("FrontTop");

        FrontTop.AddComponent<LineRenderer>();
        LineRenderer frontTop = FrontTop.GetComponent<LineRenderer>();   
        frontTop.SetPositions(new Vector3[] {v3FrontTopLeft, v3FrontTopRight});
        frontTop.startColor = color;
        frontTop.endColor = color;
        frontTop.startWidth = 0.1f;
        frontTop.endWidth = 0.1f;

        FrontTop.transform.parent = this.transform;
#endregion

#region Back Top
        GameObject BackTop = new GameObject("BackTop");

        BackTop.AddComponent<LineRenderer>();
        LineRenderer backTop = BackTop.GetComponent<LineRenderer>();   
        backTop.SetPositions(new Vector3[] {v3BackTopLeft, v3BackTopRight});
        backTop.startColor = color;
        backTop.endColor = color;
        backTop.startWidth = 0.1f;
        backTop.endWidth = 0.1f;

        BackTop.transform.parent = this.transform;
#endregion

#region Front Bottom
        GameObject FrontBottom = new GameObject("FrontBottom");

        FrontBottom.AddComponent<LineRenderer>();
        LineRenderer frontBottom = FrontBottom.GetComponent<LineRenderer>();   
        frontBottom.SetPositions(new Vector3[] {v3FrontBottomLeft, v3FrontBottomRight});
        frontBottom.startColor = color;
        frontBottom.endColor = color;
        frontBottom.startWidth = 0.1f;
        frontBottom.endWidth = 0.1f;

        FrontBottom.transform.parent = this.transform;
#endregion

#region Back Bottom
        GameObject BackBottom = new GameObject("BackBottom");

        BackBottom.AddComponent<LineRenderer>();
        LineRenderer backBottom = BackBottom.GetComponent<LineRenderer>();   
        backBottom.SetPositions(new Vector3[] {v3BackBottomLeft, v3BackBottomRight});
        backBottom.startColor = color;
        backBottom.endColor = color;
        backBottom.startWidth = 0.1f;
        backBottom.endWidth = 0.1f;

        BackBottom.transform.parent = this.transform;
#endregion

#region Right Top
        GameObject RightTop = new GameObject("RightTop");

        RightTop.AddComponent<LineRenderer>();
        LineRenderer rightTop = RightTop.GetComponent<LineRenderer>();   
        rightTop.SetPositions(new Vector3[] {v3FrontTopRight, v3BackTopRight});
        rightTop.startColor = color;
        rightTop.endColor = color;
        rightTop.startWidth = 0.1f;
        rightTop.endWidth = 0.1f;

        RightTop.transform.parent = this.transform;
#endregion

#region Left Top
        GameObject LeftTop = new GameObject("LeftTop");

        LeftTop.AddComponent<LineRenderer>();
        LineRenderer leftTop = LeftTop.GetComponent<LineRenderer>();   
        leftTop.SetPositions(new Vector3[] {v3FrontTopLeft, v3BackTopLeft});
        leftTop.startColor = color;
        leftTop.endColor = color;
        leftTop.startWidth = 0.1f;
        leftTop.endWidth = 0.1f;

        LeftTop.transform.parent = this.transform;
#endregion

#region Right Bottom
        GameObject RightBottom = new GameObject("RightBottom");

        RightBottom.AddComponent<LineRenderer>();
        LineRenderer rightBottom = RightBottom.GetComponent<LineRenderer>();   
        rightBottom.SetPositions(new Vector3[] {v3FrontBottomRight, v3BackBottomRight});
        rightBottom.startColor = color;
        rightBottom.endColor = color;
        rightBottom.startWidth = 0.1f;
        rightBottom.endWidth = 0.1f;

        RightBottom.transform.parent = this.transform;
#endregion

#region Left Bottom
        GameObject LeftBottom = new GameObject("LeftBottom");

        LeftBottom.AddComponent<LineRenderer>();
        LineRenderer leftBottom = LeftBottom.GetComponent<LineRenderer>();   
        leftBottom.SetPositions(new Vector3[] {v3FrontBottomLeft, v3BackBottomLeft});
        leftBottom.startColor = color;
        leftBottom.endColor = color;
        leftBottom.startWidth = 0.1f;
        leftBottom.endWidth = 0.1f;

        LeftBottom.transform.parent = this.transform;
#endregion

#region Right Front
        GameObject RightFront = new GameObject("RightFront");

        RightFront.AddComponent<LineRenderer>();
        LineRenderer rightFront = RightFront.GetComponent<LineRenderer>();   
        rightFront.SetPositions(new Vector3[] {v3FrontTopRight, v3FrontBottomRight});
        rightFront.startColor = color;
        rightFront.endColor = color;
        rightFront.startWidth = 0.1f;
        rightFront.endWidth = 0.1f;

        RightFront.transform.parent = this.transform;
#endregion

#region Left Front
        GameObject LeftFront = new GameObject("LeftFront");

        LeftFront.AddComponent<LineRenderer>();
        LineRenderer leftFront = LeftFront.GetComponent<LineRenderer>();   
        leftFront.SetPositions(new Vector3[] {v3FrontTopLeft, v3FrontBottomLeft});
        leftFront.startColor = color;
        leftFront.endColor = color;
        leftFront.startWidth = 0.1f;
        leftFront.endWidth = 0.1f;

        LeftFront.transform.parent = this.transform;
#endregion

#region Right Back
        GameObject RightBack = new GameObject("RightBack");

        RightBack.AddComponent<LineRenderer>();
        LineRenderer rightBack = RightBack.GetComponent<LineRenderer>();   
        rightBack.SetPositions(new Vector3[] {v3BackTopRight, v3BackBottomRight});
        rightBack.startColor = color;
        rightBack.endColor = color;
        rightBack.startWidth = 0.1f;
        rightBack.endWidth = 0.1f;

        RightBack.transform.parent = this.transform;
#endregion

#region Left Back
        GameObject LeftBack = new GameObject("LeftBack");

        LeftBack.AddComponent<LineRenderer>();
        LineRenderer leftBack = LeftBack.GetComponent<LineRenderer>();   
        leftBack.SetPositions(new Vector3[] {v3BackTopLeft, v3BackBottomLeft});
        leftBack.startColor = color;
        leftBack.endColor = color;
        leftBack.startWidth = 0.1f;
        leftBack.endWidth = 0.1f;

        LeftBack.transform.parent = this.transform;
#endregion
        
        foreach(Transform child in transform)
        {
            LineRenderer lr = child.GetComponent<LineRenderer>();
            lr.material = material;
            lr.shadowCastingMode = ShadowCastingMode.Off;
            lr.receiveShadows = false;

            //Set to debug layer
            child.gameObject.layer = 8;
        }

        //if (Input.GetKey (KeyCode.S)) {
        /*Debug.DrawLine(v3FrontTopLeft, v3FrontTopRight, color);
        Debug.DrawLine(v3FrontTopRight, v3FrontBottomRight, color);
        Debug.DrawLine(v3FrontBottomRight, v3FrontBottomLeft, color);
        Debug.DrawLine(v3FrontBottomLeft, v3FrontTopLeft, color);

        Debug.DrawLine(v3BackTopLeft, v3BackTopRight, color);
        Debug.DrawLine(v3BackTopRight, v3BackBottomRight, color);
        Debug.DrawLine(v3BackBottomRight, v3BackBottomLeft, color);
        Debug.DrawLine(v3BackBottomLeft, v3BackTopLeft, color);

        Debug.DrawLine(v3FrontTopLeft, v3BackTopLeft, color);
        Debug.DrawLine(v3FrontTopRight, v3BackTopRight, color);
        Debug.DrawLine(v3FrontBottomRight, v3BackBottomRight, color);
        Debug.DrawLine(v3FrontBottomLeft, v3BackBottomLeft, color);*/
        //}
    }

}