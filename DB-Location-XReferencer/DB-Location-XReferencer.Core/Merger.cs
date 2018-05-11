// ***********************************************************************
// Assembly         : DB-Location-XReferencer.Core
// Author           : Mark Thomas
// Created          : 10-05-2017
//
// Last Modified By : Mark Thomas
// Last Modified On : 12-02-2017
// ***********************************************************************
// <copyright file="Merger.cs" company="">
//     Copyright ©  2017
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using SharpKml.Engine;
using MPT.Excel;

namespace DB_Location_XReferencer.Core
{
    /// <summary>
    /// Class Merger.
    /// </summary>
    public static class Merger
    {

        /// <summary>
        /// Collates the locations and regions.
        /// </summary>
        /// <param name="filePathKml">The file path KML.</param>
        /// <param name="filePathExcelDb">The file path excel database.</param>
        /// <param name="filePathExcelPeakbagger">The file path excel peakbagger.</param>
        /// <param name="filePathExcelDbCsv">The file path excel database CSV.</param>
        /// <param name="filePathExcelPeakbaggerCsv">The file path excel peakbagger CSV.</param>
        /// <param name="filePathExcelPeakbaggerCsvSource">The file path excel peakbagger CSV source.</param>
        /// <returns>List&lt;Region&gt;.</returns>
        public static List<Region> CollateLocationsAndRegions(string filePathKml, 
            string filePathExcelDb = "", 
            string filePathExcelPeakbagger = "",
            string filePathExcelDbCsv = "",
            string filePathExcelPeakbaggerCsv = "",
            string filePathExcelPeakbaggerCsvSource = "")
        {
            // Get regions
            KmlFile file = Kml.OpenFile(filePathKml);
            List<Region> regions = Kml.ReadRegions(file);

            // Get the extents of all regions
            Extents regionsExtents = new Extents();
            foreach (Region region in regions)
            {
                regionsExtents.Add(region.Extents);
            }


            // Get database locations
            List<FormationMatcher> locations;
            if (string.IsNullOrEmpty(filePathExcelDbCsv))
            {
                using (ExcelHelper excelDb = Excel.OpenFile(filePathExcelDb))
                {
                    locations = Excel.ReadDBLocations(excelDb);
                }
                
                string filePath = Path.GetFileNameWithoutExtension(filePathExcelDb) + ".csv";
                WriteToCsv(filePath, locations);
            }
            else
            {
                ReadFromCsv(filePathExcelDbCsv, out locations);
            }
            
            // Add locations to the appropriate region
            foreach (Region region in regions)
            {
                region.AddLocationsByRegionName(locations);
            }

            // Get peakbagger locations
            List<Formation> locationsPeakbagger = 
                    GetPeakbaggerLocations(regionsExtents,
                                            filePathExcelPeakbagger,
                                            filePathExcelPeakbaggerCsv,
                                            filePathExcelPeakbaggerCsvSource);

            // Add locations to the appropriate region
            foreach (Region region in regions)
            {
                region.AddLocationsByCoordinates(locationsPeakbagger);
                region.MergeLocations();
                region.CondensePotentialMatches();
            }

            return regions;
        }

        /// <summary>
        /// Gets the peakbagger locations.
        /// </summary>
        /// <param name="regionsExtents">The extents of all regions of interest for which formations are retrieved.</param>
        /// <param name="filePathExcelSource">The file path excel source.</param>
        /// <param name="filePathCSV">The file path CSV.</param>
        /// <param name="filePathCSVSource">The file path CSV source.</param>
        /// <returns>List&lt;Location&gt;.</returns>
        public static List<Formation> GetPeakbaggerLocations(Extents regionsExtents,
            string filePathExcelSource, 
            string filePathCSV = "", 
            string filePathCSVSource = "")
        {
            List<Formation> locationsPeakbagger;
            if (string.IsNullOrEmpty(filePathCSV))
            {
                if (string.IsNullOrEmpty(filePathCSVSource))
                {
                    using (ExcelHelper excelPeakbagger = Excel.OpenFile(filePathExcelSource))
                    {
                        locationsPeakbagger = Excel.ReadPeakbaggerLocations(excelPeakbagger,
                            regionsExtents.MaxLatitude,
                            regionsExtents.MinLongitude,
                            regionsExtents.MaxLongitude,
                            regionsExtents.MinLongitude);
                    }
                }
                else
                {
                    locationsPeakbagger = ReadPeakbaggerLocationsFromCsv(filePathCSVSource, 
                                            regionsExtents.MaxLatitude,
                                            regionsExtents.MinLongitude,
                                            regionsExtents.MaxLongitude,
                                            regionsExtents.MinLongitude);
                }
                string filePath = Path.GetFileNameWithoutExtension(filePathExcelSource) + ".csv";
                WriteToCsv(filePath, locationsPeakbagger);
            }
            else
            {
                ReadFromCsv(filePathCSV, out locationsPeakbagger);
            }
            return locationsPeakbagger;
        }



        /// <summary>
        /// Opens the in google earth.
        /// </summary>
        /// <param name="dbLocation">The database location.</param>
        /// <param name="pathFile">The path file.</param>
        /// <param name="pathApplication">The path application.</param>
        public static void OpenInGoogleEarth(FormationMatcher dbLocation, string pathFile, string pathApplication)
        {
            Kml.WriteToKml(pathFile, dbLocation);

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            // /C Carries out the command specified by string and then terminates
            startInfo.Arguments = "/C \"" + pathApplication + "\" \"" + pathFile + "\"";
            process.StartInfo = startInfo;
            process.Start();
        }



        /// <summary>
        /// Writes formations to a CSV file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="formations">The formations.</param>
        public static void WriteToCsv(string filePath,
            List<FormationMatcher> formations)
        {
            using (TextWriter writer = File.CreateText(filePath))
            {
                var csv = new CsvWriter(writer);
                csv.WriteRecords(formations);
            }
        }

        /// <summary>
        /// Reads formations from a CSV file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="formations">The formations.</param>
        public static void ReadFromCsv(string filePath,
            out List<FormationMatcher> formations)
        {
            using (TextReader reader = File.OpenText(filePath))
            {
                var csv = new CsvReader(reader);
                csv.Configuration.PrepareHeaderForMatch = header =>
                    CultureInfo.CurrentCulture.TextInfo.ToTitleCase(header);
                var results = csv.GetRecords<FormationMatcher>();
                formations = new List<FormationMatcher>(results);
            }   
        }


        /// <summary>
        /// Writes formations to a CSV file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="formations">The formations.</param>
        public static void WriteToCsv(string filePath,
            List<Formation> formations)
        {

            using (TextWriter writer = File.CreateText(filePath))
            {
                var csv = new CsvWriter(writer);
                csv.WriteRecords(formations);
            }
        }

        /// <summary>
        /// Reads formations from a CSV file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="formations">The formations.</param>
        public static void ReadFromCsv(string filePath,
            out List<Formation> formations)
        {
            using (TextReader reader = File.OpenText(filePath))
            {
                var csv = new CsvReader(reader);
                csv.Configuration.PrepareHeaderForMatch = header =>
                    CultureInfo.CurrentCulture.TextInfo.ToTitleCase(header);
                var results = csv.GetRecords<Formation>();
                formations = new List<Formation>(results);
            }
        }

        // TODO: Read subformation
        /// <summary>
        /// Reads the www.peakbagger.com locations from the CSV file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="maxLatitude">The maximum latitude from which locations are to be read.</param>
        /// <param name="minLatitude">The minimum latitude from which locations are to be read.</param>
        /// <param name="maxLongitude">The maximum longitude from which locations are to be read.</param>
        /// <param name="minLongitude">The minimum longitude from which locations are to be read.</param>
        /// <returns>List&lt;Location&gt;.</returns>
        public static List<Formation> ReadPeakbaggerLocationsFromCsv(
            string filePath,
            double maxLatitude = 0,
            double minLatitude = 0,
            double maxLongitude = 0,
            double minLongitude = 0)
        {
            List<Formation> locations = new List<Formation>();
            using (TextReader reader = File.OpenText(filePath))
            {
                var csv = new CsvReader(reader);
                var results = csv.GetRecords<PeakbaggerLocation>().ToList();
                
                foreach (PeakbaggerLocation locationRaw in results)
                {
                    if (locationRaw.Longitude == null ||
                        locationRaw.Latitude == null)
                    {
                        continue;
                    }
                    double longitude = (double)locationRaw.Longitude;
                    double latitude = (double)locationRaw.Latitude;
                    if ((!(minLongitude <= longitude) || !(longitude <= maxLongitude)) ||
                        (!(minLatitude <= latitude) || !(latitude <= maxLatitude))) continue;
                    int elevation = 0;
                    if (locationRaw.Elevation != null)
                    {
                        elevation = (int) locationRaw.Elevation;
                    }
                    Formation location = new Formation(
                        locationRaw.Name, 
                        latitude, longitude,
                        locationRaw.OtherName,
                        elevation);
                    locations.Add(location);
                }
            }
            return locations;
        }

        //private sealed class PeakbaggerLocationRawMap : ClassMap<PeakbaggerLocation>
        //{
        //    public PeakbaggerLocationRawMap()
        //    {
        //        Map(m => m.Index).Name("Index");
        //        Map(m => m.Name).ConvertUsing(row => row.GetField("Name"));
        //    }
        //}
    }
}
