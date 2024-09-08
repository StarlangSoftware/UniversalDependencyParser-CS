using System;
using System.Collections.Generic;
using Classification.Model;
using DependencyParser.Universal;

namespace UniversalDependencyParser.Parser.TransitionBasedParser
{
    public class RandomOracle : Oracle
    {
        public RandomOracle(Model model, int windowSize) : base(model, windowSize)
        {
        }

        /// <summary>
        /// Makes a random decision based on a uniform distribution over possible actions.
        /// </summary>
        /// <param name="state">The current state of the parser.</param>
        /// <returns>A Decision object representing the randomly chosen action.</returns>
        public override Decision MakeDecision(State state)
        {
            var random = new Random();
            var command = random.Next(3);
            var relation = random.Next(UniversalDependencyRelation.UniversalDependencyTags.Length);
            switch (command) {
                case 0:
                    return new Decision(Command.LEFTARC, UniversalDependencyRelation.UniversalDependencyTags[relation], 0);
                case 1:
                    return new Decision(Command.RIGHTARC, UniversalDependencyRelation.UniversalDependencyTags[relation], 0);
                case 2:
                    return new Decision(Command.SHIFT, UniversalDependencyType.DEP, 0);
            }
            return null;
        }

        protected override List<Decision> ScoreDecisions(State state, TransitionSystem transitionSystem)
        {
            return null;
        }
    }
}