using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio : MonoBehaviour
{

    public AudioSource ClickingSource;
    public AudioClip ClickingClip;
  

    // Start is called before the first frame update
    void Start()
    {
        ClickingSource.clip = ClickingClip;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ClickingSource.Play();
        }
        //Add 10 second timer alarm


    }
}
