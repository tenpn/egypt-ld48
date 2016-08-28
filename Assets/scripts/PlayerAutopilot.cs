using UnityEngine;

// drives the yoke with random rantings
[RequireComponent(typeof(Player))]
class PlayerAutopilot : MonoBehaviour {

    //////////////////////////////////////////////////

    Player p;

    [SerializeField] float fireInterval = 2f;
    [SerializeField] float twistInterval = 2f;

    float fireCountdown = 0f;
    float twistCountdown = 0f;
    float lastAim = 0f;
    PlayerInput input = null;

    //////////////////////////////////////////////////

    void Awake() {
        p = GetComponent<Player>();
        input = GetComponent<PlayerInput>();
        input.enabled = true;
    }

    void Update() {

        if (Input.GetKeyDown(KeyCode.T)) {
            input.enabled = !input.enabled;
        }

        if (input.enabled) {
            // player is controlling!
            return;
        }

        fireCountdown -= Time.deltaTime;
        twistCountdown -= Time.deltaTime;

        if (twistCountdown < 0f) {
            float dice = Random.value;
            lastAim = dice < 0.4f ? -1f
                : dice > 0.6f ? 1f
                : 0f;
            twistCountdown = Random.value * twistInterval;
        }

        var newYoke = new PlayerYoke {
            Aim = lastAim,
            Fire = fireCountdown <= 0f,
        };
        p.ApplyYoke(newYoke);

        if (newYoke.Fire) {
            fireCountdown = Random.value * fireInterval;
        }
    }
}
