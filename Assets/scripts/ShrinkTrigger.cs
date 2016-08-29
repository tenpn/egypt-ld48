/*
  Egyptian Dare, August 2016 Ludum Dare entry  
  Copyright (C) 2016 Andrew Fray

  This program is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
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
