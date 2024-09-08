using DependencyParser.Universal;

namespace UniversalDependencyParser.Parser.TransitionBasedParser
{
    public class Decision : Candidate
    {
        private double point;

        public Decision(Command command, UniversalDependencyType relation, double point) : base(command, relation){
            this.point = point;
        }

        public double GetPoint() {
            return point;
        }
    }
}