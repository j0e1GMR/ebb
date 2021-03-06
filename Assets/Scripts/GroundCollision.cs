﻿using UnityEngine;
using System.Collections;

public class GroundCollision : MonoBehaviour
{
    public bool grounded = false;
    public bool landing = false;
    Collider2D collisionBox;
    int groundMask;

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Ground")
        {
            ////grounded = true;
            //landing = false;
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Ground")
        {
            //landing = true;
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Ground")
        {
            //grounded = false;

        }
    }

    // Use this for initialization
    void Start()
    {
        collisionBox = GetComponent<BoxCollider2D>();
        groundMask = LayerMask.GetMask("ground");
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D[] raycastHits = new RaycastHit2D[1];
        int numRaycastHits = 0;
        float rayX;
        float rayY;
        float rayXEnd;
        float rayYEnd;
        Vector2 startLine;
        Vector2 endLine;

        rayX = collisionBox.bounds.center.x;
        rayY = collisionBox.bounds.center.y - (collisionBox.bounds.extents.y/2);
        rayXEnd = collisionBox.bounds.center.x;
        rayYEnd = collisionBox.bounds.center.y - (0.55f * collisionBox.bounds.size.y);
        startLine = new Vector2(rayX, rayY);
        endLine = new Vector2(rayXEnd, rayYEnd);
        numRaycastHits = Physics2D.LinecastNonAlloc(startLine,
                                                    endLine,
                                                    raycastHits,
                                                    groundMask);
        Debug.DrawLine(startLine, endLine, Color.green);
        landing = grounded == false && numRaycastHits > 0;
        grounded = numRaycastHits > 0;
    }
}
