using System.Collections.Generic;
using Classification.Attribute;
using Classification.Instance;
using DependencyParser.Universal;

namespace UniversalDependencyParser.Parser.TransitionBasedParser
{
    public class ArcEagerInstanceGenerator : InstanceGenerator
    {
        /// <summary>
        /// Checks if the given word has a valid relation.
        /// </summary>
        /// <param name="word">The UniversalDependencyTreeBankWord to check.</param>
        /// <returns>true if the relation is valid, false otherwise.</returns>
        private bool Suitable(UniversalDependencyTreeBankWord word)
        {
            return word.GetRelation() != null;
        }

        /// <summary>
        /// Generates an Instance object based on the provided state, window size, and command.
        /// The Instance is populated with attributes derived from the words in the state.
        /// </summary>
        /// <param name="state">The state used to generate the instance.</param>
        /// <param name="windowSize">The size of the window used to extract words from the state.</param>
        /// <param name="command">The command associated with the instance.</param>
        /// <returns>The generated Instance object.</returns>
        public override Instance Generate(State state, int windowSize, string command)
        {
            var instance = new Instance(command);
            var attributes = new List<Attribute>();
            for (var i = 0; i < windowSize; i++)
            {
                var word = state.GetStackWord(i);
                if (word == null)
                {
                    attributes.Add(new DiscreteIndexedAttribute("null", 0, 18));
                    AddEmptyAttributes(attributes);
                    attributes.Add(new DiscreteIndexedAttribute("null", 0, 59));
                }
                else
                {
                    if (word.GetName() == "root")
                    {
                        attributes.Add(new DiscreteIndexedAttribute("root", 0, 18));
                        AddEmptyAttributes(attributes);
                        attributes.Add(new DiscreteIndexedAttribute("null", 0, 59));
                    }
                    else
                    {
                        attributes.Add(new DiscreteIndexedAttribute(word.GetUpos().ToString(),
                            UniversalDependencyTreeBankFeatures.PosIndex(word.GetUpos().ToString()) + 1, 18));
                        AddFeatureAttributes(word, attributes);
                        if (Suitable(word))
                        {
                            attributes.Add(new DiscreteIndexedAttribute(word.GetRelation().ToString(),
                                UniversalDependencyTreeBankFeatures.DependencyIndex(word.GetRelation().ToString()) + 1,
                                59));
                        }
                        else
                        {
                            attributes.Add(new DiscreteIndexedAttribute("null", 0, 59));
                        }
                    }
                }
            }

            for (var i = 0; i < windowSize; i++)
            {
                var word = state.GetWordListWord(i);
                if (word != null)
                {
                    attributes.Add(new DiscreteIndexedAttribute(word.GetUpos().ToString(),
                        UniversalDependencyTreeBankFeatures.PosIndex(word.GetUpos().ToString()) + 1, 18));
                    AddFeatureAttributes(word, attributes);
                }
                else
                {
                    attributes.Add(new DiscreteIndexedAttribute("root", 0, 18));
                    AddEmptyAttributes(attributes);
                }
            }

            foreach (var attribute in attributes) {
                instance.AddAttribute(attribute);
            }
            return instance;
        }
    }
}