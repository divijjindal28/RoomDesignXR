using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("MRAssets")]
    private RaycastHit hit;
    private bool readyTochangeMaterial = false;
    [SerializeField] MRAsset[] mRAssets;
    private GameObject[] assetsGameObjects;

    [Header("Menu")]
    [SerializeField] GameObject MenuParent;
    [SerializeField] GameObject MenuOption;


    //PRIVATE VARIABLES
    private GameObject target;
    private GameObject mrAssetParent;
    private GameObject[] children;
    private void Start()
    {
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.enabled = isLineEnabled;
        }
        GetMEAssetsList();

    }

    private void GetMEAssetsList() {
        assetsGameObjects = new GameObject[mRAssets.Length];

        for (int i = 0; i < mRAssets.Length; i++)
        {
            assetsGameObjects[i] = mRAssets[i].prefab;
        }
    }

    private int FindMRAssetScriptableObjectindex(GameObject neededGameObject) {
        Debug.Log("CHECKINGFUNCTIONALITY : finding scriptableobject index");
        //int currentAssetIndex = System.Array.IndexOf(assetsGameObjects, neededGameObject);
        for (int i = 0; i < assetsGameObjects.Length; i++)
        {
            Debug.Log("CHECKINGFUNCTIONALITY : asset name : "+ assetsGameObjects[i].name);
        }
        Debug.Log("CHECKINGFUNCTIONALITY : needed gameobject name : "+ neededGameObject.name);
        return System.Array.FindIndex(
        assetsGameObjects,
        go => go != null && neededGameObject.name.Contains(go.name)
    );
    }


    private void CreateMenu(GameObject neededGameObject) {
        Debug.Log("CHECKINGFUNCTIONALITY : creating menu start");
        int currentScriptableObjectIndex = FindMRAssetScriptableObjectindex(neededGameObject);
        Debug.Log("CHECKINGFUNCTIONALITY : found scriptable object index : "+ currentScriptableObjectIndex);
        if (currentScriptableObjectIndex == -1)
            return;

        List<Texture2D> textureList = mRAssets[currentScriptableObjectIndex].textures;
        Debug.Log("CHECKINGFUNCTIONALITY : got textures");
        foreach (Transform child in MenuParent.transform)
            Destroy(child.gameObject);
        for (int i = 0; i < textureList.Count; i++)
        {
            Debug.Log("CHECKINGFUNCTIONALITY : creating menu items");
            GameObject menuOptionInstance = Instantiate(MenuOption, MenuParent.transform);
            // Set texture to the menu option instance
            Debug.Log("CHECKINGFUNCTIONALITY : creating menu items images");
            menuOptionInstance.transform.GetChild(0).GetComponent<RawImage>().texture = textureList[i];
            Debug.Log("CHECKINGFUNCTIONALITY : created menu");
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


        SetMRAssetToUpdateState();
    }

    public void SetMRAssetToUpdateState() {
        Debug.Log("CHECKINGFUNCTIONALITY : starting SetMRAssetToUpdateState");
        target = hit.collider.gameObject;
        if (target == null)
            return;
        Debug.Log("CHECKINGFUNCTIONALITY : target is not null");
        mrAssetParent = target.transform.parent.gameObject;
        Debug.Log("CHECKINGFUNCTIONALITY : parent found");
        int childCount = mrAssetParent.transform.childCount;
        Debug.Log("CHECKINGFUNCTIONALITY : child Count : "+ childCount);
        children = new GameObject[childCount];

        for (int i = 0; i < childCount; i++)
        {
            children[i] = mrAssetParent.transform.GetChild(i).gameObject;
            Debug.Log("CHECKINGFUNCTIONALITY : children name : "+ children[i].name);
        }
        Debug.Log("CHECKINGFUNCTIONALITY : creating menu");
        CreateMenu(mrAssetParent);

    }

    public void ChangeAsset(int direction)
    {
        if(!readyTochangeMaterial)
            return;

        Debug.Log("CHECKINGFUNCTIONALITY : ready to swipe");
        for (int i = 0; i < children.Length; i++)
        {
            Debug.Log("CHECKINGFUNCTIONALITY : finding new target");
            if (children[i].activeSelf)
            {
                target = children[i];
            }
        }
        Debug.Log("CHECKINGFUNCTIONALITY : target found");
        int currentAssetIndex = System.Array.IndexOf(children, target);
        Debug.Log("CHECKINGFUNCTIONALITY : index found");
        if (currentAssetIndex == -1)
            return;
        currentAssetIndex += direction;
        if (currentAssetIndex < 0)
            currentAssetIndex = children.Length - 1;
        else if (currentAssetIndex >= children.Length)
            currentAssetIndex = 0;
        Debug.Log("CHECKINGFUNCTIONALITY : final index found menu");
        for (int i = 0; i < MenuParent.transform.childCount; i++)
        {

            MenuParent.transform.GetChild(i).transform.GetChild(1).gameObject.SetActive(false);
        }
        Debug.Log("CHECKINGFUNCTIONALITY : menu updated");
        MenuParent.transform.GetChild(currentAssetIndex).transform.GetChild(1).gameObject.SetActive(true);
        target.SetActive(false);
        mrAssetParent.transform.GetChild(currentAssetIndex).gameObject.SetActive(true);

        Debug.Log("CHECKINGFUNCTIONALITY : done");

    }




    //public void ChangeAsset(int direction)
    //{
    //    if (!readyTochangeMaterial)
    //        return;

    //    GameObject target = hit.collider.gameObject;
    //    if (target == null)
    //        return;



    //    GameObject mrAssetParent = target.transform.parent.gameObject;

    //    int childCount = mrAssetParent.transform.childCount;
    //    GameObject[] children = new GameObject[childCount];
    //    Debug.Log("ChangeMaterial child Count       : " + childCount);

    //    for (int i = 0; i < childCount; i++)
    //    {
    //        children[i] = mrAssetParent.transform.GetChild(i).gameObject;
    //        if (children[i].activeSelf)
    //        {
    //            target = children[i];
    //        }
    //        //mrAssetParent.transform.GetChild(i).gameObject.SetActive(false);
    //    }

    //    int currentAssetIndex = System.Array.IndexOf(children, target);
    //    Debug.Log("ChangeMaterial Target Name       : " + target.name);
    //    if (currentAssetIndex == -1)
    //        return;
    //    Debug.Log("ChangeMaterial Target Index      : " + currentAssetIndex);
    //    Debug.Log("ChangeMaterial direction         : " + direction);
    //    currentAssetIndex += direction;
    //    Debug.Log("ChangeMaterial calculated index  : " + currentAssetIndex);
    //    if (currentAssetIndex < 0)
    //        currentAssetIndex = children.Length - 1;
    //    else if (currentAssetIndex >= children.Length)
    //        currentAssetIndex = 0;
    //    target.SetActive(false);
    //    Debug.Log("ChangeMaterial final index :     " + currentAssetIndex);
    //    mrAssetParent.transform.GetChild(currentAssetIndex).gameObject.SetActive(true);



    //    else if (target.tag.Contains("MRPlane"))
    //    {
    //        if (target == null || newMaterials == null)
    //            return;

    //        Renderer rend = target.GetComponent<Renderer>();
    //        if (rend == null)
    //            return;
    //        int currentMaterialIndex = System.Array.IndexOf(newMaterials, rend.sharedMaterial);

    //        if (currentMaterialIndex == -1)
    //            return;

    //        currentMaterialIndex += direction;

    //        if (currentMaterialIndex < 0)
    //            currentMaterialIndex = newMaterials.Length - 1;
    //        else if (currentMaterialIndex >= newMaterials.Length)
    //            currentMaterialIndex = 0;

    //        rend.material = newMaterials[currentMaterialIndex];
    //    }

    //}
}
