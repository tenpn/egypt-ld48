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
        Debug.Log("canvas size:" + canvasSize);

        removalCache.Clear();
        
        foreach(var track in trackings) {
            if (track.Value == null) {
                removalCache.Add(track.Key);
            } else {
                Vector2 trackViewportPos = cam.WorldToViewportPoint(track.Value.position);
                Debug.Log(string.Format("{0} world to {1} viewport", track.Value.position, trackViewportPos));
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
