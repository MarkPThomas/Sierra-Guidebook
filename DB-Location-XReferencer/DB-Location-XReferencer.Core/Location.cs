// ***********************************************************************
// Assembly         : DB-Location-XReferencer.Core
// Author           : Mark Thomas
// Created          : 10-04-2017
//
// Last Modified By : Mark Thomas
// Last Modified On : 12-02-2017
// ***********************************************************************
// <copyright file="PeakbaggerLocation.cs" company="">
//     Copyright ©  2017
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace DB_Location_XReferencer.Core
{
    /// <summary>
    /// Represents a location.
    /// </summary>
    public class Location
    {
        /// <summary>
        /// Gets the name of the location.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; }

        /// <summary>
        /// Gets the alternative name.
        /// </summary>
        /// <value>The name of the other.</value>
        public string OtherName { get; }

        /// <summary>
        /// Gets the elevation.
        /// </summary>
        /// <value>The elevation.</value>
        public int? Elevation { get; }

        /// <summary>
        /// Gets the latitude.
        /// </summary>
        /// <value>The latitude.</value>
        public double Latitude { get; }

        /// <summary>
        /// Gets the longitude.
        /// </summary>
        /// <value>The longitude.</value>
        public double Longitude { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Location"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        /// <param name="otherName">Alternative name.</param>
        /// <param name="elevation">The elevation.</param>
        public Location(string name, 
            double latitude, 
            double longitude, 
            string otherName = "", 
            int elevation = 0)
        {
            Name = name;
            OtherName = otherName;
            if (elevation > 0)
            {
                Elevation = elevation;
            }
            Latitude = latitude;
            Longitude = longitude;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(Name))
            {
                return base.ToString();
            }
            if (string.IsNullOrEmpty(OtherName))
            {
                return Name;
            }
            return Name + " (" + OtherName + ")";
        }
    }
}
