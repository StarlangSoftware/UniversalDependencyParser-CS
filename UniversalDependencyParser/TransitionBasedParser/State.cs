using System.Collections.Generic;
using DependencyParser.Universal;

namespace UniversalDependencyParser.TransitionBasedParser
{
    public class State
    {
        private List<StackWord> stack;
        private List<StackWord> wordList;
        private List<StackRelation> relations;

        public State(List<StackWord> stack, List<StackWord> wordList, List<StackRelation> relations)
        {
            this.stack = stack;
            this.wordList = wordList;
            this.relations = relations;
        }

        public void ApplyShift()
        {
            if (wordList.Count > 0)
            {
                var word = wordList[0];
                stack.Add(word);
                wordList.RemoveAt(0);
            }
        }

        public void ApplyLeftArc(UniversalDependencyType type)
        {
            if (stack.Count > 1)
            {
                var beforeLast = stack[stack.Count - 2].GetWord();
                var index = stack[stack.Count - 1].GetToWord();
                beforeLast.SetRelation(new UniversalDependencyRelation(index, type.ToString().Replace("_", ":")));
                stack.RemoveAt(stack.Count - 2);
                relations.Add(new StackRelation(beforeLast,
                    new UniversalDependencyRelation(index, type.ToString().Replace("_", ":"))));
            }
        }

        public void ApplyRightArc(UniversalDependencyType type)
        {
            if (stack.Count > 1)
            {
                var last = stack[stack.Count - 1].GetWord();
                var index = stack[stack.Count - 2].GetToWord();
                last.SetRelation(new UniversalDependencyRelation(index, type.ToString().Replace("_", ":")));
                stack.RemoveAt(stack.Count - 1);
                relations.Add(new StackRelation(last,
                    new UniversalDependencyRelation(index, type.ToString().Replace("_", ":"))));
            }
        }

        public void ApplyArcEagerLeftArc(UniversalDependencyType type)
        {
            if (stack.Count > 0 && wordList.Count > 0)
            {
                var lastElementOfStack = stack[stack.Count - 1].GetWord();
                var index = wordList[0].GetToWord();
                lastElementOfStack.SetRelation(new UniversalDependencyRelation(index,
                    type.ToString().Replace("_", ":")));
                stack.RemoveAt(stack.Count - 1);
                relations.Add(new StackRelation(lastElementOfStack,
                    new UniversalDependencyRelation(index, type.ToString().Replace("_", ":"))));
            }
        }

        public void ApplyArcEagerRightArc(UniversalDependencyType type)
        {
            if (stack.Count > 0 && wordList.Count > 0)
            {
                var firstElementOfWordList = wordList[0].GetWord();
                var index = stack[stack.Count - 1].GetToWord();
                firstElementOfWordList.SetRelation(new UniversalDependencyRelation(index,
                    type.ToString().Replace("_", ":")));
                ApplyShift();
                relations.Add(new StackRelation(firstElementOfWordList,
                    new UniversalDependencyRelation(index, type.ToString().Replace("_", ":"))));
            }
        }

        public void ApplyReduce()
        {
            if (stack.Count > 0)
            {
                stack.RemoveAt(stack.Count - 1);
            }
        }

        public void Apply(Command command, UniversalDependencyType type, TransitionSystem transitionSystem)
        {
            switch (transitionSystem)
            {
                case TransitionSystem.ARC_STANDARD:
                    switch (command)
                    {
                        case Command.LEFTARC:
                            ApplyLeftArc(type);
                            break;
                        case Command.RIGHTARC:
                            ApplyRightArc(type);
                            break;
                        case Command.SHIFT:
                            ApplyShift();
                            break;
                    }

                    break;
                case TransitionSystem.ARC_EAGER:
                    switch (command)
                    {
                        case Command.LEFTARC:
                            ApplyArcEagerLeftArc(type);
                            break;
                        case Command.RIGHTARC:
                            ApplyArcEagerRightArc(type);
                            break;
                        case Command.SHIFT:
                            ApplyShift();
                            break;
                        case Command.REDUCE:
                            ApplyReduce();
                            break;
                    }

                    break;
            }
        }

        public int RelationSize()
        {
            return relations.Count;
        }

        public int WordListSize()
        {
            return wordList.Count;
        }

        public int StackSize()
        {
            return stack.Count;
        }

        public UniversalDependencyTreeBankWord GetStackWord(int index)
        {
            var size = stack.Count - 1;
            if (size - index < 0)
            {
                return null;
            }

            return stack[size - index].GetWord();
        }

        public UniversalDependencyTreeBankWord GetPeek()
        {
            if (stack.Count > 0)
            {
                return stack[stack.Count - 1].GetWord();
            }

            return null;
        }

        public UniversalDependencyTreeBankWord GetWordListWord(int index)
        {
            if (index > wordList.Count - 1)
            {
                return null;
            }

            return wordList[index].GetWord();
        }

        public StackRelation GetRelation(int index)
        {
            if (index < relations.Count)
            {
                return relations[index];
            }

            return null;
        }

        public new int GetHashCode()
        {
            return stack.GetHashCode() ^ wordList.GetHashCode() ^ relations.GetHashCode();
        }

        public object Clone()
        {
            var o = new State(new List<StackWord>(), new List<StackWord>(), new List<StackRelation>());
            foreach (var element in stack) {
                if (element.GetWord().GetName() != "root")
                {
                    o.stack.Add(element.Clone());
                }
                else
                {
                    o.stack.Add(new StackWord(new UniversalDependencyTreeBankWord(), element.GetToWord()));
                }
            }

            foreach (var word in wordList) {
                o.wordList.Add(word.Clone());
            }
            foreach (var relation in relations) {
                if (relation.GetWord().GetName() != "root")
                {
                    o.relations.Add(relation.Clone());
                }
                else
                {
                    o.relations.Add(new StackRelation(new UniversalDependencyTreeBankWord(), relation.GetRelation()));
                }
            }
            return o;
        }
    }
}