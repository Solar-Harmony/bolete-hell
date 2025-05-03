using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

//Déssine le preview de la ligne que le joueur déssine
[RequireComponent(typeof(MeshRenderer),typeof(MeshFilter))]
public class ShieldPreviewDrawer : MonoBehaviour
{
    private Mesh mesh;

    //Dessin de la ligne initial
    [SerializeField] private float lineWidth;
    [Tooltip("Détermine la distance qu'il faut déplacer la souris pour qu'un autre point soit ajouté")]
    [SerializeField] private float spaceBetweenPoints =  0.3f;

    [SerializeField] private GameObject linePrefab;
    
    
    private List<Vector3> points = new ();

    private List<Vector3> currentLineVertices = new();
    private List<int> currentLineTriangles = new();
    
    private Vector3 xOffset;
    
    //Params pour la simplification des points
    [Tooltip("plus le nombre est petit plus on garde de points après la simplication")]
    [SerializeField] private float tolerance = 0.1f;

    
    [field:SerializeField] 
    public float materialRefractiveIndice { get; private set; } = 10f;

    private int testInt = 0;

    void Awake()
    {
        mesh = new Mesh();
        
        GetComponent<MeshFilter>().mesh = mesh;
    }

    public void DrawPreview(Vector3 mouseWorld)
    {
        if (points.Count > 0)
        {
            SetOffset(mouseWorld);
        }

        if (points.Count == 0 || Vector3.Distance(points[^1], mouseWorld) > spaceBetweenPoints)
        {
            AddPointToMesh(mouseWorld);
            UpdateMesh();
        }
    }

    public void FinishLine(LineSO lineinfo)
    {
        GameObject lineGameObject = Instantiate(linePrefab);

        lineGameObject.name = $"test{testInt++}";
        Line line = lineGameObject.GetComponent<Line>();
        
        line.SetLineInfo(lineinfo);
        lineGameObject.GetComponent<SplineCreator>().CreateSpline(LineSimplifier.Simplify(points,tolerance),lineWidth);
        
        Destroy(gameObject);
        
    }

    //Si jamais on le change pour toujour exister dans la scene plutot que de créé un nouveau drawer chaque fois qu'on créé un preview
    private void ResetPreviewLine()
    {
        points.Clear();
        currentLineTriangles.Clear();
        mesh.SetTriangles( currentLineTriangles,0);
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
        Vector3 nextDir = (mouseWorld - points[^1]).normalized;
        Vector3 normal = new Vector3(-nextDir.y, nextDir.x, 0f);
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
        int prevIndex = (points.Count - 2) * 2;
        int currIndex = (points.Count - 1) * 2;
        
        AddFaceTriangle(prevIndex,prevIndex + 1, currIndex, currIndex + 1);
    }

    private void AddFaceTriangle(int blCorner, int brCorner, int tlCorner, int trCorner)
    {
        currentLineTriangles.AddRange(new [] {
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

        List<Vector3> result = new List<Vector3>();
        SimplifyRecursive(points, 0, points.Count - 1, tolerance * tolerance, result);
        result.Insert(0, points[0]); 
        result.Add(points[^1]); 
        return result;
    }

    private static void SimplifyRecursive(List<Vector3> points, int startIndex, int endIndex, float sqTolerance, List<Vector3> result)
    {
        float maxSqDistance = 0f;
        int index = -1;

        Vector3 startPoint = points[startIndex];
        Vector3 endPoint = points[endIndex];

        for (int i = startIndex + 1; i < endIndex; i++)
        {
            //Cherche le point le plus éloigné du segment courrant
            float sqDistance = SquareDistanceToSegment(points[i], startPoint, endPoint);
            if (sqDistance > maxSqDistance)
            {
                index = i;
                maxSqDistance = sqDistance;
            }
        }

        if (maxSqDistance > sqTolerance)
        {
            if (index != -1)
            {
                // Simplifie a gauche puis a droite du point sélectionné
                SimplifyRecursive(points, startIndex, index, sqTolerance, result);
                result.Add(points[index]);
                SimplifyRecursive(points, index, endIndex, sqTolerance, result);
            }
        }
    }


    // Va chercher la distance perpendiculaire d'un point a un segment
    private static float SquareDistanceToSegment(Vector3 point, Vector3 start, Vector3 end)
    {
        Vector3 segment = end - start;
        Vector3 projected = point - start;

        float segmentLengthSq = segment.sqrMagnitude;
        if (segmentLengthSq == 0f)
            return projected.sqrMagnitude;

        float t = Mathf.Clamp01(Vector3.Dot(projected, segment) / segmentLengthSq);
        Vector3 projection = start + t * segment;
        return (point - projection).sqrMagnitude;
    }
}
