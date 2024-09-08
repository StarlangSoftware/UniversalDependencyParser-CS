namespace UniversalDependencyParser.Parser.TransitionBasedParser
{
    public interface ScoringOracle
    {
        double score(State state);
    }
}