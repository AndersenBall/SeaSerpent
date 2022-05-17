using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioTester : MonoBehaviour
{
    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("p")) {
            audioSource.Play();

        }
    }
}
