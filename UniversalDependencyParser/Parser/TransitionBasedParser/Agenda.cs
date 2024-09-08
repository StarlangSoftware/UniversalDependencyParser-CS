using System.Collections.Generic;

namespace UniversalDependencyParser.Parser.TransitionBasedParser
{
    public class Agenda
    {
        private Dictionary<State, double> agenda;
        private int beamSize;

        public Agenda(int beamSize)
        {
            agenda = new Dictionary<State, double>();
            this.beamSize = beamSize;
        }

        /// <summary>
        /// Retrieves the set of states currently in the agenda.
        /// </summary>
        /// <returns>A set of states that are currently in the agenda.</returns>
        public Dictionary<State, double>.KeyCollection GetKeySet()
        {
            return agenda.Keys;
        }

        /// <summary>
        /// Updates the agenda with a new state if it is better than the worst state
        /// currently in the agenda or if there is room in the agenda.
        /// </summary>
        /// <param name="oracle">The ScoringOracle used to score the state.</param>
        /// <param name="current">The state to be added to the agenda.</param>
        public void UpdateAgenda(ScoringOracle oracle, State current)
        {
            if (agenda.ContainsKey(current))
            {
                return;
            }

            var point = oracle.score(current);
            if (agenda.Count < beamSize)
            {
                agenda[current] = point;
            }
            else
            {
                State worst = null;
                double worstValue = int.MaxValue;
                foreach (var key in agenda.Keys) {
                    if (agenda[key] < worstValue)
                    {
                        worstValue = agenda[key];
                        worst = key;
                    }
                }
                if (point > worstValue)
                {
                    agenda.Remove(worst);
                    agenda[current] = point;
                }
            }
        }

        /// <summary>
        /// Retrieves the best state from the agenda based on the highest score.
        /// </summary>
        /// <returns>The state with the highest score in the agenda.</returns>
        public State Best()
        {
            State best = null;
            double bestValue = int.MinValue;
            foreach (var key in agenda.Keys) {
                if (agenda[key] > bestValue)
                {
                    bestValue = agenda[key];
                    best = key;
                }
            }
            return best;
        }
    }
}