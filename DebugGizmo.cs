using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ZTool
{

    public interface IDrawGizmo
    {
        float duration { get; set; }

        void OnDrawGizmo();
    }


    public class GizmoBase : IDrawGizmo
    {
        public Color color;
        public float duration { get; set; }

        public virtual void OnDrawGizmo()
        {
            Gizmos.color = color;
        }

    }

    public class PointGizmo : GizmoBase
    {

        public PointGizmo()
        {
            startTime = Time.time;
        }


        public Vector3 point;
        public float startTime;
        public float scale;
        public string name;



        public override void OnDrawGizmo()
        {

            Gizmos.color = color;
            Gizmos.DrawLine(point + new Vector3(-1, 0, 0) * scale, point + new Vector3(1, 0, 0) * scale);
            Gizmos.DrawLine(point + new Vector3(0, -1, 0) * scale, point + new Vector3(0, 1, 0) * scale);
            Gizmos.DrawLine(point + new Vector3(0, 0, -1) * scale, point + new Vector3(0, 0, 1) * scale);

#if UNITY_EDITOR
            Handles.Label(point + new Vector3(0.01f, 0, 0), string.Format("{0} \n Time:{1:F3}", name, startTime));
#endif
        }

    }

    

   

    public class BoundGizmo : GizmoBase
    {
        public Bounds box;

        public override void OnDrawGizmo()
        {
            Gizmos.color = color;
            Gizmos.DrawWireCube(box.center, box.size);
        }

    }


    public class OBBGizmo : GizmoBase
    {
        public Vector3 center;
        public Vector3 halfSize;
        public Vector3 forward;

        public OBBGizmo(Vector3 center, Vector3 halfSize, Vector3 forward, Color color, float time)
        {
            this.center = center;
            this.halfSize = halfSize;
            this.forward = forward;
            this.color = color;
            this.duration = time;
        }

        public override void OnDrawGizmo()
        {
            var cacheMatrix = Gizmos.matrix;
            var cacheColor = Gizmos.color;


            Gizmos.color = color;
            Gizmos.matrix = Matrix4x4.TRS(center, Quaternion.LookRotation(forward, Vector3.up), Vector3.one);

            Gizmos.DrawWireCube(Vector3.zero, halfSize * 2);
            Gizmos.matrix = cacheMatrix;
            Gizmos.color = cacheColor;
        }

    }



    public class LineGizmo : GizmoBase
    {
        public Vector3 start;
        public Vector3 end;


        public override void OnDrawGizmo()
        {

            Gizmos.color = color;
            Gizmos.DrawLine(start, end);
            Gizmos.DrawSphere(end, 0.05f);
        }
    }


    public class QuaternionGizmo : GizmoBase
    {
        public Vector3 point;
        public Vector3 from;
        public Vector3 to;
        public float scale = 1f;
        private float startTime;
        public override void OnDrawGizmo()
        {
            startTime += Time.deltaTime;

            var sdir = point + from * scale;
            var edir = point + to * scale;
            Gizmos.color = color;
            Gizmos.DrawLine(point, sdir);
            Gizmos.DrawLine(point, edir);
            Gizmos.DrawSphere(sdir, 0.05f);
            Gizmos.DrawSphere(edir, 0.05f);




            Quaternion s = Quaternion.LookRotation(from);
            Quaternion e = Quaternion.LookRotation(to);
            var t = ((int)(startTime * 100)) % 100 / 100f;
            var delta = Quaternion.Lerp(s, e, t);

            var drawDir = delta * Vector3.forward;


            var end = point + drawDir * scale;

            Gizmos.DrawLine(point, end);
            Gizmos.DrawSphere(end, 0.05f);

        }

    }


    public class DebugGizmo : MonoInstance<DebugGizmo>
    {

        // Update is called once per frame
        void Update()
        {

#if UNITY_EDITOR
            Step(Time.deltaTime);
#endif
        }



        private List<IDrawGizmo> destroyList = new List<IDrawGizmo>();
        public void Step(float deltaTime)
        {
            for (int i = boxes.Count - 1; i >= 0; i--)
            {
                var k = boxes[i];
                k.duration -= deltaTime;
                if (k.duration <= 0)
                {
                    destroyList.Add(k);
                }
            }

            foreach (var k in destroyList)
            {
                boxes.Remove(k);
            }
            destroyList.Clear();
        }


        public void Draw(IDrawGizmo s)
        {
#if UNITY_EDITOR
            boxes.Add(s);
#endif
        }

        public void Draw(Vector3 point, Color color, string name, float time, float scale)
        {
#if UNITY_EDITOR
            PointGizmo bs = new PointGizmo() { point = point, name = name, color = color, duration = time, scale = scale };
            boxes.Add(bs);
#endif
        }

        public void Draw(float x, float y, float z, string name, float time, float scale)
        {
            Draw(new Vector3(x, y, z), Color.green, name, time, scale);
        }

        public void Draw(Vector3 point, Color color, float time, float scale)
        {
            Draw(point, color, string.Empty, time, scale);
        }

        public void DrawSingle(int uid, float x, float y, float z, string name, float time, float scale)
        {
#if UNITY_EDITOR
            if (!cacheBoxes.TryGetValue(uid, out IDrawGizmo box))
            {
                PointGizmo bs = new PointGizmo() { point = new Vector3(x, y, z), name = name, color = Color.green, duration = time, scale = scale };
                boxes.Add(bs);
                cacheBoxes.Add(uid, bs);
            }
            else
            {
                boxes.Remove(box);

                PointGizmo bs = box as PointGizmo;
                bs.point = new Vector3(x, y, z);
                bs.name = name;
                bs.color = Color.green;
                bs.duration = time;
                bs.scale = scale;
                bs.startTime = Time.time;
                boxes.Add(bs);
            }
#endif
        }

       

       



        private List<IDrawGizmo> boxes = new List<IDrawGizmo>();
        private Dictionary<int, IDrawGizmo> cacheBoxes = new Dictionary<int, IDrawGizmo>();

        private void OnDrawGizmos()
        {
            foreach (var k in boxes)
            {
                k.OnDrawGizmo();
            }
        }

    }
}


