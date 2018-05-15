// ***********************************************************************
// Assembly         : DB-Location-XReferencer.Core
// Author           : Mark Thomas
// Created          : 10-10-2017
//
// Last Modified By : Mark Thomas
// Last Modified On : 12-01-2017
// ***********************************************************************
// <copyright file="Extents.cs" company="">
//     Copyright ©  2017
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using SharpKml.Base;

namespace DB_Location_XReferencer.Core
{
    /// <summary>
    /// Represents the latitude/longitude bounds of a shape or line.
    /// </summary>
    public class Extents
    {
        /// <summary>
        /// The maximum latitude
        /// </summary>
        private const double _maxLatitude = -90;
        /// <summary>
        /// The minimum latitude
        /// </summary>
        private const double _minLatitude = 90;
        /// <summary>
        /// The maximum longitude
        /// </summary>
        private const double _maxLongitude = -180;
        /// <summary>
        /// The minimum longitude
        /// </summary>
        private const double _minLongitude = 180;

        /// <summary>
        /// Gets the maximum latitude.
        /// </summary>
        /// <value>The maximum latitude.</value>
        public double MaxLatitude { get; private set; } = _maxLatitude;
        /// <summary>
        /// Gets the minimum latitude.
        /// </summary>
        /// <value>The minimum latitude.</value>
        public double MinLatitude { get; private set; } = _minLatitude;
        /// <summary>
        /// Gets the maximum longitude.
        /// </summary>
        /// <value>The maximum longitude.</value>
        public double MaxLongitude { get; private set; } = _maxLongitude;

        /// <summary>
        /// Gets the minimum longitude.
        /// </summary>
        /// <value>The minimum longitude.</value>
        public double MinLongitude { get; private set; } = _minLongitude;


        /// <summary>
        /// Updates the extents to include the specified coordinate.
        /// </summary>
        /// <param name="coordinate">The coordinate.</param>
        public void Add(Vector coordinate)
        {
            if (coordinate.Latitude > MaxLatitude)
            {
                MaxLatitude = Math.Min(coordinate.Latitude, _minLatitude);
            }
            if (coordinate.Latitude < MinLatitude)
            {
                MinLatitude = Math.Max(coordinate.Latitude, _maxLatitude);
            }

            if (coordinate.Longitude > MaxLongitude)
            {
                MaxLongitude = Math.Min(coordinate.Longitude, _minLongitude);
            }
            if (coordinate.Longitude < MinLongitude)
            {
                MinLongitude = Math.Max(coordinate.Longitude, _maxLongitude);
            }
        }

        /// <summary>
        /// Updates the extents to include the specified coordinates.
        /// </summary>
        /// <param name="coordinates">The coordinates.</param>
        public void Add(IEnumerable<Vector> coordinates)
        {
            foreach (Vector coordinate in coordinates)
            {
                Add(coordinate);
            }
        }

        /// <summary>
        /// Updates the extents to include the specified extents.
        /// </summary>
        /// <param name="extents">The extents.</param>
        public void Add(Extents extents)
        {
            if (extents.MaxLatitude > MaxLatitude)
            {
                MaxLatitude = extents.MaxLatitude;
            }
            if (extents.MinLatitude < MinLatitude)
            {
                MinLatitude = extents.MinLatitude;
            }
            if (extents.MaxLongitude > MaxLongitude)
            {
                MaxLongitude = extents.MaxLongitude;
            }
            if (extents.MinLongitude < MinLongitude)
            {
                MinLongitude = extents.MinLongitude;
            }
        }

        /// <summary>
        /// Determines whether the coordinate lies within the extents.
        /// </summary>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        /// <returns><c>true</c> if the specified coordinates are within the extents; otherwise, <c>false</c>.</returns>
        public bool IsWithinExtents(double latitude,
            double longitude)
        {
            return ((MinLatitude <= latitude && latitude <= MaxLatitude) &&
                    (MinLongitude <= longitude && longitude <= MaxLongitude));
        }

        /// <summary>
        /// Returns a new extents object.
        /// </summary>
        /// <param name="coordinates">The coordinates.</param>
        /// <returns>Extents.</returns>
        public static Extents GetExtents(IEnumerable<Vector> coordinates)
        {
            Extents extents = new Extents();
            extents.Add(coordinates);
            return extents;
        }
    }
}
