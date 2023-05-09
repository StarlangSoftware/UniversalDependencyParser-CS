using System.Collections.Generic;
using Classification.Instance;
using DependencyParser.Universal;

namespace UniversalDependencyParser.TransitionBasedParser
{
    public class ArcStandardTransitionParser : TransitionParser
    {
        public ArcStandardTransitionParser() : base()
        {
        }

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