using System.Collections.Generic;
using UnityEngine;

namespace MultiUserKit
{
    //[ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter))]
    public class CableMeshGenerator : MonoBehaviour
    {
        public Transform cableStart;
            public float cableLength=1;
            float _cableLength;
        public int segmentsPerMeter=25;
        int _segmentsPerMeter;
        public GameObject cableSegmentPrefab;
        public GameObject cableEnd;
        public class CablePartInfo
        {
            public CablePartInfo(Vector3[] vertices, int[] triangles)
            {
                this.vertices = vertices;
                this.triangles = triangles;
            }

            public Vector3[] vertices;
            public int[] triangles;
        }

        public Transform testObject1;
        public Transform testObject2;
        public Transform testObject3;
        Mesh mesh;

        Vector3[] vertices;
        int[] triangles;
        public float width;
        public float length;

        public List<Transform> cablePoints=new List<Transform>();
        public int point;

        void Awake()
        {
            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
        }

        void CalculateCableSegments()
        {
            if (cableLength != _cableLength || segmentsPerMeter != _segmentsPerMeter)
            {
                float cableSegmentCount = cableLength * segmentsPerMeter;

                for(int i = 0; i < cablePoints.Count; i++)
                {
                    Destroy(cablePoints[i].gameObject);
                }
                cablePoints.Clear();

                
                for (int i = 0; i < cableSegmentCount; i++)
                {
                    GameObject go = Instantiate(cableSegmentPrefab, transform);
                    go.transform.localPosition = new Vector3(0, 0, ((i / cableSegmentCount) * cableLength));
                    cablePoints.Add(go.transform);
                    Rigidbody rb = go.GetComponent<Rigidbody>();
                    ConfigurableJoint joint = go.GetComponent<ConfigurableJoint>();
                    if (i == 0)
                    {
                        rb.isKinematic = true;
                        rb.useGravity = false;
                        //joint.connectedBody = GetComponent<Rigidbody>();
                    }
                    else
                    {
                        //rb.isKinematic = true;
                        //rb.useGravity = false;
                        rb.isKinematic = false;
                        rb.useGravity = true;
                        joint.connectedBody = cablePoints[i - 1].GetComponent<Rigidbody>();
                    }
                }
                if (cableEnd != null)
                {
                    GameObject go = Instantiate(cableEnd, transform);
                    go.transform.localPosition = new Vector3(0, 0,  cableLength);
                    cablePoints.Add(go.transform);
                    Rigidbody rb = go.GetComponent<Rigidbody>();
                    ConfigurableJoint joint = go.GetComponent<ConfigurableJoint>();
                    rb.isKinematic = true;
                    rb.useGravity = false;
                    joint.connectedBody = cablePoints[(int)cableSegmentCount - 1].GetComponent<Rigidbody>();

                }

                //Debug.Log((int)cableSegmentCount);
                _cableLength = cableLength;
                _segmentsPerMeter = segmentsPerMeter;
            }
        }

        private void Update()
        {
            CalculateCableSegments();

            List<Vector3> vertices=new List<Vector3>();
            List<int> triangles=new List<int>();
            for (int i =0; i < cablePoints.Count - 1; i++)
            {
                Quaternion startRot = Quaternion.Euler(0, 0, 0);
                Quaternion endRot = Quaternion.Euler(0, 0, 0);
                if (i > 0)
                {
                    Vector3 dir1 = (cablePoints[i].position - cablePoints[i - 1].position).normalized;
                    Vector3 dir2 = (cablePoints[i].position - cablePoints[i + 1].position).normalized;
                    //cablePoints[i].rotation = Quaternion.FromToRotation(Vector3.forward, dir1 - dir2);
                    startRot = Quaternion.FromToRotation(Vector3.forward, dir1 - dir2);
                }
                    if (i < cablePoints.Count - 2) {
                        Vector3 dir3 = (cablePoints[i+1].position - cablePoints[i].position).normalized;
                        Vector3 dir4 = (cablePoints[i+1].position - cablePoints[i + 2].position).normalized;
                        endRot = Quaternion.FromToRotation(Vector3.forward, dir3 - dir4);
                    }
                    //cablePoints[i].rotation = Quaternion.FromToRotation(cablePoints[i - 1].up, cablePoints[i - 1].forward); ;
                    //cablePoints[i].rotation = Quaternion.LookRotation(cablePoints[i - 1].forward - cablePoints[i].forward, cablePoints[i - 1].up);
                
                //Vector3 currentOffset = cablePoints[i - 1].InverseTransformPoint(cablePoints[i].transform.position);
                //Vector3 desiredOffset = cablePoints[i - 1].InverseTransformPoint(box.transform.position);
                //arm.transform.localRotation *= Quaternion.FromToRotation(currentOffset, desiredOffset); 

                //cablePoints[i].rotation = Quaternion.LookRotation(cablePoints[i].position - cablePoints[i - 1].position + cablePoints[i].position - cablePoints[i + 1].position, Vector3.up);
                CablePartInfo info = UpdateRay(cablePoints[i], cablePoints[i + 1], startRot, endRot, width, i);
                vertices.AddRange(info.vertices);
                triangles.AddRange(info.triangles);
            }
            UpdateMesh(vertices.ToArray(), triangles.ToArray());

        }
        private void LateUpdate()
        {
            for (int i = 0; i < cablePoints.Count - 1; i++)
            {
           
                if (i > 0)
                {
                    Vector3 dir1 = (cablePoints[i].position - cablePoints[i - 1].position).normalized;
                    Vector3 dir2 = (cablePoints[i].position - cablePoints[i + 1].position).normalized;
                    cablePoints[i].rotation = Quaternion.FromToRotation(Vector3.forward, dir1 - dir2);
                    //startRot = Quaternion.FromToRotation(Vector3.forward, dir1 - dir2);

                    //cablePoints[i].rotation = Quaternion.FromToRotation(cablePoints[i - 1].up, cablePoints[i - 1].forward); ;
                    //cablePoints[i].rotation = Quaternion.LookRotation(cablePoints[i - 1].forward - cablePoints[i].forward, cablePoints[i - 1].up);
                }
            }
        }
        void Test()
        {
            //testObject1.rotation =

            //Vector3 forwardDirection = forwardPosition - origin;
            //Vector3 rightDirection = rightPosition - origin;
            //Vector3 upDirection = Vector3.Cross(forwardDirection, rightDirection);

            //Quaternion orientation = Quaternion.LookRotation(forwardDirection, upDirection);

            //transform.rotation = orientation;

            Vector3 handDir0 = (testObject1.position - testObject2.position).normalized;
            Vector3 handDir1 = (testObject1.position - testObject3.position).normalized;
            testObject1.rotation = Quaternion.FromToRotation(Vector3.up, handDir1- handDir0);
            //testObject.position = testObject2.position + ((testObject2.up)-(testObject2.right)) *length;
        }
        //public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
        //{
        //    return Quaternion.Euler(angles) * (point - pivot) + pivot;
        //}

        //public static float AngleOffAroundAxis(Vector3 v, Vector3 forward, Vector3 axis)
        //{
        //    Vector3 right = Vector3.Cross(axis, forward).normalized;
        //    forward = Vector3.Cross(right, axis).normalized;
        //    return Mathf.Atan2(Vector3.Dot(v, right), Vector3.Dot(v, forward)) * Mathf.Rad2Deg;
        //}

        public CablePartInfo UpdateRay(Transform start, Transform end, Quaternion startRot, Quaternion endRot, float width, int index)
        {
            float w = width / 2;


            vertices = new Vector3[]
            {
            //new Vector3(start.position.x-(start.right* w).x,start.position.y+(start.up * w).y,start.position.z+(start.forward * w).z),
            //new Vector3(start.position.x+(start.right* w).x,start.position.y+(start.up * w).y,start.position.z+(start.forward * w).z),
            //new Vector3(start.position.x+(start.right* w).x,start.position.y-(start.up * w).y,start.position.z-(start.forward * w).z),
            //new Vector3(start.position.x-(start.right* w).x,start.position.y-(start.up * w).y,start.position.z-(start.forward * w).z),

            //new Vector3(end.position.x-(end.right* w).x,end.position.y+(end.up * w).y,end.position.z+(end.forward * w).z),
            //new Vector3(end.position.x+(end.right* w).x,end.position.y+(end.up * w).y,end.position.z+(end.forward * w).z),
            //new Vector3(end.position.x+(end.right* w).x,end.position.y-(end.up * w).y,end.position.z-(end.forward * w).z),
            //new Vector3(end.position.x-(end.right* w).x,end.position.y-(end.up * w).y,end.position.z-(end.forward * w).z),

            //RotatePointAroundPivot(new Vector3(start.position.x-w,start.position.y+w,start.position.z),start.position, new Vector3(Quaternion.LookRotation(start.position-end.position).x, Quaternion.LookRotation(start.position-end.position).y,Quaternion.LookRotation(start.position-end.position).z)),
            //RotatePointAroundPivot(new Vector3(start.position.x+w,start.position.y+w,start.position.z),start.position, new Vector3(Quaternion.LookRotation(start.position-end.position).x, Quaternion.LookRotation(start.position-end.position).y,Quaternion.LookRotation(start.position-end.position).z)),
            //RotatePointAroundPivot(new Vector3(start.position.x+w,start.position.y-w,start.position.z),start.position, new Vector3(Quaternion.LookRotation(start.position-end.position).x, Quaternion.LookRotation(start.position-end.position).y,Quaternion.LookRotation(start.position-end.position).z)),
            //RotatePointAroundPivot(new Vector3(start.position.x-w,start.position.y-w,start.position.z),start.position, new Vector3(Quaternion.LookRotation(start.position-end.position).x, Quaternion.LookRotation(start.position-end.position).y,Quaternion.LookRotation(start.position-end.position).z)),

            //RotatePointAroundPivot(new Vector3(end.position.x-w,end.position.y+w,end.position.z),start.position, new Vector3(Quaternion.LookRotation(start.position-end.position).x, Quaternion.LookRotation(start.position-end.position).y,Quaternion.LookRotation(start.position-end.position).z)),
            //RotatePointAroundPivot(new Vector3(end.position.x+w,end.position.y+w,end.position.z),start.position, new Vector3(Quaternion.LookRotation(start.position-end.position).x, Quaternion.LookRotation(start.position-end.position).y,Quaternion.LookRotation(start.position-end.position).z)),
            //RotatePointAroundPivot(new Vector3(end.position.x+w,end.position.y-w,end.position.z),start.position, new Vector3(Quaternion.LookRotation(start.position-end.position).x, Quaternion.LookRotation(start.position-end.position).y,Quaternion.LookRotation(start.position-end.position).z)),
            //RotatePointAroundPivot(new Vector3(end.position.x-w,end.position.y-w,end.position.z),start.position, new Vector3(Quaternion.LookRotation(start.position-end.position).x, Quaternion.LookRotation(start.position-end.position).y,Quaternion.LookRotation(start.position-end.position).z)),

            //new Vector3(end.position.x-w,start.position.y+w,start.position.z)+(start.forward-end.forward),
            //new Vector3(start.position.x+w,start.position.y+w,start.position.z)+(start.forward-end.forward),
            //new Vector3(start.position.x+w,start.position.y-w,start.position.z)+(start.forward-end.forward),
            //new Vector3(start.position.x-w,start.position.y-w,start.position.z)+(start.forward-end.forward),

            //new Vector3(end.position.x-w,end.position.y+w,end.position.z)+(start.forward-end.forward),
            //new Vector3(end.position.x+w,end.position.y+w,end.position.z)+(start.forward-end.forward),
            //new Vector3(end.position.x+w,end.position.y-w,end.position.z)+(start.forward-end.forward),
            //new Vector3(end.position.x-w,end.position.y-w,end.position.z)+(start.forward-end.forward),

                  //testObject.position = testObject2.position + ((testObject2.up)-(testObject2.right)) *length;

            start.localPosition + (startRot * Vector3.right* -w + startRot * Vector3.up * w),
            start.localPosition + (startRot * Vector3.right* w + startRot * Vector3.up * w),
            start.localPosition + (startRot * Vector3.right* w + startRot * Vector3.up * -w),
            start.localPosition + (startRot * Vector3.right* -w + startRot * Vector3.up * -w),

            end.localPosition + (endRot * Vector3.right* -w + endRot * Vector3.up * w),
            end.localPosition + (endRot * Vector3.right* w + endRot * Vector3.up * w),
            end.localPosition + (endRot * Vector3.right* w + endRot * Vector3.up * -w),
            end.localPosition + (endRot * Vector3.right* -w + endRot * Vector3.up * -w),


            //start.position - start.right* w + start.up * w,
            //start.position + start.right* w + start.up * w,
            //start.position + start.right* w - start.up * w,
            //start.position - start.right* w - start.up * w,

            //end.position - end.right* w + end.up * w,
            //end.position + end.right* w + end.up * w,
            //end.position + end.right* w - end.up * w,
            //end.position - end.right* w - end.up * w,

            };
            //new Vector3(start.position.x-w,start.position.y+w,start.position.z),
            //new Vector3(start.position.x+w,start.position.y+w,start.position.z),
            //new Vector3(start.position.x+w,start.position.y-w,start.position.z),
            //new Vector3(start.position.x-w,start.position.y-w,start.position.z),

            //new Vector3(end.position.x-w,end.position.y+w,end.position.z),
            //new Vector3(end.position.x+w,end.position.y+w,end.position.z),
            //new Vector3(end.position.x+w,end.position.y-w,end.position.z),
            //new Vector3(end.position.x-w,end.position.y-w,end.position.z),

            Debug.DrawLine(start.position, start.position + startRot * Vector3.up * w * 4, Color.green);
            Debug.DrawLine(end.position, end.position + endRot * Vector3.up * w * 5, Color.red);

            triangles = new int[]
            {
            //0,1,2,
            //0,2,3,

            //4,6,5,
            //4,7,6,

            2,1,5,
            2,5,6,

            1,0,4,
            1,4,5,

            0,3,7,
            0,7,4,

            3,2,6,
            3,6,7
            };

            for(int i = 0; i < triangles.Length; i++)
                triangles[i] += index * 8;

            return new CablePartInfo(vertices, triangles);

        }


        void UpdateMesh(Vector3[] vertices, int[] triangles)
        {
            mesh.Clear();

            mesh.vertices = vertices;
            mesh.triangles = triangles;

            mesh.RecalculateNormals();
        }
    }
}