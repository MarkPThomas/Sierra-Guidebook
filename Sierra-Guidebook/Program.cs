using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MPT.Trees;

namespace Sierra_Guidebook
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 1 && args[0] == "runTests")
            {
                Tests();
                return;
            }
            else if (args.Length < 2)
            {
                Console.WriteLine("Insufficient arguments given.");
                return;
            }

            string currentExcelPath = args[1];
            string currentDirectoryPath = Path.GetDirectoryName(currentExcelPath);
            if (!File.Exists(currentExcelPath))
            {
                Console.WriteLine("File does not exist {0}", currentExcelPath);
                return;
            }

            string rootName = Path.GetFileNameWithoutExtension(currentExcelPath);
            DirectorySyncer directorySyncer = new DirectorySyncer(rootName);
            
            // Close Excel from here?
            switch (args[0])
            {
                case "syncExcel":
                    directorySyncer.SyncExcelToDirectories(currentExcelPath, currentDirectoryPath);
                    break;
                case "syncDirectories":
                    directorySyncer.SyncDirectoriesToExcel(currentExcelPath, currentDirectoryPath);
                    break;
                case "syncBothExcelAndDirectories":
                    directorySyncer.SyncDirectoriesAndExcel(currentExcelPath, currentDirectoryPath);
                    break;
                default:
                    break;
            }
            // Reopen Excel if closed
        }
       
        static void Tests()
        {
            // Gets path of program
            string currentPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            // Setup
            Console.WriteLine("Setup");
            string rootName = "Sierra Guidebook";
            string excelFileName = rootName + ".xlsx";
            string originalExcelFileName = rootName + " - Original.xlsx";
            string excelTempFileName = rootName + "_Temp.xlsx";
            string testComplete = "Test complete. Press key to continue";
            ITreeNode directories = CreateTree(rootName);

            TreeNodeIO.CreateDirectories(currentPath, directories, isRootNode: true);
            UpdateExcelFile(originalExcelFileName, excelFileName);
            DirectorySyncer directorySyncer = new DirectorySyncer(rootName);

            Console.WriteLine("Press key to begin tests");
            Console.ReadKey();

            //// Test
            //Console.WriteLine("Test: Read Existing Directory");
            //directorySyncer.ReadFromDirectories(Path.Combine(currentPath, directories.Name));
            //Console.WriteLine(testComplete);
            //Console.ReadKey();

            //// Test
            //Console.WriteLine("Test: Read Excel");
            //directorySyncer.ReadFromExcel(Path.Combine(currentPath, excelFileName));
            //Console.WriteLine(testComplete);
            //Console.ReadKey();

            // Test
            Console.WriteLine("Test: Read Existing Directory & Write to Excel");
            directorySyncer.SyncExcelToDirectories(Path.Combine(currentPath, excelFileName), currentPath);
            Console.WriteLine(testComplete);
            Console.ReadKey();

            //// Test
            //UpdateExcelFile(originalExcelFileName, excelFileName);
            //Console.WriteLine("Test: Read Excel & Write to Existing Directory");
            //directorySyncer.SyncDirectoriesToExcel(Path.Combine(currentPath, excelFileName), currentPath);
            //Console.WriteLine(testComplete);
            //Console.ReadKey();

            //// Test
            //UpdateExcelFile(originalExcelFileName, excelFileName);
            //TreeNodeIO.RemoveDirectories(currentPath, directories);
            //TreeNodeIO.CreateDirectories(currentPath, directories, isRootNode: true);
            //Console.WriteLine("Test: Sync Directories and Excel");
            //directorySyncer.SyncDirectoriesAndExcel(Path.Combine(currentPath, excelFileName), currentPath);
            //Console.WriteLine(testComplete);
            //Console.ReadKey();


            // Teardown
            Console.WriteLine("Teardown");
            UpdateExcelFile(originalExcelFileName, excelFileName);
            TreeNodeIO.RemoveDirectories(currentPath, directories);
            Console.WriteLine("Press key to exit");
            Console.ReadKey();
        }

        static void UpdateExcelFile(string originalExcelFileName, string excelFileName)
        {
            try
            {
                File.Copy(originalExcelFileName, excelFileName, overwrite: true);
            }
            catch (Exception)
            {
                // No action.
            }
        }

        static TreeNode CreateTree(string rootName)
        {
            // Generate list of names.
            List<string> parentAreas = NumberedList("ParentArea", 4);
            List<string> childAreas = NumberedList("ChildArea", 5);
            List<string> formations = NumberedList("Formation", 3);

            List<string> formationsSub = new List<string>();
            foreach (string formationName in formations)
            {
                formationsSub.AddRange(NumberedList(formationName+"~Sub", 2));
            }
            
            List<string> routes = NumberedList("Route", 6);

            // Generate and assemble nodes.
            ITreeNode rootNode = new TreeNode(rootName);

            ITreeNode parentArea = new TreeNode();
            ITreeNode childArea = new TreeNode();
            ITreeNode formation = new TreeNode();
            ITreeNode route = new TreeNode();
            int repeatedRoute = 3;

            parentArea = AddAreaType(parentAreas[0]);
            //
            childArea = AddAreaType(childAreas[0]);

            formation = AddBasicType(formations[0]);
            formation.AddChild(AddBasicType(routes[0]));

            childArea.AddChild(formation);

            formation = AddBasicType(formations[1]);
            formation.AddChild(AddBasicType(routes[1]));
            formation.AddChild(AddBasicType(routes[2]));

            childArea.AddChild(formation);

            parentArea.AddChild(childArea);

            //
            childArea = AddAreaType(childAreas[1]);

            formation = AddBasicType(formations[1]);
            formation.AddChild(AddBasicType(routes[repeatedRoute]));

            childArea.AddChild(formation);

            formation = AddBasicType(formationsSub[2]);                 // Sub-Formation
            formation.AddChild(AddBasicType(routes[repeatedRoute]));    // Repeated route from parent formation
            formation.AddChild(AddBasicType(routes[4]));
            
            childArea.AddChild(formation);

            formation = AddBasicType(formations[2]);
            formation.AddChild(AddBasicType(routes[repeatedRoute]));    // Repeated route from another formation

            childArea.AddChild(formation);

            parentArea.AddChild(childArea);

            //
            rootNode.AddChild(parentArea);
            //

            // Duplicate structure within other parent/child areas
            parentArea = AddAreaType(parentAreas[1]);
            //
            childArea = AddAreaType(childAreas[2]);

            formation = AddBasicType(formations[0]);
            formation.AddChild(AddBasicType(routes[0]));

            childArea.AddChild(formation);

            formation = AddBasicType(formations[1]);
            formation.AddChild(AddBasicType(routes[1]));
            formation.AddChild(AddBasicType(routes[2]));

            childArea.AddChild(formation);

            parentArea.AddChild(childArea);

            //
            childArea = AddAreaType(childAreas[3]);

            formation = AddBasicType(formations[1]);
            formation.AddChild(AddBasicType(routes[repeatedRoute]));

            childArea.AddChild(formation);

            formation = AddBasicType(formationsSub[2]);                 // Sub-Formation
            formation.AddChild(AddBasicType(routes[repeatedRoute]));    // Repeated route from parent formation
            formation.AddChild(AddBasicType(routes[4]));

            childArea.AddChild(formation);

            formation = AddBasicType(formations[2]);
            formation.AddChild(AddBasicType(routes[repeatedRoute]));    // Repeated route from another formation
            
            childArea.AddChild(formation);

            parentArea.AddChild(childArea);

            //
            rootNode.AddChild(parentArea);
            //

            // Empty parent area
            parentArea = AddAreaType(parentAreas[2]);

            rootNode.AddChild(parentArea);

            // Empty child area
            parentArea = AddAreaType(parentAreas[3]);
            childArea = AddAreaType(childAreas[4]);

            parentArea.AddChild(childArea);
            
            rootNode.AddChild(parentArea);

            return (rootNode as TreeNode);
        }

        static ITreeNode AddBasicType(string name)
        {
            ITreeNode node = new TreeNode(name);
            node = AddSupportingFiles(node);
            return node;
        }

        static ITreeNode AddAreaType(string name)
        {
            ITreeNode node = new TreeNode(name);
            node = AddSupportingFilesArea(node);
            return node;
        }

        static ITreeNode AddSupportingFiles(ITreeNode node)
        {
            node.AddChild(new TreeNode("_Research"));
            node.AddChild(new TreeNode("_Development"));
            node.AddChild(new TreeNode("_Final"));
            return node;
        }

        static ITreeNode AddSupportingFilesArea(ITreeNode node)
        {
            node = AddSupportingFiles(node);
            node.AddChild(new TreeNode("_Photogenic"));
            return node;
        }


        static List<string> NumberedList(string name, int maxNumber)
        {
            List<string> numberedList = new List<string>();
            for (int i = 0; i < maxNumber; i++)
            {
                numberedList.Add(name + (i + 1));
            }
            return numberedList;
        }

       
    }
}
