public enum MatchModType {
    MirrorSides,
    Gravity,
    Ball,
}

public class MatchMod {
    public MatchModType Type;
    public float Strength;
    public BallMod Ball;

    public override string ToString() {
        return Type.ToString();
    }
}

