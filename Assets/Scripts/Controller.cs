using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {

    public GameObject[,] grid;
    public float distance = 1;
    public float normalMovDowneRate = 1;
    public float fastMoveDownRate = 0.2f;
    private float moveDownTimer;
    public float moveSidewaysRate = 0.1f;
    private float moveSidewaysTimer;

    public Sprite[] sprites;
    public float topPos = 3.5f;
    public GameObject piecePrefab;
    public NextPiece nextPiece;
    public GameObject currentPiece;
    public Vector3 startPoint;
    public Vector2 piecePos;
    public int rows = 9;
    public int columns = 6;

    public bool gameOver = false;

    private Debug debug;

    // Use this for initialization
    void Start () {
        grid = new GameObject[columns,rows];
        GenerateNextPiece();
        SpawnNextPiece();
        moveDownTimer = Time.time + normalMovDowneRate;
        print(grid.Length);
        debug = GetComponent<Debug>();
    }
	
	// Update is called once per frame
	void Update () {
        if (!gameOver)
        {
            movePiece();
        }
	}

    //randomly pic the next piece and display it in the next piece box
    void GenerateNextPiece()
    {
        int newValue = Random.Range(0, 3);
        nextPiece.newPiece(newValue);
    }

    //spawn the next piece in the frame and make it the active piece
    void SpawnNextPiece()
    {
        currentPiece = Instantiate(piecePrefab, startPoint, Quaternion.identity);
        currentPiece.GetComponent<Piece>().value = nextPiece.getValue();
        Sprite sprite = sprites[nextPiece.getValue()];
        currentPiece.GetComponent<SpriteRenderer>().sprite = sprite;
        piecePos = new Vector2(0, 0);
    }

    //place the piece in the grid and get the next piece ready
    void placePiece(Vector2 pos)
    {
        int x = (int)piecePos.x;
        int y = (int)piecePos.y;

        grid[x, y] = currentPiece;

        checkForMatches(x,y);

        SpawnNextPiece();
        GenerateNextPiece();
    }

    public int gridVal(int x, int y)
    {
        if (grid[x,y] != null)
        {
            return grid[x, y].GetComponent<Piece>().value;
        }
        return -1;
    }

    public bool gridChecked(int x, int y)
    {
        if (grid[x, y] != null)
        {
            return grid[x, y].GetComponent<Piece>().beenChecked;
        }
        return true;
    }

    void gridMarkChecked(int x, int y, bool check)
    {
        if (grid[x, y] != null)
        {
            grid[x, y].GetComponent<Piece>().beenChecked = check;
        }
    }

    //search grid for any connections 3 or greater and delete them and give the player points
    void checkForMatches(int startX, int startY)
    {
        int value = gridVal(startX, startY);
        //check spaces on each side for matches
        List<GameObject> matches = new List<GameObject>();
        matches = getMatch(startX, startY, value, matches);
        //debug.printGridCheck();
        print(matches.Count);
        if (matches.Count >= 3)
        {
            scorePlayer(matches.Count);
            DestroyMatchedPieces(matches);
            GridUncheckAll();
            moveRemainingPiecesDown();
        }
        GridUncheckAll();
    }

    List<GameObject> getMatch(int x, int y, int value, List<GameObject> matches)
    {
        gridMarkChecked(x, y, true);
        matches.Add(grid[x, y]);

        matches = checkGridSpace(x + 1, y, value, matches);

        matches = checkGridSpace(x - 1, y, value, matches);

        matches = checkGridSpace(x, y + 1, value, matches);

        matches = checkGridSpace(x, y - 1, value, matches);


        return matches;
    }


    List<GameObject> checkGridSpace(int x, int y, int value, List<GameObject> matches)
    {
        if (x < columns && y < rows && x >= 0 && y >= 0)
        {
            if (gridVal(x, y) == value && !gridChecked(x, y))
            {
                matches = getMatch(x, y, value, matches);
            }
        }
        return matches;
    }

    void scorePlayer(int matches)
    {

    }

    void DestroyMatchedPieces(List<GameObject> matches)
    {
        foreach (GameObject match in matches)
        {
            Destroy(match);
        }
    }

    void GridUncheckAll()
    {
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                if (grid[x, y] != null)
                {
                    if (gridChecked(x, y))
                    {
                        gridMarkChecked(x, y, false);
                    }
                }
            }
        }
    }

    //move remaining pieces after a match down until they are all as low as possible
    void moveRemainingPiecesDown()
    {
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                if (grid[x,y+1] == null && grid[x,y] != null)
                {
                    PieceFall(x, y);
                }
            }
        }
    }

    //the piece will fall until it hits the ground or the piece below it
    void PieceFall(int x, int y)
    {
        int distance = 1;

        while (grid[x, y + distance + 1] == null && y + distance + 1 != rows)
        {
            distance++;
        }

        grid[x, y + distance] = grid[x, y];

        grid[x, y] = null;
    }

    //move the active piece down one step if there are no obstructions in the way;
    void movePiece()
    {
        if (piecePos.y == rows - 1 || grid[(int)piecePos.x, (int)piecePos.y + 1] != null)
        {
            placePiece(piecePos);
        }
        else
        {
            if (Input.GetKey(KeyCode.RightArrow) 
                && piecePos.x != columns - 1 
                && grid[(int)piecePos.x + 1, (int)piecePos.y] == null)
            {
                if (Input.GetKeyDown(KeyCode.RightArrow) 
                    || moveSidewaysTimer < Time.time )
                {
                    piecePos += new Vector2(1, 0);
                    currentPiece.transform.position += distance * transform.right;
                    moveSidewaysTimer = Time.time + moveSidewaysRate;
                }
            }

            if (Input.GetKey(KeyCode.LeftArrow) 
                && piecePos.x != 0
                && grid[(int)piecePos.x - 1, (int)piecePos.y] == null)
            {

                if (Input.GetKeyDown(KeyCode.LeftArrow) 
                    || moveSidewaysTimer < Time.time)
                {
                    piecePos += new Vector2(-1, 0);
                    currentPiece.transform.position += -distance * transform.right;
                    moveSidewaysTimer = Time.time + moveSidewaysRate;
                }
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                if (Input.GetKeyDown(KeyCode.DownArrow) || moveDownTimer < Time.time)
                {
                    piecePos += new Vector2(0, 1);
                    currentPiece.transform.position += -distance * transform.up;
                    moveDownTimer = Time.time + fastMoveDownRate;
                }
            }
            if (moveDownTimer < Time.time)
            {
                if (piecePos.y < rows - 1)
                {
                    piecePos += new Vector2(0, 1);
                    currentPiece.transform.position += -distance * transform.up;
                    moveDownTimer = Time.time + normalMovDowneRate;
                }

            }
        }
    }
}
