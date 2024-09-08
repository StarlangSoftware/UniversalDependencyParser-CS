using System.Collections.Generic;
using Classification.Attribute;
using Classification.Instance;
using DependencyParser.Universal;

namespace UniversalDependencyParser.Parser.TransitionBasedParser
{
    public abstract class InstanceGenerator
    {
        /// <summary>
        /// Abstract method for generating an instance based on the current state, window size, and command.
        /// </summary>
        /// <param name="state">The current state of the parser.</param>
        /// <param name="windowSize">The size of the window used for feature extraction.</param>
        /// <param name="command">The command to be used for generating the instance.</param>
        /// <returns>The generated {@link Instance} object.</returns>
        public abstract Instance Generate(State state, int windowSize, string command);

        /// <summary>
        /// Adds an attribute for a specific feature type of a given word to the list of attributes.
        /// </summary>
        /// <param name="word">The word whose feature value is used to create the attribute.</param>
        /// <param name="attributes">The list of attributes to which the new attribute will be added.</param>
        /// <param name="featureType">The type of the feature to be extracted from the word.</param>
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

        /// <summary>
        /// Adds a set of default (empty) attributes to the list of attributes. These attributes represent
        /// various feature types with default "null" values.
        /// </summary>
        /// <param name="attributes">The list of attributes to which the default attributes will be added.</param>
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

        /// <summary>
        /// Adds attributes for various feature types of a given word to the list of attributes.
        /// </summary>
        /// <param name="word">The word whose feature values are used to create the attributes.</param>
        /// <param name="attributes">The list of attributes to which the new attributes will be added.</param>
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