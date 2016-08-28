using UnityEngine;
using System.Collections.Generic;

// helps unblock static goals
class ShrinkTrigger : MonoBehaviour {

    //////////////////////////////////////////////////

    class ShrinkTarget {
        public float InitRadius;
        public CircleCollider2D Collider;
        public float LifetimeInTrigger;
    }

    Dictionary<Collider2D, ShrinkTarget> targets = new Dictionary<Collider2D, ShrinkTarget>();

    [SerializeField] AnimationCurve sizeOverTime;

    //////////////////////////////////////////////////

    void OnTriggerEnter2D(Collider2D other) {
        if (targets.ContainsKey(other)) {
            // uh oh this shouldn't happen but try to recover
            targets[other].LifetimeInTrigger = 0f;
        } else {
            var circle = other as CircleCollider2D;
            if (circle == null) {
                return;
            }
            targets[other] = new ShrinkTarget {
                InitRadius = circle.radius,
                Collider = circle,
            };
        }
    }

    void Update() {
        foreach(var target in targets.Values) {
            target.LifetimeInTrigger += Time.deltaTime;
            target.Collider.radius =
                target.InitRadius * sizeOverTime.Evaluate(target.LifetimeInTrigger);
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (targets.ContainsKey(other)) {
            var target = targets[other];
            target.Collider.radius = target.InitRadius;
            targets.Remove(other);
        }
    }
}
