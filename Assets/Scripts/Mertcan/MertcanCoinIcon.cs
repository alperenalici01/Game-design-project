using UnityEngine;

// Aynı altın coin simgesini HUD ve zafer ekranında doku oluşturmadan çizer.
public class MertcanCoinIcon : UnityEngine.UI.MaskableGraphic
{
    protected override void OnPopulateMesh(UnityEngine.UI.VertexHelper vertices)
    {
        vertices.Clear();
        Rect bounds = rectTransform.rect;
        Vector2 center = bounds.center;
        float radius = Mathf.Min(bounds.width, bounds.height) * 0.5f;
        DrawDisk(vertices, center, radius, new Color32(170, 100, 22, 255));
        DrawDisk(vertices, center, radius * 0.80f, new Color32(255, 202, 66, 255));
        DrawDisk(vertices, center + new Vector2(-radius * 0.10f, radius * 0.10f),
            radius * 0.50f, new Color32(255, 222, 116, 255));
    }

    private static void DrawDisk(UnityEngine.UI.VertexHelper vertices, Vector2 center,
        float radius, Color32 tint)
    {
        int first = vertices.currentVertCount;
        vertices.AddVert(center, tint, Vector2.zero);
        const int sides = 10;
        for (int i = 0; i < sides; i++)
        {
            float angle = (i * 360f / sides + 18f) * Mathf.Deg2Rad;
            vertices.AddVert(center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius,
                tint, Vector2.zero);
        }
        for (int i = 0; i < sides; i++)
            vertices.AddTriangle(first, first + 1 + i, first + 1 + (i + 1) % sides);
    }
}
