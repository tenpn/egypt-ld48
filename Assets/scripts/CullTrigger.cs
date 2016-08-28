using UnityEngine;

// anyone entering this box is DEAD
class CullTrigger : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D other) {
        var ball = other.GetComponent<Ball>();
        if (ball != null) {
            Destroy(other.gameObject);
        }
    }
}
