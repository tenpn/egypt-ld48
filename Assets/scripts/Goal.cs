using UnityEngine;

class Goal : MonoBehaviour {
    //////////////////////////////////////////////////

    Match activeMatch = null;

    //////////////////////////////////////////////////

    void Awake() {
        activeMatch = FindObjectOfType<Match>();
    }

    void OnTriggerEnter2D(Collider2D other) {
        var ball = other.GetComponent<Ball>();
        if (ball != null) {
            activeMatch.Score(ball);
            Destroy(other.gameObject);
        }
    }
}
