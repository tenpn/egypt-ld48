public enum MatchModType {
    MirrorSides,
    Gravity,
}

public class MatchMod {
    public MatchModType Type;
    public float Strength;

    public override string ToString() {
        return Type.ToString();
    }
}

