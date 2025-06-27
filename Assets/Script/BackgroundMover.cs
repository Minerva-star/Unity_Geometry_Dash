using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMover : MonoBehaviour
{
    public float moveSpeed = 2f;

    public GameObject bgObj1;
    public GameObject bgObj2;
    private GameManager gameManager; 

    private float bgWidth;
    private bool isPaused = false;

    private void Start() 
    {
        if (bgObj1 != null && bgObj1.GetComponent<SpriteRenderer>() != null)
        {
            bgWidth = bgObj1.GetComponent<SpriteRenderer>().bounds.size.x;
        }
        SetupInitialPositions();
         gameManager = FindObjectOfType<GameManager>();
         transform.position = new Vector3(0, 0, 0);
    }

    private void SetupInitialPositions()
    {
        if (bgObj1 != null && bgObj2 != null)
        {
            bgObj1.transform.position = Vector3.zero;
            bgObj2.transform.position = new Vector3(bgWidth, 0, 0);
        }


    }


    private void Update()
    {
        if (!isPaused)
        {
            MoveBackgrounds();
        }
    }

    private void MoveBackgrounds()
    {
        float playerSpeed = 0f;
        if(gameManager!=null && gameManager.player !=null)
        {
            playerSpeed = gameManager.player.RightVelocity;
        } 
        float currentMoveSpeed = playerSpeed;
        
        if (bgObj1 != null)
        {
            bgObj1.transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
        }

        if (bgObj2 != null)
        {
            bgObj2.transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
        }
 transform.Translate(Vector3.right * currentMoveSpeed * Time.deltaTime) ;



        if (bgObj1.transform.position.x <= -bgWidth)
        {
            bgObj1.transform.position = new Vector3(bgObj2.transform.position.x + bgWidth, 0, 0);
        }

        if (bgObj2.transform.position.x <= -bgWidth)
        {
            bgObj2.transform.position = new Vector3(bgObj1.transform.position.x + bgWidth, 0, 0);
        }


    }

  
    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }


    public void SetPaused(bool paused)
    {
        isPaused = paused;
    }


}
