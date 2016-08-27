using UnityEngine;

class Match : MonoBehaviour {

    public void Score(Player scorer) {
        int pIndex = scorer == players[0] ? 0 : 1;
        ++scores[pIndex];
        Debug.Log("player " + scorer.Name + " scored!");
    }

    //////////////////////////////////////////////////

    int[] scores = new int[2];
    Player[] players;

    //////////////////////////////////////////////////

    void Awake() {
        players = FindObjectsOfType<Player>();
    }
}
