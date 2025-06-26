using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FailBehavior : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerBehavior>() is PlayerBehavior player)
        {
            //player.Retry();
        }
    }
}
