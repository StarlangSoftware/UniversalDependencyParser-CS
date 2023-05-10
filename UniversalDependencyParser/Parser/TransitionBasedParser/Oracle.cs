using System.Collections.Generic;
using Classification.Model;
using DependencyParser.Universal;

namespace UniversalDependencyParser.TransitionBasedParser
{
    public abstract class Oracle
    {
        protected Model commandModel;
        protected int windowSize;

        public Oracle(Model model, int windowSize)
        {
            commandModel = model;
            this.windowSize = windowSize;
        }

        public abstract Decision MakeDecision(State state);
        protected abstract List<Decision> ScoreDecisions(State state, TransitionSystem transitionSystem);

        protected string FindBestValidEagerClassInfo(Dictionary<string, double> probabilities, State state)
        {
            var bestValue = 0.0;
            var best = "";
            foreach (var key in probabilities.Keys) {
                if (probabilities[key] > bestValue)
                {
                    if (key == "SHIFT" || key == "RIGHTARC")
                    {
                        if (state.WordListSize() > 0)
                        {
                            best = key;
                            bestValue = probabilities[key];
                        }
                    }
                    else if (state.StackSize() > 1)
                    {
                        if (!(key == "REDUCE" && state.GetPeek().GetRelation() == null))
                        {
                            best = key;
                            bestValue = probabilities[key];
                        }
                    }
                }
            }
            return best;
        }

        protected string FindBestValidStandardClassInfo(Dictionary<string, double> probabilities, State state)
        {
            var bestValue = 0.0;
            var best = "";
            foreach (var key in probabilities.Keys) {
                if (probabilities[key] > bestValue)
                {
                    if (key == "SHIFT")
                    {
                        if (state.WordListSize() > 0)
                        {
                            best = key;
                            bestValue = probabilities[key];
                        }
                    }
                    else if (state.StackSize() > 1)
                    {
                        best = key;
                        bestValue = probabilities[key];
                    }
                }
            }
            return best;
        }

        protected Candidate GetDecisionCandidate(string best)
        {
            string command, relation;
            UniversalDependencyType type;
            if (best.Contains("("))
            {
                command = best.Substring(0, best.IndexOf('('));
                relation = best.Substring(best.IndexOf('(') + 1, best.IndexOf(')') - best.IndexOf('(') - 1);
                type = UniversalDependencyRelation.GetDependencyTag(relation);
            }
            else
            {
                command = best;
                type = UniversalDependencyType.DEP;
            }

            switch (command)
            {
                case "SHIFT":
                    return new Candidate(Command.SHIFT, type);
                case "REDUCE":
                    return new Candidate(Command.REDUCE, type);
                case "LEFTARC":
                    return new Candidate(Command.LEFTARC, type);
                case "RIGHTARC":
                    return new Candidate(Command.RIGHTARC, type);
            }

            return null;
        }
    }
}