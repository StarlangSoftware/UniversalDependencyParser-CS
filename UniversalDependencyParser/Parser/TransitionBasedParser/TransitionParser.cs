using System.Collections.Generic;
using Classification.DataSet;
using Classification.Instance;
using DependencyParser.Universal;

namespace UniversalDependencyParser.Parser.TransitionBasedParser
{
    public abstract class TransitionParser
    {
        /// <summary>
        /// Creates a new {@link UniversalDependencyTreeBankSentence} with the same words as the input sentence,
        /// but with null heads, effectively cloning the sentence structure without dependencies.
        /// </summary>
        /// <param name="universalDependencyTreeBankSentence">the sentence to be cloned</param>
        /// <returns>a new {@link UniversalDependencyTreeBankSentence} with copied words but no dependencies</returns>
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
        
        /// <summary>
        /// Simulates parsing a corpus of sentences, returning a dataset of instances created by parsing each sentence.
        /// </summary>
        /// <param name="corpus">the corpus to be parsed</param>
        /// <param name="windowSize">the size of the window used in parsing</param>
        /// <returns>a {@link DataSet} containing instances from parsing each sentence in the corpus</returns>
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

        /// <summary>
        /// Parses a single sentence and returns a list of instances that represent the parsing process.
        /// </summary>
        /// <param name="sentence">the sentence to be parsed</param>
        /// <param name="windowSize">the size of the window used in parsing</param>
        /// <returns>a list of {@link Instance} objects representing the parsing process</returns>
        public abstract List<Instance> SimulateParse(UniversalDependencyTreeBankSentence sentence, int windowSize);

        /// <summary>
        /// Parses a single sentence using a specified oracle and returns the parsed sentence with dependencies.
        /// </summary>
        /// <param name="universalDependencyTreeBankSentence">the sentence to be parsed</param>
        /// <param name="oracle">the oracle used for guiding the parsing process</param>
        /// <returns>a {@link UniversalDependencyTreeBankSentence} with dependencies parsed</returns>
        public abstract UniversalDependencyTreeBankSentence DependencyParse(
            UniversalDependencyTreeBankSentence universalDependencyTreeBankSentence, Oracle oracle);

        /// <summary>
        /// Checks if there are any states in the agenda that still have words to process or have more than one item in the stack.
        /// </summary>
        /// <param name="agenda">the agenda containing the states</param>
        /// <returns>true if there are states to process, false otherwise</returns>
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

        /// <summary>
        /// Initializes the parsing state with a stack containing one empty {@link StackWord} and a word list containing all words in the sentence.
        /// </summary>
        /// <param name="sentence">the sentence to initialize the state with</param>
        /// <returns>a {@link State} representing the starting point for parsing</returns>
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

        /// <summary>
        /// Constructs possible parsing candidates based on the current state and transition system.
        /// </summary>
        /// <param name="transitionSystem">the transition system used (ARC_STANDARD or ARC_EAGER)</param>
        /// <param name="state">the current parsing state</param>
        /// <returns>a list of possible {@link Candidate} actions to be applied</returns>
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

        /// <summary>
        /// Performs dependency parsing with beam search to find the best parse for a given sentence.
        /// </summary>
        /// <param name="oracle">the scoring oracle used for guiding the search</param>
        /// <param name="beamSize">the size of the beam for beam search</param>
        /// <param name="universalDependencyTreeBankSentence">the sentence to be parsed</param>
        /// <param name="transitionSystem">the transition system used (ARC_STANDARD or ARC_EAGER)</param>
        /// <returns>the best parsing state from the beam search</returns>
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

        /// <summary>
        /// Parses a corpus of sentences using the given oracle and returns a new corpus with the parsed sentences.
        /// </summary>
        /// <param name="universalDependencyTreeBankCorpus">the corpus to be parsed</param>
        /// <param name="oracle">the oracle used for guiding the parsing process</param>
        /// <returns>a {@link UniversalDependencyTreeBankCorpus} containing the parsed sentences</returns>
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