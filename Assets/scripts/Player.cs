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
    [SerializeField] SpriteRenderer statusLight;
    [SerializeField] float minAngle = -20f;
    [SerializeField] float maxAngle = 30f;
    [SerializeField] Color p1BallCol = Color.blue;
    [SerializeField] Color p2BallCol = Color.green;
    [SerializeField] ParticleSystem shootPfx;

    [SerializeField] PlayerIndex p;

    float timeToFire = 0f;

    //////////////////////////////////////////////////

    void FixedUpdate() {
        var axis = p == PlayerIndex.P1 ? "P1Aim" : "P2Aim";
        var aim = Input.GetAxis(axis);
        var currentAngles = transform.eulerAngles;

        // wrap back into range so 0 is flat
        while(currentAngles.z > 180) {
            currentAngles.z -= 360;
        }

        while(currentAngles.z < -180) {
            currentAngles.z += 360;
        }

        // players are on different sides, so different actions depending on flip

        currentAngles.z += aim * rotateSpeed * Time.deltaTime * transform.localScale.x;
        if (transform.localScale.x > 0f) {
            currentAngles.z = Mathf.Clamp(currentAngles.z, minAngle, maxAngle);
        } else {
            currentAngles.z = Mathf.Clamp(currentAngles.z, -maxAngle, -minAngle);
        }
        transform.eulerAngles = currentAngles;

        timeToFire -= Time.deltaTime;

        bool canFire = HoldFire == false && timeToFire <= 0f;

        statusLight.color = canFire ? Color.green : Color.red;

        var button = p == PlayerIndex.P1 ? "P1Fire" : "P2Fire";
        if (canFire && Input.GetButtonDown(button)) {
            var newBall = Instantiate(ballPrefab);
            newBall.owner = this;
            newBall.transform.position = emittPoint.position;
            newBall.Color = p == PlayerIndex.P1 ? p1BallCol : p2BallCol;
            float flipper = transform.localScale.x;
            newBall.phys.AddForce(transform.right * fireForce * flipper);
            timeToFire = cooldown;
            shootPfx.Play();
        }
    }
}
