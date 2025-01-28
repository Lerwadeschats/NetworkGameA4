using UnityEngine;
using UnityEditor;
using UnityEditor.Build;

public class Builder
{
    public static int serverVersion = 0;

    public static string serverPath = "/Builds/Serv/Server.exe ";
    public static string clientPath = "/Builds/client/Client.exe ";


    public static readonly string[] serverScenesPath = { "Assets/Server/Scenes/ServerHost.unity", "Assets/Génération/Generation/Scenes/Generation.unity" };
    public static readonly string[] clientScenesPath = { "Assets/Server/Scenes/Client.unity", "Assets/Génération/Generation/Scenes/Generation.unity" };


    public static Texture2D icon;
    public static string serverIconPath = "Assets/Icon/iconfinder-technologymachineelectronicdevice29-4026431_113337.png";
    public static string clientIconPath = "Assets/Icon/pngwing.com.png";

    [MenuItem("Build/Build Server")]
    public static void CreateServerBuild()
    {
        PlayerSettings.productName = "Serv V: 0.0" + serverVersion;

        icon = AssetDatabase.LoadAssetAtPath<Texture2D>(serverIconPath);
        Texture2D[] t = new Texture2D[8] { icon, null, null, null, null, null, null, null };

        PlayerSettings.SetIcons(NamedBuildTarget.Standalone, t,IconKind.Any);
            
        BuildPipeline.BuildPlayer(serverScenesPath, serverPath, BuildTarget.StandaloneWindows, BuildOptions.AutoRunPlayer);

        Debug.Log("Server built in C:/Builds/Server/");
    }


    [MenuItem("Build/Build Client")]
    public static void CreateClientBuild()
    {
        PlayerSettings.productName = "Client V: 0.0" + serverVersion;

        icon = AssetDatabase.LoadAssetAtPath<Texture2D>(clientIconPath);
        Texture2D[] t = new Texture2D[8] { icon, null, null, null, null, null, null, null };

        PlayerSettings.SetIcons(NamedBuildTarget.Standalone, t, IconKind.Any);

        BuildPipeline.BuildPlayer(clientScenesPath, clientPath, BuildTarget.StandaloneWindows, BuildOptions.AutoRunPlayer);

        Debug.Log("Client built in C:/Builds/Client/");
    }


    [MenuItem("Build/Build Both")]
    public static void DoBoth()
    {
        CreateServerBuild();
        CreateClientBuild();
    }




}
