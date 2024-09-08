using System.Collections.Generic;
using Classification.Instance;
using Classification.Model;
using DependencyParser.Universal;

namespace UniversalDependencyParser.Parser.TransitionBasedParser
{
    public class ArcStandardOracle : Oracle
    {
        public ArcStandardOracle(Model model, int windowSize) : base(model, windowSize)
        {
        }

        public override Decision MakeDecision(State state)
        {
            string best;
            var instanceGenerator = new SimpleInstanceGenerator();
            Instance instance = instanceGenerator.Generate(state, this.windowSize, "");
            best = FindBestValidStandardClassInfo(commandModel.PredictProbability(instance), state);
            var decisionCandidate = GetDecisionCandidate(best);
            if (decisionCandidate.GetCommand() == Command.SHIFT) {
                return new Decision(Command.SHIFT, UniversalDependencyType.DEP, 0.0);
            }
            return new Decision(decisionCandidate.GetCommand(), decisionCandidate.GetUniversalDependencyType(), 0.0);
        }

        protected override List<Decision> ScoreDecisions(State state, TransitionSystem transitionSystem)
        {
            return null;
        }
    }
}