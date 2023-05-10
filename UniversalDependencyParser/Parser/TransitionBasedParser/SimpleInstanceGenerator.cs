using System.Collections.Generic;
using Classification.Attribute;
using Classification.Instance;
using DependencyParser.Universal;

namespace UniversalDependencyParser.TransitionBasedParser
{
    public class SimpleInstanceGenerator : InstanceGenerator
    {
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
                }
                else
                {
                    if (word.GetName() == "root")
                    {
                        attributes.Add(new DiscreteIndexedAttribute("root", 0, 18));
                        AddEmptyAttributes(attributes);
                    }
                    else
                    {
                        attributes.Add(new DiscreteIndexedAttribute(word.GetUpos().ToString(),
                            UniversalDependencyTreeBankFeatures.PosIndex(word.GetUpos().ToString()) + 1, 18));
                        AddFeatureAttributes(word, attributes);
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