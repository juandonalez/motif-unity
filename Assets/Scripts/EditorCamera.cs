using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorCamera : MonoBehaviour
{
    public Hitbox hitbox;
    Transform t;
    public float scrollSpeed = 0;
    float moveSpeed = 5.0f;

    void Awake()
    {
        t = transform;
        hitbox = new Hitbox();
        hitbox.SetPosition(t.position.x, t.position.y);
        hitbox.SetHeight(Camera.main.orthographicSize*2);
        hitbox.SetWidth(hitbox.height*Camera.main.aspect);
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.L))
        {
            t.position += new Vector3(moveSpeed*Time.deltaTime, 0, 0);
        }
        if(Input.GetKey(KeyCode.J))
        {
            t.position -= new Vector3(moveSpeed*Time.deltaTime, 0, 0);
        }
        if(Input.GetKey(KeyCode.K))
        {
            t.position -= new Vector3(0, moveSpeed*Time.deltaTime, 0);
        }
        if(Input.GetKey(KeyCode.I))
        {
            t.position += new Vector3(0, moveSpeed*Time.deltaTime, 0);
        }

        hitbox.SetPosition(t.position.x, t.position.y);
    }
}
