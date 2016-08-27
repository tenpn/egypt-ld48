using UnityEngine;

class Player : MonoBehaviour {
    //////////////////////////////////////////////////

    [SerializeField] Ball ballPrefab;
    [SerializeField] Transform emittPoint;
    [SerializeField] float fireForce;
    [SerializeField] float rotateSpeed;

    enum PlayerIndex {
        P1,
        P2,
    }
    [SerializeField] PlayerIndex p;

    //////////////////////////////////////////////////

    void FixedUpdate() {
        var axis = p == PlayerIndex.P1 ? "P1Aim" : "P2Aim";
        var button = p == PlayerIndex.P1 ? "P1Fire" : "P2Fire";

        var aim = Input.GetAxis(axis);
        var currentAngles = transform.eulerAngles;
        currentAngles.z += aim * rotateSpeed * Time.deltaTime;
        transform.eulerAngles = currentAngles;

        if (Input.GetButtonDown(button)) {
            var newBall = Instantiate(ballPrefab);
            newBall.owner = this;
            newBall.transform.position = emittPoint.position;
            float flipper = p == PlayerIndex.P1 ? 1f : -1f;
            newBall.phys.AddForce(transform.right * fireForce * flipper);
        }
    }
}