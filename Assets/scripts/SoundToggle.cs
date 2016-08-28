using UnityEngine;
using UnityEngine.EventSystems;

class SoundToggle : MonoBehaviour {

    // callback for UI button
    public void ToggleSound() {
        AudioListener.volume = 1f - AudioListener.volume;
        // because tapping interferes with player control
        var events = FindObjectOfType<EventSystem>();
        events.SetSelectedGameObject(null);
    }
}
