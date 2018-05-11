// ***********************************************************************
// Assembly         : DB-Location-XReferencer.Core
// Author           : Mark Thomas
// Created          : 10-04-2017
//
// Last Modified By : Mark Thomas
// Last Modified On : 12-01-2017
// ***********************************************************************
// <copyright file="DBRegion.cs" company="">
//     Copyright ©  2017
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using SharpKml.Base;

namespace DB_Location_XReferencer.Core
{
    /// <summary>
    /// Represents a geographical region.
    /// </summary>
    public class Region
    {
        /// <summary>
        /// The tolerance used for comparing locations.
        /// </summary>
        private const double TOLERANCE = 1E-10;

        /// <summary>
        /// The name of the region.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; }

        /// <summary>
        /// The boundary coordinates of the region.
        /// </summary>
        /// <value>The boundary.</value>
        public IEnumerable<Vector> Boundary { get; }

        /// <summary>
        /// The extents bounding box of the region.
        /// </summary>
        /// <value>The extents.</value>
        public Extents Extents { get; private set; }

        /// <summary>
        /// Gets the locations within the region.
        /// </summary>
        /// <value>The locations.</value>
        public List<FormationMatcher> Locations { get; } = new List<FormationMatcher>();

        /// <summary>
        /// Gets the locations base.
        /// </summary>
        /// <value>The locations base.</value>
        public List<Location> LocationsBase { get; } = new List<Location>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Region"/> class.
        /// </summary>
        /// <param name="regionName">Name of the region.</param>
        /// <param name="regionBoundary">The region boundary.</param>
        public Region(string regionName, IEnumerable<Vector> regionBoundary)
        {
            Name = regionName;
            Boundary = regionBoundary;
            setExtents();
        }

        /// <summary>
        /// Adds the locations to the region if the region names match.
        /// </summary>
        /// <param name="locations">The locations.</param>
        public void AddLocationsByRegionName(IEnumerable<FormationMatcher> locations)
        {
            foreach (FormationMatcher location in locations)
            {
                AddLocationByRegionName(location);
            }
        }

        /// <summary>
        /// Adds the location to the region if the region name matches.
        /// </summary>
        /// <param name="location">The location.</param>
        public void AddLocationByRegionName(FormationMatcher location)
        {
            if (string.Compare(location.RegionName, Name, StringComparison.Ordinal) == 0)
            {
                Locations.Add(location);
            }
        }


        /// <summary>
        /// Adds the locations to the region if the coordinates lie within the region shape.
        /// </summary>
        /// <param name="locations">The locations.</param>
        public void AddLocationsByCoordinates(IEnumerable<Location> locations)
        {
            foreach (Location location in locations)
            {
                AddLocationByCoordinates(location);
            }
        }

        /// <summary>
        /// Adds the location to the region if the coordinates lie within the region shape.
        /// </summary>
        /// <param name="location">The location.</param>
        public void AddLocationByCoordinates(Location location)
        {
            if (isWithinExtents(location) &&
                isWithinShape(location))
            {
                LocationsBase.Add(location);
            }
        }


        /// <summary>
        /// Merges the locations.
        /// </summary>
        public void MergeLocations()
        {
            foreach (Location potentialLocation in LocationsBase)
            {
                foreach (FormationMatcher location in Locations)
                {
                    location.AddIfPotentialMatchByName(potentialLocation);
                }
            }
        }

        /// <summary>
        /// Condenses the potential matches.
        /// </summary>
        public void CondensePotentialMatches()
        {
            foreach (FormationMatcher location in Locations)
            {
                location.CondensePotentialMatches();
            }
        }




        /// <summary>
        /// Sets the extents.
        /// </summary>
        private void setExtents()
        {
            Extents = Extents.GetExtents(Boundary);
        }

        /// <summary>
        /// Determines whether the specified location is within extents.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns><c>true</c> if the specified location is within extents; otherwise, <c>false</c>.</returns>
        private bool isWithinExtents(Location location)
        {
            return Extents.IsWithinExtents(location.Latitude, location.Longitude);
        }
        
        #region Point Lies Within Shape

        /// <summary>
        /// Determines whether the specified location is within the shape.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns><c>true</c> if the specified location is within the shape; otherwise, <c>false</c>.</returns>
        private bool isWithinShape(Location location)
        {
            // 3. If # intersections%2 == 0 (even) => point is outside.
            //    If # intersections%2 == 1 (odd) => point is inside.
            // Note: Condition of vertex intersection (# == 1) is not handled, so is treated as inside by default.
            return (numberOfIntersections(location.Longitude, location.Latitude) % 2 == 1);
        }

        /// <summary>
        /// The numbers of shape boundary intersections a horizontal line makes when projecting to the right from the provided point.
        /// </summary>
        /// <param name="xPtN">The x-coordinate of pt n.</param>
        /// <param name="yPtN">The y-coordinate of pt n.</param>
        /// <returns>System.Int32.</returns>
        private int numberOfIntersections(double xPtN, double yPtN)
        {
            // 1. Check horizontal line projection from a pt. n to the right
            // 2. Count # of intersections of the line with shape edges   

            Vector[] boundaryCoordinates = Boundary.ToArray();
            // Note shape coordinates from XML already repeat starting node as an ending node.
            // No need to handle wrap-around below.

            int numberOfIntersections = 0;
            for (int i = 0; i < boundaryCoordinates.Length - 1; i++)
            {
                Vector ptI = boundaryCoordinates[i];
                if (pointLiesOnVertex(xPtN, yPtN, ptI)) { return 1; }

                Vector ptJ = boundaryCoordinates[i + 1];

                if (segmentIsHorizontal(ptI, ptJ))
                {
                    // Segment would be parallel to line projection.
                    continue;
                }
                if (!pointIsLeftOfSegmentEnd(xPtN, ptI, ptJ))
                {
                    // Pt is to the right of the segment.
                    continue;
                }
                if (!pointIsWithinSegmentHeight(yPtN, ptI, ptJ))
                {
                    // Point is out of vertical bounds of the segment extents.
                    continue;
                }
                if (segmentIsVertical(ptI, ptJ))
                {
                    numberOfIntersections++;
                    continue;
                }
                double xIntersection = intersectionCoordinateX(yPtN, ptI, ptJ);
                if (pointIsLeftOfSegmentIntersection(xPtN, xIntersection))
                {
                    numberOfIntersections++;
                }
            }
            return numberOfIntersections;
        }

        /// <summary>
        /// Determines if the point lies on the shape vertex.
        /// </summary>
        /// <param name="xPtN">The x-coordinate of pt n.</param>
        /// <param name="yPtN">The y-coordinate of pt n.</param>
        /// <param name="ptI">The vertex i.</param>
        /// <returns><c>true</c> if the point lies on the shape vertex, <c>false</c> otherwise.</returns>
        private bool pointLiesOnVertex(double xPtN, double yPtN, Vector ptI)
        {
            return ((Math.Abs(xPtN - ptI.Longitude) < TOLERANCE && Math.Abs(yPtN - ptI.Latitude) < TOLERANCE));
        }

        /// <summary>
        /// Determines if the shape segment is horizontal.
        /// </summary>
        /// <param name="ptI">The vertex i.</param>
        /// <param name="ptJ">The vertex j.</param>
        /// <returns><c>true</c> if the shape segment is horizontal, <c>false</c> otherwise.</returns>
        private bool segmentIsHorizontal(Vector ptI, Vector ptJ)
        {
            return Math.Abs(ptI.Latitude - ptJ.Latitude) < TOLERANCE;
        }

        /// <summary>
        /// Determines if the segment is vertical.
        /// </summary>
        /// <param name="ptI">The vertex i.</param>
        /// <param name="ptJ">The vertex j.</param>
        /// <returns><c>true</c> if the segment is vertical, <c>false</c> otherwise.</returns>
        private bool segmentIsVertical(Vector ptI, Vector ptJ)
        {
            return Math.Abs(ptI.Longitude - ptJ.Longitude) < TOLERANCE;
        }

        /// <summary>
        /// Determines if the point lies within the segment height.
        /// </summary>
        /// <param name="yPtN">The y-coordinate of pt n.</param>
        /// <param name="ptI">The vertex i.</param>
        /// <param name="ptJ">The vertex j.</param>
        /// <returns><c>true</c> if the point lies within the segment height, <c>false</c> otherwise.</returns>
        private bool pointIsWithinSegmentHeight(double yPtN, Vector ptI, Vector ptJ)
        {
            return (Math.Min(ptI.Latitude, ptJ.Latitude) <= yPtN && yPtN <= Math.Max(ptI.Latitude, ptJ.Latitude));
        }

        /// <summary>
        /// Determines if the point lies to the left of the segment end.
        /// </summary>
        /// <param name="xPtN">The x-coordinate of pt n.</param>
        /// <param name="ptI">Vertex i.</param>
        /// <param name="ptJ">Vertex j.</param>
        /// <returns><c>true</c> if the point lies to the left of the segment end, <c>false</c> otherwise.</returns>
        private bool pointIsLeftOfSegmentEnd(double xPtN, Vector ptI, Vector ptJ)
        {
            return xPtN <= Math.Max(ptI.Longitude, ptJ.Longitude);
        }

        /// <summary>
        /// The x-coordinate of the intersection of the horizontally projected line with the provided segment.
        /// </summary>
        /// <param name="yPtN">The y-coordinate of pt n, where the projection starts.</param>
        /// <param name="ptI">Vertex i of the segment.</param>
        /// <param name="ptJ">Vertex j of the segment.</param>
        /// <returns>System.Double.</returns>
        private double intersectionCoordinateX(double yPtN, Vector ptI, Vector ptJ)
        {
            return (yPtN - ptI.Latitude) * ((ptJ.Longitude - ptI.Longitude) / (ptJ.Latitude - ptI.Latitude)) + ptI.Longitude;
        }

        /// <summary>
        /// Determines if the point is to the left of the horizontally projected segment intersection.
        /// </summary>
        /// <param name="xPtN">The x-coordinate of pt n.</param>
        /// <param name="xIntersection">The x-coordinate of the intersection of the projected line.</param>
        /// <returns><c>true</c> if the point is to the left of the horizontally projected segment intersection, <c>false</c> otherwise.</returns>
        private bool pointIsLeftOfSegmentIntersection(double xPtN, double xIntersection)
        {
            return xPtN <= xIntersection;
        }
        #endregion


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
            return Name;
        }
    }
}

