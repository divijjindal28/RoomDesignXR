using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(fileName = "MRAsset", menuName = "Scriptable Objects/MRAsset")]
public class MRAsset : ScriptableObject
{
    public GameObject prefab;
    public List<Texture2D> textures;

}
