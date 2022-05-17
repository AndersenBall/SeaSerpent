using UnityEngine;
using UnityEngine.AI;


public class NavMeshUnitInterface : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    

    private void Start()
    {
       navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
    }

    //takes in local position based on boat. sets destination path of nav mesh based on global position.
    public void SetDestination(Vector3 destination) {
        //Debug.Log("Local Passed Value" + destination);
        //Debug.Log("World Value on static ship " + transform.parent.TransformPoint(destination));
        navMeshAgent.SetDestination(transform.parent.TransformPoint(destination));
    }

    public Vector3 GetLocalPosition() {
        return transform.localPosition;
    }
    public Vector3 GetRotation()
    {
        return transform.eulerAngles;
    }

    public Vector3 GetVelocity() {
        return navMeshAgent.velocity;
    }

    public float GetMaxSpeed() {
        return navMeshAgent.speed;
    }

}