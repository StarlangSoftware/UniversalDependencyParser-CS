using DependencyParser.Universal;

namespace UniversalDependencyParser.Parser.TransitionBasedParser
{
    public class StackWord
    {
        private UniversalDependencyTreeBankWord word;
        private int toWord;

        public StackWord(){
            word = new UniversalDependencyTreeBankWord();
            toWord = 0;
        }

        public StackWord Clone(){
            return new StackWord(word.Clone(), toWord);
        }
        public StackWord(UniversalDependencyTreeBankWord word, int toWord){
            this.word = word;
            this.toWord = toWord;
        }

        public UniversalDependencyTreeBankWord GetWord() {
            return word;
        }

        public int GetToWord() {
            return toWord;
        }

    }
}