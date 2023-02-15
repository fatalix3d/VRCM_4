using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundHelper : MonoBehaviour
{
    public static BackgroundHelper Instance { get; private set; }
    [SerializeField] private Material _backgroundMaterial;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance == this)
        {
            Destroy(gameObject);
        }   
    }

    public void SetTexture(Texture2D tex)
    {
        if(tex!=null)
        _backgroundMaterial.mainTexture = tex;
    }
   
}
