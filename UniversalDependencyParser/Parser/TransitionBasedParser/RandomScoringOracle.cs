using System;

namespace UniversalDependencyParser.Parser.TransitionBasedParser
{
    public class RandomScoringOracle : ScoringOracle
    {
        public double score(State state)
        {
            Random random = new Random();
            return random.NextDouble();
        }
    }
}