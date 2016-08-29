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
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Assertions;

// cuz you can't just child a label to a transform and get it to render on-screen!
class FloatingLabels : MonoBehaviour {

    public void AttachLabel(string value, Transform parent) {
        var newLabel = Instantiate(childPrefab);
        newLabel.gameObject.SetActive(true);
        newLabel.text = value;
        trackings[newLabel.transform as RectTransform] = parent;
        newLabel.transform.SetParent(transform);
    }

    //////////////////////////////////////////////////

    [SerializeField] Text childPrefab;

    IList<RectTransform> removalCache = new List<RectTransform>();
    Dictionary<RectTransform,Transform> trackings = new Dictionary<RectTransform, Transform>();
    RectTransform canvasRect;

    //////////////////////////////////////////////////

    void Awake() {
        childPrefab.gameObject.SetActive(false);
        canvasRect = GetComponentInParent<Canvas>().transform as RectTransform;
    }

    void LateUpdate() {

        var cam = Camera.main;
        Assert.IsNotNull(cam);
        var canvasSize = canvasRect.sizeDelta;

        removalCache.Clear();
        
        foreach(var track in trackings) {
            if (track.Value == null) {
                removalCache.Add(track.Key);
            } else {
                Vector2 trackViewportPos = cam.WorldToViewportPoint(track.Value.position);
                var trackScreenPos = new Vector2(
                    (trackViewportPos.x*canvasSize.x)-(canvasSize.x*0.5f),
                    (trackViewportPos.y*canvasSize.y)-(canvasSize.y*0.5f));
                track.Key.anchoredPosition = trackScreenPos;
            }
        }

        foreach(var rem in removalCache) {
            trackings.Remove(rem);
            Destroy(rem.gameObject);
        }
    }
}
