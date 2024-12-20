using TMPro;
using UnityEngine;

public class GetTMProLeftVertex : MonoBehaviour
{
    public Vector3[] vertices;
    public float[] vectorX;
    private TextMeshPro textMesh;

    private void Start()
    {
	textMesh = GetComponent<TextMeshPro>();
	vertices = textMesh.mesh.vertices;
    }

    public float GetLeftmostVertex()
    {
	return textMesh.mesh.bounds.extents.x;
    }

}
