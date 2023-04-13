using UnityEngine;

public class InteractiveObjectViewer : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;

    [SerializeField]
    private Transform container;

    [SerializeField]
    private float interactiveDistanse = 2f;

    //Stored required properties.
    private InteractiveObject interactive;
    private GrabbableObject grabbable;
    private GrabbableObject currentGrabbableObject;

    private void Update()
    {
        if(currentGrabbableObject != null && Input.GetKeyDown(KeyCode.Q))
        {
            currentGrabbableObject.transform.SetParent(null);
            currentGrabbableObject.Drop();
            currentGrabbableObject = null;
        }

        if (Input.GetKeyDown(KeyCode.E) && currentGrabbableObject != null)
        {
            InteractiveObject interactiveObject = currentGrabbableObject.GetComponent<InteractiveObject>();
            if(interactiveObject != null)
            {
                interactiveObject.Interact();
            }
        }

        if(interactive != null && Input.GetKeyDown(KeyCode.E))
        {
            interactive.Interact();
        }

        if(grabbable != null && currentGrabbableObject == null && Input.GetKeyDown(KeyCode.F))
        {
            currentGrabbableObject = grabbable;
            currentGrabbableObject.Grab();
            currentGrabbableObject.transform.SetParent(container);
            currentGrabbableObject.transform.localPosition = Vector3.zero;
        }
    }

    private void FixedUpdate()
    {
        FindInteractiveObject();
    }

    private void FindInteractiveObject()
    {
        if(Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out RaycastHit hitinfo, interactiveDistanse))
        {
            if(hitinfo.collider.TryGetComponent(typeof(InteractiveObject), out Component interactiveObj))
            {
                interactive = interactiveObj as InteractiveObject;
            }
            else
            {
                interactive = null;
            }

            if (hitinfo.collider.TryGetComponent(typeof(GrabbableObject), out Component grabbableObj))
            {
                grabbable = grabbableObj as GrabbableObject;
            }
            else
            {
                grabbable = null;
            }
        }
        else
        {
            interactive = null;
            grabbable = null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(mainCamera.transform.position, mainCamera.transform.position + (mainCamera.transform.forward * interactiveDistanse));
    }
}
