using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;

[AddComponentMenu("Camera-Control/Player Camera Controller")]
public class PlayerCameraController : MonoBehaviour
{

    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    public RotationAxes axes = RotationAxes.MouseXAndY;
    public float sensitivityX = 15F;
    public float sensitivityY = 15F;
    public bool freezeRotation = false;
    public Transform playerObject;
    public float headBobIntensity = 3f;
    public float returnToIdleSpeed = 3f;
    public float headBobAmount = 0.05f;
    public float minimumX = -360F;
    public float maximumX = 360F;

    public float minimumY = -60F;
    public float maximumY = 60F;

    private float rotationX = 0F;
    private float rotationY = 0F;

    private List<float> rotArrayX = new List<float>();
    private float rotAverageX = 0F;

    private List<float> rotArrayY = new List<float>();
    private float rotAverageY = 0F;
    private Vector3 originalPos;
    public float frameCounter = 20;

    private float idleCounter;
    public static bool isLocked = false;
    Quaternion originalRotation;
    Quaternion playerOriginalRotation;

    void Update()
    {
        if (isLocked || GameHandler.isPaused)
        {
            return;
        }
        if (!freezeRotation)
        {
            if (axes == RotationAxes.MouseXAndY)
            {
                rotAverageY = 0f;
                rotAverageX = 0f;

                rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                rotationX += Input.GetAxis("Mouse X") * sensitivityX;
                rotationY = ClampAngle(rotationY, minimumY, maximumY);
                rotArrayY.Add(rotationY);
                rotArrayX.Add(rotationX);

                if (rotArrayY.Count >= frameCounter)
                {
                    rotArrayY.RemoveAt(0);
                }
                if (rotArrayX.Count >= frameCounter)
                {
                    rotArrayX.RemoveAt(0);
                }

                for (int j = 0; j < rotArrayY.Count; j++)
                {
                    rotAverageY += rotArrayY[j];
                }
                for (int i = 0; i < rotArrayX.Count; i++)
                {
                    rotAverageX += rotArrayX[i];
                }

                rotAverageY /= rotArrayY.Count;
                rotAverageX /= rotArrayX.Count;

                rotAverageY = ClampAngle(rotAverageY, minimumY, maximumY);
                rotAverageX = ClampAngle(rotAverageX, minimumX, maximumX);

                Quaternion yQuaternion = Quaternion.AngleAxis(rotAverageY, Vector3.left);
                Quaternion xQuaternion = Quaternion.AngleAxis(rotAverageX, Vector3.up);

                transform.localRotation = originalRotation * yQuaternion;
                // Rotate player around X Axis instead of camera.
                playerObject.transform.localRotation = playerOriginalRotation * xQuaternion;
            }
        }
        if ((Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) && playerObject.GetComponent<PlayerMovementController>().IsGrounded())
        {
            Headbob(idleCounter, headBobAmount / 10f, headBobAmount);
            idleCounter += Time.deltaTime * playerObject.GetComponent<Rigidbody>().velocity.magnitude * headBobIntensity;
        } else
        {
            idleCounter = 0;
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPos, Time.deltaTime * headBobIntensity * returnToIdleSpeed);
            if (Mathf.Approximately(transform.localPosition.y, originalPos.y))
            {
                transform.localPosition = originalPos;
            }
        }
    }

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb)
            rb.freezeRotation = true;
        originalRotation = transform.localRotation;
        playerOriginalRotation = playerObject.transform.localRotation;
        originalPos = transform.localPosition;
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        angle = angle % 360;
        if ((angle >= -360F) && (angle <= 360F))
        {
            if (angle < -360F)
            {
                angle += 360F;
            }
            if (angle > 360F)
            {
                angle -= 360F;
            }
        }
        return Mathf.Clamp(angle, min, max);
    }

    private void Headbob(float pZ, float pXIntensity, float pYIntensity)
    {
        transform.localPosition = originalPos + new Vector3(Mathf.Cos(pZ) * pXIntensity, Mathf.Sin(pZ * 2) * pYIntensity, 0);
    }

    public static void LockCamera()
    {
        isLocked = true;
    }

    public static void UnlockCamera()
    {
        isLocked = false;
    }

    public static void ToggleLockedCamera()
    {
        isLocked = !isLocked;
    }
}