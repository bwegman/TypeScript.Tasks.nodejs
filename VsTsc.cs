using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace TypeScript.Tasks
{
    public class VsTsc : ToolTask
    {
        private List<TaskItem> javascriptOutputs = new List<TaskItem>();

        public new string ToolPath { get; set; }

        public override string ToolExe { get; set; }

        public string Configurations { get; set; }

        public ITaskItem[] FullPathsToFiles { get; set; }

        public string OutFile { get; set; }

        public string OutDir { get; set; }

        public string ProjectDir { get; set; }

        public string TscJs { get; set; }

        [Output]
        public ITaskItem[] GeneratedJavascript
        {
            get
            {
                return (ITaskItem[])this.javascriptOutputs.ToArray();
            }
            set
            {
            }
        }

        protected override string ToolName
        {
            get
            {
                if (string.IsNullOrEmpty(this.ToolExe))
                    this.ToolExe = "node.exe";
                return this.ToolExe;
            }
        }

        protected override string GenerateCommandLineCommands()
        {
            return TscJs ?? "tsc.js";
        }

        protected override Encoding ResponseFileEncoding
        {
            get
            {
                return Encoding.Unicode;
            }
        }

        protected override string GenerateFullPathToTool()
        {
            if (string.IsNullOrEmpty(this.ToolPath))
                this.ToolPath = Path.Combine(VsTsc.GenerateProgramFiles32(), "Microsoft SDKs\\TypeScript\\1.0");
            string path = Path.Combine(this.ToolPath, this.ToolName);
            if (!File.Exists(path))
                this.Log.LogError(Strings.ToolsVersionWarning, new object[1]
                {
                    (object) path
                });
            return path;
        }

        internal static string GenerateProgramFiles32()
        {
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            if (string.IsNullOrEmpty(folderPath))
                folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            return folderPath;
        }

        protected override void LogEventsFromTextOutput(string singleLine, MessageImportance messageImportance)
        {
            Match match = Regex.Match(singleLine, "\\sTS\\d+:\\s", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                int num = match.Index + match.Length;
                singleLine = singleLine.Substring(0, num).Replace('/', '\\') + string.Format("Build: {0}", (object)singleLine.Substring(num));
            }
            base.LogEventsFromTextOutput(singleLine, messageImportance);
        }

        protected override string GenerateResponseFileCommands()
        {
            if (string.IsNullOrEmpty(this.Configurations))
                this.Log.LogWarning(Strings.NoConfigurations, new object[0]);
            StringBuilder stringBuilder = new StringBuilder();
            string referencesFilePath = VsTsc.GetReferencesFilePath(this.ProjectDir);
            foreach (ITaskItem taskItem in this.FullPathsToFiles)
            {
                string metadata = taskItem.GetMetadata("FullPath");
                string str = " \"" + metadata + "\"";
                if (string.Equals(metadata, referencesFilePath, StringComparison.InvariantCultureIgnoreCase))
                {
                    stringBuilder = stringBuilder.Insert(0, str);
                    if (referencesFilePath != null)
                        this.LogEventsFromTextOutput(string.Format("Located references file at: '{0}'", (object)referencesFilePath), MessageImportance.Low);
                }
                else
                    stringBuilder = stringBuilder.Append(str);
            }
            return this.Configurations + ((object)stringBuilder).ToString();
        }

        protected override bool SkipTaskExecution()
        {
            if (this.FullPathsToFiles != null && Enumerable.Count<ITaskItem>((IEnumerable<ITaskItem>)this.FullPathsToFiles) != 0)
                return false;
            this.Log.LogWarning(Strings.NoFilesToCompile, new object[0]);
            return true;
        }

        protected override int ExecuteTool(string pathToTool, string responseFileCommands, string commandLineCommands)
        {
            this.GenerateOutputPaths();
            return base.ExecuteTool(pathToTool, responseFileCommands, commandLineCommands);
        }

        private string ComputeCommonDirectoryPath()
        {
            string str1 = string.Empty;
            string[] strArray1 = (string[])null;
            int num = -1;
            foreach (object obj in this.FullPathsToFiles)
            {
                string str2 = obj.ToString();
                if (!str2.EndsWith(".d.ts"))
                {
                    string[] strArray2 = str2.Split(new char[1]
                    {
                        Path.DirectorySeparatorChar
                    });
                    if (num == -1)
                    {
                        strArray1 = strArray2;
                        num = strArray2.Length;
                    }
                    else
                    {
                        bool flag = false;
                        for (int index = 0; index < num && index < strArray2.Length; ++index)
                        {
                            if (string.Compare(strArray1[index], strArray2[index], true) != 0)
                            {
                                num = index;
                                flag = true;
                                if (index == 0)
                                    return string.Empty;
                                else
                                    break;
                            }
                        }
                        if (!flag && strArray2.Length < num)
                            num = strArray2.Length;
                    }
                }
            }
            string path1 = strArray1[0];
            for (int index = 1; index < num; ++index)
                path1 = Path.Combine(path1, strArray1[index]);
            return path1;
        }

        private void GenerateOutputPaths()
        {
            if (string.IsNullOrEmpty(this.OutDir))
            {
                if (string.IsNullOrEmpty(this.OutFile))
                {
                    this.processFilePathsAndChangeExtensions((string)null);
                }
                else
                {
                    TaskItem taskItem = new TaskItem(this.OutFile);
                    taskItem.SetMetadata("DestinationRelativePath", this.OutFile);
                    this.javascriptOutputs.Add(taskItem);
                }
            }
            else
            {
                string commonDirectoryPath = this.ComputeCommonDirectoryPath();
                if (!string.IsNullOrEmpty(commonDirectoryPath))
                    this.processFilePathsAndChangeExtensions(commonDirectoryPath);
                else
                    this.processFilePathsAndChangeExtensions((string)null);
            }
        }

        private void processFilePathsAndChangeExtensions(string commonDirectoryPathToReplace)
        {
            foreach (object obj in this.FullPathsToFiles)
            {
                string path = obj.ToString();
                if (!path.EndsWith(".d.ts"))
                {
                    if (!string.IsNullOrEmpty(commonDirectoryPathToReplace))
                        path = Path.Combine(this.OutDir, path.Replace(commonDirectoryPathToReplace, ""));
                    string str = Path.ChangeExtension(path, "js");
                    TaskItem taskItem = new TaskItem(str);
                    taskItem.SetMetadata("DestinationRelativePath", str);
                    this.javascriptOutputs.Add(taskItem);
                }
            }
        }

        private static string GetReferencesFilePath(string projectDirectory)
        {
            if (!string.IsNullOrEmpty(projectDirectory))
                return Path.Combine(projectDirectory, "_references.ts");
            else
                return (string)null;
        }
    }
}