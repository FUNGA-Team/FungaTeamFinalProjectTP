using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class OnStartGame : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    // Start is called before the first frame update
    void Start()
    {
        videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, "Los Senderos de Fugum - Titulos Iniciales.mp4");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
