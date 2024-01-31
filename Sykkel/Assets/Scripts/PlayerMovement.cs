using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Transform cameraTransform;
    [SerializeField] SceneRenderer sceneRenderer;
    [SerializeField] Crank crank;

    float angularVelocity = 0;
    float rotation = 0;
    float currentSpeed = 0;

    void FixedUpdate()
    {
        float crankSpeed = crank.crankSpeed;
        crankSpeed = Mathf.Clamp(crankSpeed, -1, 1);
        currentSpeed += crankSpeed;
        currentSpeed *= .99f;

        float lean = -Input.acceleration.x;
        lean += -Input.GetAxisRaw("Horizontal"); // For testing on computer

        RotateCamera(lean);

        transform.position += transform.up * .001f * currentSpeed;
    }

    void RotateCamera(float lean) {
        angularVelocity += lean / 10;
        angularVelocity += rotation / 9000;
        angularVelocity = Mathf.Clamp(angularVelocity, -1, 1);

        rotation += angularVelocity;
        rotation = Mathf.Clamp(rotation, -90, 90);

        cameraTransform.eulerAngles = new Vector3(0, 0, rotation); 
        // Using a playdate, camera rotation is likely not this simple
        // Rotation should be relatively simple to implement if that is the case
        
        transform.Rotate(0, 0, rotation / 45);

        if (Mathf.Abs(rotation) > 89) {
            ResetGame();
        }
    }

    void ResetGame() {
        rotation = 0;
        angularVelocity = 0;
        currentSpeed = 0;

        transform.position = Vector2.zero;
        transform.rotation = Quaternion.identity;
        cameraTransform.rotation = Quaternion.identity;
    }
}
