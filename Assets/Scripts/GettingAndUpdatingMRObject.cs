using UnityEngine;
using TMPro;

public class GettingAndUpdatingMRObject : MonoBehaviour
{
    [Header("Ray Settings")]
    public Transform rayOrigin;
    public float rayDistance = 100f;
    public LayerMask rayLayerMask;

    [Header("Line Renderer")]
    public LineRenderer lineRenderer;

    [Header("UI")]
    public TextMeshProUGUI objectNameText;

    private bool isLineEnabled = true;

    [Header("Materials")]
    [SerializeField]
    private Material[] newMaterials;


    private RaycastHit hit;
    private bool readyTochangeMaterial = false;
    private void Start()
    {
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.enabled = isLineEnabled;
        }
    }

    private void Update()
    {
        if (!isLineEnabled)
            return;

        UpdateRayAndLine();
    }

    /// <summary>
    /// Main logic for raycast + line renderer update
    /// </summary>
    private void UpdateRayAndLine()
    {
        if (rayOrigin == null || lineRenderer == null)
            return;

        Vector3 startPoint = rayOrigin.position;
        Vector3 direction = rayOrigin.forward;

        

        bool isHit = Physics.Raycast(
            startPoint,
            direction,
            out hit,
            rayDistance
        );

        lineRenderer.SetPosition(0, startPoint);
        
        if (isHit)
        {
            lineRenderer.SetPosition(1, hit.point);
            
            if (objectNameText != null)
                objectNameText.text = "HIT";
        }
        else
        {
            lineRenderer.SetPosition(1, startPoint + direction * rayDistance);

            if (objectNameText != null)
                objectNameText.text = "";
        }
    }

    /// <summary>
    /// Call this function to enable/disable line renderer
    /// </summary>
    public void ToggleLineRenderer(bool state)
    {
        isLineEnabled = state;

        if (lineRenderer != null)
            lineRenderer.enabled = state;

        if (!state && objectNameText != null)
            objectNameText.text = "";

        Debug.Log("Line Renderer is " + (state ? "Enabled" : "Disabled"));

        if (!state)
        {
            if (hit.collider.gameObject != null)
            {
                readyTochangeMaterial = true;
            }
            else {
                readyTochangeMaterial = false;
            }

        }
        else {
            readyTochangeMaterial = false;
        }
        


    }

    public void ChangeMaterial(int direction)
    {
        if(!readyTochangeMaterial)
            return;

        GameObject target = hit.collider.gameObject;

        if (target.tag.Contains("MRAsset"))
        {
            if (target == null)
                return;

            GameObject mrAssetParent = target.transform.parent.gameObject;
            int childCount = mrAssetParent.transform.childCount;
            GameObject[] children = new GameObject[childCount];

            for (int i = 0; i < childCount; i++)
            {
                children[i] = mrAssetParent.transform.GetChild(i).gameObject;
                //mrAssetParent.transform.GetChild(i).gameObject.SetActive(false);
            }

            int currentAssetIndex = System.Array.IndexOf(children, target);

            if (currentAssetIndex == -1)
                return;

            currentAssetIndex += direction;

            if (currentAssetIndex < 0)
                currentAssetIndex = children.Length - 1;
            else if (currentAssetIndex >= children.Length)
                currentAssetIndex = 0;
            target.SetActive(false);
            mrAssetParent.transform.GetChild(currentAssetIndex).gameObject.SetActive(true);


        }
        else if (target.tag.Contains("MRPlane"))
        {
            if (target == null || newMaterials == null)
                return;

            Renderer rend = target.GetComponent<Renderer>();
            if (rend == null)
                return;
            int currentMaterialIndex = System.Array.IndexOf(newMaterials, rend.sharedMaterial);

            if (currentMaterialIndex == -1)
                return;

            currentMaterialIndex += direction;

            if (currentMaterialIndex < 0)
                currentMaterialIndex = newMaterials.Length - 1;
            else if (currentMaterialIndex >= newMaterials.Length)
                currentMaterialIndex = 0;

            rend.material = newMaterials[currentMaterialIndex];
        }
        
    }
}
