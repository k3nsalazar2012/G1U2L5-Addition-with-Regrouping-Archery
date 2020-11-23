// c# / unity class
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// MathTekSetup Editor Window
/// set up project
/// </summary>
public class MathTekSetup : EditorWindow
{
    string gameID = "";
    public string[] sceneNames;
    
    [MenuItem("Mathtek/Set Up")]
    public static void ShowWindow()
    {        
        EditorWindow.GetWindow(typeof(MathTekSetup));
    }

    [MenuItem("Mathtek/Clear Cache")]
    public static void ClearCache()
    {
        Caching.ClearCache();
    }

    private void OnGUI()
    {
        minSize = new Vector2(400f, 260f);

        GUILayout.Space(10);
        GUILayout.Label("Add All Scenes Here", EditorStyles.boldLabel);
        ScriptableObject target = this;
        SerializedObject so = new SerializedObject(target);
        SerializedProperty stringsProperty = so.FindProperty("sceneNames");

        EditorGUILayout.PropertyField(stringsProperty, true); // True means show children
        so.ApplyModifiedProperties(); // Remember to apply modified properties

        GUILayout.Space(20);

        gameID = EditorGUILayout.TextField("Game ID", gameID);
        GUILayout.Space(5);

        GUILayout.Space(10);

        GUILayout.Label("Update Directory Names And Assetbundle Names", EditorStyles.boldLabel);

        if (GUILayout.Button("Update Directories and Assetbundle Names"))
        {            
            AssetDatabase.MoveAsset(string.Format("Assets/{0}/Resources/{0}", 1), string.Format("Assets/{0}/Resources/{1}",1,gameID));
            AssetDatabase.MoveAsset("Assets/1", string.Format("Assets/{0}",gameID));

            int count = 1;
            string path;
            string assetBundleName;

            foreach (string s in sceneNames)
            {
                path = string.Format("Assets/{0}/Scenes/{1}.unity", gameID, s);
                assetBundleName = string.Format("{0}/{0}_{1}", gameID, count);
                AssetImporter.GetAtPath(path).SetAssetBundleNameAndVariant(assetBundleName, "");
                count++;
            }
        }


        GUILayout.Space(10);
        GUILayout.Label("Build Assetbundles", EditorStyles.boldLabel);

        if (GUILayout.Button("Build Assetbundles Windows"))
        {
            if(IsReady())
                AssetbundlesWindows();
        }

        if (GUILayout.Button("Build Assetbundles WebGL"))
        {
            if (IsReady())
                AssetbundlesWebGL();
        }

        if (GUILayout.Button("Build Assetbundles Android"))
        {
            if (IsReady())
                AssetbundlesAndroid();
        }

        if (GUILayout.Button("Build Assetbundles iOS"))
        {
            if (IsReady())
                AssetbundlesIOS();
        }
    }

    private void OnEnable()
    {
        sceneNames = new string[] { "Title", "Game" };
    }

    private void AssetbundlesWindows()
    {
        string path = EditorUtility.SaveFolderPanel("Save Bundle", "", "");
        if (path.Length != 0)
        {
            if (Directory.Exists(path + "/Windows")) Directory.Delete(path + "/Windows", true);
            Directory.CreateDirectory(path + "/Windows");
            BuildPipeline.BuildAssetBundles(path + "/Windows", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);

            DirectoryInfo dir = new DirectoryInfo(path + "/Windows/" + gameID + "/");
            ComputeFilesSize(dir);
        }
    }

    private void AssetbundlesWebGL()
    {
        string path = EditorUtility.SaveFolderPanel("Save Bundle", "", "");
        if (path.Length != 0)
        {
            if (Directory.Exists(path + "/WebGL")) Directory.Delete(path + "/WebGL", true);
            Directory.CreateDirectory(path + "/WebGL");
            BuildPipeline.BuildAssetBundles(path + "/WebGL", BuildAssetBundleOptions.None, BuildTarget.WebGL);

            DirectoryInfo dir = new DirectoryInfo(path + "/WebGL/" + gameID + "/");
            ComputeFilesSize(dir);
        }
    }

    private void AssetbundlesAndroid()
    {
        string path = EditorUtility.SaveFolderPanel("Save Bundle", "", "");
        if (path.Length != 0)
        {
            if (Directory.Exists(path + "/Android")) Directory.Delete(path + "/Android", true);
            Directory.CreateDirectory(path + "/Android");
            BuildPipeline.BuildAssetBundles(path + "/Android", BuildAssetBundleOptions.None, BuildTarget.Android);

            DirectoryInfo dir = new DirectoryInfo(path + "/Android/" + gameID + "/");
            ComputeFilesSize(dir);
        }
    }

    private void AssetbundlesIOS()
    {
        string path = EditorUtility.SaveFolderPanel("Save Bundle", "", "");
        if (path.Length != 0)
        {
            if (Directory.Exists(path + "/iOS")) Directory.Delete(path + "/iOS", true);
            Directory.CreateDirectory(path + "/iOS");
            BuildPipeline.BuildAssetBundles(path + "/iOS", BuildAssetBundleOptions.None, BuildTarget.iOS);

            DirectoryInfo dir = new DirectoryInfo(path + "/iOS/" + gameID + "/");
            ComputeFilesSize(dir);
        }
    }

    private void ComputeFilesSize(DirectoryInfo dir)
    {
        FileInfo[] files = dir.GetFiles();
        string filename = @dir.FullName + "size.txt";

        List<long> sizes = new List<long>();
        foreach (var f in files)
        {
            if (f.Extension == "")
            {
                sizes.Add(f.Length);
            }
        }

        long total = 0;
        foreach (var s in sizes)
        {
            total += s;
        }

        string sizeString = total.ToString();

        foreach (var s in sizes)
        {
            sizeString += "," + s;
        }

        Debug.Log("size: " + sizeString);
        File.WriteAllText(filename, sizeString);
    }

    private bool IsReady()
    {
        if (gameID == "")
        {
            Debug.Log("Game ID field must not be null or empty");
            return false;
        }
        else
            return true;
    }
}