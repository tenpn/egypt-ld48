using UnityEngine;
using UnityEngine.UI;

class PlayerAmmoSlider : MonoBehaviour {

    //////////////////////////////////////////////////

    Slider slide;
    [SerializeField] Player target;
    [SerializeField] Image fill;

    //////////////////////////////////////////////////

    void Awake() {
        slide = GetComponent<Slider>();
    }

    void Update() {
        slide.value = target.Ammo / target.MaxAmmo;
        fill.color = target.Ammo >= 1f ? Color.green : Color.red;
    }
}
