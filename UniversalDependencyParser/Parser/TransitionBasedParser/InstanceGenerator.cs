using System.Collections.Generic;
using Classification.Attribute;
using Classification.Instance;
using DependencyParser.Universal;

namespace UniversalDependencyParser.TransitionBasedParser
{
    public abstract class InstanceGenerator
    {
        public abstract Instance Generate(State state, int windowSize, string command);

        private void AddAttributeForFeatureType(UniversalDependencyTreeBankWord word, List<Attribute> attributes,
            string featureType)
        {
            var feature = word.GetFeatureValue(featureType);
            var numberOfValues = UniversalDependencyTreeBankFeatures.NumberOfValues("tr", featureType) + 1;
            if (feature != null)
            {
                attributes.Add(new DiscreteIndexedAttribute(feature,
                    UniversalDependencyTreeBankFeatures.FeatureValueIndex("tr", featureType, feature) + 1,
                    numberOfValues));
            }
            else
            {
                attributes.Add(new DiscreteIndexedAttribute("null", 0, numberOfValues));
            }
        }

        protected void AddEmptyAttributes(List<Attribute> attributes)
        {
            attributes.Add(new DiscreteIndexedAttribute("null", 0,
                UniversalDependencyTreeBankFeatures.NumberOfValues("tr", "PronType") + 1));
            attributes.Add(new DiscreteIndexedAttribute("null", 0,
                UniversalDependencyTreeBankFeatures.NumberOfValues("tr", "NumType") + 1));
            attributes.Add(new DiscreteIndexedAttribute("null", 0,
                UniversalDependencyTreeBankFeatures.NumberOfValues("tr", "Number") + 1));
            attributes.Add(new DiscreteIndexedAttribute("null", 0,
                UniversalDependencyTreeBankFeatures.NumberOfValues("tr", "Case") + 1));
            attributes.Add(new DiscreteIndexedAttribute("null", 0,
                UniversalDependencyTreeBankFeatures.NumberOfValues("tr", "Definite") + 1));
            attributes.Add(new DiscreteIndexedAttribute("null", 0,
                UniversalDependencyTreeBankFeatures.NumberOfValues("tr", "Degree") + 1));
            attributes.Add(new DiscreteIndexedAttribute("null", 0,
                UniversalDependencyTreeBankFeatures.NumberOfValues("tr", "VerbForm") + 1));
            attributes.Add(new DiscreteIndexedAttribute("null", 0,
                UniversalDependencyTreeBankFeatures.NumberOfValues("tr", "Mood") + 1));
            attributes.Add(new DiscreteIndexedAttribute("null", 0,
                UniversalDependencyTreeBankFeatures.NumberOfValues("tr", "Tense") + 1));
            attributes.Add(new DiscreteIndexedAttribute("null", 0,
                UniversalDependencyTreeBankFeatures.NumberOfValues("tr", "Aspect") + 1));
            attributes.Add(new DiscreteIndexedAttribute("null", 0,
                UniversalDependencyTreeBankFeatures.NumberOfValues("tr", "Voice") + 1));
            attributes.Add(new DiscreteIndexedAttribute("null", 0,
                UniversalDependencyTreeBankFeatures.NumberOfValues("tr", "Evident") + 1));
            attributes.Add(new DiscreteIndexedAttribute("null", 0,
                UniversalDependencyTreeBankFeatures.NumberOfValues("tr", "Polarity") + 1));
            attributes.Add(new DiscreteIndexedAttribute("null", 0,
                UniversalDependencyTreeBankFeatures.NumberOfValues("tr", "Person") + 1));
        }

        protected void AddFeatureAttributes(UniversalDependencyTreeBankWord word, List<Attribute> attributes)
        {
            AddAttributeForFeatureType(word, attributes, "PronType");
            AddAttributeForFeatureType(word, attributes, "NumType");
            AddAttributeForFeatureType(word, attributes, "Number");
            AddAttributeForFeatureType(word, attributes, "Case");
            AddAttributeForFeatureType(word, attributes, "Definite");
            AddAttributeForFeatureType(word, attributes, "Degree");
            AddAttributeForFeatureType(word, attributes, "VerbForm");
            AddAttributeForFeatureType(word, attributes, "Mood");
            AddAttributeForFeatureType(word, attributes, "Tense");
            AddAttributeForFeatureType(word, attributes, "Aspect");
            AddAttributeForFeatureType(word, attributes, "Voice");
            AddAttributeForFeatureType(word, attributes, "Evident");
            AddAttributeForFeatureType(word, attributes, "Polarity");
            AddAttributeForFeatureType(word, attributes, "Person");
        }
    }
}