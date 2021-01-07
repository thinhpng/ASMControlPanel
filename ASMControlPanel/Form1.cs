using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Permissions;
using System.Security.AccessControl;
using System.Management;

namespace ASMControlPanel {
    public partial class Form1 : Form {
        const string STARTPMBAT = @"c:\ASM\Project\Bin\StartPM.bat";
        const string WNETWAKEUP_FILE = @"c:\ASM\Project\Bin\WNetWakeUp.exe";
        const string DEFAULT_RCSIM_DIR = @"c:\ASM\Project\Bin";
        const string DEFAULT_PROJECT_DIR = @"c:\Project";
        const string DEFAULT_CONFIG_DIR = @"UPC\SystemFile\Config";
        const string DEFAULT_MAIN_FILE = @"C:\Project\UPC\Bin\Main.exe";
        const string DEFAULT_MCIFSim_FILE = @"C:\Project\UPC\Bin\MCIFSim.ini";
        const string KILL_RC_BAT_FILE = @"c:\ASM\Project\Bin\KillRC.bat";
        const string ASM_TOOLS_FILE = @".\ASM\ASMToolList.txt";
        const string ASM_UTILS_FILE = @".\ASM\ASMUtils.bat";
        const string PROJECT_SHARE_NAME = "'Project$'";
        const string HEADER = @"Module  ,Target Name        ,Target Location    ,Start in           ,Parameter,";

        List<string> RC_PROCESSES = new List<string> { "SimPmScanIF.exe", "xpPMWin32.exe" }; //mininum for initial checking RC Sim running status 
        bool RC1EXIST = false;
        bool RC2EXIST = false;
        bool RC3EXIST = false;
        bool RC4EXIST = false;
        bool needSwap = true;
        string P1FTXT, P2FTXT, P3FTXT, P4FTXT;

        bool IS_RCSIM = false;
        //public bool IS_RCSIM { get { return IS_RCSIM; } set { IS_RCSIM = value; } }
        string LASTSELECTED; //eg. c:\Project-P47-3.17b24-005_Config-045
        string RUNNING_PROJECT; //eg. c:\Project-Intel-1.74a2-002_Config-003
        string SELECTED_RC;
        string SELECTED_RC_BIN_DIR;
        string RC_CONFIG_FILE;
        string PLATFORM;
        string RCSIM_FILE;
        string SELECTED_MAPTOOL;
        string SELECTED_VNCTOOL;
        string PMSIMFILE_DIR;
        string PMSIMFILE_FROM;
        string RC_TYPE;
        string MODULE;
        string SPECIFIC_PROJ_DIR_NAME; //eg. Project-Intel-1.74a2-002_Config-003
        string SOFTREV;

        public Form1() {
            logger("*********** ", DateTime.Now.ToString(), " ***********");
            if (Directory.Exists(DEFAULT_PROJECT_DIR)) {
                SOFTREV = System.Diagnostics.FileVersionInfo.GetVersionInfo(DEFAULT_MAIN_FILE).ProductVersion;
            } else {
                SOFTREV = "NOT FOUND: " + DEFAULT_PROJECT_DIR;
            }
            logger("SOFTREV: ", SOFTREV);
            InitializeComponent();
            this.StartPosition = FormStartPosition.Manual;
            PrepareProjectSelection();
            PrepareToolMapSelection();
            PrepareToolVncSelection();
            logger("*****************************************************");
            logger("***********  Initialization Completed!!!  ***********");
            logger("*****************************************************");
        }

        private bool IsRCSimRunning() {
            IS_RCSIM = false;
            foreach (string pn in RC_PROCESSES) {
                //logger("Processing: ", pn);
                string fn = Path.GetFileNameWithoutExtension(Path.Combine(DEFAULT_RCSIM_DIR, pn));
                //logger("Verifying: ", fn);
                Process[] pname = Process.GetProcessesByName(fn);
                //
                if (pname.Length != 0) { //It is currently running.
                    logger(pn, " is running!");
                    GetRCSimStatus();
                    IS_RCSIM = true;
                    return true;
                } else
                    logger(pn, " is NOT running!");
            }
            logger("RCSim is not running!");
            return false;
        }

        private void GetRCSimStatus() {
            logger("-> GetRCSimStatus()");
            RCSIM_FILE = new DirectoryInfo(DEFAULT_RCSIM_DIR).GetFiles("PMC*_Sim.txt").OrderByDescending(f => f.LastWriteTime).First().Name;
            if (RCSIM_FILE.Contains("PMC1")) {
                checkBoxRC1.Checked = true;
                SELECTED_RC = "PMC1";
            } else if (RCSIM_FILE.Contains("PMC2")) {
                checkBoxRC2.Checked = true;
                SELECTED_RC = "PMC2";
            } else if (RCSIM_FILE.Contains("PMC3")) {
                checkBoxRC3.Checked = true;
                SELECTED_RC = "PMC3";
            } else if (RCSIM_FILE.Contains("PMC4")) {
                checkBoxRC4.Checked = true;
                SELECTED_RC = "PMC4";
            }
            logger("SELECTED_RC: ", SELECTED_RC);
        }

        private bool IsMainRunning() {
            try {
                Process[] pname = Process.GetProcessesByName("Main");
                if (pname.Length != 0) {//EI is currently running.
                    logger("Main.exe is running!");
                    return true;
                } else {
                    logger("Main.exe Not running!");
                    return false;
                }
            } catch (Exception e) {
                logger("Exception: ", e.ToString());
                return false;
            }
        }

        private string GetSpecificProjectFolderName(string dir) {
            logger("-> GetSpecificProjectFolderName(", dir, ")");
            string fileName = "";
            /* try
                        {
                            DirectoryInfo di = new DirectoryInfo(dir);
                            fileName = di.GetFiles("Project*").OrderByDescending(f => f.LastWriteTime).First().Name;
                            logger("Found : ", fileName);
                        }
                        catch (Exception ex)
                        {
                            logger("Exception: ", ex.ToString());
                        }*/

            try {
                //string fileName = new DirectoryInfo(dir).GetFiles("Project*").OrderByDescending(f => f.LastWriteTime).First().Name;
                FileInfo[] fi = new DirectoryInfo(dir).GetFiles("*Project_*");
                if (fi.Length > 0) {
                    fileName = fi.OrderByDescending(f => f.LastWriteTime).First().Name; //Just need the newest one                     
                    logger("Found : ", fileName);
                    string fileExtension = Path.GetExtension(Path.Combine(dir, fileName));
                    if (Regex.Match(fileExtension, ".txt|.png|.jpg|.jpeg|.gif|.bmp", RegexOptions.IgnoreCase).Success) {
                        fileName = Path.GetFileNameWithoutExtension(Path.Combine(dir, fileName));
                        logger("OnlyName : ", fileName);
                    }
                }
            } catch (Exception ex) {
                logger("Exception: ", ex.ToString());
            }

            return fileName;
        }

        private void GetSpecificPMInfo(string pmConfigFile) {
            logger("-> GetSpecificPMInfo: ", pmConfigFile);
            using (StreamReader inFile = new StreamReader(pmConfigFile, false)) {
                Regex r = new Regex(@"Type of Equipment\s+=\s(.*)", RegexOptions.IgnoreCase);
                string line = inFile.ReadLine(); //throw away the header line
                while ((line = inFile.ReadLine()) != null) {
                    Match m = r.Match(line);
                    if (m.Success) {
                        RC_TYPE = m.Groups[1].Value;
                        logger("RC_TYPE: ", RC_TYPE);
                        PMSIMFILE_DIR = Path.Combine(PMSIMFILE_DIR, RC_TYPE.ToUpper());
                        logger("PMSIMFILE_DIR: ", PMSIMFILE_DIR);
                        break;
                    }
                }
            }
        }

        private void SetRCCheckboxes(string[] fileNames) {
            logger("-> SetRCCheckboxes()");
            RC1EXIST = RC2EXIST = RC3EXIST = RC4EXIST = false;

            foreach (string f in fileNames) {
                if (f.Contains("p1f")) {
                    RC1EXIST = true;
                    P1FTXT = f;
                    logger("RC1 available! ", P1FTXT);                    
                }

                if (f.Contains("p2f")) {
                    RC2EXIST = true;
                    P2FTXT = f;
                    logger("RC2 available! ", P2FTXT);
                }

                if (f.Contains("p3f")) {
                    RC3EXIST = true;
                    P3FTXT = f;
                    logger("RC3 available! ", P3FTXT);
                }

                if (f.Contains("p4f")) {
                    RC4EXIST = true;
                    P4FTXT = f;
                    logger("RC4 available! ", P4FTXT);
                }
            }
            checkBoxRC1.Enabled = RC1EXIST;
            checkBoxRC2.Enabled = RC2EXIST;
            checkBoxRC3.Enabled = RC3EXIST;
            checkBoxRC4.Enabled = RC4EXIST;
        }

        private void PrepareToolVncSelection() {
            logger("-> PrepareToolVncSelection()");
            DirectoryInfo dirInfo = new DirectoryInfo(@".\ASM");
            dirInfo.GetFiles("*.vnc").ToList().ForEach(f => comboBox3.Items.Add(f.Name));
        }

        string UNCtoMappedDrive(string uncPath) {
            Microsoft.Win32.RegistryKey rootKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("network");
            logger(rootKey.GetValueNames());

            foreach (string subKey in rootKey.GetSubKeyNames()) {
                Microsoft.Win32.RegistryKey mappedDriveKey = rootKey.OpenSubKey(subKey);
                //logger(subKey);
                if (string.Compare((string)mappedDriveKey.GetValue("RemotePath", ""), uncPath, true) == 0)
                    return subKey.ToUpperInvariant() + @":\";
            }

            //logger(uncPath);
            return uncPath;
        }

        private Dictionary<string, string> GetMappedDrives() {
            logger("-> GetMappedDrives");
            System.Diagnostics.Process net = new
            System.Diagnostics.Process();
            net.StartInfo.FileName = "Net.exe";
            net.StartInfo.CreateNoWindow = true;
            net.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            net.StartInfo.UseShellExecute = false;
            net.StartInfo.RedirectStandardOutput = true;
            net.StartInfo.Arguments = "use";
            net.Start();
            string line = null;
            var mappedDrives = new Dictionary<string, string>();
            while ((line = net.StandardOutput.ReadLine()) != null) {
                //logger("line: ", line);
                if (line.StartsWith("OK")) {
                    logger("MappedDrive: ", line);
                    //Regex.Match(line, "OK\b+(.*?)\b+(.*?)\b+");
                    foreach (Match match in Regex.Matches(line, @"OK\s+(.*?)\s+(.*?)\s+", RegexOptions.IgnoreCase)) {
                        //logger("Got1: ", match.Groups[1].Value, " Got2: ", match.Groups[2].Value);
                        mappedDrives.Add(match.Groups[1].Value, match.Groups[2].Value);
                    }
                }
            }

            return mappedDrives;
        }

        private void PrepareToolMapSelection() {
            logger("-> PrepareToolMapSelection()");
            Dictionary<string, string> mappedDrives = GetMappedDrives();
            button2.Enabled = true; //Map Button
            button4.Enabled = false; //UnMap Button

            using (StreamReader inFile = new StreamReader(ASM_TOOLS_FILE, false)) {
                comboBox2.Items.Clear(); //refresh  
                string line = inFile.ReadLine(); //throw away the header line
                while ((line = inFile.ReadLine()) != null && line.Substring(0, 2) != "//") {
                    //logger("Processing line: ", line);
                    string tool = line.TrimEnd(' ', '\t');
                    comboBox2.Items.Add(tool);
                    foreach (var d in mappedDrives) {
                        if (d.Value.Contains(tool)) {
                            comboBox2.SelectedItem = tool;
                            //SELECTED_MAPTOOL = tool;
                            logger(SELECTED_MAPTOOL, " mapped to Drive: ", d.Key);
                            label3.Text = CombinedString(SELECTED_MAPTOOL, " mapped to Drive: ", d.Key);
                            button2.Enabled = false; //Map Button
                            button4.Enabled = true; //UnMap Button
                        }
                    }
                }
            }
        }

        private void PrepareProjectSelection() {
            logger("-> PrepareProjectSelection()");
            try {
                comboBox1.Items.Clear(); //refresh        
                if (Directory.Exists(DEFAULT_PROJECT_DIR)) {
                    comboBox1.Items.Add(DEFAULT_PROJECT_DIR);
                    logger("Added comboBox1[0] with ", DEFAULT_PROJECT_DIR);
                }
                string[] projects = Directory.GetDirectories(@"c:\", "*Project_*", SearchOption.TopDirectoryOnly);
                foreach (string project in projects) {
                    comboBox1.Items.Add(project);
                }

                if (String.IsNullOrEmpty(LASTSELECTED)) { //ASMControlPanel just starts up. Default selection is the first item in the list  
                    if (Directory.Exists(DEFAULT_PROJECT_DIR)) {
                        comboBox1.SelectedItem = DEFAULT_PROJECT_DIR;
                        //GetPlatformInfo(DEFAULT_PROJECT_DIR);
                    } else {
                        comboBox1.SelectedIndex = 0;
                    }                    
                    logger("Initial LASTSELECTED: ", LASTSELECTED);
                } else {
                    logger("LASTSELECTED: ", LASTSELECTED);
                }
                string fullPathDir = Path.Combine(LASTSELECTED, @"UPC\SystemFile\Config\");
                string[] fileNames = Directory.GetFiles(fullPathDir, PLATFORM + ".p*f.txt");

                //GenerateSpecificProjectFolder(LASTSELECTED);

                if (IsMainRunning()) { //Eagle-I is currently running
                    GetPlatformInfo(LASTSELECTED);
                    StartStop_Button.Text = "STOP";
                    StartStop_Button.BackColor = Color.Salmon;

                    if (IsRCSimRunning()) {
                        RCSIMSelection(true);
                        comboBox1.Enabled = false;
                    }

                    label1.Text = CombinedString(RUNNING_PROJECT, " is running");
                    SetRCSIMFeatureVisible(false);
                } else {
                    StartStop_Button.Text = "START";
                    StartStop_Button.BackColor = Color.LawnGreen;
                    SetRCSIMFeatureVisible(true);
                }
                SetRCCheckboxes(fileNames);
                label1.BackColor = Color.LightBlue;
                label1.Refresh();
            } catch (Exception e) {
                logger("Exception: ", e.ToString());
            }
        }

        //Invoked only once EI running to get info
        private void GetPlatformInfo(string projDir) {
            logger("-> GetPlatformInfo(): ", projDir);
            string fullPathDir = Path.Combine(projDir, DEFAULT_CONFIG_DIR);
            //logger("fullPathDir: ", fullPathDir);

            if (new DirectoryInfo(fullPathDir).GetFiles("EagleXP.*").Count() > 0) {
                PLATFORM = "EagleXP";
                PMSIMFILE_DIR = @".\ASM\ALD";
            } else if (new DirectoryInfo(fullPathDir).GetFiles("Epi.*").Count() > 0) {
                PLATFORM = "Epi";
                PMSIMFILE_DIR = @".\ASM\EPI";
            } else if (new DirectoryInfo(fullPathDir).GetFiles("Synergis.*").Count() > 0) {
                PLATFORM = "Synergis";
                PMSIMFILE_DIR = @".\ASM\SYN";
            }
            logger("\t\t<<<< PLATFORM: ", PLATFORM, " >>>>");
            //RC_CONFIG_FILE = Path.Combine(DEFAULT_PROJECT_DIR, DEFAULT_CONFIG_DIR, PLATFORM + ".p1f.txt"); //Default to PM1 file

            //SetRCProcesses();
            string[] filenames = Directory.GetFiles(fullPathDir, PLATFORM + ".p*f.txt"); //epi_beta.p2f.txt (p44 might name it that way) 
            //SetRCCheckboxes(filenames);

/*            if (filenames.Length > 0)
                RC_CONFIG_FILE = filenames[0]; //there should be only one matched with module

            logger("RC_CONFIG_FILE: ", RC_CONFIG_FILE);*/

            //StreamReader file = new StreamReader(RC_CONFIG_FILE);
            //Regex r = new Regex(@"Type of Equipment\s+=\s(.*)", RegexOptions.IgnoreCase);
            //string line;

            //while ((line = file.ReadLine()) != null){
            //    Match m = r.Match(line);
            //    if (m.Success){
            //        RC_TYPE = m.Groups[1].Value;
            //        logger("RC_TYPE: ", RC_TYPE);
            //        PMSIMFILE_DIR = Path.Combine(PMSIMFILE_DIR, RC_TYPE.ToUpper());
            //        logger("PMSIMFILE_DIR: ", PMSIMFILE_DIR);
            //        break;
            //    }
            //}
            //file.Close();

            logger("RUNNING_PROJECT: ", RUNNING_PROJECT);
            if (projDir == DEFAULT_PROJECT_DIR) {
                RUNNING_PROJECT = Path.Combine(@"c:\", GetSpecificProjectFolderName(DEFAULT_PROJECT_DIR));
                logger("RUNNING_PROJECT:", RUNNING_PROJECT);
                if (RUNNING_PROJECT == @"c:\" || RUNNING_PROJECT.StartsWith(@"c:\Project_" + SOFTREV) || RUNNING_PROJECT.Contains("_Project_")) { //New one does not have specific project folder file created yet.
                    string OldName = RUNNING_PROJECT;
                    RUNNING_PROJECT = Path.Combine(@"c:\", GenerateStandardFormatProjectFolderName());
                    logger("Now current RUNNING_PROJECT: ", RUNNING_PROJECT);
                }
                label1.Text = CombinedString(RUNNING_PROJECT, " is currently active");
                label1.Refresh();
            }
        }

        private void label1_TextChanged(object sender, EventArgs e) {
            logger("Label Text Changed: ", label1.Text);
        }

        private void label1_Click(object sender, EventArgs e) {
            logger("-> label1_Click()");
            //StartWNetWakeUpProcess();
            //CreateKillPMCBatFile();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) {
            StartStop_Button.Text = "START";
            StartStop_Button.BackColor = Color.LawnGreen;
            LASTSELECTED = comboBox1.SelectedItem.ToString();            
            logger("LASTSELECTED: ", LASTSELECTED);
            SetAvailableRCByProject(LASTSELECTED);

            if (LASTSELECTED == DEFAULT_PROJECT_DIR) {
                if (IsMainRunning()) { //EI is currently running. So Stop is optionable
                    StartStop_Button.Text = "STOP";
                    StartStop_Button.BackColor = Color.Salmon;
                }
            }
        }

        private void SetAvailableRCByProject(string LASTSELECTED) {
            logger("-> SetAvailableRCByProject(", LASTSELECTED, ")");
            string fullPathDir = Path.Combine(LASTSELECTED, @"UPC\SystemFile\Config\");
            //logger("fullPathDir: ", fullPathDir);
            GetPlatformInfo(LASTSELECTED);
        }

        private void SwapProject(bool shouldSwap) {
            logger("-> SwapProject(", shouldSwap, ")");
            if (!shouldSwap) {
                logger("No need to swap projects");
                return;
            }
            label1.Text = "Swapping Projects...";
            label1.BackColor = Color.LightGray;
            label1.Refresh();
            StartStop_Button.Enabled = false;

            try {
                //string targetDir = Path.Combine(@"c:\", GetSpecificProjectFolderName(DEFAULT_PROJECT_DIR)); //RUNNING_PROJECT = DEFAULT_PROJECT_DIR
                if (Directory.Exists(RUNNING_PROJECT)) { //The same name already existed !
                    logger("Found existed Project Folder: ", RUNNING_PROJECT);
                    string customTail = "-" + DateTime.Now.ToString("yyyyMMddTHHmmss");
                    string specficProjectFolderFileName = Path.Combine(DEFAULT_PROJECT_DIR, RUNNING_PROJECT.Substring(3));
                    string newSpecificProjectFileName = specficProjectFolderFileName + customTail;
                    //File.Move(specficProjectFolderFileName, newSpecificProjectFileName); //rename the file
                    RUNNING_PROJECT += customTail;
                    logger("New Specific RUNNING_PROJECT: ", RUNNING_PROJECT);
                }

                if (Directory.Exists(DEFAULT_PROJECT_DIR)) {
                    // Make a reference to a directory.
                    //DirectoryInfo di = new DirectoryInfo(DEFAULT_PROJECT_DIR);
                    logger("Start moving current Project!");
                    //di.MoveTo(RUNNING_PROJECT);
                    Directory.Move(DEFAULT_PROJECT_DIR, RUNNING_PROJECT); //Save current active one to specific name
                    logger("Renamed ", DEFAULT_PROJECT_DIR, " -> ", RUNNING_PROJECT);
                } else {
                    logger("Currently no ", DEFAULT_PROJECT_DIR);
                }

                if (!String.IsNullOrEmpty(RUNNING_PROJECT)) {
                    if (comboBox1.FindString(RUNNING_PROJECT) < 1) {
                        comboBox1.Items.Add(RUNNING_PROJECT); //Add it back to selection list so it can be selectable  
                        logger("Added to selection list: ", RUNNING_PROJECT);
                    } else {
                        logger(RUNNING_PROJECT, " already exists in selection list !");
                    }
                } else {
                    logger("First time not default project selected, and default project does not exist");
                }                

                Directory.Move(LASTSELECTED, DEFAULT_PROJECT_DIR); //Make specific seleted on to active one
                logger("Renamed ", LASTSELECTED, " -> ", DEFAULT_PROJECT_DIR);
                CreateSpecificProjectFolderNameFile(LASTSELECTED);

                StartStop_Button.Enabled = true; //Finished swapping
                needSwap = false;
                //current SOFTREV
                SOFTREV = System.Diagnostics.FileVersionInfo.GetVersionInfo(DEFAULT_MAIN_FILE).ProductVersion;
                logger("Now SOFTREV: ", SOFTREV);
            } catch (IOException ioe) {
                //if (ioe.GetBaseException().Source == "mscorlib"){
                //    DialogResult dialogResult = MessageBox.Show("Close all FileExplore Windows?", "Ms mscorlib exception", MessageBoxButtons.YesNo);
                //    if (dialogResult == DialogResult.Yes)
                //    {
                //        //do something
                //        foreach (Process p in Process.GetProcessesByName("explorer")) //Make sure no any open explorer blocks the Directory.Move below
                //        {
                //            p.Kill();
                //        }
                //        SwapProject(true);
                //    }
                //    else if (dialogResult == DialogResult.No)
                //    {
                //        //do something else
                //        System.Environment.Exit(1); //O: good, 1:error exit
                //    }
                //    MessageBox.Show("A file or folder is opened. Please close all files and windows explorer before retry!");
                //    logger("Ms mscorlib exception. Dir is not free to change. Can't swap projects");
                //    System.Environment.Exit(1); //O: good, 1:error exit
                //}
                if (ioe.GetBaseException().Source == "mscorlib") {
                    MessageBox.Show("A file or folder is opened. Please close all files and windows explorer before retry!");
                    logger("Ms mscorlib exception. Dir is not free to change. Can't swap projects");
                    foreach (Process p in Process.GetProcessesByName("explorer")) //Make sure no any open explorer blocks the Directory.Move below
                    {
                        p.Kill();
                    }
                    System.Environment.Exit(1); //O: good, 1:error exit
                }
            } catch (Exception ex) {
                logger("Could not change ", DEFAULT_PROJECT_DIR, " -> ", RUNNING_PROJECT);
                logger("Exception: ", ex.GetType().Name, " Base EX source: ", ex.GetBaseException().Source);
                throw;
            }
        }
        private void CreateSpecificProjectFolderNameFile(string folderName) {
            logger("-> CreateSpecificProjectFolderNameFile(", folderName, ")");
            string fileName = Path.Combine(DEFAULT_PROJECT_DIR, folderName.Substring(3));
            logger("fileName: ", fileName);
            if (!File.Exists(fileName)) {
                File.CreateText(fileName);
                logger("The file does not exist. Created a new file for specific folder name.");
            } else {
                logger("Specific folder name file already exists.");
            }
        }
        //Return either as is existed specific one found 
        //or modified one with additonal timestamp 
        //or newly made specific with standard naming 
        private string GenerateSpecificProjectFolder(string specificProjectFolder) {
            logger("-> GenerateSpecificProjectFolder(", specificProjectFolder, ")");
            SPECIFIC_PROJ_DIR_NAME = GetSpecificProjectFolderName(specificProjectFolder);
            logger("SPECIFIC_PROJ_DIR_NAME: ", SPECIFIC_PROJ_DIR_NAME);

            string createdFile = "";

            if (specificProjectFolder != DEFAULT_PROJECT_DIR) {
                bool found = false;
                if (!string.IsNullOrEmpty(SPECIFIC_PROJ_DIR_NAME)) {
                    Regex r = new Regex(SPECIFIC_PROJ_DIR_NAME, RegexOptions.IgnoreCase);
                    Match m = r.Match(specificProjectFolder);
                    found = m.Success;
                }

                if (!found) {
                    createdFile = Path.Combine(specificProjectFolder, specificProjectFolder.Substring(3));
                }
            } else {
                if (string.IsNullOrEmpty(RC_CONFIG_FILE)) {
                    GetPlatformInfo(specificProjectFolder);
                }

                if (string.IsNullOrEmpty(SPECIFIC_PROJ_DIR_NAME)) { //LASTSELECTED does not have specific project folder name yet.
                    SPECIFIC_PROJ_DIR_NAME = GenerateStandardFormatProjectFolderName();
                    createdFile = Path.Combine(specificProjectFolder, SPECIFIC_PROJ_DIR_NAME);
                }
            }

            if (!string.IsNullOrEmpty(createdFile)) {
                var myFile = File.CreateText(createdFile);
                logger("createdFile: ", createdFile);
                myFile.Close(); //Must close it; otherwise, might cause trouble for other file/folder changes at later time    

            } else {
                logger("Already exists file: ", SPECIFIC_PROJ_DIR_NAME);
            }

            if (specificProjectFolder == DEFAULT_PROJECT_DIR) {
                RUNNING_PROJECT = Path.Combine(@"c:\", SPECIFIC_PROJ_DIR_NAME);
                logger("Set RUNNING_PROJECT: ", RUNNING_PROJECT);
            }

            return SPECIFIC_PROJ_DIR_NAME;
        }
        // Either empty (very unlikey and must be troubleshooting)
        // or a custom string returned for standadizing specific project folder name to use
        private string GenerateStandardFormatProjectFolderName() {
            logger("-> GenerateStandardFormatProjectFolderName()");
            string line;
            string config = "";
            string oldFile = "";
            //string oldSpecificFolderName = "";
            string customer = "None";
            char[] charsToTrim = { ' ' };
            string specificFoler = "";

            try {
                StreamReader file = new StreamReader(Path.Combine(DEFAULT_PROJECT_DIR, "ConfigVersion.txt"));
                while ((line = file.ReadLine()) != null) {
                    config = line.Trim(charsToTrim);
                    //oldSpecificFolderName = "Project_" + SOFTREV + "_" + config;
                    //oldFile = Path.Combine(DEFAULT_PROJECT_DIR, oldSpecificFolderName);
                    string platformID = config.Substring(0, config.IndexOf('-'));
                    logger("platformID: ", platformID);
                    string sourcePath = @"\\asmusfs01\Eagle-I Software\Software Configurations";
                    string[] dirs = Directory.GetDirectories(sourcePath, "*", SearchOption.TopDirectoryOnly);
                    logger("Querying sourcePath: ", sourcePath);

                    foreach (string dir in dirs) {
                        logger("Working on ...", dir);
                        bool found = false;
                        string[] sdirs = Directory.GetDirectories(dir, "*", SearchOption.TopDirectoryOnly);
                        foreach (string sdir in sdirs) {
                            Match m = Regex.Match(sdir, platformID, RegexOptions.IgnoreCase);
                            found = m.Success;
                            if (found) {
                                customer = dir.Substring(sourcePath.Length+1);
                                logger("FOUND: ", sdir);
                                logger("customer: ", customer);
                                SOFTREV = System.Diagnostics.FileVersionInfo.GetVersionInfo(DEFAULT_MAIN_FILE).ProductVersion;
                                //oldSpecificFolderName = "Project_" + SOFTREV + "_" + config;
                                oldFile = Path.Combine(DEFAULT_PROJECT_DIR, RUNNING_PROJECT.Substring(3));
                                specificFoler = "Project_" + customer + "_" + SOFTREV + "_" + config;
                                break;
                            }
                        }
                        if (found)
                            break;
                    }
                }
                file.Close();
                specificFoler = "Project_" + customer + "_" + SOFTREV + "_" + config;
               
                //Thread.Sleep(1000);
                logger("RUNNING_PROJECT: ", RUNNING_PROJECT);
                logger("specificFoler: ", specificFoler);

                string newFile = Path.Combine(DEFAULT_PROJECT_DIR, specificFoler);
                var myFile = File.CreateText(newFile);
                myFile.Close(); //Must close it; otherwise, might cause trouble for other file/folder changes at later time    
                logger("created specificFoler File: ", specificFoler);

                if (File.Exists(oldFile) && oldFile != newFile) {
                    File.Delete(oldFile);
                    logger("Deleted oldFile: ", oldFile);
                    comboBox1.Items.Remove(RUNNING_PROJECT);
                    comboBox1.Refresh();
                } else {
                    logger("Not deleting file: ", oldFile);
                }

            } catch (Exception e) {
                logger("The process failed: {0}", e.ToString());
            }

            return specificFoler;
        }

        private void StartStop_Click(object sender, EventArgs e) {
            logger("\t>>>> Button clicked: ", StartStop_Button.Text, " <<<<");
            string eiStatus;
            bool RcSimOption = false;
            SetRCSIMFeatureVisible(RcSimOption);

            logger("RUNNING_PROJECT: ", RUNNING_PROJECT, " vs. LASTSELECTED: ", LASTSELECTED);

            if (StartStop_Button.Text == "START") { //There was a click on "START" button
                if (comboBox1.Enabled)
                    comboBox1.Enabled = false; //No new selection allowed at this time.

                //if (RUNNING_PROJECT == LASTSELECTED)
                if (isSelectedProjectCurrentProject()) {
                    logger("The selected one is the current active project: ", LASTSELECTED);
                    if (IsMainRunning()) { //EI is currently running. So we need to terminate the project
                        MessageBox.Show(LASTSELECTED + " is currently running !");
                        logger(LASTSELECTED, " is running but was selected to START!");
                    } else {
                        //StartMainExecutableFile(DEFAULT_PROJECT_DIR + " : " + RUNNING_PROJECT.Substring(3));
                        StartMainExecutableFile(RUNNING_PROJECT);
                        GetPlatformInfo(DEFAULT_PROJECT_DIR);
                    }
                } else if (!IsMainRunning()) {
                    logger("The selected one is the new project to run: ", LASTSELECTED);
                    SwapProject(needSwap); //Swap only if the selected one is NOT the current c:\Project one
                    StartMainExecutableFile(LASTSELECTED);
                } else {
                    logger("Switching to the new project to run: ", LASTSELECTED);
                    if (Directory.Exists(DEFAULT_PROJECT_DIR)) {
                        GetPlatformInfo(DEFAULT_PROJECT_DIR);

                        if (RUNNING_PROJECT == DEFAULT_PROJECT_DIR ||
                            string.IsNullOrEmpty(RUNNING_PROJECT) && DEFAULT_PROJECT_DIR == LASTSELECTED) {
                            RUNNING_PROJECT = Path.Combine(@"c:\", GenerateStandardFormatProjectFolderName());
                            logger("Created RUNNING_PROJECT:", RUNNING_PROJECT);
                            var myFile = File.CreateText(RUNNING_PROJECT.Insert(3, @"Project\"));
                            logger("Created SPECIFIC_PROJ_DIR_NAME for RUNNING_PROJECT: ", RUNNING_PROJECT);
                            myFile.Close(); //Must close it; otherwise, might cause trouble for other file/folder changes at later time 
                        }
                    }

                    if (IsMainRunning()) { //EI is currently running. So we need to terminate the current project first
                        StopMainExecutableFile(RUNNING_PROJECT);
                    }

                    SwapProject(needSwap); //Swap only if the selected one is NOT the current c:\Project one
                    StartMainExecutableFile(LASTSELECTED);
                }
                label1.Text = CombinedString(RUNNING_PROJECT, " is running");
                label1.Refresh();
                StartStop_Button.Text = "STOP";
                StartStop_Button.BackColor = Color.Salmon;
                comboBox1.SelectedItem = DEFAULT_PROJECT_DIR;
                comboBox1.Refresh();
                eiStatus = "EI START";
            } else { //There was a click on "STOP" button
                needSwap = true; //Ready for newly possible project change in user's selection 
                if (LASTSELECTED == DEFAULT_PROJECT_DIR)
                    //StopMainExecutableFile(LASTSELECTED + " : " + RUNNING_PROJECT.Substring(3));
                    StopMainExecutableFile(RUNNING_PROJECT);
                else
                    StopMainExecutableFile(LASTSELECTED);

                StartStop_Button.Text = "START";
                StartStop_Button.BackColor = Color.LawnGreen;
                comboBox1.SelectedItem = DEFAULT_PROJECT_DIR;

                label1.Text = "Select Project To Run ...";
                comboBox1.Enabled = true;
                RcSimOption = true;
                RCSIMSelection(false);
                eiStatus = "EI STOP";
            }

            //GetPlatformInfo(DEFAULT_PROJECT_DIR);
            SetRCSIMFeatureVisible(RcSimOption);
            logger("*********************************************");
            logger("***********  ", eiStatus, " Completed!!! **********");
            logger("*********************************************");
        }

        private bool isSelectedProjectCurrentProject() {
            if (LASTSELECTED == RUNNING_PROJECT || LASTSELECTED == DEFAULT_PROJECT_DIR) {
                return true;
            } else {
                return false;
            }
        }

        private void StartMainExecutableFile(string project) {
            logger("-> StartMainExecutableFile(", project, ")");
            StartStop_Button.Enabled = false;
            StartStop_Button.BackColor = Color.LightGray;
            StartStop_Button.Refresh();
            label1.Text = CombinedString("Starting ", project);
            label1.BackColor = Color.LightGray;
            label1.Refresh();
            StartStop_Button.IsAccessible = false;

            Process mainProcess = new Process();
            mainProcess.StartInfo.FileName = DEFAULT_MAIN_FILE;
            mainProcess.EnableRaisingEvents = true;
            //mainProcess.Exited += new EventHandler(mainProcess_Exited);
            mainProcess.Start();
            Thread.Sleep(60000);
            StartStop_Button.Enabled = true;
            label1.Text = "Select Project To Run ...";
            label1.BackColor = Color.LightBlue;
            label1.Refresh();
            RUNNING_PROJECT = LASTSELECTED;
            logger("Updated RUNNING_PROJECT to ", RUNNING_PROJECT);
        }

        private void StopMainExecutableFile(string project) {
            logger("-> StopMainExecutableFile(", project, ")");
            StartStop_Button.Enabled = false;
            StartStop_Button.BackColor = Color.LightGray;
            StartStop_Button.Refresh();
            label1.Text = CombinedString("Stopping ", project);
            label1.BackColor = Color.LightGray;
            label1.Refresh();

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.FileName = "cmd.exe";
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = "/c taskkill /F /T /IM Main.exe";
            Process stopProcess = Process.Start(startInfo);
            stopProcess.EnableRaisingEvents = true;
            stopProcess.Exited += new EventHandler(mainProcess_Exited);
            stopProcess.WaitForExit();
            stopProcess.Close();
            Thread.Sleep(5000);
            StartStop_Button.Enabled = true;
            label1.Text = "Select Project To Run ...";
            label1.BackColor = Color.LightBlue;
            label1.Refresh();
        }

        private void mainProcess_Exited(object sender, System.EventArgs e) {
            logger("Main.exe Stopped!");
            //StartStop_Button.Text = "Start";
        }

        //private void EndProcessTree(string imageName)
        //{
        //    logger("Starts EndProcessTree!");
        //    Process.Start(new ProcessStartInfo
        //    {
        //        FileName = "taskkill",
        //        Arguments = @"/im {imageName} /f /t",
        //        CreateNoWindow = true,
        //        UseShellExecute = false
        //    }).WaitForExit();
        //}

        private void logger(params object[] items) {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(Directory.GetCurrentDirectory() + @"\ASMControlPanel.log", true)) {
                //line.ToList().ForEach(i => string.Join);
                file.WriteLine(CombinedString(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.ff"), " ", string.Join("", items)));
                Debug.WriteLine(CombinedString(DateTime.Now.ToString(), " ", string.Join("", items)));
            }
        }

        private void Form1_Load_1(object sender, EventArgs e) {
            this.Location = new System.Drawing.Point(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Right - this.Width, 0);
        }

        private void label2_Click(object sender, EventArgs e) {
            logger("-> label2_Click()");
            //Dictionary<string, string> myDrives = GetMappedDrives();
            //GetMappedDrives().ToList().ForEach(kv => logger(kv.Key));
            foreach (KeyValuePair<string, string> drive in GetMappedDrives()) {
                string driveLetter = drive.Key;
                string driveServer = drive.Value;
                logger("driveLetter: ", driveLetter, " driveServer: ", driveServer);
            }
        }

        private void checkBoxRC1_CheckedChanged(object sender, EventArgs e) {
            logger("->checkBoxRC1_CheckedChanged()");
            SELECTED_RC = "PMC1";
            MODULE = ".p1f.txt";
            if (StartStop_Button.Text == "START")
                RCCheckBoxManager(checkBoxRC1.Checked, PLATFORM + MODULE, e);
        }

        private void checkBoxRC2_CheckedChanged(object sender, EventArgs e) {
            logger("->checkBoxRC2_CheckedChanged()");
            SELECTED_RC = "PMC2";
            MODULE = ".p2f.txt";
            if (StartStop_Button.Text == "START")
                RCCheckBoxManager(checkBoxRC2.Checked, PLATFORM + MODULE, e);
        }

        private void checkBoxRC3_CheckedChanged(object sender, EventArgs e) {
            logger("->checkBoxRC3_CheckedChanged()");
            SELECTED_RC = "PMC3";
            MODULE = ".p3f.txt";
            if (StartStop_Button.Text == "START")
                RCCheckBoxManager(checkBoxRC3.Checked, PLATFORM + MODULE, e);
        }

        private void checkBoxRC4_CheckedChanged(object sender, EventArgs e) {
            logger("->checkBoxRC4_CheckedChanged()");
            SELECTED_RC = "PMC4";
            MODULE = ".p4f.txt";
            if (StartStop_Button.Text == "START")
                RCCheckBoxManager(checkBoxRC4.Checked, PLATFORM + MODULE, e);
        }

        private void RCCheckBoxManager(bool option, string configFile, EventArgs e) { //false: Reset, true: Set
            logger("-> RCCheckBoxManager(", option, "): ", SELECTED_RC);

            SELECTED_RC_BIN_DIR = Path.Combine(DEFAULT_PROJECT_DIR, SELECTED_RC, "Bin");
            RC_CONFIG_FILE = Path.Combine(DEFAULT_PROJECT_DIR, DEFAULT_CONFIG_DIR, configFile);

            SetRCSIMFeatureVisible(true);
            PrepareRCSimulation(option);
            //PMSimulation(option);

            if (option) {
                comboBox1.Enabled = false;
                if (checkBoxRC1.CheckState == 0 || (SELECTED_RC == "PMC1" && !File.Exists(RC_CONFIG_FILE))) {
                    checkBoxRC1.Enabled = false;
                }

                if (checkBoxRC2.CheckState == 0 || (SELECTED_RC == "PMC2" && !File.Exists(RC_CONFIG_FILE))) {
                    checkBoxRC2.Enabled = false;
                }

                if (checkBoxRC3.CheckState == 0 || (SELECTED_RC == "PMC3" && !File.Exists(RC_CONFIG_FILE))) {
                    checkBoxRC3.Enabled = false;
                }

                if (checkBoxRC4.CheckState == 0 || (SELECTED_RC == "PMC4" && !File.Exists(RC_CONFIG_FILE))) {
                    checkBoxRC4.Enabled = false;
                }

                if (Directory.Exists(DEFAULT_PROJECT_DIR) && StartStop_Button.Text == "START")
                    StartStop_Click(this, e);
            }
        }

        private void PMSimulation(bool option) { //true: enable PM simulation, false: disable 
            logger("->PMSimulation:", option);
            if (!option) {
                StartKillRCBatFile();
                ModifyMCIFSimFile("=False", "=True"); //disable option selected
            } else {
                if (File.Exists(WNETWAKEUP_FILE)) {
                    logger(WNETWAKEUP_FILE, " already exists!");
                } else {
                    logger(WNETWAKEUP_FILE, " is coppied now!");
                    File.Copy(Path.Combine(SELECTED_RC_BIN_DIR, "WNetWakeUp.exe"), Path.Combine(DEFAULT_RCSIM_DIR, "WNetWakeUp.exe"), true);
                }
                ShareProjectFolder();
                StartWNetWakeUpProcess();
            }
        }

        private bool IsPMSimReady() {
            logger("->IsPMSimReady()");
            bool isIdentical = false;

            if (File.Exists(Path.Combine(DEFAULT_RCSIM_DIR, SELECTED_RC + "_Sim.txt"))) {
                if (Directory.Exists(DEFAULT_RCSIM_DIR)) {
                    logger(DEFAULT_RCSIM_DIR, " already exists ! Comparing files !");

                    System.IO.DirectoryInfo sourcePathInfo = new System.IO.DirectoryInfo(SELECTED_RC_BIN_DIR);
                    System.IO.DirectoryInfo targetPathINfo = new System.IO.DirectoryInfo(DEFAULT_RCSIM_DIR);

                    // Take a snapshot of the file system.  
                    IEnumerable<System.IO.FileInfo> sourceList = sourcePathInfo.GetFiles("*.exe", System.IO.SearchOption.AllDirectories);
                    IEnumerable<System.IO.FileInfo> targetList = targetPathINfo.GetFiles("*.exe", System.IO.SearchOption.AllDirectories);

                    //A custom file comparer defined below  
                    FileCompare myFileCompare = new FileCompare();

                    // This query determines whether the two folders contain  
                    // identical file lists, based on the custom file comparer  
                    // that is defined in the FileCompare class.  
                    // The query executes immediately because it returns a bool.  
                    isIdentical = sourceList.SequenceEqual(targetList, myFileCompare);                    
                }
            }
            logger("Are they identical? ", isIdentical);
            return isIdentical;
        }

        void PrepareRCSimulation(bool option) { //true: enable PM simulation, false: disable 
            logger("-> PrepareRCSimulation(", option, ")");
            IS_RCSIM = option;
            string specificExt = "_Sim.txt"; //ALD & EPI

            if (PLATFORM == "Synergis")
                specificExt = "_Sim_Local.txt"; //This file does not have "PM-LINK" line. PM-HSMS uses local loopback IP instead UPC's IP.

            if (!option) {
                StartKillRCBatFile();
                ModifyMCIFSimFile("=False", "=True"); //No RC simulation checked so disable it
 // comment this block to let it every time deleting existing files under c:\ASM\Project\Bin to copy new files
            } else if (IsPMSimReady()) {
                logger("PMSim Previously Existed and Ready!");
                ShareProjectFolder();
                ModifyMCIFSimFile("=True", "=False"); //RC simulation option checked so enable it

                if (!File.Exists(STARTPMBAT)) {
                    CreatePMStartBatFile();
                }
                //RCSIM_FILE = Path.Combine(DEFAULT_PROJECT_DIR, SELECTED_RC, "Bin", SELECTED_RC + specificExt);
                //StartWNetWakeUpProcess();
                RunStartPMBatFile();                
            } else {
                if (LASTSELECTED != RUNNING_PROJECT && LASTSELECTED != DEFAULT_PROJECT_DIR) {
                    logger("Attempt running PM Sim on newly switching project");
                    SwapProject(needSwap);
                }

                ModifyMCIFSimFile("=True", "=False"); //RC simulation option checked so enable it

                if (Directory.Exists(DEFAULT_RCSIM_DIR)) {
                    logger(DEFAULT_RCSIM_DIR, " already exists ! Delete existing files !");
                    Directory.GetFiles(DEFAULT_RCSIM_DIR).ToList().ForEach(f => File.Delete(Path.GetFullPath(f)));
                } else {
                    logger(DEFAULT_RCSIM_DIR, " did not exist !");
                    Directory.CreateDirectory(DEFAULT_RCSIM_DIR);
                }

                //if (!Directory.Exists(DEFAULT_RCSIM_DIR)) {
                //    logger(DEFAULT_RCSIM_DIR, " did not exist !");
                //    Directory.CreateDirectory(DEFAULT_RCSIM_DIR);
                //}

                logger("RC_CONFIG_FILE: ", RC_CONFIG_FILE);
                StreamReader file = new StreamReader(RC_CONFIG_FILE);
                Regex r = new Regex(@"Type of Equipment\s+=\s(.*)", RegexOptions.IgnoreCase);
                string line;

                while ((line = file.ReadLine()) != null) {
                    Match m = r.Match(line);
                    if (m.Success) {
                        RC_TYPE = m.Groups[1].Value;
                        logger("RC_TYPE: ", RC_TYPE);
                        PMSIMFILE_DIR = Path.Combine(PMSIMFILE_DIR, RC_TYPE.ToUpper());
                        logger("PMSIMFILE_DIR: ", PMSIMFILE_DIR);
                        break;
                    }
                }
                file.Close();

                PMSIMFILE_FROM = Path.Combine(PMSIMFILE_DIR, SELECTED_RC + specificExt);
                logger("PMSIMFILE_FROM: ", PMSIMFILE_FROM);
                RCSIM_FILE = Path.Combine(DEFAULT_PROJECT_DIR, SELECTED_RC, "Bin", SELECTED_RC + specificExt);
                logger("RCSIM_FILE: ", RCSIM_FILE);
                File.Copy(PMSIMFILE_FROM, RCSIM_FILE, true);
                GetRCSimProcessesFromFile(RCSIM_FILE);
                CreateKillPMCBatFile();
                //DirectoryInfo dirInfo = new DirectoryInfo(@".\ASM");
                //dirInfo.GetFiles().ToList().ForEach(f => f.CopyTo(Path.Combine(DEFAULT_RCSIM_DIR, f.Name), true));
                //File.Copy(Path.Combine(SELECTED_RC_BIN_DIR, "WNetWakeUp.exe"), Path.Combine(DEFAULT_RCSIM_DIR, "WNetWakeUp.exe"), true);
                //logger(Path.Combine(SELECTED_RC_BIN_DIR, "WNetWakeUp.exe"));
                if (File.Exists(WNETWAKEUP_FILE)) {
                    logger(WNETWAKEUP_FILE, " already exists!");
                } else {
                    logger(WNETWAKEUP_FILE, " is coppied now!");
                    File.Copy(Path.Combine(SELECTED_RC_BIN_DIR, "WNetWakeUp.exe"), Path.Combine(DEFAULT_RCSIM_DIR, "WNetWakeUp.exe"), true);
                }
                ShareProjectFolder();
                StartWNetWakeUpProcess();
            }
        }

        private void StartAsmUtilsBatFile(string toolName, string cmd) {
            logger("-> StartAsmUtilsBatFile(", toolName, ")");
            label1.Text = "Starting AsmUtils.bat";
            string AsmUtils_Arg = CombinedString(cmd, " ", toolName);
            Process mainProcess = new Process();
            mainProcess.StartInfo.FileName = ASM_UTILS_FILE;
            mainProcess.StartInfo.Arguments = AsmUtils_Arg;
            mainProcess.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();
            mainProcess.EnableRaisingEvents = true;
            mainProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //mainProcess.Exited += new EventHandler(mainProcess_Exited);
            mainProcess.Start();

            //if (StartStop_Button.Text == "STOP")
            //    label1.Text = CombinedString(RUNNING_PROJECT, " is running");
            //else
            //    label1.Text = "WNetWakeUp started. Click START to run your Project!";
        }

        private void RunStartPMBatFile() {
            logger("-> RunStartPMBatFile()");
            label1.Text = "Starting " + STARTPMBAT;
            Process mainProcess = new Process();
            mainProcess.StartInfo.FileName = STARTPMBAT;
            mainProcess.StartInfo.WorkingDirectory = DEFAULT_RCSIM_DIR;
            mainProcess.EnableRaisingEvents = true;
            //mainProcess.Exited += new EventHandler(mainProcess_Exited);
            mainProcess.Start();
        }

        private void StartKillRCBatFile() {
            logger("-> StartKillRCBatFile()");
            label1.Text = "Starting KillRC.Bat";
            Process mainProcess = new Process();
            mainProcess.StartInfo.FileName = KILL_RC_BAT_FILE;
            mainProcess.StartInfo.WorkingDirectory = DEFAULT_RCSIM_DIR;
            mainProcess.EnableRaisingEvents = true;
            //mainProcess.Exited += new EventHandler(mainProcess_Exited);
            mainProcess.Start();
            IS_RCSIM = false;
            logger("IS_RCSIM: ", IS_RCSIM);
        }

        private void RCSIMSelection(bool selection) { //false : unchecked, true: checked
            logger("-> RCSIMSelection(", selection, ")");
            switch (SELECTED_RC) {
                case "PMC1":
                    checkBoxRC1.Checked = selection;
                    break;
                case "PMC2":
                    checkBoxRC2.Checked = selection;
                    break;
                case "PMC3":
                    checkBoxRC3.Checked = selection;
                    break;
                case "PMC4":
                    checkBoxRC4.Checked = selection;
                    break;
            }
        }

        private void SetRCSIMFeatureVisible(bool state) { //false: invisible, true: visible
            logger("-> SetRCSIMFeatureVisible(", state, ")");
            //RCSIMSelection(true);
            label2.Enabled = state;
            checkBoxRC1.Enabled = state & RC1EXIST;
            checkBoxRC2.Enabled = state & RC2EXIST;
            checkBoxRC3.Enabled = state & RC3EXIST;
            checkBoxRC4.Enabled = state & RC4EXIST;
        }

        private void StartWNetWakeUpProcess() {
            logger("-> StartWNetWakeUpProcess()");
            label1.Text = "Starting WNetWakeUp.exe";
            string WNETWAKEUP_ARG = CombinedString(@" -Remote \\127.0.0.1 -SharPath Project$ -", SELECTED_RC, " -f ", RCSIM_FILE);
            logger("WNETWAKEUP_ARG = ", WNETWAKEUP_ARG);
            Process mainProcess = new Process();
            mainProcess.StartInfo.FileName = WNETWAKEUP_FILE;
            mainProcess.StartInfo.Arguments = WNETWAKEUP_ARG;
            mainProcess.StartInfo.WorkingDirectory = DEFAULT_RCSIM_DIR;
            mainProcess.EnableRaisingEvents = true;
            //mainProcess.Exited += new EventHandler(mainProcess_Exited);
            mainProcess.Start();

            if (StartStop_Button.Text == "STOP")
                label1.Text = CombinedString(RUNNING_PROJECT, " is running");
            else
                label1.Text = "WNetWakeUp started. Click START to run your Project!";
        }

        private void CreatePMStartBatFile() {
            logger("-> CreatePMStartBatFile()");
            using (StreamWriter outputFile = new StreamWriter(STARTPMBAT, false)) {
                RCSIM_FILE = Path.Combine(DEFAULT_RCSIM_DIR, SELECTED_RC + "_Sim.txt");
                logger("RCSIM_FILE: ", RCSIM_FILE);
                string[] lines = File.ReadAllLines(RCSIM_FILE);
                Regex r = new Regex(@".*?,(.*?exe)\s+,.*?,.*?,(.*?),.*", RegexOptions.IgnoreCase);
                outputFile.WriteLine(@"@echo off");
                outputFile.WriteLine(@"cd " + DEFAULT_RCSIM_DIR);
                for (int i = 0; i < lines.Length; i++) {
                    string line = lines[i];
                    Match m = r.Match(line);
                    if (m.Success) {
                        string command = m.Groups[1].Value;
                        logger("command: ", command);
                        string param = m.Groups[2].Value;
                        logger("params: ", param);
                        outputFile.WriteLine("start " + command + " " + param);
                    }
                }
            }
        }

        private void CreateKillPMCBatFile() {
            logger("-> CreateKillPMCBatFile()");
            using (StreamWriter outputFile = new StreamWriter(KILL_RC_BAT_FILE, false)) {
                outputFile.WriteLine(@"@echo off");
                outputFile.WriteLine(@"taskkill /f /im WNetWakeUp.exe");
                outputFile.WriteLine(CombinedString("for /f ", "\"skip=1 tokens=2 delims=, eol=#\" ", "%%G IN (", RCSIM_FILE, ") do taskkill /f /im %%G"));
            }
        }

        private void ModifyMCIFSimFile(string from, string to) {
            logger("-> ModifyMCIFSimFile(", from, " -> ", to, ")");

            string[] lines = File.ReadAllLines(DEFAULT_MCIFSim_FILE);
            string[] updatedLines = new string[lines.Length];

            for (int i = 0; i < lines.Length; i++) {
                string line = lines[i];
                //logger("line: ", line);

                if (line == CombinedString(SELECTED_RC, from))
                    line = CombinedString(SELECTED_RC, to);
                else if (line == CombinedString("SUS", from))
                    line = CombinedString("SUS", to);

                updatedLines[i] = line;
                //logger("Updated line: ", line);
            }
            File.WriteAllLines(DEFAULT_MCIFSim_FILE, updatedLines);
        }

        private void GetRCSimProcessesFromFile(string rcSimFile) {
            logger("-> GetRCSimProcessesFromFile(", rcSimFile, ")");
            using (StreamReader inFile = new StreamReader(rcSimFile, false)) {
                RC_PROCESSES.Clear(); //Clear to update
                string line = inFile.ReadLine(); //throw away the header line
                while ((line = inFile.ReadLine()) != null) {
                    //logger("Processing line: ", line);
                    string processFile = line.Split(',').Where(i => i.Contains(".exe")).FirstOrDefault().Trim();
                    RC_PROCESSES.Add(processFile);
                    //logger("\tGot processFile: ", processFile);
                }
                logger("RC_PROCESSES: ", String.Join(", ", RC_PROCESSES));
            }
        }

        private string CombinedString(params object[] items) {
            return string.Join("", items);
        }

        void ShareProjectFolder() {
            logger("-> ShareProjectFolder()");
            int found = new ManagementObjectSearcher("Select * from Win32_LogicalShareSecuritySetting where Name = " + PROJECT_SHARE_NAME).Get().Count;
            if (found < 1) {
                ManagementClass classObj = new ManagementClass("win32_share");
                ManagementBaseObject inParams = classObj.GetMethodParameters("Create");
                inParams.SetPropertyValue("Description", "Shared Project");
                inParams.SetPropertyValue("Name", "Project$");
                inParams.SetPropertyValue("Path", DEFAULT_PROJECT_DIR);
                inParams.SetPropertyValue("Type", 0x0);
                inParams.SetPropertyValue("MaximumAllowed", null);
                inParams.SetPropertyValue("Password", null);
                inParams.SetPropertyValue("Access", null); // Make Everyone has full control access.
                ManagementBaseObject outParams = classObj.InvokeMethod("Create", inParams, null);
                logger(DEFAULT_PROJECT_DIR, " is shared now !");
            } else {
                logger(DEFAULT_PROJECT_DIR, " already shared !");
            }
        }

        private void label3_Click(object sender, EventArgs e) {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e) {
            logger("-> comboBox2_SelectedIndexChanged()");

            SELECTED_MAPTOOL = comboBox2.SelectedItem.ToString();
            logger("SELECTED_MAPTOOL: <", SELECTED_MAPTOOL, ">");
        }

        private void button2_Click(object sender, EventArgs e) {
            logger("-> Map Button Clicked for ", SELECTED_MAPTOOL);
            try {
                String[] predrives = Environment.GetLogicalDrives();

                if (!String.IsNullOrEmpty(SELECTED_MAPTOOL)) {
                    StartAsmUtilsBatFile(SELECTED_MAPTOOL, "-m");
                    button2.Enabled = false;
                }

                DriveInfo[] allDrives = DriveInfo.GetDrives();
                Thread.Sleep(2000);
                String[] postdrives = Environment.GetLogicalDrives();
                IEnumerable<string> aOnlyNumbers = postdrives.Except(predrives);
                foreach (var n in aOnlyNumbers) {
                    logger(SELECTED_MAPTOOL, " mapped to Drive: ", n);
                    label3.Text = CombinedString(SELECTED_MAPTOOL, " mapped to Drive: ", n.Substring(0, 1));
                    button2.Enabled = false; //Map Button
                    button4.Enabled = true; //UnMap Button  
                }
            } catch (Exception ex) {
                logger("EXCEPTION: ", ex.Message);
            }
        }

        private void button4_Click(object sender, EventArgs e) {
            logger("-> UnMap Button Clicked for ", SELECTED_MAPTOOL);

            if (!String.IsNullOrEmpty(SELECTED_MAPTOOL)) {
                String[] predrives = Environment.GetLogicalDrives();
                StartAsmUtilsBatFile(SELECTED_MAPTOOL, "-u");
                Thread.Sleep(2000);
                String[] postdrives = Environment.GetLogicalDrives();
                IEnumerable<string> aOnlyNumbers = predrives.Except(postdrives);
                foreach (var n in aOnlyNumbers) {
                    logger(SELECTED_MAPTOOL, " Unmapped to Drive: ", n);
                    label3.Text = CombinedString(SELECTED_MAPTOOL, " Unmapped to Drive: ", n.Substring(0, 1));
                    button2.Enabled = true; //Map Button
                    button4.Enabled = false; //UnMap Button                    
                }
            }
        }

        private void label3_Click_1(object sender, EventArgs e) {

        }

        private void label4_Click(object sender, EventArgs e) {

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e) {
            SELECTED_VNCTOOL = comboBox3.SelectedItem.ToString();
            logger("-> ToolVncSelected : ", SELECTED_VNCTOOL);
        }

        private void button3_Click(object sender, EventArgs e) {
            logger("-> VNC Button Cicked!");
            StartVncToTool(SELECTED_VNCTOOL);
        }

        private void StartVncToTool(string vncToolFile) {
            logger("-> StartVncToTool(", vncToolFile, ")");
            label4.Text = CombinedString("Starting VNC to ", vncToolFile);
            Process mainProcess = new Process();
            mainProcess.StartInfo.FileName = Path.Combine(Directory.GetCurrentDirectory(), @".\ASM", vncToolFile);
            mainProcess.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();
            mainProcess.EnableRaisingEvents = true;
            //mainProcess.Exited += new EventHandler(mainProcess_Exited);
            mainProcess.Start();
        }

        // This implementation defines a very simple comparison  
        // between two FileInfo objects. It only compares the name  
        // of the files being compared and their length in bytes.  
        class FileCompare : System.Collections.Generic.IEqualityComparer<System.IO.FileInfo> {
            public FileCompare() {
            }

            public bool Equals(System.IO.FileInfo f1, System.IO.FileInfo f2) {
                return (f1.Name == f2.Name &&
                        f1.Length == f2.Length);
            }

            // Return a hash that reflects the comparison criteria. According to the
            // rules for IEqualityComparer<T>, if Equals is true, then the hash codes must  
            // also be equal. Because equality as defined here is a simple value equality, not  
            // reference identity, it is possible that two or more objects will produce the same  
            // hash code.  
            public int GetHashCode(System.IO.FileInfo fi) {
                string s = $"{fi.Name}{fi.Length}";
                return s.GetHashCode();
            }
        }
    }
}
