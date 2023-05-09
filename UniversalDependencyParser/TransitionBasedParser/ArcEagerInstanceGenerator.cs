using System.Collections.Generic;
using Classification.Attribute;
using Classification.Instance;
using DependencyParser.Universal;

namespace UniversalDependencyParser.TransitionBasedParser
{
    public class ArcEagerInstanceGenerator : InstanceGenerator
    {
        private bool Suitable(UniversalDependencyTreeBankWord word)
        {
            return word.GetRelation() != null;
        }

        public override Instance Generate(State state, int windowSize, string command)
        {
            var instance = new Instance(command);
            var attributes = new List<Attribute>();
            for (var i = 0; i < windowSize; i++)
            {
                UniversalDependencyTreeBankWord word = state.GetStackWord(i);
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

            for (int i = 0; i < windowSize; i++)
            {
                UniversalDependencyTreeBankWord word = state.GetWordListWord(i);
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