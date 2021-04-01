using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class EditorShortcuts 
{
    [MenuItem("Tanks/Load menu %q")]
    public static void LoadMenu()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/MenuScene.unity");
    }

    [MenuItem("Tanks/Load lobby %w")]
    public static void LoadLobby()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/LobbyScene.unity");
    }

    [MenuItem("Tanks/Load game %e")]
    public static void LoadGame()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/GameScene.unity");
    }
}
