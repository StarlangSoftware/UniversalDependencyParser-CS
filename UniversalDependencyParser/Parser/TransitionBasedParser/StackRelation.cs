using DependencyParser.Universal;

namespace UniversalDependencyParser.TransitionBasedParser
{
    public class StackRelation
    {
        private UniversalDependencyTreeBankWord word;
        private UniversalDependencyRelation relation;

        public StackRelation(UniversalDependencyTreeBankWord word, UniversalDependencyRelation relation){
            this.word = word;
            this.relation = relation;
        }

        public StackRelation Clone(){
            return new StackRelation(word.Clone(), relation);
        }

        public UniversalDependencyTreeBankWord GetWord() {
            return word;
        }

        public UniversalDependencyRelation GetRelation() {
            return relation;
        }

    }
}