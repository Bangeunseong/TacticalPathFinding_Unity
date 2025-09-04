using System.IO;
using UnityEditor;
using UnityEngine;

namespace _Tools
{
    public static class Setup
    {
        [MenuItem("Tools/Setup/Create Default Folders")]
        public static void CreateDefaultFolders()
        {
            Folders.CreateDefault("_Project", "Animation", "Artwork", "Materials", "Prefabs", 
                "ScriptableObjects", "Scripts", "Input", "Settings");
            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/Setup/Import Favorite Assets")]
        public static void ImportFavorites()
        {
            Assets.ImportAsset("DOTween HOTween v2.unitypackage", "Demigiant/Editor ExtensionsAnimation");
            Assets.ImportAsset("Serialized Dictionary.unitypackage", "ayellowpaper/Editor ExtensionsUtilities");
        }

        public static void ImportBasics()
        {
            
        }

        static class Folders
        {
            public static void CreateDefault(string root, params string[] folders)
            {
                var fullPath = Path.Combine(Application.dataPath, root);
                foreach (var folder in folders)
                {
                    var path = Path.Combine(fullPath, folder);
                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                }
            }
        }
    }

    static class Assets
    {
        public static void ImportAsset(string asset, string subfolder,
            string folder = "C:/Users/bange/AppData/Roaming/Unity/Asset Store-5.x")
        {
            AssetDatabase.ImportPackage(Path.Combine(folder, subfolder, asset), false);
        }
    }
}