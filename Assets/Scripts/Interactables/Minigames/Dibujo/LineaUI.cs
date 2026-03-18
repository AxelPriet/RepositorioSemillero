using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LineaUI : Graphic
{
    [HideInInspector] public List<Vector2> puntos = new List<Vector2>();
    [HideInInspector] public float grosor = 4f;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        if (puntos == null || puntos.Count < 2) return;

        for (int i = 0; i < puntos.Count - 1; i++)
        {
            Vector2 a = puntos[i];
            Vector2 b = puntos[i + 1];

            Vector2 dir = b - a;
            if (dir.sqrMagnitude < 0.0001f) continue;
            dir.Normalize();

            Vector2 perp = new Vector2(-dir.y, dir.x) * (grosor * 0.5f);

            int idx = vh.currentVertCount;

            UIVertex vert = new UIVertex();
            vert.color = color;
            vert.uv0 = Vector2.zero;

            vert.position = new Vector3(a.x - perp.x, a.y - perp.y, 0); vh.AddVert(vert);
            vert.position = new Vector3(a.x + perp.x, a.y + perp.y, 0); vh.AddVert(vert);
            vert.position = new Vector3(b.x + perp.x, b.y + perp.y, 0); vh.AddVert(vert);
            vert.position = new Vector3(b.x - perp.x, b.y - perp.y, 0); vh.AddVert(vert);

            vh.AddTriangle(idx, idx + 1, idx + 2);
            vh.AddTriangle(idx, idx + 2, idx + 3);
        }
    }

    public void ActualizarColor(Color c)
    {
        color = c;
        SetVerticesDirty();
    }

    public void Refrescar()
    {
        SetVerticesDirty();
    }
}