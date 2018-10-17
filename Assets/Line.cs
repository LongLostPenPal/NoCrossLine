using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Line : MonoBehaviour {
    LineRenderer LineRenderer;
    public List<Line> CrossLine=new List<Line>();

	// Use this for initialization
	void Awake () {
		LineRenderer = GetComponent<LineRenderer>();
        LineRenderer.startWidth = 0.07f;
	    LineRenderer.startColor = Color.green;
	    LineRenderer.endColor = Color.green;
	}
    public void SetPosition(int index,Vector3 position)
    {
        LineRenderer.SetPosition(index,position);
    }
    public void SetPositions(Vector3[] vector3s)
    {
        LineRenderer.SetPositions(vector3s);
    }
    public void SetLineMaterial(Material lineRendererMat)
    {
        LineRenderer.material = lineRendererMat;
    }
    public void SetColor(Color color)
    {
        LineRenderer.startColor = color;
        LineRenderer.endColor = color;
    }
}
