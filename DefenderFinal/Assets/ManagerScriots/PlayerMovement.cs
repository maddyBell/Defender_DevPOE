using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Transform rainOrigin;
    public Transform fogOrigin;
    public float moveSpeed = 5f;
    public float rotationSpeed = 100f; // Degrees per second

    private CharacterController controller;


    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            Debug.LogWarning("No CharacterController found on the player. Please add one.");
        }
       
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal"); // A/D
        float moveZ = Input.GetAxis("Vertical");   // W/S

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        move.y = 0; // Ensure no vertical movement

        if (controller != null)
        {
            controller.Move(move * moveSpeed * Time.deltaTime);
        }
        else
        {
            transform.Translate(move * moveSpeed * Time.deltaTime, Space.World);
        }
    }

    void HandleRotation()
    {
        float rotationInput = 0f;

        if (Input.GetKey(KeyCode.Q))
            rotationInput = -1f;
        else if (Input.GetKey(KeyCode.E))
            rotationInput = 1f;

        transform.Rotate(Vector3.up * rotationInput * rotationSpeed * Time.deltaTime);
    }
}
