using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DB_Location_XReferencer.Core;

namespace DB_Location_XReferencer.Console
{
    class Program
    {
        const string googleEarthPath = @"C:\Program Files (x86)\Google\Google Earth Pro\client\googleearth.exe";
        const string excelDB = @"C:\Users\Mark\Google Drive\Sierra Guidebook\Sierra Guidebook.xlsm";
        const string excelScraper = @"C:\Users\Mark\Documents\Projects\Website\MarkPThomas-Documentation\Tools\Peakbagger Scraper.xlsm";

        static void Main(string[] args)
        {
            //testDummyPts();
            //testWithExcel();
            //testWithCSV();
            testWithCSVSource();
        }

        private static void testDummyPts()
        {
            string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (assemblyFolder == null) { return; }
            string kmlPath = Path.Combine(assemblyFolder, "testDummyPts.kml");

            FormationMatcher location = new FormationMatcher("Foo Region", "Bar Formation", "FooBar");
            Location pbLocationInside = new Location("Foo", 42.544, -121.45);
            location.AddIfPotentialMatchByName(pbLocationInside);

            Kml.WriteToKml(kmlPath, location);
            //Merger.OpenInGoogleEarth(location, kmlPath, googleEarthPath);
        }

        private static void testWithExcel()
        {
            string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (assemblyFolder == null) { return; }
            string kmlPath = Path.Combine(assemblyFolder, "testWithExcel.kml");
            string kmlInputPath = Path.Combine(assemblyFolder, "Areas.kml");

            List<Region> regions = Merger.CollateLocationsAndRegions(kmlInputPath, excelDB, excelScraper);
            Kml.WriteToKml(kmlPath, regions);
        }

        private static void testWithCSV()
        {
            string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (assemblyFolder == null) { return; }
            string kmlPath = Path.Combine(assemblyFolder, "testWithCSV.kml");
            string kmlInputPath = Path.Combine(assemblyFolder, "Areas.kml");
            string excelPBCsv = Path.Combine(assemblyFolder, "Peakbagger Scraper.csv");
            string excelDBCsv = Path.Combine(assemblyFolder, "Sierra Guidebook.csv");

            List<Region> regions = Merger.CollateLocationsAndRegions(kmlInputPath, excelDB, excelScraper, excelDBCsv, excelPBCsv);
            Kml.WriteToKml(kmlPath, regions);
        }

        private static void testWithCSVSource()
        {
            string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (assemblyFolder == null) { return; }
            string kmlInputPath = Path.Combine(assemblyFolder, "Areas.kml");
            string excelPBCsvSource = Path.Combine(assemblyFolder, "Peakbagger Scraper - Peaks.csv");
            string excelDBCsv = Path.Combine(assemblyFolder, "Sierra Guidebook.csv");
            string kmlPath = Path.Combine(assemblyFolder, "testWithCSV.kml");
            
            List<Region> regions = Merger.CollateLocationsAndRegions(kmlInputPath, excelDB, "", excelDBCsv, "", excelPBCsvSource);

            List<Location> locationsCertain = new List<Location>();
            //List<UncertainLocations> locationsUncertain = new List<UncertainLocations>();
            Dictionary<string, List<Location>> rectify = new Dictionary<string, List<Location>>();
            foreach (Region region in regions)
            {
                foreach (FormationMatcher location in region.Locations)
                {
                    if (location.MatchedLocation != null)
                    {
                        locationsCertain.Add(location.PossibleLocations[0]);
                    }
                    else if (location.PossibleLocations.Count > 1)
                    {
                        foreach (Location possibleLocation in location.PossibleLocations)
                        {
                            string name = location.ToString();
                            if (rectify.ContainsKey(name))
                            {
                                rectify[location.ToString()].Add(possibleLocation);
                            }
                            else
                            {
                                List<Location> possibleLocations = new List<Location>{ possibleLocation };
                                rectify.Add(name, possibleLocations);
                            }
                            //UncertainLocations uncertainLocation = new UncertainLocations
                            //{
                            //    Name =  location.FormationName,
                            //    PotentialMatch = possibleLocation
                            //};
                            //locationsUncertain.Add(uncertainLocation);
                        }
                    }
                }
            }
            
            Kml.WriteToKml(kmlPath, regions);
            //Kml.WriteMatchesToKml(kmlPath, locationsCertain);
        }

        //private struct UncertainLocations
        //{
        //    public string Name;
        //    public PeakbaggerLocation PotentialMatch;

        //    public override string ToString()
        //    {
        //        return Name + " - " + PotentialMatch;
        //    }
        //}
    }
}
