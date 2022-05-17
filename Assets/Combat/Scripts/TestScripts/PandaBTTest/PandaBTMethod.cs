using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

public class PandaBTMethod : MonoBehaviour
{ 
    // Start is called before the first frame update
    [Task]
    public bool greaterThenTen = false;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("s")) {
            greaterThenTen = greaterThenTen ? false : true;
        }
    }
    [Task]
    public void MoveRight()
    {
        gameObject.transform.position = gameObject.transform.position + new Vector3(.05f, 0, 0);
        if (gameObject.transform.position.x > 10)
            Task.current.Succeed();

    }
    [Task]
    public void MoveLeft()
    {
        gameObject.transform.position = gameObject.transform.position + new Vector3(-.05f, 0, 0);
        if (gameObject.transform.position.x < -10)
            Task.current.Fail();
    }
    [Task]
    public void MoveBack()
    {
        gameObject.transform.position = gameObject.transform.position + new Vector3(0, .05f, 0);
        if (gameObject.transform.position.y > 10)
            Task.current.Succeed();
    }
}