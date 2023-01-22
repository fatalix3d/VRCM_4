using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RenderHeads.Media.AVProVideo;
    
public class MediaPlayerController : MonoBehaviour
{
    [SerializeField] private MediaPlayer _mp;
    [SerializeField] private RawImage _prevImage;


    private void Update()
    {
        if (Input.GetKey(KeyCode.W))
            TakeScreen();
    }

    private void TakeScreen()
    {
        var t = _mp.TextureProducer.GetTexture(0);
        _prevImage.texture = t;
    }
}
