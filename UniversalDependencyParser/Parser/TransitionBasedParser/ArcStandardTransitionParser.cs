using System.Collections.Generic;
using Classification.Instance;
using DependencyParser.Universal;

namespace UniversalDependencyParser.Parser.TransitionBasedParser
{
    public class ArcStandardTransitionParser : TransitionParser
    {
        public ArcStandardTransitionParser() : base()
        {
        }

        /// <summary>
        /// Checks if there are more relations with a specified ID in the list of words.
        /// </summary>
        /// <param name="wordList">The list of words to check.</param>
        /// <param name="id">The ID to check for.</param>
        /// <returns>True if no more relations with the specified ID are found; false otherwise.</returns>
        private bool CheckForMoreRelation(List<StackWord> wordList, int id)
        {
            foreach (var word in wordList)
            {
                if (word.GetWord().GetRelation().To() == id)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Simulates the parsing process for a given sentence using the Arc Standard parsing algorithm.
        /// </summary>
        /// <param name="sentence">The sentence to be parsed.</param>
        /// <param name="windowSize">The size of the window used for feature generation.</param>
        /// <returns>An ArrayList of {@link Instance} objects representing the parsed actions.</returns>
        public override List<Instance> SimulateParse(UniversalDependencyTreeBankSentence sentence, int windowSize)
        {
            UniversalDependencyTreeBankWord top, beforeTop;
            UniversalDependencyRelation topRelation, beforeTopRelation;
            InstanceGenerator instanceGenerator = new SimpleInstanceGenerator();
            var instanceList = new List<Instance>();
            var wordList = new List<StackWord>();
            var stack = new List<StackWord>();
            for (var j = 0; j < sentence.WordCount(); j++)
            {
                wordList.Add(new StackWord((UniversalDependencyTreeBankWord)sentence.GetWord(j), j + 1));
            }

            stack.Add(new StackWord());
            var state = new State(stack, wordList, new List<StackRelation>());
            if (wordList.Count > 0)
            {
                instanceList.Add(instanceGenerator.Generate(state, windowSize, "SHIFT"));
                MoveFromWordListToStack(stack, wordList);
                if (wordList.Count > 1)
                {
                    instanceList.Add(instanceGenerator.Generate(state, windowSize, "SHIFT"));
                    MoveFromWordListToStack(stack, wordList);
                }

                while (wordList.Count > 0 || stack.Count > 1)
                {
                    top = stack[stack.Count - 1].GetWord();
                    topRelation = top.GetRelation();
                    if (stack.Count > 1)
                    {
                        beforeTop = stack[stack.Count - 2].GetWord();
                        beforeTopRelation = beforeTop.GetRelation();
                        if (beforeTop.GetId() == topRelation.To() && CheckForMoreRelation(wordList, top.GetId()))
                        {
                            instanceList.Add(instanceGenerator.Generate(state, windowSize, "RIGHTARC(" + topRelation + ")"));
                            stack.RemoveAt(stack.Count - 1);
                        }
                        else if (top.GetId() == beforeTopRelation.To())
                        {
                            instanceList.Add(instanceGenerator.Generate(state, windowSize, "LEFTARC(" + beforeTopRelation + ")"));
                            stack.RemoveAt(stack.Count - 2);
                        }
                        else
                        {
                            if (wordList.Count > 0)
                            {
                                instanceList.Add(instanceGenerator.Generate(state, windowSize, "SHIFT"));
                                MoveFromWordListToStack(stack, wordList);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (wordList.Count > 0)
                        {
                            instanceList.Add(instanceGenerator.Generate(state, windowSize, "SHIFT"));
                            MoveFromWordListToStack(stack, wordList);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            return instanceList;
        }

        /// <summary>
        /// Performs dependency parsing on the given sentence using the provided oracle.
        /// </summary>
        /// <param name="universalDependencyTreeBankSentence">The sentence to be parsed.</param>
        /// <param name="oracle">The oracle used to make parsing decisions.</param>
        /// <returns>The parsed sentence with dependency relations established.</returns>
        public override UniversalDependencyTreeBankSentence DependencyParse(
            UniversalDependencyTreeBankSentence universalDependencyTreeBankSentence, Oracle oracle)
        {
            var sentence = CreateResultSentence(universalDependencyTreeBankSentence);
            var state = InitialState(sentence);
            while (state.WordListSize() > 0 || state.StackSize() > 1) {
                var decision = oracle.MakeDecision(state);
                switch (decision.GetCommand()) {
                    case Command.SHIFT:
                        state.ApplyShift();
                        break;
                    case Command.LEFTARC:
                        state.ApplyLeftArc(decision.GetUniversalDependencyType());
                        break;
                    case Command.RIGHTARC:
                        state.ApplyRightArc(decision.GetUniversalDependencyType());
                        break;
                }
            }
            return sentence;
        }
    }
}