using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using MPT.Excel;
using MPT.Trees;

namespace Sierra_Guidebook
{
    public class DirectorySyncer
    {
        public string RootName { get; private set; }

        public string RootPath { get; private set; }

        public string ExcelPath { get; private set; }

        public ITreeNode ModelWindows { get; private set; }
        public ITreeNode ModelVirtual { get; private set; }

        private Directories pathsWindows = new Directories();
        private Directories pathsVirtual = new Directories();

        public DirectorySyncer(string rootName)
        {
            RootName = rootName;
            ModelWindows = new TreeNode(rootName);
            ModelVirtual = new TreeNode(rootName);
        }

        public void SyncDirectoriesAndExcel(string excelPath = "", string rootPath = "")
        {
            if (!dataValid(excelPath, rootPath, checkIfExists: true)) { return; }

            ConsoleHeader1Writer("Syncing Directories and Excel");
            SyncDirectoriesToExcel();
            SyncExcelToDirectories();
        }

        public void SyncDirectoriesToExcel(string excelPath = "", string rootPath = "")
        {
            if (!dataValid(excelPath, rootPath)) { return; }

            ConsoleHeader1Writer("Syncing Directories to Excel");
            ReadFromExcel(ExcelPath);
            WriteToDirectories(RootPath);
        }



        public void SyncExcelToDirectories(string excelPath = "", string rootPath = "")
        {
            if (!dataValid(excelPath, rootPath, checkIfExists: true)) { return; }

            ConsoleHeader1Writer("Syncing Excel to Directories");
            ReadFromDirectories(Path.Combine(RootPath, RootName));
            WriteToExcel(ExcelPath);
        }


        public void ReadFromDirectories(string rootPath)
        {
            if (!rootDirectoryPathValid(rootPath, checkIfExists: true)) { return; }
            ConsoleHeader1Writer("Reading from directories ...");

            populateAllExistingDirectories(pathsWindows);
            ModelWindows = createModel(pathsWindows);
        }


        public void ReadFromExcel(string excelPath)
        {
            if (!excelPathValid(excelPath)) { return; }
            ConsoleHeader1Writer("Reading from Excel ...");

            List<List<string>> routePaths = new List<List<string>>();
            using (ExcelHelper excel = new ExcelHelper(ExcelPath))
            {
                try
                {
                    routePaths = excel.RangeValuesBelowHeaderAndCorrespondingColumns("Route", "Formation_Key", "SubFormation_Key", "AreaChild_Key", "AreaParent_Key");
                }
                catch (COMException eCom)
                {
                    if (eCom.Message.Contains(ExcelWrapper.OBJECT_REQUIRED))
                    {
                        Console.WriteLine("reading oops");
                        routePaths = new List<List<string>>();
                    }
                    excel.SetTotalShutdown();
                }
            }
              
            if (routePaths.Count == 0) { return; }

            pathsVirtual = parseListsToPaths(routePaths[4],
                                            routePaths[3],
                                            assembleFormationsNames(routePaths[1], routePaths[2]),
                                            routePaths[0]);

            ModelVirtual = createModel(pathsVirtual);
        }


        public void WriteToDirectories(string path)
        {
            ConsoleHeader1Writer("Writing to directories ...");
            TreeNodeIO.CreateDirectories(path, ModelVirtual, isRootNode: true);
        }

        public void WriteToExcel(string excelPath = "")
        {
            if (!excelPathValid(excelPath)) { return; }

            // Update model to current Excel state
            ConsoleHeader1Writer("Getting Excel current state ...");
            ReadFromExcel(ExcelPath);

            ConsoleHeader1Writer("Writing to Excel ...");
            // Get list of new formations
            List<string> newFormationPaths = new List<string>();
            foreach (string path in pathsWindows.FormationPaths)
            {
                if (!formationPathExistsInModel(path, ModelVirtual))
                {
                    newFormationPaths.Add(path);
                }
            }

            // Get lits of new routes
            List<string> newRoutePaths = new List<string>();
            foreach (string path in pathsWindows.RoutePaths)
            {
                if (!routePathExistsInModel(path, ModelVirtual))
                {
                    newRoutePaths.Add(path);
                }
            }
 
            using (ExcelHelper excel = new ExcelHelper(ExcelPath))
            {
                try
                {                    
                    foreach (string path in newFormationPaths)
                    {
                        Dictionary<string, string> rangeNamesValues = assembleRangeNamesValuesFromFormationTab(path);
                        Console.WriteLine("Writing formation {0} to Excel", path);
                        writeRowToExcel(excel, rangeNamesValues, "Formation");
                    }

                    foreach (string path in newRoutePaths)
                    {
                        Dictionary<string, string> rangeNamesValues = assembleRangeNamesValuesFromRouteTab(path);
                        Console.WriteLine("Writing route {0} to Excel", path);
                        writeRowToExcel(excel, rangeNamesValues, "Route");
                    }
                    excel.Save();
                }
                catch (COMException eCom)
                {
                    if (eCom.Message.Contains(ExcelWrapper.OBJECT_REQUIRED))
                    {
                        Console.WriteLine("writing oops");
                    }
                    excel.SetTotalShutdown();
                }
            }
        }

        private Dictionary<string, string> assembleRangeNamesValuesFromFormationTab(string path)
        {
            string formationName = getDirectoryName(path, 0);
            string formationSubName = "";
            assignFormationNames(ref formationName, ref formationSubName);
            string areaChildName = getDirectoryName(path, 1);

            Dictionary<string, string> rangeNamesValues = new Dictionary<string, string>();
            rangeNamesValues.Add("AreaChild", areaChildName);
            rangeNamesValues.Add("Formation", formationName);
            rangeNamesValues.Add("SubFormation", formationSubName);
            return rangeNamesValues;
        }


        private Dictionary<string, string> assembleRangeNamesValuesFromRouteTab(string path)
        {
            string routeName = getDirectoryName(path, 0);
            string formationName = getDirectoryName(path, 1);
            string formationSubName = "";
            assignFormationNames(ref formationName, ref formationSubName);
            string areaChildName = getDirectoryName(path, 2);
            string areaParentName = getDirectoryName(path, 3);

            Dictionary<string, string> rangeNamesValues = new Dictionary<string, string>();
            rangeNamesValues.Add("AreaParent_Key", areaParentName);
            rangeNamesValues.Add("AreaChild_Key", areaChildName);
            rangeNamesValues.Add("Formation_Key", formationName);
            rangeNamesValues.Add("SubFormation_Key", formationSubName);
            rangeNamesValues.Add("Route", routeName);

            return rangeNamesValues;
        }

        private void assignFormationNames(ref string formationName, ref string formationSubName)
        {
            if (formationName.Contains(SubFormation.DEMARCATOR))
            {
                string[] formationParts = formationName.Split((char)SubFormation.DEMARCATOR);
                formationName = formationParts[0];
                formationSubName = formationParts[1];
            }
        }

        private void writeRowToExcel(ExcelHelper excel, Dictionary<string, string> rangeNamesvalues, string rangeNameReference)
        {
            int lastRowOffset = excel.GetRowOffsetLastFilled(rangeNameReference);
            excel.DuplicateRowDown(rangeNameReference, lastRowOffset);
            excel.ClearRowContents(rangeNameReference, lastRowOffset + 1);
            excel.WriteValues(rangeNamesvalues, lastRowOffset + 1);
        }


        private string getDirectoryName(string path, int numberDirectoriesUp = 0)
        {
            if (numberDirectoriesUp > 0)
            {
                for (int i = 1; i <= numberDirectoriesUp; i++)
                {
                    path = Path.GetDirectoryName(path);
                }
            }

            return Path.GetFileName(path);
        }


        private Directories parseListsToPaths(List<string> parentAreas,
                                        List<string> childAreas,
                                        List<string> formations,
                                        List<string> routes)
        {
            Directories paths = new Directories();
            if (parentAreas.Count != routes.Count ||
                childAreas.Count != routes.Count ||
                formations.Count != routes.Count) { return paths; }

            string pathStub = Path.Combine(@"C:\", RootName);

            for (int i = 0; i < routes.Count; i++)
            {
                string path = "";
                path = updatePathsList(paths.ParentAreaPaths, pathStub, parentAreas[i]);
                path = updatePathsList(paths.ChildAreaPaths, path, childAreas[i]);
                path = updatePathsList(paths.FormationPaths, path, formations[i]);
                path = updatePathsList(paths.RoutePaths, path, routes[i]);
            }
            return paths;
        }

        private string updatePathsList(List<string> paths, string pathStub, string directory)
        {
            string path = Path.Combine(pathStub, directory.Replace('"', '\''));
            if (!paths.Contains(path))
            {
                paths.Add(path);
            }
            return path;
        }

        private List<string> assembleFormationsNames(List<string> formations, List<string> subFormations)
        {
            List<string> fullFormationNames = new List<string>();
            for (int i = 0; i < formations.Count; i++)
            {
                if (string.IsNullOrEmpty(subFormations[i]))
                {
                    fullFormationNames.Add(formations[i]);
                }
                else
                {
                    fullFormationNames.Add(formations[i] + "~" + subFormations[i]);
                }
            }
            return fullFormationNames;
        }

        private ITreeNode createModel(Directories paths)
        {
            ITreeNode Model = new TreeNode(RootName);

            // Add Parent Regions
            foreach (string path in paths.ParentAreaPaths)
            {
                addParentRegion(path, Model);
            }

            // Add Child Regions
            foreach (string path in paths.ChildAreaPaths)
            {
                addChildRegion(path, Model);
            }

            // Add Formations
            foreach (string path in paths.FormationPaths)
            {
                addFormation(path, Model);
            }

            // Add Routes
            foreach (string path in paths.RoutePaths)
            {
                addRoute(path, Model);
            }

            return Model;
        }

        private void addParentRegion(string path, ITreeNode model)
        {
            string name = getDirectoryName(path);
            ITreeNode regionParent = new ParentRegion(name);

            model.AddChild(regionParent);
        }

        private void addChildRegion(string path, ITreeNode model)
        {
            string name = getDirectoryName(path);
            getParentRegion(path, numberDirectoriesUp: 1, model: model).AddChild(new ChildRegion(name));
        }

        private void addFormation(string path, ITreeNode model)
        {
            string childRegionName = getDirectoryName(path, numberDirectoriesUp: 1);
            getParentRegion(path, numberDirectoriesUp: 2, model: model).GetChildRegion(childRegionName).AddChild(createFormation(path));
        }

        private void addRoute(string path, ITreeNode model)
        {
            string name = getDirectoryName(path);
            string childRegionName = getDirectoryName(path, numberDirectoriesUp: 2);
            string formationName = getDirectoryName(path, numberDirectoriesUp: 1);
            getParentRegion(path, numberDirectoriesUp: 3, model: model).GetChildRegion(childRegionName).GetFormation(formationName).AddChild(new Route(name));
        }


        private ParentRegion getParentRegion(string path, int numberDirectoriesUp, ITreeNode model)
        {
            string parentRegionName = getDirectoryName(path, numberDirectoriesUp: numberDirectoriesUp);
            return model.GetChild(new ParentRegion(parentRegionName)) as ParentRegion;
        }

        private ITreeNode createFormation(string path)
        {
            string name = getDirectoryName(path);
            ITreeNode formation;
            if (!name.Contains(SubFormation.DEMARCATOR))
            {
                formation = new Formation(name);
            }
            else
            {
                formation = new SubFormation(name);
            }
            return formation;
        }



        private void populateAllExistingDirectories(Directories paths)
        {
            paths.ParentAreaPaths = getExistingDirectories(RootPath);
            paths.ChildAreaPaths = populateExistingPaths(paths.ParentAreaPaths);
            paths.FormationPaths = populateExistingPaths(paths.ChildAreaPaths);
            paths.RoutePaths = populateExistingPaths(paths.FormationPaths);
        }

        private List<string> populateExistingPaths(List<string> paths)
        {
            List<string> childDirectories = new List<string>();
            foreach (string path in paths)
            {
                List<string> tempChildDirectories = getExistingDirectories(path);
                foreach (string childDirectory in tempChildDirectories)
                {
                    childDirectories.Add(Path.Combine(path, childDirectory));
                }
            }
            return childDirectories;
        }

        private List<string> getExistingDirectories(string rootPath)
        {
            List<string> existingDirectories = new List<string>();
            if (!Directory.Exists(rootPath)) { return existingDirectories; }

            string[] directories = Directory.GetDirectories(rootPath);
            foreach (string directory in directories)
            {
                if (!pathsWindows.Standard.Contains(Path.GetFileName(directory)))
                {
                    existingDirectories.Add(directory);
                }
            }
            return existingDirectories;
        }

        private bool formationPathExistsInModel(string path, ITreeNode model)
        {
            string formationName = getDirectoryName(path);
            string areaChildName = getDirectoryName(path, numberDirectoriesUp: 1);
            string areaParentName = getDirectoryName(path, numberDirectoriesUp: 2);

            ParentRegion parentArea = new ParentRegion(areaParentName);
            if (model.ContainsChild(parentArea))
            {
                parentArea = (ParentRegion)model.GetChild(parentArea);
                if (parentArea.ContainsChildRegion(areaChildName))
                {
                    ChildRegion childRegion = parentArea.GetChildRegion(areaChildName);
                    return childRegion.ContainsFormation(formationName);
                }
            }
            return false;
        }

        private bool routePathExistsInModel(string path, ITreeNode model)
        {
            string routeName = getDirectoryName(path);
            string formationName = getDirectoryName(path, numberDirectoriesUp: 1);
            string areaChildName = getDirectoryName(path, numberDirectoriesUp: 2);
            string areaParentName = getDirectoryName(path, numberDirectoriesUp: 3);

            ParentRegion parentArea = new ParentRegion(areaParentName);
            if (model.ContainsChild(parentArea))
            {
                parentArea = (ParentRegion)model.GetChild(parentArea);
                if (parentArea.ContainsChildRegion(areaChildName))
                {
                    ChildRegion childRegion = parentArea.GetChildRegion(areaChildName);
                    if (childRegion.ContainsFormation(formationName))
                    {
                        Formation formation = childRegion.GetFormation(formationName);
                        return formation.ContainsRoute(routeName);
                    }
                }
            }
            return false;
        }

        private bool dataValid(string excelPath = "", string rootPath = "", bool checkIfExists = false)
        {
            return (excelPathValid(excelPath) && rootDirectoryPathValid(rootPath, checkIfExists));
        }

        private bool excelPathValid(string excelPath = "")
        {
            if (!string.IsNullOrEmpty(excelPath)) { ExcelPath = excelPath; }
            if (string.IsNullOrEmpty(ExcelPath)) { return false; }
            if (!File.Exists(ExcelPath))
            {
                ConsoleHeader1Writer("Excel file at the following path does not exist:");
                Console.WriteLine(ExcelPath);
                ConsoleHeader1Writer("Exiting program ...");
                return false;
            }
            return true;
        }

        private bool rootDirectoryPathValid(string rootPath = "", bool checkIfExists = false)
        {
            if (!string.IsNullOrEmpty(rootPath)) { RootPath = rootPath; }
            if (string.IsNullOrEmpty(RootPath)) { return false; }
            if (!Directory.Exists(RootPath))
            {
                ConsoleHeader1Writer("Route directory at the following path does not exist:");
                Console.WriteLine(RootPath);
                ConsoleHeader1Writer("Exiting program ...");
                return false;
            }
            return true;
        }

        private void ConsoleHeader1Writer(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}

