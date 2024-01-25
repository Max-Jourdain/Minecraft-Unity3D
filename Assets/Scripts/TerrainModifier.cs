using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TerrainModifier : MonoBehaviour
{
    public LayerMask groundLayer;
    public Camera playerCamera; // Assign your player's camera here
    public float rayLength = 400; // Max distance for raycast


    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse click
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            Debug.DrawRay(ray.origin, ray.direction * rayLength, Color.red, 10f);

            if (Physics.Raycast(ray, out hit, rayLength, groundLayer))
            {
                
            }
        }
    }


}
