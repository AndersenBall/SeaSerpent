using UnityEngine;
namespace Combat.Scripts.PlayerScripts
{
    public class PlayerBoatControls: MonoBehaviour
    {
        
        public BoatControls boatControl { get; set; }

        public bool isDriving { get; set; }= false;
        
        //boat controls variables
        float sideways = 0;
        float throtal = 0;
    
        void Start()
        {
            boatControl = gameObject.GetComponentInParent<BoatControls>();
        }

        // Update is called once per frame
        void Update()
        {
            if (isDriving) {
                sideways = (Input.GetKey(KeyCode.A) ? -1f : 0f) +
                           (Input.GetKey(KeyCode.D) ? 1f : 0f);
                boatControl.SetTurn(sideways);
                if (Input.GetKeyDown(KeyCode.W)) {
                    throtal = throtal + .1f;
                    throtal = Mathf.Clamp(throtal, -1, 1);
                    boatControl.SetForward(throtal);
                }
                else if (Input.GetKeyDown(KeyCode.S)) {
                    throtal = throtal - .1f;
                    throtal = Mathf.Clamp(throtal, -1, 1);
                    boatControl.SetForward(throtal);
                }
                        

            }
        }
        
        
        
    }
}