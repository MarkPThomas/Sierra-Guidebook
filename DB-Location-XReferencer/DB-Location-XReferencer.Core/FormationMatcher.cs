// ***********************************************************************
// Assembly         : DB-Location-XReferencer.Core
// Author           : Mark Thomas
// Created          : 10-04-2017
//
// Last Modified By : Mark Thomas
// Last Modified On : 12-02-2017
// ***********************************************************************
// <copyright file="FormationMatcher.cs" company="">
//     Copyright ©  2017
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System.Linq;
using MPT.String.Characters;
using MPT.String.Words;
using sLib = MPT.String.StringLibrary;

namespace DB_Location_XReferencer.Core
{

    /// <summary>
    /// Matches formations of known locations to those where only the name and/or region are known.
    /// </summary>
    public class FormationMatcher
    {
        /// <summary>
        /// The name of the region that the formation lies within.
        /// </summary>
        /// <value>The name of the region.</value>
        public string RegionName { get; }

        /// <summary>
        /// The name of the formation.
        /// </summary>
        /// <value>The name of the formation.</value>
        public string FormationName { get; }

        /// <summary>
        /// The name of the sub-formation.
        /// </summary>
        /// <value>The name of the sub formation.</value>
        public string SubFormationName { get; }


        /// <summary>
        /// The matched location.
        /// </summary>
        /// <value>The matched location.</value>
        public Location MatchedLocation { get; private set; }

        /// <summary>
        /// The possible locations.
        /// </summary>
        /// <value>The possible locations.</value>
        public List<Location> PossibleLocations { get; } = new List<Location>();


        /// <summary>
        /// Initializes a new instance of the <see cref="FormationMatcher"/> class.
        /// </summary>
        /// <param name="regionName">Name of the region.</param>
        /// <param name="formationName">Name of the formation.</param>
        /// <param name="subformationName">Name of the sub-formation.</param>
        public FormationMatcher(string regionName, 
            string formationName, 
            string subformationName = "")
        {
            RegionName = regionName;
            FormationName = formationName;
            SubFormationName = subformationName;
        }


        /// <summary>
        /// Adds the location if it is a potential match based on the various formation names.
        /// </summary>
        /// <param name="location">The location.</param>
        public void AddIfPotentialMatchByName(Location location)
        {
            if (potentialMatchByName(location))
            {
                PossibleLocations.Add(location);
            }
        }

        // TODO: This can now check sub-formation names
        /// <summary>
        /// There is a potential match based on the various formation names.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns><c>true</c> if is a potential match based on the various formation names, <c>false</c> otherwise.</returns>
        private bool potentialMatchByName(Location location)
        {
            if (SubFormationName.Contains("Ericsson Crag #3") &&
                (location.Name.Contains("Ericsson Crag #3") || location.OtherName.Contains("Ericsson Crag #3")))
            {
                System.Console.WriteLine("FooBar");
            }

            // First check if any combination of one name contains the other
            if (eitherContainsOther(FormationName, location.Name) ||
                eitherContainsOther(FormationName, location.OtherName) ||
                eitherContainsOther(SubFormationName, location.Name) ||
                eitherContainsOther(SubFormationName, location.OtherName))
            return true;

            // Next, redo the check with the names singular and made non-possessive
            string formationNameNormalized = FormationName.ApplyToIndividualWords(singularNonPossessiveWord);
            string subFormationNameNormalized = SubFormationName.ApplyToIndividualWords(singularNonPossessiveWord);
            string potentialLocationNameNormalized = location.Name.ApplyToIndividualWords(singularNonPossessiveWord);
            string potentialLocationOtherNameNormalized = location.OtherName.ApplyToIndividualWords(singularNonPossessiveWord);

            return (eitherContainsOther(formationNameNormalized, potentialLocationNameNormalized) ||
                    eitherContainsOther(formationNameNormalized, potentialLocationOtherNameNormalized) ||
                    eitherContainsOther(subFormationNameNormalized, potentialLocationNameNormalized) ||
                    eitherContainsOther(subFormationNameNormalized, potentialLocationOtherNameNormalized));
        }


        /// <summary>
        /// Either string contains the other.
        /// </summary>
        /// <param name="one">One string.</param>
        /// <param name="other">The other string.</param>
        /// <returns><c>true</c> if either string contains the other, <c>false</c> otherwise.</returns>
        private bool eitherContainsOther(string one, 
            string other)
        {
            return (one.Contains(other) ||
                    other.Contains(one));
        }


        /// <summary>
        /// Selects the matching location based on the index provided.
        /// </summary>
        /// <param name="index">The index of the location chosen from <see cref="PossibleLocations"/>.</param>
        public void ChooseMatch(int index)
        {
            if (index > PossibleLocations.Count - 1) throw new System.IndexOutOfRangeException();
            MatchedLocation = PossibleLocations[index];
        }

        // TODO: Work out better what is to be done here.
        /// <summary>
        /// Condenses the potential matches if a singular match can be found.
        /// </summary>
        public void CondensePotentialMatches()
        {
            if (isSingularMatch()) { MatchedLocation = PossibleLocations[0]; }
        }

        // TODO: Work out better what is to be done here.
        /// <summary>
        /// Determines whether there is a single possible match where no user input is needed.
        /// </summary>
        /// <returns><c>true</c> if there is only one likely/possible match; otherwise, <c>false</c>.</returns>
        private bool isSingularMatch()
        {
            switch (PossibleLocations.Count)
            {
                case 0:
                    return false;
                case 1:
                    return true;
                default: // For now, let's loosely assume that the following potential matches are reliable and don't require user selection
                    // Check subformation first, as it is the lowest level for naming.
                    return probableMatchByName(!string.IsNullOrEmpty(SubFormationName) ? SubFormationName : FormationName, PossibleLocations);
            }
        }

        /// <summary>
        /// Determines whether the name has any probable match to the locations listed.
        /// This method uses stricter and more specific checks than <see cref="potentialMatchByName"/>.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="possibleLocations">The possible locations.</param>
        /// <returns><c>true</c> if the name has any possible match to the locations listed; otherwise, <c>false</c>.</returns>
        private bool probableMatchByName(string name, IEnumerable<Location> possibleLocations)
        {
            return possibleLocations.Any(location =>
                                            string.CompareOrdinal(name, location.Name) == 0 ||
                                            string.CompareOrdinal(name, location.OtherName) == 0) ||
                                                PossibleLocations.Any(location =>
                                                    string.CompareOrdinal(normalizeGeographicName(name), normalizeGeographicName(location.Name)) == 0 ||
                                                    string.CompareOrdinal(normalizeGeographicName(name), normalizeGeographicName(location.OtherName)) == 0);
        }





        /// <summary>
        /// Normalizes the geographic name, handling irregular cases such as possession, pluralization, elevations, numbering, abbreviations, similar terms, etc.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        private string normalizeGeographicName(string name)
        {
            string modifiedName = name;

            // Handle cases such as Eichorn's Pinnacle vs. Eichorns Pinnacle
            modifiedName = modifiedName.ApplyToIndividualWords(singularNonPossessiveWord);

            // Remove characters used for altitudes, e.g. Point 13,754 = Point 13754
            modifiedName = modifiedName.Replace(",", string.Empty);

            // Remove words relating to numbering, e.g. Crag #3 or Crag Number 3 = Crag 3
            modifiedName = modifiedName.Replace("#", string.Empty);
            modifiedName = modifiedName.ApplyToIndividualWords(sLib.RemoveWord, "number");

            // Remove possible character for abbreviations & acronyms, e.g. Mt. = Mt
            modifiedName = modifiedName.Replace(".", string.Empty);
            modifiedName = modifiedName.ApplyToIndividualWords(sLib.ReplaceUsingMap,
                                new Dictionary<string, string>()
                                {
                                    { "mt", "mount"},
                                    { "mtn", "mountain"},
                                    { "mtns", "mountains"},
                                    { "pk", "peak"},
                                    { "pks", "peaks"},
                                    { "pt", "point"},
                                    { "pts", "points"},
                                });

            // Substitute a consistent term for terms that are variable, such as spire vs. needle.
            modifiedName = modifiedName.ApplyToIndividualWords(sLib.ReplaceWithConstant, 
                                new List<string>()
                                {
                                    "pinnacle",
                                    "tower",
                                    "needle"
                                },
                                "spire");

            return modifiedName;
        }

        /// <summary>
        /// Makes the word singular and non-possessive.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns>System.String.</returns>
        private string singularNonPossessiveWord(string word)
        {
            string modifiedWord = word.ToLower();
            modifiedWord = modifiedWord.ToSingular();
            modifiedWord = modifiedWord.FromPossessive();

            return modifiedWord;
        }




        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(FormationName))
            {
                return base.ToString();
            }
            if (string.IsNullOrEmpty(SubFormationName))
            {
                return FormationName;
            }
            return FormationName + " (" + SubFormationName + ")";
        }
    }
}
