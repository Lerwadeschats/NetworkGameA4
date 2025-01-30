using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
//using System.Diagnostics;

public class Builder
{
    public static int serverVersion = 0;

    public static string serverPath = "/Builds/Server/Server.exe ";
    public static string clientPath = "/Builds/client/Client.exe ";

    public static readonly string[] serverScenesPath = { "Assets/Server/Scenes/ServerHost.unity", "Assets/Génération/Generation/Scenes/Generation.unity" };
    public static readonly string[] clientScenesPath = { "Assets/Server/Scenes/Client.unity", "Assets/Génération/Generation/Scenes/Generation.unity" };

    public static string serverIconPath = "Assets/Icon/iconfinder-technologymachineelectronicdevice29-4026431_113337.png";
    public static string clientIconPath = "Assets/Icon/pngwing.com.png";

    [MenuItem("Build/Build Server")]
    public static void CreateServerBuild()
    {
        PlayerSettings.productName = "Server";

        Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>(serverIconPath);
        Texture2D[] t = new Texture2D[8] { icon, null, null, null, null, null, null, null };

        PlayerSettings.SetIcons(NamedBuildTarget.Standalone, t, IconKind.Any);

        BuildPipeline.BuildPlayer(serverScenesPath, serverPath, BuildTarget.StandaloneWindows64, BuildOptions.AutoRunPlayer);

        Debug.Log("Server built in " + serverPath);
    }


    [MenuItem("Build/Build Client")]
    public static void CreateClientBuild()
    {
        PlayerSettings.productName = "Client";

        Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>(clientIconPath);
        Texture2D[] t = new Texture2D[8] { icon, null, null, null, null, null, null, null };

        PlayerSettings.SetIcons(NamedBuildTarget.Standalone, t, IconKind.Any);

        BuildPipeline.BuildPlayer(clientScenesPath, clientPath, BuildTarget.StandaloneWindows64, BuildOptions.AutoRunPlayer);

        Debug.Log("Client built in " + clientPath);
    }


    [MenuItem("Build/Build Both %g")]
    public static void DoBoth()
    {
        CreateServerBuild();
        CreateClientBuild();
        //UnityEngine.Debug.Log("Server built in " + serverPath);
    }

    [MenuItem("Build/Run Last Server %y")]
    public static void RunServer()
    {
        System.Diagnostics.Process process = new System.Diagnostics.Process();
        process.StartInfo.FileName = "\\Builds\\Server/Server.exe";
        process.Start();
    }

    [MenuItem("Build/Run Last Client %h")]
    public static void RunClient()
    {
        System.Diagnostics.Process process = new System.Diagnostics.Process();
        process.StartInfo.FileName = "\\Builds\\client/Client.exe";
        process.Start();
    }


}
