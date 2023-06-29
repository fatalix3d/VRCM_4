using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileNameTest : MonoBehaviour
{
    [SerializeField] private string InputStr = string.Empty;
    [SerializeField] private Material _mat;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CheckFileName(InputStr);
        }
    }

    private void CheckFileName(string str)
    {
        if (str.Contains("_TB"))
        {
            Debug.Log("3d TOP DOWN");
            _mat.SetInt("Stereo", 1);
        }
        else if (str.Contains("_SBS"))
        {
            Debug.Log("3d SIDE BY SIDE");
            _mat.SetInt("Stereo", 2);
        }
        else
        {
            Debug.Log("2d DEFAULT");
            _mat.SetInt("Stereo", 0);
        }
    }
}
