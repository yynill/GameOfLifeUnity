using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoardScript : MonoBehaviour
{

    // Objects to be assigned in the Unity Editor
    public GameObject outline;
    public GameObject aliveCell1;
    public GameObject aliveCell2;
    public GameObject mainCamera;

    //public GameObject deadCell;

    // Variables that can be modified in the Inspector
    public int size = 0;
    private int previousSize = -1;
    public int targetFrameRate = 0;
    public float actualFrameRate;

    public float survicalMinValue = 0f;
    public float survicalMaxValue = 27f;
    public float birthlMinValue = 0f;
    public float birthlMaxValue = 27f;

    public float liveCellRatio = 0.2f;
    private int counter = 0;


    public bool isPlaying = false;

    // Internal variables
    private int[,,] gameState;
    private List<GameObject> instantiatedCells = new List<GameObject>();
    private GameObject outlineObject;


    // Start is called before the first frame update
    void Start()
    {
        QualitySettings.vSyncCount = 0;
    }

    private float updateInterval = 1.0f; // Update every second
    private float updateTimer = 0.0f;


    // Update is called once per frame
    void Update()
    {
        if (isPlaying)
        {
            updateTimer += Time.deltaTime * targetFrameRate;

            if (updateTimer >= updateInterval)
            {
                CleanupObjects();
                UpdateState();
                DrawCube();
                DrawOutline();
                updateTimer = 0.0f;

                actualFrameRate = (int)(1f / Time.unscaledDeltaTime);
            }
        }
        else
        {
            if (size != previousSize)
            {
                onValidate();
                previousSize = size;
            }
        }
        RotateCamera();
    }

    public void onValidate()
    {
        CleanupObjects();
        InitializeGameState();
        DrawCube();
        if (Application.isPlaying)
        {
            DrawOutline();
        }
        UpdateCameraPosition(size);
    }

    // Initialize the game state based on size
    void InitializeGameState()
    {
        gameState = new int[size, size, size];

        // Set every entry to 0 (dead)
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    gameState[x, y, z] = 0;
                }
            }
        }
    }

    // Set the inner cells to alive based on the size
    public void SetCoreStartConfig()
    {
        CleanupObjects();

        int centerX = size / 2;
        int centerY = size / 2;
        int centerZ = size / 2;

        if (size == 0) { }
        else if (size == 1)
        {
            gameState[centerX, centerY, centerZ] = 1;
        }
        // set inner 8 cells to alive when even size 
        else if (size % 2 == 0)
        {
            for (int x = centerX - 1; x <= centerX; x++)
            {
                for (int y = centerY - 1; y <= centerY; y++)
                {
                    for (int z = centerZ - 1; z <= centerZ; z++)
                    {
                        gameState[x, y, z] = 1;
                    }
                }
            }
        }
        // set inner 27 cells to alive when odd size 
        else
        {
            for (int x = centerX - 1; x <= centerX + 1; x++)
            {
                for (int y = centerY - 1; y <= centerY + 1; y++)
                {
                    for (int z = centerZ - 1; z <= centerZ + 1; z++)
                    {
                        gameState[x, y, z] = 1;
                    }
                }
            }
        }

        DrawCube();
        DrawOutline();
    }




    public void SetRandomStartConfig()
    {
        onValidate();
        Debug.Log("d");

        counter = 0;
        System.Random random = new System.Random();

        int totalCells = size * size * size;
        int maxCells = Mathf.RoundToInt(totalCells * liveCellRatio);

        for (int i = 0; i < maxCells; i++)
        {
            int x, y, z;

            do
            {
                x = random.Next(0, size);
                y = random.Next(0, size);
                z = random.Next(0, size);
            } while (gameState[x, y, z] != 0);

            gameState[x, y, z] = 1;
            counter++;
        }
        DrawCube();
        DrawOutline();
    }


    // Draw the cube based on the game state
    void DrawCube()
    {
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    int state = gameState[x, y, z];
                    PlaceCell(state, x, y, z);
                }
            }
        }
    }

    // Draw a big cube outline
    void DrawOutline()
    {
        int board_x = size;
        int board_y = size;
        int board_z = size;

        Vector3 outlineScale = new Vector3(board_x, board_y, board_z);
        Vector3 outlinePosition = new Vector3((board_x - 1) * 0.5f, (board_y - 1) * 0.5f, (board_z - 1) * 0.5f);

        outlineObject = Instantiate(outline, outlinePosition, Quaternion.identity);
        outlineObject.transform.localScale = outlineScale;
    }


    // Place a cell at a given position
    void PlaceCell(int state, int x, int y, int z)
    {
        Vector3 position = new Vector3(x, y, z);
        // If alive
        if (state == 1)
        {
            instantiatedCells.Add(Instantiate(aliveCell1, position, Quaternion.identity));
        }
        else if (state == 2)
        {
            instantiatedCells.Add(Instantiate(aliveCell2, position, Quaternion.identity));
        }
        // If dead - as nothing (saves rendering power)
        else if (state == 0)
        {
            //instantiatedCells.Add(Instantiate(deadCell, position, Quaternion.identity));
        }
    }

    // Update camera position based on the size of the cube
    void UpdateCameraPosition(int size)
    {
        float distance = size * Mathf.Sqrt(2);
        mainCamera.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        float halfSize = size / 2f;

        if (size % 2 == 0) // even 
        {
            mainCamera.transform.position = new Vector3(
                halfSize - 0.5f,         // x
                halfSize - 0.5f,         // y
                -distance                // z
            );
        }
        else if (size % 2 != 0) // odd
        {
            mainCamera.transform.position = new Vector3(
                halfSize - 0.5f,                 // x
                halfSize - 0.5f,                 // y 
                -distance                 // z 
            );
        }
    }

    public float rotationSpeed = 1000f;
    void RotateCamera()
    {
        // Check if the mainCamera is assigned
        if (mainCamera != null)
        {
            // Calculate the center of the cube based on its size
            Vector3 cubeCenter = new Vector3((size - 0.5f) / 2f, (size - 0.5f) / 2f, (size - 0.5f) / 2f);

            // Calculate the direction from the camera to the center of the cube
            Vector3 toCenter = cubeCenter - mainCamera.transform.position;

            // Calculate the rotation to look at the center of the cube
            Quaternion targetRotation = Quaternion.LookRotation(toCenter);

            // Smoothly rotate towards the target rotation
            mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime * size);

            // Orbit around the center while always facing inwards
            mainCamera.transform.Translate(Vector3.right * rotationSpeed * Time.deltaTime * size);
        }
        else
        {
            Debug.LogWarning("No mainCamera assigned for camera rotation.");
        }
    }


    // Deleting all cells and outline before new
    void CleanupObjects()
    {
        foreach (var cell in instantiatedCells)
        {
            Destroy(cell);
        }

        if (outlineObject != null)
        {
            Destroy(outlineObject);
        }

        instantiatedCells.Clear();
    }

    // Update the game state based on the rules
    void UpdateState()
    {
        int[,,] nextGameState = new int[size, size, size];

        // iterate of all cubes in generation
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    int this_x = x;
                    int this_y = y;
                    int this_z = z;

                    int visible_neighbors = 0;                                      // number of visible neighbours
                    int curent_visibility = gameState[this_x, this_y, this_z];      // curent visibility (state)

                    foreach (var (i, j, k) in new List<(int, int, int)>             // surrounding neighbours
                    {
                        (-1, -1, -1), (-1, -1, 0), (-1, -1, 1),
                        (-1, 0, -1),  (-1, 0, 0),  (-1, 0, 1),
                        (-1, 1, -1),  (-1, 1, 0),  (-1, 1, 1),
                        (0, -1, -1),  (0, -1, 0),  (0, -1, 1),
                        (0, 0, -1),                (0, 0, 1),
                        (0, 1, -1),   (0, 1, 0),   (0, 1, 1),
                        (1, -1, -1),  (1, -1, 0),  (1, -1, 1),
                        (1, 0, -1),   (1, 0, 0),   (1, 0, 1),
                        (1, 1, -1),   (1, 1, 0),   (1, 1, 1)
                    })
                    {
                        int new_x = this_x + i;                 // neighbor row
                        int new_y = this_y + j;                 // neighbor column
                        int new_z = this_z + k;                 // neighbor depth

                        // Check if the neighboring cell exists
                        if (new_x >= 0 && new_x < size &&
                            new_y >= 0 && new_y < size &&
                            new_z >= 0 && new_z < size)
                        {
                            visible_neighbors += gameState[new_x, new_y, new_z];
                        }
                        else
                        {
                            continue; // Skip the rest of the code in this iteration and move to the next iteration
                        }
                    }
                    // update self state

                    // birth rule
                    if (curent_visibility == 0 && (birthlMinValue < visible_neighbors && visible_neighbors < birthlMaxValue))
                    {
                        nextGameState[this_x, this_y, this_z] = 1;
                    }

                    // death rule
                    else if (curent_visibility != 0 && (visible_neighbors < survicalMinValue || visible_neighbors > survicalMaxValue))
                    {
                        nextGameState[this_x, this_y, this_z] = 0;
                    }

                    // survival rule
                    else if (curent_visibility != 0 && (visible_neighbors >= survicalMinValue && visible_neighbors <= survicalMaxValue))
                    {
                        nextGameState[this_x, this_y, this_z] = 2;
                    }
                }
            }
        }
        gameState = nextGameState;
    }
}
