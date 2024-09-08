using System.Collections.Generic;
using DependencyParser.Universal;

namespace UniversalDependencyParser.Parser.TransitionBasedParser
{
    public class State
    {
        private List<StackWord> stack;
        private List<StackWord> wordList;
        private List<StackRelation> relations;

        /// <summary>
        /// Constructs a State object with given stack, wordList, and relations.
        /// </summary>
        /// <param name="stack">The stack of words in the parser state.</param>
        /// <param name="wordList">The list of words to be processed.</param>
        /// <param name="relations">The relations established between words.</param>
        public State(List<StackWord> stack, List<StackWord> wordList, List<StackRelation> relations)
        {
            this.stack = stack;
            this.wordList = wordList;
            this.relations = relations;
        }

        /// <summary>
        /// Applies the SHIFT operation to the parser state.
        /// Moves the first word from the wordList to the stack.
        /// </summary>
        public void ApplyShift()
        {
            if (wordList.Count > 0)
            {
                var word = wordList[0];
                stack.Add(word);
                wordList.RemoveAt(0);
            }
        }

        /// <summary>
        /// Applies the LEFTARC operation to the parser state.
        /// Creates a relation from the second-to-top stack element to the top stack element
        /// and then removes the second-to-top element from the stack.
        /// </summary>
        /// <param name="type">The type of the dependency relation.</param>
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

        /// <summary>
        /// Applies the RIGHTARC operation to the parser state.
        /// Creates a relation from the top stack element to the second-to-top stack element
        /// and then removes the top element from the stack.
        /// </summary>
        /// <param name="type">The type of the dependency relation.</param>
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

        /// <summary>
        /// Applies the ARC_EAGER_LEFTARC operation to the parser state.
        /// Creates a relation from the last element of the stack to the first element of the wordList
        /// and then removes the top element from the stack.
        /// </summary>
        /// <param name="type"></param>
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

        /// <summary>
        /// Applies the ARC_EAGER_RIGHTARC operation to the parser state.
        /// Creates a relation from the first element of the wordList to the top element of the stack
        /// and then performs a SHIFT operation.
        /// </summary>
        /// <param name="type">The type of the dependency relation.</param>
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

        /// <summary>
        /// Applies the REDUCE operation to the parser state.
        /// Removes the top element from the stack.
        /// </summary>
        public void ApplyReduce()
        {
            if (stack.Count > 0)
            {
                stack.RemoveAt(stack.Count - 1);
            }
        }

        /// <summary>
        /// Applies a specific command based on the transition system.
        /// </summary>
        /// <param name="command">The command to be applied (e.g., SHIFT, LEFTARC, RIGHTARC, REDUCE).</param>
        /// <param name="type">The type of dependency relation, relevant for ARC operations.</param>
        /// <param name="transitionSystem">The transition system (e.g., ARC_STANDARD, ARC_EAGER) that determines which command to apply.</param>
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

        /// <summary>
        /// Returns the number of relations established in the current state.
        /// </summary>
        /// <returns>The size of the relations list.</returns>
        public int RelationSize()
        {
            return relations.Count;
        }

        /// <summary>
        /// Returns the number of words remaining in the wordList.
        /// </summary>
        /// <returns>The size of the wordList.</returns>
        public int WordListSize()
        {
            return wordList.Count;
        }

        /// <summary>
        /// Returns the number of words currently in the stack.
        /// </summary>
        /// <returns>The size of the stack.</returns>
        public int StackSize()
        {
            return stack.Count;
        }

        /// <summary>
        /// Retrieves a specific word from the stack based on its position.
        /// </summary>
        /// <param name="index">The position of the word in the stack.</param>
        /// <returns>The word at the specified position, or null if the index is out of bounds.</returns>
        public UniversalDependencyTreeBankWord GetStackWord(int index)
        {
            var size = stack.Count - 1;
            if (size - index < 0)
            {
                return null;
            }

            return stack[size - index].GetWord();
        }

        /// <summary>
        /// Retrieves the top word from the stack.
        /// </summary>
        /// <returns>The top word of the stack, or null if the stack is empty.</returns>
        public UniversalDependencyTreeBankWord GetPeek()
        {
            if (stack.Count > 0)
            {
                return stack[stack.Count - 1].GetWord();
            }

            return null;
        }

        /// <summary>
        /// Retrieves a specific word from the wordList based on its position.
        /// </summary>
        /// <param name="index">The position of the word in the wordList.</param>
        /// <returns>The word at the specified position, or null if the index is out of bounds.</returns>
        public UniversalDependencyTreeBankWord GetWordListWord(int index)
        {
            if (index > wordList.Count - 1)
            {
                return null;
            }

            return wordList[index].GetWord();
        }

        /// <summary>
        /// Retrieves a specific relation based on its index.
        /// </summary>
        /// <param name="index">The position of the relation in the relations list.</param>
        /// <returns>The relation at the specified position, or null if the index is out of bounds.</returns>
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