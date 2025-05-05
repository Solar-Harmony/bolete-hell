using System.Collections.Generic;
using UnityEngine;

namespace Shields
{
    //Déssine le preview de la ligne que le joueur déssine
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class ShieldPreviewDrawer : MonoBehaviour
    {
        //Dessin de la ligne initial
        [SerializeField] private float lineWidth;

        [Tooltip("Détermine la distance qu'il faut déplacer la souris pour qu'un autre point soit ajouté")]
        [SerializeField]
        private float spaceBetweenPoints = 0.3f;

        [SerializeField] private GameObject linePrefab;

        //Params pour la simplification des points
        [Tooltip("plus le nombre est petit plus on garde de points après la simplication")] [SerializeField]
        private float tolerance = 0.1f;


        [field: SerializeField] public float materialRefractiveIndice { get; private set; } = 10f;

        private readonly List<int> currentLineTriangles = new();

        private readonly List<Vector3> currentLineVertices = new();
        private Mesh mesh;


        private readonly List<Vector3> points = new();

        private int testInt;

        private Vector3 xOffset;

        private void Awake()
        {
            mesh = new Mesh();

            GetComponent<MeshFilter>().mesh = mesh;
        }

        public void DrawPreview(Vector3 mouseWorld)
        {
            if (points.Count > 0) SetOffset(mouseWorld);

            if (points.Count == 0 || Vector3.Distance(points[^1], mouseWorld) > spaceBetweenPoints)
            {
                AddPointToMesh(mouseWorld);
                UpdateMesh();
            }
        }

        public void FinishLine(LineSO lineInfo)
        {
            var lineGameObject = Instantiate(linePrefab);

            lineGameObject.name = $"test{testInt++}";
            var line = lineGameObject.GetComponent<Line>();

            line.SetLineInfo(lineInfo);
            lineGameObject.GetComponent<SplineCreator>()
                .CreateSpline(LineSimplifier.Simplify(points, tolerance), lineWidth);

            Destroy(gameObject);
        }

        //Si jamais on le change pour toujour exister dans la scene plutot que de créé un nouveau drawer chaque fois qu'on créé un preview
        private void ResetPreviewLine()
        {
            points.Clear();
            currentLineTriangles.Clear();
            mesh.SetTriangles(currentLineTriangles, 0);
            currentLineVertices.Clear();
            mesh.SetVertices(currentLineVertices);
            mesh.RecalculateNormals();
        }

        private void UpdateMesh()
        {
            mesh.SetVertices(currentLineVertices);
            mesh.SetTriangles(currentLineTriangles, 0);
            mesh.RecalculateNormals();
        }

        private void SetOffset(Vector3 mouseWorld)
        {
            var nextDir = (mouseWorld - points[^1]).normalized;
            var normal = new Vector3(-nextDir.y, nextDir.x, 0f);
            xOffset = normal * (lineWidth / 2);
        }

        private void AddPointToMesh(Vector3 pointPos)
        {
            points.Add(pointPos);
            currentLineVertices.Add(pointPos - xOffset);
            currentLineVertices.Add(pointPos + xOffset);
            AddTriangles();
        }

        private void AddTriangles()
        {
            if (points.Count <= 1) return;
            var prevIndex = (points.Count - 2) * 2;
            var currIndex = (points.Count - 1) * 2;

            AddFaceTriangle(prevIndex, prevIndex + 1, currIndex, currIndex + 1);
        }

        private void AddFaceTriangle(int blCorner, int brCorner, int tlCorner, int trCorner)
        {
            currentLineTriangles.AddRange(new[]
            {
                blCorner, brCorner, trCorner,
                blCorner, trCorner, tlCorner
            });
        }
    }

    public static class LineSimplifier
    {
        //Utilise l'algorithme de Ramer–Douglas–Peucker

        public static List<Vector3> Simplify(List<Vector3> points, float tolerance)
        {
            if (points == null || points.Count < 3)
                return new List<Vector3>(points);

            var result = new List<Vector3>();
            SimplifyRecursive(points, 0, points.Count - 1, tolerance * tolerance, result);
            result.Insert(0, points[0]);
            result.Add(points[^1]);
            return result;
        }

        private static void SimplifyRecursive(List<Vector3> points, int startIndex, int endIndex, float sqTolerance,
            List<Vector3> result)
        {
            var maxSqDistance = 0f;
            var index = -1;

            var startPoint = points[startIndex];
            var endPoint = points[endIndex];

            for (var i = startIndex + 1; i < endIndex; i++)
            {
                //Cherche le point le plus éloigné du segment courrant
                var sqDistance = SquareDistanceToSegment(points[i], startPoint, endPoint);
                if (sqDistance > maxSqDistance)
                {
                    index = i;
                    maxSqDistance = sqDistance;
                }
            }

            if (maxSqDistance > sqTolerance)
                if (index != -1)
                {
                    // Simplifie a gauche puis a droite du point sélectionné
                    SimplifyRecursive(points, startIndex, index, sqTolerance, result);
                    result.Add(points[index]);
                    SimplifyRecursive(points, index, endIndex, sqTolerance, result);
                }
        }


        // Va chercher la distance perpendiculaire d'un point a un segment
        private static float SquareDistanceToSegment(Vector3 point, Vector3 start, Vector3 end)
        {
            var segment = end - start;
            var projected = point - start;

            var segmentLengthSq = segment.sqrMagnitude;
            if (segmentLengthSq == 0f)
                return projected.sqrMagnitude;

            var t = Mathf.Clamp01(Vector3.Dot(projected, segment) / segmentLengthSq);
            var projection = start + t * segment;
            return (point - projection).sqrMagnitude;
        }
    }
}