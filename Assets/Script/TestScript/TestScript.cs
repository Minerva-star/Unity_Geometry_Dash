using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestScript : MonoBehaviour
{
    public Text showText;
    void Start()
    {

        //progressText.text = $"����: {progress:F1}%";
        float progress = 98.763f;
        showText.text = $"����: {progress:F1}%";
    }

    // Update is called once per frame
    void Update()
    {

    }
}
