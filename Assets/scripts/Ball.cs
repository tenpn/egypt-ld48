using UnityEngine;

class Ball : MonoBehaviour {
    public Rigidbody2D phys = null;
    public Player owner = null;

    public Color Color {
        set { GetComponent<SpriteRenderer>().color = value; }
    }
}
