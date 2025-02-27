using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[Overlay(typeof(SceneView), id: ID_SCENE_VIEWER_OVERLAY, displayName:"Scene Viewer")]
[Icon("Assets/Sprites/Icons/unity_scene.png")]
public class SceneViewerEditor : Overlay
{
    private const string ID_SCENE_VIEWER_OVERLAY = "sceneViewerOverlay";
 
    public override VisualElement CreatePanelContent()
    {
        var root = new VisualElement
        {
            style =
            {
                width = new StyleLength(new Length(120, LengthUnit.Pixel)),
                fontSize = 10
            }
        };
        CreateSceneButtons(root);

        //Create the panel again when the scene list has been changed.
        EditorBuildSettings.sceneListChanged += () => CreateSceneButtons(root);

        return root;
    }

    private void CreateSceneButtons(VisualElement root)
    {
        root.Clear();
        for (int i = 0; i < EditorSceneManager.sceneCountInBuildSettings; i++)
        {
            int tempIndex = i;

            var sceneButton = new Button(() => ButtonCallback(tempIndex));

            string fileName = Path.GetFileName(SceneUtility.GetScenePathByBuildIndex(tempIndex));

            //Removes the extension part of the file name (e.g: "MainScene.unity" -> "MainScene")
            sceneButton.text = fileName.Substring(0, fileName.Length - 6);

            root.Add(sceneButton);
        }
    }

    private void ButtonCallback(int index)
    {
        if (EditorSceneManager.GetActiveScene().isDirty)
        {
            int dialogResult = EditorUtility.DisplayDialogComplex(
                "Scene has been modified",
                "Do you want to save the changes you made in the current scene?",
                "Save", "Don't Save", "Cancel");

            switch (dialogResult)
            {
                case 0: //Save and open the new scene
                    EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
                    EditorSceneManager.OpenScene(SceneUtility.GetScenePathByBuildIndex(index));
                    break;
                case 1: //Open the new scene without saving current.
                    EditorSceneManager.OpenScene(SceneUtility.GetScenePathByBuildIndex(index));
                    break;
                case 2: //Cancel process (Basically do nothing for now.)
                    break;
                default:
                    Debug.LogWarning("Something went wrong when switching scenes.");
                    break;
            }
        }
        else
        {
            EditorSceneManager.OpenScene(SceneUtility.GetScenePathByBuildIndex(index));
        }
    }
}
