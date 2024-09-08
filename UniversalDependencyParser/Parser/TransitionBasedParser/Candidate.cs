using DependencyParser.Universal;

namespace UniversalDependencyParser.Parser.TransitionBasedParser
{
    public class Candidate
    {
        private Command command;
        private UniversalDependencyType universalDependencyType;

        public Candidate(Command command, UniversalDependencyType universalDependencyType){
            this.command = command;
            this.universalDependencyType = universalDependencyType;
        }

        public Command GetCommand() {
            return command;
        }

        public UniversalDependencyType GetUniversalDependencyType() {
            return universalDependencyType;
        }

    }
}