using System.Collections.Generic;

namespace UniversalDependencyParser.TransitionBasedParser
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

        public Dictionary<State, double>.KeyCollection GetKeySet()
        {
            return agenda.Keys;
        }

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