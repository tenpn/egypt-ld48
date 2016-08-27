using UnityEngine;

class Player : MonoBehaviour {

    public string Name {
        get { return p.ToString(); }
    }
    
    public enum PlayerIndex {
        P1,
        P2,
    }

    public PlayerIndex Index {
        get { return p; }
    }

    // match starts with no firing
    public bool HoldFire = true;
    
    //////////////////////////////////////////////////

    [SerializeField] Ball ballPrefab;
    [SerializeField] Transform emittPoint;
    [SerializeField] float fireForce;
    [SerializeField] float rotateSpeed;
    [SerializeField] float cooldown = 1f;

    [SerializeField] PlayerIndex p;

    float timeToFire = 0f;

    //////////////////////////////////////////////////

    void FixedUpdate() {
        var axis = p == PlayerIndex.P1 ? "P1Aim" : "P2Aim";
        var button = p == PlayerIndex.P1 ? "P1Fire" : "P2Fire";

        var aim = Input.GetAxis(axis);
        var currentAngles = transform.eulerAngles;
        currentAngles.z += aim * rotateSpeed * Time.deltaTime;
        transform.eulerAngles = currentAngles;

        timeToFire -= Time.deltaTime;

        if (HoldFire == false && timeToFire <= 0f && Input.GetButtonDown(button)) {
            var newBall = Instantiate(ballPrefab);
            newBall.owner = this;
            newBall.transform.position = emittPoint.position;
            float flipper = p == PlayerIndex.P1 ? 1f : -1f;
            newBall.phys.AddForce(transform.right * fireForce * flipper);
            timeToFire = cooldown;
        }
    }
}
