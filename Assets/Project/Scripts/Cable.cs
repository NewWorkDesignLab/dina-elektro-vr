using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Cable : MonoBehaviour
{
    public Transform cableStart;
    public Transform cableEndPoint;
    public float cableLength = 1;
    public int segmentsPerMeter = 25;
    public GameObject cableSegmentPrefab;
    public GameObject cableEnd;
    public bool fixEnd = true;
    public class CablePartInfo
    {
        //public CablePartInfo(List<Vector3> vertices, List<int> triangles)
        //{
        //    this.vertices = vertices;
        //    this.triangles = triangles;
        //}

        public CablePartInfo()
        {
            this.vertices = new List<Vector3>();
            this.triangles = new List<int>();
        }

        public List<Vector3> vertices;
        public List<int> triangles;
    }
    Mesh mesh;

    public List<Transform> pointTransforms=new List<Transform>();
    public  List<Point> cablePoints = new List<Point>();
    public Stick[] sticks;
    public Vector3[] points1;
    public Transform target;
    public float gravity = 0.1f;
    public float stiffness = 0.5f;
    List<Vector3> verticesList=new List<Vector3>();
    List<int> trianglesList=new List<int>();
    //Vector3[] vertices;
    //int[] triangles;
    public float width = 0.1f;

    bool firstFrame=true;


    void Start()
    {
        CalculateCableSegments();
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        sticks = new Stick[pointTransforms.Count - 1];

        for (int i = 0; i < pointTransforms.Count; i++)
        {
            cablePoints.Add(new Point());
            if (i == 0 || (fixEnd && i == pointTransforms.Count - 1))
                cablePoints[i].locked = true;

            cablePoints[i].position = pointTransforms[i].position;
            if (i > 0)
            {
                sticks[i - 1] = new Stick();
                sticks[i - 1].pointA = cablePoints[i - 1];
                sticks[i - 1].pointB = cablePoints[i];
                sticks[i - 1].length = Vector3.Distance(sticks[i - 1].pointA.position, sticks[i - 1].pointB.position);

            }
        }
    }

    void FixedUpdate()
    {
        Simulate();
    }

    void CalculateCableSegments()
    {
        float cableSegmentCount = cableLength * segmentsPerMeter;
        for (int i = 0; i < cableSegmentCount; i++)
        {
            GameObject go = Instantiate(cableSegmentPrefab, transform);
            go.transform.position = new Vector3(cableStart.position.x, cableStart.position.y, cableStart.position.z+((i / cableSegmentCount) * cableLength));
            pointTransforms.Add(go.transform);
        }
        if (cableEnd != null)
        {
            GameObject go = Instantiate(cableEnd, transform);
            go.transform.position = new Vector3(cableStart.position.x, cableStart.position.y, cableStart.position.z + cableLength);
            pointTransforms.Add(go.transform);
        }
    }
    private void Update()
    {
        verticesList.Clear();
        trianglesList.Clear();
        for (int i = 0; i < pointTransforms.Count; i++)
        {
            if (!cablePoints[i].locked)
            {
                pointTransforms[i].position = cablePoints[i].position;
            }
            else
            {
                cablePoints[i].position = pointTransforms[i].position;
            }
            Quaternion startRot = Quaternion.Euler(0, 0, 0);
            Quaternion endRot = Quaternion.Euler(0, 0, 0);

            if (!firstFrame)
            {
                if (i == 0)
                {
                    startRot = cableStart.rotation;
                }
                if (i > 0 && i < cablePoints.Count - 1)
                {
                    Vector3 dir1 = (cablePoints[i].position - cablePoints[i - 1].position).normalized;
                    Vector3 dir2 = (cablePoints[i].position - cablePoints[i + 1].position).normalized;
                    startRot = Quaternion.FromToRotation(Vector3.forward, dir1 - dir2);
                }
                if (i < cablePoints.Count - 2)
                {
                    Vector3 dir3 = (cablePoints[i + 1].position - cablePoints[i].position).normalized;
                    Vector3 dir4 = (cablePoints[i + 1].position - cablePoints[i + 2].position).normalized;
                    endRot = Quaternion.FromToRotation(Vector3.forward, dir3 - dir4);
                }
                //if(i == cablePoints.Count - 2)
                //{
                //    Vector3 dir3 = (cablePoints[i].position - cablePoints[i-1].position).normalized;
                //    Vector3 dir4 = (cablePoints[i].position - cablePoints[i + 1].position).normalized;
                //    endRot = Quaternion.FromToRotation(Vector3.forward, dir3 - dir4);
                //    cableEndPoint.rotation = endRot;
                //}
                if (i == cablePoints.Count - 2)
                {
                    endRot = cableEndPoint.rotation;
                }
                if (i < cablePoints.Count - 1)
                {
                    CablePartInfo info = GetCablePartInfo(cablePoints[i].position, cablePoints[i + 1].position, startRot, endRot, width, i);
                    verticesList.AddRange(info.vertices);
                    trianglesList.AddRange(info.triangles);
                }
            }
        }
        if (!firstFrame)
            UpdateMesh(verticesList.ToArray(), trianglesList.ToArray());

        pointTransforms[pointTransforms.Count - 1].position = cableEndPoint.position;
        pointTransforms[0].position = cableStart.position;
        firstFrame = false;

    }
    void UpdateMesh(Vector3[] vertices, int[] triangles)
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }

    public CablePartInfo GetCablePartInfo(Vector3 start, Vector3 end, Quaternion startRot, Quaternion endRot, float width, int index)
    {
        float w = width / 2;
        CablePartInfo info = new CablePartInfo();

        info.vertices.AddRange(new Vector3[]
        {
            start + (startRot * Vector3.right* -w + startRot * Vector3.up * w),
            start + (startRot * Vector3.right* w + startRot * Vector3.up * w),
            start + (startRot * Vector3.right* w + startRot * Vector3.up * -w),
            start + (startRot * Vector3.right* -w + startRot * Vector3.up * -w),

            end + (endRot * Vector3.right* -w + endRot * Vector3.up * w),
            end + (endRot * Vector3.right* w + endRot * Vector3.up * w),
            end + (endRot * Vector3.right* w + endRot * Vector3.up * -w),
            end + (endRot * Vector3.right* -w + endRot * Vector3.up * -w),
        });

        Debug.DrawLine(start, start + startRot * Vector3.up * w * 4, Color.green);
        Debug.DrawLine(end, end + endRot * Vector3.up * w * 5, Color.red);

        if (index == 0)
        {
            info.triangles.AddRange(new int[]
            { 
                0,1,2,
                0,2,3, 
            });
        }
        else if(index == pointTransforms.Count - 2)
        {
            info.triangles.AddRange(new int[]
            {
                4,6,5,
                4,7,6,
            });
        }
        info.triangles.AddRange(new int[]
        {
            2,1,5,
            2,5,6,

            1,0,4,
            1,4,5,

            0,3,7,
            0,7,4,

            3,2,6,
            3,6,7
        });

        for (int i = 0; i < info.triangles.Count; i++)
            info.triangles[i] += index * 8;

        return info;

    }

    [System.Serializable]
    public class Point
    {
        public Vector3 position, prevPosition;
        public GameObject sceneObject;
        public bool locked;
        public bool raycastHit;
    }
    [System.Serializable]
    public class Stick
    {
        public Point pointA, pointB;
        public float length;
    }
    void Simulate()
    {
        int numIterations = 100;
        for (int i = 0; i < cablePoints.Count; i++)
        {
            Point p = cablePoints[i];
            //RaycastHit hit;
            //bool h = false;
            //if (Physics.SphereCast((p.position + (p.position - p.prevPosition) + (Vector3.down * gravity * Time.deltaTime * Time.deltaTime)), 0.1f, Vector3.down * 0.01f, out hit, 0.1f))
            //{
            //    if (!hit.collider.name.Contains("Sphere"))
            //        h = true;
            //    //p.locked = true;
            //    //p.raycastHit = true;
            //    //Debug.Log(hit.collider.name);
            //}
            //if (h==false &&  i != 0 && i != cablePoints.Count - 1)
            //{
            //    p.locked = false;
            //    //h = false;
            //    p.raycastHit = false;
            //}
            if (!p.locked && !p.raycastHit)
            {

                if ((p.position + (p.position - p.prevPosition) + (Vector3.down * gravity * Time.deltaTime * Time.deltaTime)).y > 0)
                {
                    Vector3 positionBeforeUpdate = p.position;

                //if (!h)
                //{

                    p.position += p.position - p.prevPosition;
                    p.position += Vector3.down * gravity * Time.deltaTime * Time.deltaTime;

                p.prevPosition = positionBeforeUpdate;
                }

            }
        }
        for (int i = 0; i < numIterations; i++)
        {
            foreach (Stick stick in sticks)
            {
                Vector3 stickCentre = (stick.pointA.position + stick.pointB.position) / 2;
                Vector3 stickDir = (stick.pointA.position - stick.pointB.position).normalized;

                if (!stick.pointA.locked && !stick.pointA.raycastHit)
                    stick.pointA.position = stickCentre + stickDir * stick.length / 2;
                if (!stick.pointB.locked && !stick.pointB.raycastHit)
                    stick.pointB.position = stickCentre - stickDir * stick.length / 2;
            }
        }
    }
}