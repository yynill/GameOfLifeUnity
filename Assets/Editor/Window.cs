using UnityEngine;
using UnityEditor;

public class Window : EditorWindow
{
    private BoardScript boardScript;

    [MenuItem("Window/Input")]
    public static void ShowWindow()
    {
        Window window = GetWindow<Window>("Input");
        window.boardScript = FindObjectOfType<BoardScript>();

        if (window.boardScript == null)
        {
            Debug.Log("BoardScript not found!");
        }
    }


    // Window code and layout
    private void OnGUI()
    {
        GUILayout.Space(10);

        // Input field for int size
        GUILayout.Label("[Cube = Size^3]", EditorStyles.boldLabel);
        boardScript.size = EditorGUILayout.IntSlider("Cube Size: ", boardScript.size, 0, 99);
        GUILayout.Space(10);


        // Frame rate
        GUILayout.Label("Frame Rate", EditorStyles.boldLabel);
        boardScript.targetFrameRate = EditorGUILayout.IntSlider("Frame Rate: ", boardScript.targetFrameRate, 1, 240);
        GUILayout.Space(10);


        // Display actual frame rate
        GUILayout.Label("Actual Frame Rate: " + boardScript.actualFrameRate.ToString("F2") + " FPS");
        GUILayout.Space(10);


        // Life Cells Ratio 
        GUILayout.Label("Alive Cells Ratio", EditorStyles.boldLabel);
        boardScript.liveCellRatio = EditorGUILayout.Slider("Alive Cells Ratio: ", boardScript.liveCellRatio, 0.0f, 1.0f);
        GUILayout.Space(10);

        if (GUILayout.Button("Random Start Config") && !boardScript.isPlaying)
        {
            boardScript.SetRandomStartConfig();
        }

        if (GUILayout.Button("Core Start Config") && !boardScript.isPlaying)
        {
            boardScript.SetCoreStartConfig();
        }

        GUILayout.Space(30);

        // Centered and bold "Game Rules" label
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Game Rules", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        // Dual Slider

        // Survival Rule
        GUILayout.Label("Survival Rule", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Min Value: " + Mathf.Round(boardScript.survicalMinValue));
        GUILayout.FlexibleSpace();
        GUILayout.Label("Max Value: " + Mathf.Round(boardScript.survicalMaxValue));
        GUILayout.EndHorizontal();

        Rect sliderRectSurvival = EditorGUILayout.GetControlRect(false, 20);
        EditorGUI.MinMaxSlider(sliderRectSurvival, ref boardScript.survicalMinValue, ref boardScript.survicalMaxValue, 0, 27);
        GUILayout.Space(10);

        // Birth Rule
        GUILayout.Label("Birth Rule", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Min Value: " + Mathf.Round(boardScript.birthlMinValue));
        GUILayout.FlexibleSpace();
        GUILayout.Label("Max Value: " + Mathf.Round(boardScript.birthlMaxValue));
        GUILayout.EndHorizontal();


        Rect sliderRectBirth = EditorGUILayout.GetControlRect(false, 20);
        EditorGUI.MinMaxSlider(sliderRectBirth, ref boardScript.birthlMinValue, ref boardScript.birthlMaxValue, 0, 27);
        GUILayout.Space(10);

        GUILayout.Space(50);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Play") && !boardScript.isPlaying)
        {
            // Handle play button click
            boardScript.isPlaying = true;
            Debug.Log("Play button clicked");
        }

        if (GUILayout.Button("Reset") && boardScript.isPlaying)
        {
            // Handle pause button click
            boardScript.isPlaying = false;
            Debug.Log("Pause button clicked");
            boardScript.onValidate();
        }

        GUILayout.EndHorizontal();
    }
}
