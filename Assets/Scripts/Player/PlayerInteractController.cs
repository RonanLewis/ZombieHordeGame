using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerInteractController : MonoBehaviour
{
    public float maxInteractRange = 3.5f;
    public float holdDistance = 2f;
    public Material highlightMaterial;
    private Camera playerCam;
    private Transform heldObject;
    public RaycastHit hoHit;
    public GameObject previousHit;


    // Start is called before the first frame update
    void Start()
    {
        GetCamera();
    }

    // Update is called once per frame
    private void Update()
    {
        if (GameHandler.isPaused)
        {
            return;
        }
        LookAtObject();
        CheckInteracts();
        if (heldObject && !heldObject.GetComponent<InteractableObject>().grabbedBy)
        {
            heldObject.GetComponent<InteractableObject>().Drop();
            heldObject = null;
        }
    }

    private void GetCamera()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).CompareTag("MainCamera"))
            {
                playerCam = transform.GetChild(i).GetComponent<Camera>();
            }
        }
    }

    private void CheckInteracts()
    {
        InteractWithObject();
    }

    private void HoldItem(Transform item)
    {
        heldObject = item;
        float sizeCheck = (Vector3.Distance(hoHit.point, heldObject.GetComponent<Collider>().bounds.center) + heldObject.GetComponent<InteractableObject>().minHoldDistance);
        if (sizeCheck > maxInteractRange && !Mathf.Approximately(sizeCheck, maxInteractRange))
        {
            // Too big to pick up.
            Debug.Log(155);
            heldObject.GetComponent<InteractableObject>().Drop();
            heldObject = null;
        }
        else
        {
            heldObject.GetComponent<InteractableObject>().Hold(transform);

        }

    }

    private void InteractWithObject()
    {
        if (Input.GetButtonDown("Interact") && hoHit.transform)
        {
            if (hoHit.transform.GetComponent<Item>())
            {
                GetComponent<PlayerInventory>().Add(hoHit.transform.gameObject);
                return;
            }
            if (hoHit.transform.GetComponent<InteractableObject>() && !heldObject)
            {
                HoldItem(hoHit.transform);
                return;
            }
        }
        if (heldObject)
        {
            if (Input.GetButtonDown("Interact"))
            {
                heldObject.GetComponent<InteractableObject>().Drop();
                heldObject = null;
            }
            if (Input.GetMouseButtonDown(1)) // Throw held object;
            {
                heldObject.GetComponent<InteractableObject>().Throw();
                heldObject = null;
            }
            if (Input.GetMouseButton(2)) // Rotate held object;
            {
                playerCam.GetComponent<PlayerCameraController>().freezeRotation = true;
                heldObject.GetComponent<InteractableObject>().Rotate(new Vector3(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0));
            }
            else
            {
                playerCam.GetComponent<PlayerCameraController>().freezeRotation = false;
            }
        }
        else
        {
            playerCam.GetComponent<PlayerCameraController>().freezeRotation = false;
        }
    }
    private void LookAtObject()
    {
        int layerMask = 1 << 8;
        layerMask = ~layerMask;
        Debug.DrawRay(playerCam.transform.position, playerCam.transform.TransformDirection(Vector3.forward), Color.red);
        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.TransformDirection(Vector3.forward), out hoHit, maxInteractRange, layerMask))
        {
            if (hoHit.transform.GetComponent<Item>())
            {
                Debug.Log(1);
                if (previousHit != hoHit.collider.gameObject)//If we're not pointing at the previous target
                {
                    Debug.Log(2);
                    GameObject newTarget = hoHit.transform.gameObject;
                    Renderer newTargetRenderer = newTarget.GetComponent<Renderer>();
                    List<Material> newTargetmats = newTargetRenderer.sharedMaterials.ToList<Material>();

                    if (!newTargetmats.Contains(highlightMaterial))
                    {
                        Debug.Log(3);
                        newTargetmats.Add(highlightMaterial);
                    }
                    newTargetRenderer.sharedMaterials = newTargetmats.ToArray<Material>();
                    previousHit = newTarget;
                }
            }
            else
            {
                if (previousHit != null)
                {
                    Renderer prevTargetRenderer = previousHit.GetComponent<Renderer>();
                    List<Material> prevTargetmats = prevTargetRenderer.sharedMaterials.ToList<Material>();

                    if (prevTargetmats.Contains(highlightMaterial))
                    {
                        prevTargetmats.Remove(highlightMaterial);
                    }
                    prevTargetRenderer.sharedMaterials = prevTargetmats.ToArray<Material>();
                    previousHit = null;//Clear reference
                }
            }

        }
        else
        {
            if (previousHit != null)
            {
                Renderer prevTargetRenderer = previousHit.GetComponent<Renderer>();
                List<Material> prevTargetmats = prevTargetRenderer.sharedMaterials.ToList<Material>();

                if (prevTargetmats.Contains(highlightMaterial))
                {
                    prevTargetmats.Remove(highlightMaterial);
                }
                prevTargetRenderer.sharedMaterials = prevTargetmats.ToArray<Material>();
                previousHit = null;//Clear reference
            }
        }
    }
}
