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

    IList<int> removeCache = new List<int>();
    Dictionary<int, ShrinkTarget> targets = new Dictionary<int, ShrinkTarget>();

    [SerializeField] AnimationCurve sizeOverTime;

    //////////////////////////////////////////////////

    void OnTriggerEnter2D(Collider2D other) {
        int otherID = other.GetInstanceID();
        if (targets.ContainsKey(otherID)) {
            // uh oh this shouldn't happen but try to recover
            targets[otherID].LifetimeInTrigger = 0f;
        } else {
            var circle = other as CircleCollider2D;
            if (circle == null) {
                return;
            }
            targets[otherID] = new ShrinkTarget {
                InitRadius = circle.radius,
                Collider = circle,
            };
        }
    }

    void Update() {
        removeCache.Clear();
        foreach(var colliderTarget in targets) {
            var target = colliderTarget.Value;
            
            if (target.Collider == null) {
                removeCache.Add(colliderTarget.Key);
                continue;
            }

            // single balls don't shrink
            if (targets.Count > 1) {
                target.LifetimeInTrigger += Time.deltaTime;
            }
            target.Collider.radius =
                target.InitRadius * sizeOverTime.Evaluate(target.LifetimeInTrigger);
        }

        foreach(var removeID in removeCache) {
            targets.Remove(removeID);
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        int otherID = other.GetInstanceID();
        if (targets.ContainsKey(otherID)) {
            var target = targets[otherID];
            target.Collider.radius = target.InitRadius;
            targets.Remove(otherID);
        }
    }
}
