using System.Collections.Generic;
using Classification.Model;
using DependencyParser.Universal;

namespace UniversalDependencyParser.Parser.TransitionBasedParser
{
    public abstract class Oracle
    {
        protected Model commandModel;
        protected int windowSize;

        /// <summary>
        /// Constructs an Oracle with the given model and window size.
        /// </summary>
        /// <param name="model">the model used for making predictions</param>
        /// <param name="windowSize">the size of the window used in parsing</param>
        public Oracle(Model model, int windowSize)
        {
            commandModel = model;
            this.windowSize = windowSize;
        }

        /// <summary>
        /// Abstract method to be implemented by subclasses to make a parsing decision based on the current state.
        /// </summary>
        /// <param name="state">the current parsing state</param>
        /// <returns>a {@link Decision} object representing the action to be taken</returns>
        public abstract Decision MakeDecision(State state);
        
        /// <summary>
        /// Abstract method to be implemented by subclasses to score potential decisions based on the current state and transition system.
        /// </summary>
        /// <param name="state">the current parsing state</param>
        /// <param name="transitionSystem">the transition system being used (e.g., ARC_STANDARD or ARC_EAGER)</param>
        /// <returns>a list of {@link Decision} objects, each with a score indicating its suitability</returns>
        protected abstract List<Decision> ScoreDecisions(State state, TransitionSystem transitionSystem);

        /// <summary>
        /// Finds the best valid parsing action for the ARC_EAGER transition system based on probabilities.
        /// Ensures the action is applicable given the current state.
        /// </summary>
        /// <param name="probabilities">a map of actions to their associated probabilities</param>
        /// <param name="state"></param>
        /// <returns>the best action as a string, or an empty string if no valid action is found</returns>
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

        /// <summary>
        /// Finds the best valid parsing action for the ARC_STANDARD transition system based on probabilities.
        /// Ensures the action is applicable given the current state.
        /// </summary>
        /// <param name="probabilities">a map of actions to their associated probabilities</param>
        /// <param name="state">the current parsing state</param>
        /// <returns>the best action as a string, or an empty string if no valid action is found</returns>
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

        /// <summary>
        /// Converts a string representation of the best action into a {@link Candidate} object.
        /// </summary>
        /// <param name="best">the best action represented as a string, possibly with a dependency type in parentheses</param>
        /// <returns>a {@link Candidate} object representing the action, or null if the action is unknown</returns>
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