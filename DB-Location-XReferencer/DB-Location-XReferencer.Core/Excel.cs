// ***********************************************************************
// Assembly         : DB-Location-XReferencer.Core
// Author           : Mark Thomas
// Created          : 10-04-2017
//
// Last Modified By : Mark Thomas
// Last Modified On : 12-01-2017
// ***********************************************************************
// <copyright file="Excel.cs" company="">
//     Copyright ©  2017
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

using MPT.Excel;

namespace DB_Location_XReferencer.Core
{
    /// <summary>
    /// Reads the location data from the specified Excel files in order to return a list of data objects representing the locations.
    /// </summary>
    public static class Excel
    {
        /// <summary>
        /// Opens the Excel file.
        /// </summary>
        /// <param name="filePath">The file path to the Excel file.</param>
        /// <returns>ExcelHelper.</returns>
        public static ExcelHelper OpenFile(string filePath)
        {
            return new ExcelHelper(filePath);
        }

        /// <summary>
        /// Reads all of the database locations and returns a list of objects with data.
        /// </summary>
        /// <param name="excel">The excel application.</param>
        /// <returns>List&lt;DBLocation&gt;.</returns>
        public static List<FormationMatcher> ReadDBLocations(ExcelHelper excel)
        {
            List<FormationMatcher> locations = new List<FormationMatcher>();
            List<string> areas = excel.RangeValuesBelowHeader("AreaChild");
            List<string> names = excel.RangeValuesBelowHeader("Formation");
            int numberOfRows = names.Count;
            List<string> latitudes = excel.RangeValuesBelowHeader("Latitude", includeNull: true, numberOfRows: numberOfRows);
            List<string> longitudes = excel.RangeValuesBelowHeader("Longitude", includeNull: true, numberOfRows: numberOfRows);
            List<string> otherNames = excel.RangeValuesBelowHeader("SubFormation", includeNull: true, numberOfRows: numberOfRows);

            for (int i = 0, length = names.Count; i < length; i++)
            {
                double longitude;
                if (!double.TryParse(longitudes[i], out longitude))
                {
                    longitude = 0;
                }
                double latitude;
                if (!double.TryParse(latitudes[i], out latitude))
                {
                    latitude = 0;
                }
                if (Math.Abs(longitude) < 1 && Math.Abs(latitude) < 1)
                {
                    FormationMatcher location = new FormationMatcher(areas[i], names[i], otherNames[i]);
                    locations.Add(location);
                }
            }
            return locations;
        }

        // TODO: Hande sub-formations
        /// <summary>
        /// Reads the peakbagger locations that lie within the latitude/longitude bounds given and returns a list of objects with data.
        /// </summary>
        /// <param name="excel">The Excel application.</param>
        /// <param name="maxLatitude">The maximum latitude.</param>
        /// <param name="minLatitude">The minimum latitude.</param>
        /// <param name="maxLongitude">The maximum longitude.</param>
        /// <param name="minLongitude">The minimum longitude.</param>
        /// <returns>List&lt;PeakbaggerLocation&gt;.</returns>
        public static List<Formation> ReadPeakbaggerLocations(
            ExcelHelper excel, 
            double maxLatitude = 0,
            double minLatitude = 0,
            double maxLongitude = 0,
            double minLongitude = 0)
        {
            List<Formation> locations = new List<Formation>();
            List<string> names = excel.RangeValuesBelowHeader("peakName");
            List<string> latitudes = excel.RangeValuesBelowHeader("peakLatitude");
            List<string> longitudes = excel.RangeValuesBelowHeader("peakLongitude");
            int numberOfRows = names.Count;
            List<string> elevations = excel.RangeValuesBelowHeader("elevationFt", includeNull: true, numberOfRows: numberOfRows);
            List<string> otherNames = excel.RangeValuesBelowHeader("peakNameAlt", includeNull: true, numberOfRows: numberOfRows);

            
            for (int i = 0, length = latitudes.Count; i < length; i++)
            {
                double longitude;
                if (!double.TryParse(longitudes[i], out longitude)) { continue; }
                double latitude;
                if (!double.TryParse(latitudes[i], out latitude)) { continue; }
                if ((minLongitude <= longitude && longitude <= maxLongitude) &&
                    (minLatitude <= latitude && latitude <= maxLatitude))
                {
                    int elevation;
                    int.TryParse(elevations[i], out elevation);
                    Formation location = new Formation(names[i], latitude, longitude, otherNames[i], elevation);
                    locations.Add(location);
                }
            }
            return locations;
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
    }
}
