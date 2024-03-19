using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
// GridManager
// It, uh, manages the grid of sprite tiles.
//
//
// Version History:
// ---------------
// 2021-07-01: Initial version, using camera size/aspect ratio
// 2021-11-08: Switched from camera size, and now using grid rows/cols/size/spacing for 'world space' grid.
// 2021-11-09: Added TileContainer child so that tiles have parent to which everything scales for easier "local world space" usage.
// 2021-11-12: Fixed local rotation of tiles so that they respect rotation of parent tile container (e.g. place grids on 4 walls of a room)
//
//
[RequireComponent(typeof(SpriteLoader))]
public class GridManager : MonoBehaviour
{

    //public Sprite sprite;
    public float[,] Grid;
    private int Vertical, Horizontal, Columns, Rows;

    private Sprite[] mySpritePool = null;
    private List<GameObject> spawnedTiles = null;


    [SerializeField]
    private bool assignRandomShading = false;

    // Tile Changer "drip feed"
    [SerializeField]
    private bool changeTilesAutomatically  = true;

    // Change these to something else if needed
    [SerializeField]
    private float minChangeDelayInSeconds = 1.0f;
    [SerializeField]
    private float maxChangeDelayInSeconds = 5.0f;

    private float changeCounter = 0.0f;
    private float currentChangeDelayInSeconds = 0.0f;

    // Edit: new "on a plane" stuff
    [SerializeField, Tooltip("Old method using camera size and aspect ratio. Default false")]
    private bool useCameraForSize = false;
    [SerializeField, Tooltip("The child TileContainer object of GridManager")]
    private Transform tileContainer = null;
    [SerializeField, Tooltip("")]
    private float tileScale = 1.0f;
    [Range(0.5f, 10.0f)]
    [SerializeField, Tooltip("Space between tile centers (e.g. tiles of size 1 with spacing 0.5 will be 0.5 units center to center")]
    private float tileSpacing = 0.5f;
    [SerializeField, Tooltip("Number of rows of tiles")]
    private int rowCount = 3;
    [SerializeField, Tooltip("Number of columns of tiles")]
    private int columnCount = 3;

    private int xMiddle = 0;
    private int yMiddle = 0;
	
	// Set sorting order for sprites

	public int sortingOrder = 1;
    


    void Start()
    {

        Debug.Log("Press <Space> to change all tiles, press <Enter> to change a single tile");

        if (useCameraForSize)
        {

            Vertical = (int)Camera.main.orthographicSize;
            Horizontal = (int)(Vertical * Camera.main.aspect);

            Columns = Horizontal * 2;
            Rows = Vertical * 2;

        }
        else
        {

            Columns = columnCount;
            Rows = rowCount;

            xMiddle = Mathf.FloorToInt(columnCount / 2);
            yMiddle = Mathf.FloorToInt(rowCount / 2);

            //Debug.Log("xMid=" + xMiddle + "(" + columnCount + "/2),yMid=" + yMiddle + "(" + rowCount + "/2)");

        }

        Grid = new float[Columns, Rows];

        // Fetch pool from sprite loader
        mySpritePool = gameObject.GetComponent<SpriteLoader>().SpriteList;
        spawnedTiles = new List<GameObject>();

        populateGridWithSprites();

        currentChangeDelayInSeconds = Random.Range(minChangeDelayInSeconds, maxChangeDelayInSeconds);

        if(tileContainer == null)
        {
            Debug.LogError("GridManager:: TileContainer is not assigned in the Inspector. That's numberwang.");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Q))
        {
            populateGridWithSprites();
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            changeSingleTile();
        }

        if (changeTilesAutomatically)
        {

            changeCounter += Time.deltaTime;

            if(changeCounter > currentChangeDelayInSeconds)
            {

                changeCounter = 0.0f;
                currentChangeDelayInSeconds = Random.Range(minChangeDelayInSeconds, maxChangeDelayInSeconds);
                changeSingleTile();

            }

        }

    }

    private void populateGridWithSprites()
    {

        for(int i = 0; i < spawnedTiles.Count; i++)
        {
            Destroy(spawnedTiles[i].gameObject);
        }

        spawnedTiles.Clear();

        for (int i = 0; i < Columns; i++)
        {

            for (int j = 0; j < Rows; j++)
            {

                Grid[i, j] = Random.Range(0.0f, 1.0f);
                SpawnTile(i, j, Grid[i, j]);

            }

        }

    }


    private void changeSingleTile()
    {

        int randomTileIndex = Random.Range(0, spawnedTiles.Count);

        // TODO: This does not take into account randomly selecting the same sprite that is already assigned to this tile space.
        spawnedTiles[randomTileIndex].GetComponent<SpriteRenderer>().sprite = mySpritePool[Random.Range(0, mySpritePool.Length - 1)];

    }


    private void SpawnTile(int x, int y, float value)
    {

        //Debug.Log("<b>SpawnTile(" + x + "," + y + ")</b>");

        GameObject tempTile = new GameObject("x: "+x+"Y: "+y);
        

        if (useCameraForSize)
        {
            tempTile.transform.position = new Vector3(x - (Horizontal - 0.5f), y - (Vertical - 0.5f));
        }
        else
        {

            // Just assume grid manager will always be at "bottom left" of target square area where we're spawning tiles.
            float xPos = (x * tileSpacing) + (tileSpacing / 2.0f);
            float yPos = (y * tileSpacing) + (tileSpacing / 2.0f);

            //Debug.Log("TILE(" + x + "," + y + ")=>pos(" + xPos + "," + yPos + ")");
            tempTile.transform.parent = tileContainer;
            tempTile.transform.localPosition = tileContainer.transform.localPosition + (new Vector3(xPos, yPos, 0.0f));
            tempTile.transform.localRotation = Quaternion.identity;
            tempTile.transform.localScale = Vector3.one * tileScale;

        }


        SpriteRenderer s = tempTile.AddComponent<SpriteRenderer>();

        // Get a random sprite from the sprite pool
        s.sprite = mySpritePool[Random.Range(0, mySpritePool.Length-1)];
		
		
		//
		s.sortingOrder = sortingOrder;
		

        if (assignRandomShading)
        {
            s.color = new Color(value, value, value);
        }

        // Lastly, add our spawned tile to list of spawned tiles so we can destroy it later.
        spawnedTiles.Add(tempTile);

    }


#if UNITY_EDITOR

    private void OnDrawGizmos()
    {

        if (useCameraForSize == false && tileContainer != null)
        {

            Color c = Color.red;
            c.a = 0.5f;
            Gizmos.color = c;

            Gizmos.DrawLine(tileContainer.transform.position, tileContainer.transform.position + tileContainer.transform.up * (rowCount * tileSpacing * tileContainer.transform.localScale.y));

            c = Color.green;
            c.a = 0.5f;
            Gizmos.color = c;

            Gizmos.DrawLine(tileContainer.transform.position + tileContainer.transform.up * (rowCount * tileSpacing * tileContainer.transform.localScale.y), tileContainer.transform.position + tileContainer.transform.up * (rowCount * tileSpacing * tileContainer.transform.localScale.y) + tileContainer.transform.right * (columnCount * tileSpacing * tileContainer.transform.localScale.x));

            c = Color.blue;
            c.a = 0.5f;
            Gizmos.color = c;

            Gizmos.DrawLine(tileContainer.transform.position + tileContainer.transform.up * (rowCount * tileSpacing * tileContainer.transform.localScale.y) + tileContainer.transform.right * (columnCount * tileSpacing * tileContainer.transform.localScale.x), tileContainer.transform.position + tileContainer.transform.right * (columnCount * tileSpacing * tileContainer.transform.localScale.x));

            c = Color.yellow;
            c.a = 0.5f;
            Gizmos.color = c;

            Gizmos.DrawLine(tileContainer.transform.position, tileContainer.transform.position + tileContainer.transform.right * (rowCount * tileSpacing * tileContainer.transform.localScale.x));

        }

    }

#endif

}