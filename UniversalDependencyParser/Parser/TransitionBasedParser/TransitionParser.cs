using System.Collections.Generic;
using Classification.DataSet;
using Classification.Instance;
using DependencyParser.Universal;

namespace UniversalDependencyParser.TransitionBasedParser
{
    public abstract class TransitionParser
    {
        protected UniversalDependencyTreeBankSentence CreateResultSentence(
            UniversalDependencyTreeBankSentence universalDependencyTreeBankSentence)
        {
            var sentence = new UniversalDependencyTreeBankSentence();
            for (var i = 0; i < universalDependencyTreeBankSentence.WordCount(); i++)
            {
                var word = (UniversalDependencyTreeBankWord)universalDependencyTreeBankSentence.GetWord(i);
                sentence.AddWord(new UniversalDependencyTreeBankWord(word.GetId(), word.GetName(), word.GetLemma(),
                    word.GetUpos(), word.GetXpos(), word.GetFeatures(), null, word.GetDeps(), word.GetMisc()));
            }

            return sentence;
        }

        protected void MoveFromWordListToStack(List<StackWord> stack, List<StackWord> wordList)
        {
            var word = wordList[0];
            stack.Add(word);
            wordList.RemoveAt(0);
        }
        
        public DataSet SimulateParseOnCorpus(UniversalDependencyTreeBankCorpus corpus, int windowSize)
        {
            var dataSet = new DataSet();
            for (var i = 0; i < corpus.SentenceCount(); i++)
            {
                dataSet.AddInstanceList(SimulateParse((UniversalDependencyTreeBankSentence)corpus.GetSentence(i),
                    windowSize));
            }

            return dataSet;
        }

        public abstract List<Instance> SimulateParse(UniversalDependencyTreeBankSentence sentence, int windowSize);

        public abstract UniversalDependencyTreeBankSentence DependencyParse(
            UniversalDependencyTreeBankSentence universalDependencyTreeBankSentence, Oracle oracle);

        private bool CheckStates(Agenda agenda)
        {
            foreach (var state in agenda.GetKeySet())
            {
                if (state.WordListSize() > 0 || state.StackSize() > 1)
                {
                    return true;
                }
            }

            return false;
        }

        protected State InitialState(UniversalDependencyTreeBankSentence sentence)
        {
            var wordList = new List<StackWord>();
            for (var i = 0; i < sentence.WordCount(); i++)
            {
                wordList.Add(new StackWord((UniversalDependencyTreeBankWord)sentence.GetWord(i), i + 1));
            }

            var stack = new List<StackWord>();
            stack.Add(new StackWord());
            return new State(stack, wordList, new List<StackRelation>());
        }

        private List<Candidate> ConstructCandidates(TransitionSystem transitionSystem, State state)
        {
            if (state.StackSize() == 1 && state.WordListSize() == 0)
            {
                return new List<Candidate>();
            }

            var subsets = new List<Candidate>();
            if (state.WordListSize() > 0)
            {
                subsets.Add(new Candidate(Command.SHIFT, UniversalDependencyType.DEP));
            }

            if (transitionSystem == TransitionSystem.ARC_EAGER && state.StackSize() > 0)
            {
                subsets.Add(new Candidate(Command.REDUCE, UniversalDependencyType.DEP));
            }

            for (var i = 0; i < UniversalDependencyRelation.UniversalDependencyTypes.Length; i++)
            {
                var type = UniversalDependencyRelation.GetDependencyTag(
                    UniversalDependencyRelation.UniversalDependencyTypes[i]);
                if (transitionSystem == TransitionSystem.ARC_STANDARD && state.StackSize() > 1)
                {
                    subsets.Add(new Candidate(Command.LEFTARC, type));
                    subsets.Add(new Candidate(Command.RIGHTARC, type));
                }
                else if (transitionSystem == TransitionSystem.ARC_EAGER && state.StackSize() > 0 &&
                         state.WordListSize() > 0)
                {
                    subsets.Add(new Candidate(Command.LEFTARC, type));
                    subsets.Add(new Candidate(Command.RIGHTARC, type));
                }
            }

            return subsets;
        }

        public State DependencyParseWithBeamSearch(ScoringOracle oracle, int beamSize,
            UniversalDependencyTreeBankSentence universalDependencyTreeBankSentence,
            TransitionSystem transitionSystem)
        {
            var sentence = CreateResultSentence(universalDependencyTreeBankSentence);

            var initialState = InitialState(sentence);
            var agenda = new Agenda(beamSize);
            agenda.UpdateAgenda(oracle, (State)initialState.Clone());
            while (CheckStates(agenda))
            {
                foreach (var state in agenda.GetKeySet()) {
                    var subsets = ConstructCandidates(transitionSystem, state);
                    foreach (var subset in subsets) {
                        var command = subset.GetCommand();
                        var type = subset.GetUniversalDependencyType();
                        var cloneState = (State)state.Clone();
                        cloneState.Apply(command, type, transitionSystem);
                        agenda.UpdateAgenda(oracle, (State)cloneState.Clone());
                    }
                }
            }

            return agenda.Best();
        }

        public UniversalDependencyTreeBankCorpus DependencyParseCorpus(
            UniversalDependencyTreeBankCorpus universalDependencyTreeBankCorpus, Oracle oracle)
        {
            var corpus = new UniversalDependencyTreeBankCorpus();
            for (var i = 0; i < universalDependencyTreeBankCorpus.SentenceCount(); i++)
            {
                var sentence =
                    (UniversalDependencyTreeBankSentence)universalDependencyTreeBankCorpus.GetSentence(i);
                corpus.AddSentence(DependencyParse(sentence, oracle));
            }

            return corpus;
        }
    }
}