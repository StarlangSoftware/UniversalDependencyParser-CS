namespace UniversalDependencyParser.TransitionBasedParser
{
    public interface ScoringOracle
    {
        double score(State state);
    }
}