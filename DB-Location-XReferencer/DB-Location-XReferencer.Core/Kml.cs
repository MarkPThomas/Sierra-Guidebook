// ***********************************************************************
// Assembly         : DB-Location-XReferencer.Core
// Author           : Mark Thomas
// Created          : 10-04-2017
//
// Last Modified By : Mark Thomas
// Last Modified On : 10-13-2017
// ***********************************************************************
// <copyright file="Kml.cs" company="">
//     Copyright ©  2017
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpKml.Base;
using SharpKml.Engine;
using Dom = SharpKml.Dom;

namespace DB_Location_XReferencer.Core
{
    /// <summary>
    /// Reads and writes data to KML files.
    /// </summary>
    public static class Kml
    {
        /// <summary>
        /// Opens the KML file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>KmlFile.</returns>
        public static KmlFile OpenFile(string filePath)
        {
            KmlFile file;
            try
            {
                using (FileStream stream = File.Open(filePath, FileMode.Open))
                {
                    file = KmlFile.Load(stream);
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex.GetType() + "\n" + ex.Message);
                return null;
            }

            if (file.Root != null) return file;
            DisplayError("Unable to find any recognized Kml in the specified file.");
            return null;
        }

        /// <summary>
        /// Reads the region types into a list of region data objects.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>List&lt;DBRegion&gt;.</returns>
        public static List<Region> ReadRegions(KmlFile file)
        {
            Dom.Kml kml = file.Root as Dom.Kml;
            if (kml == null)
            {
                DisplayError("Unable to find any recognized Kml root in the specified kml file.");
                return null;
            }
            List<Dom.Placemark> placemarks = new List<Dom.Placemark>();
            ExtractPlacemarks(kml.Feature, placemarks);

            // Sort using their names
            placemarks.Sort((a, b) => string.CompareOrdinal(a.Name, b.Name));

            return (from placemark in placemarks
                    let polygon = placemark.Geometry as Dom.Polygon
                    where polygon != null
                    let coordinateCollection = polygon.OuterBoundary.LinearRing.Coordinates
                    select new Region(placemark.Name, coordinateCollection)).ToList();
        }

        // WriteMatchesToKml
        /// <summary>
        /// Writes the list of regions to a KML file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="dbRegions">The database regions.</param>
        public static void WriteToKml(string filePath, List<Region> dbRegions)
        {
            // This is the root element of the file
            Dom.Kml kml = AssembleToKml(dbRegions);

            WriteToKml(filePath, kml);
        }

        /// <summary>
        /// Writes regions folder with locations to a KML file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="dbRegion">The database region.</param>
        public static void WriteToKml(string filePath, Region dbRegion)
        {
            // This is the root element of the file
            Dom.Kml kml = AssembleToKml(dbRegion);

            WriteToKml(filePath, kml);
        }

        /// <summary>
        /// Writes location to a KML file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="dbLocation">The database location.</param>
        public static void WriteToKml(string filePath, FormationMatcher dbLocation)
        {
            // This is the root element of the file
            Dom.Kml kml = AssembleToKml(dbLocation);

            WriteToKml(filePath, kml);
        }

        /// <summary>
        /// Writes the KML to a KML file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="kml">The KML object.</param>
        public static void WriteToKml(string filePath, Dom.Kml kml)
        {
            using (FileStream file = File.Create(filePath))
            {
                Serializer serializer = new Serializer();
                serializer.Serialize(kml, file);
            }
        }



        /// <summary>
        /// Assembles the list of regions to a KML file.
        /// </summary>
        /// <param name="dbRegions">The database regions.</param>
        /// <returns>Dom.Kml.</returns>
        public static Dom.Kml AssembleToKml(List<Region> dbRegions)
        {
            // This is the root element of the file
            Dom.Kml kml = new Dom.Kml {Feature = AssembleToFolder(dbRegions)};
            
            return kml;
        }

        /// <summary>
        /// Assembles region folder with locations to a KML file.
        /// </summary>
        /// <param name="dbRegion">The database region.</param>
        /// <returns>Dom.Kml.</returns>
        public static Dom.Kml AssembleToKml(Region dbRegion)
        {
            // This is the root element of the file
            Dom.Kml kml = new Dom.Kml {Feature = AssembleToFolder(dbRegion)};
            
            return kml;
        }

        /// <summary>
        /// Assembles location to KML.
        /// </summary>
        /// <param name="dbLocation">The database location.</param>
        /// <returns>Dom.Kml.</returns>
        public static Dom.Kml AssembleToKml(FormationMatcher dbLocation)
        {
            // This is the root element of the file
            Dom.Kml kml = new Dom.Kml {Feature = AssembleToFolder(dbLocation)};
            
            return kml;
        }



        /// <summary>
        /// Assembles list of regions to folder.
        /// </summary>
        /// <param name="dbRegions">The database regions.</param>
        /// <returns>Dom.Folder.</returns>
        private static Dom.Folder AssembleToFolder(List<Region> dbRegions)
        {
            Dom.Folder regions = new Dom.Folder {Name = "Regions"};

            foreach (Region region in dbRegions)
            {
                Dom.Folder regionFolder = AssembleToFolder(region);
                regions.AddFeature(regionFolder);
            }
            return regions;
        }

        /// <summary>
        /// Assembles region folder with locations.
        /// </summary>
        /// <param name="dbRegion">The database region.</param>
        /// <returns>Dom.Folder.</returns>
        private static Dom.Folder AssembleToFolder(Region dbRegion)
        {
            Dom.Folder region = new Dom.Folder {Name = dbRegion.Name};

            foreach (FormationMatcher location in dbRegion.Locations)
            {
                Dom.Folder locationFolder = AssembleToFolder(location);
                region.AddFeature(locationFolder);
            }
            return region;
        }

        /// <summary>
        /// Assembles location to folder.
        /// </summary>
        /// <param name="dbLocation">The database location.</param>
        /// <returns>Dom.Folder.</returns>
        private static Dom.Folder AssembleToFolder(FormationMatcher dbLocation)
        {
            Dom.Folder locationGroup = new Dom.Folder {Name = dbLocation.FormationName};
            if (!string.IsNullOrWhiteSpace(dbLocation.SubFormationName))
            {
                locationGroup.Name += " (" + dbLocation.SubFormationName + ")";
            }
            Location location = dbLocation.MatchedLocation;
            if (location == null) { return locationGroup; }

            // This will be used for the placemark
            Dom.Point point = new Dom.Point { Coordinate = new Vector(location.Latitude, location.Longitude) };

            Dom.Placemark placemark = new Dom.Placemark { Name = location.Name };
            if (!string.IsNullOrWhiteSpace(location.OtherName))
            {
                placemark.Name += " (" + location.OtherName + ")";
            }
            placemark.Geometry = point;

            locationGroup.AddFeature(placemark);
            //foreach (PeakbaggerLocation location in dbLocation.PossibleLocations)
            //{
            //    // This will be used for the placemark
            //    Dom.Point point = new Dom.Point {Coordinate = new Vector(location.Latitude, location.Longitude)};

            //    Dom.Placemark placemark = new Dom.Placemark {Name = location.Name};
            //    if (!string.IsNullOrWhiteSpace(location.OtherName))
            //    {
            //        placemark.Name += " (" + location.OtherName + ")";
            //    }
            //    placemark.Geometry = point;

            //    locationGroup.AddFeature(placemark);
            //}
            return locationGroup;

            // Add to the root
            //kml.Feature = locationGroup;

            //WriteToKml(filePath, kml);
            //Console.WriteLine(serializer.Xml);
        }



        /// <summary>
        /// Displays the error.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void DisplayError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
        }

        /// <summary>
        /// Extracts the placemarks from the feature.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <param name="placemarks">The list to add the placemark to.</param>
        private static void ExtractPlacemarks(Dom.Feature feature, List<Dom.Placemark> placemarks)
        {
            // Is the passed in value a Placemark?
            Dom.Placemark placemark = feature as Dom.Placemark;
            if (placemark != null)
            {
                placemarks.Add(placemark);
            }
            else
            {
                // Is it a Container, as the Container might have a child Placemark?
                Dom.Container container = feature as Dom.Container;
                if (container == null) return;
                // Check each Feature to see if it's a Placemark or another Container
                foreach (var f in container.Features)
                {
                    ExtractPlacemarks(f, placemarks);
                }
            }
        }
    }
}
