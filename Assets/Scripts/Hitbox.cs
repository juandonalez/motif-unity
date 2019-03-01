using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox
{
    public Vector2 position = new Vector3(0f, 0f);
    public Vector2 origin = new Vector3(0f, 0f);
    public Vector2 max = new Vector3(0f, 0f);
    public float width = 0;
    public float height = 0;

    public bool Intersects(Hitbox h)
    {
        return (
            origin.x <= h.max.x &&
            h.origin.x <= max.x &&
            origin.y <= h.max.y &&
            h.origin.y <= max.y
        );
    }

    void Recalculate()
    {
        float halfWidth = width/2;
        float halfHeight = height/2;
        origin.x = position.x - halfWidth;
        origin.y = position.y - halfHeight;
        max.x = position.x + halfWidth;
        max.y = position.y + halfHeight;
    }

    public void Resize(float w, float h)
    {
        width += w;
        height += h;
        Recalculate();        
    }

    public void SetHeight(float h)
    {
        height = h;
        Recalculate();
    }

    public void SetPosition(float x, float y)
    {
        position.x = x;
        position.y = y;
        Recalculate();
    }

    public void SetWidth(float w)
    {
        width = w;
        Recalculate();
    }

    public void Translate(float x, float y)
    {
        position.x += x;
        position.y += y;
        Recalculate();
    }
}
