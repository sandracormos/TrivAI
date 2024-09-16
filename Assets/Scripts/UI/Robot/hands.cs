using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class hands : MonoBehaviour
{
    public Image imageToMove;
    public Image obstacle; // List of UI images to avoid collision with
    public float moveSpeed = 1f;
    public float angleDegrees = 45f;
    public float collisionThreshold = 0.1f;

    private Vector3 moveDirection;
    private bool isMoving = true;

    void Start()
    {
        // Calculate movement direction based on angle
        moveDirection = Quaternion.Euler(0, 0, angleDegrees) * Vector3.up;
    }

    void Update()
    {
        if (isMoving)
        {
            // Move the image along the calculated direction
            imageToMove.rectTransform.position += moveDirection * moveSpeed * Time.deltaTime;

            // Check for collision with obstacles

            if (AreImagesColliding(imageToMove, obstacle))
            {
                // Stop movement if collision detected
                isMoving = false;
            }
            
        }
    }

    // Function to check for collision between two UI images
    bool AreImagesColliding(Image image1, Image image2)
    {
        RectTransform rectTransform1 = image1.rectTransform;
        RectTransform rectTransform2 = image2.rectTransform;

        // Iterate over each step
        for (float i = 0; i <= 1; i += collisionThreshold)
        {
            // Calculate the position for this step
            Vector3 position1 = Vector3.Lerp(rectTransform1.position, rectTransform2.position, i);

            // Check if the position is inside both images
            if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform1, position1) &&
                RectTransformUtility.RectangleContainsScreenPoint(rectTransform2, position1))
            {
                return true; // Collision detected
            }
        }

        return false; // No collision detected
    }
}
