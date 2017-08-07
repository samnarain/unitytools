/* a backup from Da Viking Code - a scriptable wizard to switch between Unity build targets */

using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.IO;
 
public class FastBuildSwitcher:ScriptableWizard {
 
    public BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
 
    [MenuItem("Tools/Fast Build Switcher")]
    static void CreateWizard () {
 
        ScriptableWizard.DisplayWizard<FastBuildSwitcher>("Switch Platform", "Switch");
    }
 
    void OnWizardCreate() {
 
        //Debug.Log("current platform: " + EditorUserBuildSettings.activeBuildTarget);
        //Debug.Log("next platform: " + buildTarget);
 
        if (EditorUserBuildSettings.activeBuildTarget == buildTarget) {
 
            Debug.LogWarning("You set the same next platform than the current one!");
 
            return;
        }
 
        //save current Library folder state
        if (Directory.Exists("Library-" + EditorUserBuildSettings.activeBuildTarget))
            DirectoryClear("Library-" + EditorUserBuildSettings.activeBuildTarget);
 
        DirectoryCopy("Library", "Library-" + EditorUserBuildSettings.activeBuildTarget, true);
 
        //restore new target Library folder state
        if (Directory.Exists("Library-" + buildTarget)) {
 
            DirectoryClear("Library");
            Directory.Delete("Library", true);
 
            Directory.Move("Library-" + buildTarget, "Library");
        }
 
        EditorUserBuildSettings.SwitchActiveBuildTarget(buildTarget);
    }
 
    void DirectoryClear(string FolderName) {
        DirectoryInfo dir = new DirectoryInfo(FolderName);
 
        foreach(FileInfo fi in dir.GetFiles())
            fi.Delete();
 
        foreach (DirectoryInfo di in dir.GetDirectories()) {
             
            DirectoryClear(di.FullName);
            di.Delete(true);
        }
    }
 
    void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs) {
         
        DirectoryInfo dir = new DirectoryInfo(sourceDirName);
        DirectoryInfo[] dirs = dir.GetDirectories();
 
        // If the source directory does not exist, throw an exception.
        if (!dir.Exists)
            throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName);
 
        // If the destination directory does not exist, create it.
        if (!Directory.Exists(destDirName))
            Directory.CreateDirectory(destDirName);
 
 
        // Get the file contents of the directory to copy.
        FileInfo[] files = dir.GetFiles();
 
        foreach (FileInfo file in files) {
            // Create the path to the new copy of the file.
            string temppath = Path.Combine(destDirName, file.Name);
 
            // Copy the file.
            file.CopyTo(temppath, false);
        }
 
        // If copySubDirs is true, copy the subdirectories.
        if (copySubDirs)
            foreach (DirectoryInfo subdir in dirs) {
                 
                // Create the subdirectory.
                string temppath = Path.Combine(destDirName, subdir.Name);
 
                // Copy the subdirectories.
                DirectoryCopy(subdir.FullName, temppath, copySubDirs);
            }
    }
}
