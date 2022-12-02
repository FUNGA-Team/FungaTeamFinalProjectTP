using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AC;

public class AntorchaController : MonoBehaviour
{
    public float topDistance = 0.4f;
    // Update is called once per frame
    void Update()
    {

        Vector3 playerPosition = AC.KickStarter.player.transform.position;

        transform.position = new Vector3(playerPosition.x, playerPosition.y + topDistance, 0f);
    }
}
