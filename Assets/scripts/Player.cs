using UnityEngine;

class Player : MonoBehaviour {
    //////////////////////////////////////////////////

    [SerializeField] Rigidbody2D ballPrefab;
    [SerializeField] Transform emittPoint;
    [SerializeField] float fireForce;

    float timeToNext = 0;

    //////////////////////////////////////////////////

    void FixedUpdate() {
        timeToNext -= Time.deltaTime;
        if (timeToNext < 0f) {

            var newBall = Instantiate(ballPrefab);
            newBall.transform.position = emittPoint.position;
            newBall.AddForce(transform.right * fireForce);
            
            timeToNext = 1.5f;
        }
    }
}
