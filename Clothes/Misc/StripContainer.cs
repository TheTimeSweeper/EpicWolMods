using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StripContainer", menuName = "Epic/StripContainer")]
public class StripContainer : ScriptableObject
{
    [SerializeField]
    public List<Texture2D> CapeTextures;
    [SerializeField]
    public List<Texture2D> CapeUnderTextures;
    [SerializeField]
    public List<Texture2D> UnderGarmentTextures;
    [SerializeField]
    public List<Texture2D> ShinyTextures;
    [SerializeField]
    public List<Texture2D> SkinTextures;
}
