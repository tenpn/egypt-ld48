public enum MatchModType {
    MirrorSides,
}

public class MatchMod {
    public MatchModType Type;

    public override string ToString() {
        return Type.ToString();
    }
}

