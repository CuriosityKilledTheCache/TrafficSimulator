using Simulator.Splines;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

namespace Simulator.Road {

    [RequireComponent(typeof(SplineSampler), typeof(MeshFilter), typeof(MeshRenderer))]
    public class RoadMeshGenerator : MonoBehaviour {
        [SerializeField] private SplineSampler splineSampler;

        [SerializeField]
        private RoadMeshType roadMeshType;

        [SerializeField]
        [UnityEngine.Range(1, 200)]
        private float resolution;

        [SerializeField]
        [UnityEngine.Range(0f, 10f)]
        private float width;

        List<Vector3> vertsP1;
        List<Vector3> vertsP2;

        //private void Update() {
        //    SetVerts();
        //    BuildMesh();
        //}

        private void OnEnable() {
            Spline.Changed += OnSplineChanged;
        }

        private void OnDisable() {
            Spline.Changed -= OnSplineChanged;
        }

        private void Start() {
            if (splineSampler == null) {
                splineSampler = GetComponent<SplineSampler>();
            }
            SetVerts();
            BuildMesh();
        }


        private void SetVerts() {
            vertsP1 = new List<Vector3>();
            vertsP2 = new List<Vector3>();

            float step = 1f / (float)resolution;
            for (int i = 0; i < resolution + 1; i++) {
                float t = step * i;
                splineSampler.SampleSplineWidth(t, width, out Vector3 p1, out Vector3 p2);
                vertsP1.Add(p1);
                vertsP2.Add(p2);
            }

        }

        private void BuildMesh() {
            //MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
            MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
            Mesh m = new();
            //Mesh m = gameObject.AddComponent<MeshFilter>().mesh;
            List<Vector3> verts = new();
            List<int> tris = new();
            int numberOfVerts = vertsP1.Count;

            for (int i = 1; i <= numberOfVerts; i++) {
                Vector3 p1 = vertsP1[i - 1];
                Vector3 p2 = vertsP2[i - 1];
                Vector3 p3;
                Vector3 p4;

                if (i == numberOfVerts) {
                    //p3 = vertsP1[0];
                    //p4 = vertsP2[0];
                    continue;
                }
                else {
                    p3 = vertsP1[i];
                    p4 = vertsP2[i];
                }

                int offset = 4 * (i - 1);
                int t1 = offset + 0;
                int t2 = offset + 2;
                int t3 = offset + 3;

                int t4 = offset + 3;
                int t5 = offset + 1;
                int t6 = offset + 0;

                verts.AddRange(new List<Vector3>() { p1, p2, p3, p4 });
                tris.AddRange(new List<int>() { t1, t2, t3, t4, t5, t6 });

            }

            m.SetVertices(verts);
            m.SetTriangles(tris, 0);

            meshFilter.mesh = m;
        }

        private void OnSplineChanged(Spline arg1, int arg2, SplineModification arg3) {
            Rebuild();
        }

        public void Rebuild() {
            SetVerts();
            BuildMesh();
        }



        #region debug
#if UNITY_EDITOR
        private void OnDrawGizmosSelected() {
            if (!Application.isPlaying)
                return;

            for (int i = 0; i < vertsP1.Count; i++) {
                Handles.SphereHandleCap(0, transform.TransformPoint(vertsP1[i]), Quaternion.identity, 0.5f, EventType.Repaint);
                Handles.SphereHandleCap(0, transform.TransformPoint(vertsP2[i]), Quaternion.identity, 0.5f, EventType.Repaint);
                Handles.DrawLine(transform.TransformPoint(vertsP1[i]), transform.TransformPoint(vertsP2[i]));
            }

        }
#endif
        #endregion
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(RoadMeshGenerator))]
    public class RoadMeshGeneratorEditor : Editor {
        public override void OnInspectorGUI() {
            //base.OnInspectorGUI();

            DrawDefaultInspector();

            RoadMeshGenerator roadMeshGenerator = (RoadMeshGenerator)target;

            if (GUILayout.Button("Regenerate road mesh")) {
                roadMeshGenerator.Rebuild();
            }

        }
    }

#endif

}