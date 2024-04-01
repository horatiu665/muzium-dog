using ToyBoxHHH;
using UnityEngine;
using UnityEngine.UI;

public class VRRecordFrontend : MonoBehaviour
{
    public VRRecordAnimation recordAnimation;

    public Image recordingImage;

    public string animationName = "New Animation";

    [DebugButton]
    public void CreateNewAnimation()
    {
#if UNITY_EDITOR
        // create scriptable object of type VRAnimationData
        VRAnimationData newAnimation = ScriptableObject.CreateInstance<VRAnimationData>();
        newAnimation.name = "New Animation";
        // save the scriptable object
        // ensure folder exists
        string folderPath = "Assets/VRAnimRecording/Animations";
        if (!UnityEditor.AssetDatabase.IsValidFolder(folderPath))
        {
            UnityEditor.AssetDatabase.CreateFolder("Assets/VRAnimRecording", "Animations");
        }
        var filename = animationName;
        // ensure filename is unique, otherwise increment
        int i = 0;
        while (UnityEditor.AssetDatabase.LoadAssetAtPath<VRAnimationData>(folderPath + "/" + filename + ".asset") != null)
        {
            i++;
            filename = animationName + i;
        }

        string path = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(folderPath + "/" + filename + ".asset");
        UnityEditor.AssetDatabase.CreateAsset(newAnimation, path);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
        // assign the new animation to the record animation
        recordAnimation.animationData = newAnimation;
        // ping
        UnityEditor.EditorGUIUtility.PingObject(newAnimation);
#endif

    }

    private void Update()
    {
        if (recordingImage != null)
            recordingImage.enabled = recordAnimation.isRecording;

        if (GetRecordingToggleInputDown())
        {
            recordAnimation.isRecording = !recordAnimation.isRecording;
        }
    }

    private bool GetRecordingToggleInputDown()
    {
        // also get VR controller input???
        return Input.GetKeyDown(KeyCode.R);
    }


}