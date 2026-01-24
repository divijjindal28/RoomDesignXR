using System;
using System.Collections.Generic;
using UnityEngine;
using Meta.XR.MRUtilityKit;

public class GetRoomAssetData : MonoBehaviour
{
    private MRUKRoom currentRoom;
    private bool roomReady = false;

    void OnEnable()
    {
        if (MRUK.Instance != null)
        {
            MRUK.Instance.SceneLoadedEvent.AddListener(OnSceneLoaded);
        }
    }

    void OnDisable()
    {
        if (MRUK.Instance != null)
        {
            MRUK.Instance.SceneLoadedEvent.RemoveListener(OnSceneLoaded);
        }
    }

    private void OnSceneLoaded()
    {
        Debug.Log("GetRoomAssetData : MRUK Room Loading");
        currentRoom = MRUK.Instance.GetCurrentRoom();
        roomReady = true;

        Debug.Log("GetRoomAssetData : MRUK Room Loaded and Ready");


        var bedAnchor = GetFirstAnchorByLabel(MRUKAnchor.SceneLabels.BED);
        Debug.Log("GetRoomAssetData : MRUK bedAnchor Loaded");
        if (bedAnchor != null)
        {
            Vector3 pos = bedAnchor.GetAnchorCenter();
            Quaternion rot = bedAnchor.transform.rotation;

            Debug.Log($"GetRoomAssetData : Bed at {pos}");
        }
    }

    /// <summary>
    /// Returns all anchors matching the given label
    /// Example: SceneLabels.BED, TABLE, COUCH
    /// </summary>
    public List<MRUKAnchor> GetAnchorsByLabel(MRUKAnchor.SceneLabels label)
    {
        if (!roomReady || currentRoom == null)
        {
            Debug.LogWarning("Room not ready yet.");
            return null;
        }

        List<MRUKAnchor> results = new List<MRUKAnchor>();

        foreach (var anchor in currentRoom.Anchors)
        {
            if ((anchor.Label & label) != 0)
            {
                results.Add(anchor);
            }
        }

        return results;
    }

    /// <summary>
    /// Convenience method to get center positions
    /// </summary>
    public List<Vector3> GetPositionsByLabel(MRUKAnchor.SceneLabels label)
    {
        var anchors = GetAnchorsByLabel(label);
        Debug.Log("GetRoomAssetData : MRUK bedAnchor loading test : "+ anchors);
        if (anchors == null) return null;

        List<Vector3> positions = new List<Vector3>();
        foreach (var a in anchors)
        {
            positions.Add(a.GetAnchorCenter());
        }

        return positions;
    }

    /// <summary>
    /// Returns first anchor found (most common use)
    /// </summary>
    public MRUKAnchor GetFirstAnchorByLabel(MRUKAnchor.SceneLabels label)
    {
        Debug.Log("GetRoomAssetData : MRUK bedAnchor loading started");
        var anchors = GetAnchorsByLabel(label);
        Debug.Log("GetRoomAssetData : MRUK label Loaded");
        if (anchors != null && anchors.Count > 0)
            return anchors[0];

        return null;
    }
}
