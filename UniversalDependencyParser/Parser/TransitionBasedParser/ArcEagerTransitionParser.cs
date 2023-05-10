using System.Collections.Generic;
using Classification.Instance;
using DependencyParser.Universal;

namespace UniversalDependencyParser.TransitionBasedParser
{
    public class ArcEagerTransitionParser : TransitionParser
    {
        public ArcEagerTransitionParser() : base()
        {
        }

        public override List<Instance> SimulateParse(UniversalDependencyTreeBankSentence sentence, int windowSize)
        {
            UniversalDependencyTreeBankWord top, first;
            UniversalDependencyRelation topRelation = null, firstRelation;
            InstanceGenerator instanceGenerator = new ArcEagerInstanceGenerator();
            var instanceList = new List<Instance>();
            var wordMap = new Dictionary<int, UniversalDependencyTreeBankWord>();
            var wordList = new List<StackWord>();
            var stack = new List<StackWord>();
            for (var j = 0; j < sentence.WordCount(); j++)
            {
                var word = (UniversalDependencyTreeBankWord)sentence.GetWord(j);
                var clone = word.Clone();
                clone.SetRelation(null);
                wordMap[j + 1] = word;
                wordList.Add(new StackWord(clone, j + 1));
            }

            stack.Add(new StackWord());
            var state = new State(stack, wordList, new List<StackRelation>());
            while (wordList.Count > 0 || stack.Count > 1)
            {
                if (wordList.Count != 0)
                {
                    first = wordList[0].GetWord();
                    firstRelation = wordMap[wordList[0].GetToWord()].GetRelation();
                }
                else
                {
                    first = null;
                    firstRelation = null;
                }

                top = stack[stack.Count - 1].GetWord();
                if (top.GetName() != "root")
                {
                    topRelation = wordMap[stack[stack.Count - 1].GetToWord()].GetRelation();
                }

                if (stack.Count > 1)
                {
                    if (firstRelation != null && firstRelation.To() == top.GetId())
                    {
                        instanceList.Add(instanceGenerator.Generate(state, windowSize,
                            "RIGHTARC(" + firstRelation + ")"));
                        var word = wordList[0];
                        wordList.RemoveAt(0);
                        stack.Add(new StackWord(wordMap[word.GetToWord()], word.GetToWord()));
                    }
                    else if (first != null && topRelation != null && topRelation.To() == first.GetId())
                    {
                        instanceList.Add(instanceGenerator.Generate(state, windowSize, "LEFTARC(" + topRelation + ")"));
                        stack.RemoveAt(stack.Count - 1);
                    }
                    else if (wordList.Count > 0)
                    {
                        instanceList.Add(instanceGenerator.Generate(state, windowSize, "SHIFT"));
                        MoveFromWordListToStack(stack, wordList);
                    }
                    else
                    {
                        instanceList.Add(instanceGenerator.Generate(state, windowSize, "REDUCE"));
                        stack.RemoveAt(stack.Count - 1);
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

            return instanceList;
        }

        public override UniversalDependencyTreeBankSentence DependencyParse(
            UniversalDependencyTreeBankSentence universalDependencyTreeBankSentence, Oracle oracle)
        {
            UniversalDependencyTreeBankSentence sentence = CreateResultSentence(universalDependencyTreeBankSentence);
            State state = InitialState(sentence);
            while (state.WordListSize() > 0 || state.StackSize() > 1)
            {
                var decision = oracle.MakeDecision(state);
                switch (decision.GetCommand())
                {
                    case Command.SHIFT:
                        state.ApplyShift();
                        break;
                    case Command.LEFTARC:
                        state.ApplyArcEagerLeftArc(decision.GetUniversalDependencyType());
                        break;
                    case Command.RIGHTARC:
                        state.ApplyArcEagerRightArc(decision.GetUniversalDependencyType());
                        break;
                    case Command.REDUCE:
                        state.ApplyReduce();
                        break;
                }
            }

            return sentence;
        }
    }
}