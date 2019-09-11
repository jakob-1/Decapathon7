using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public int score = 0;
    public Text scoreText;

    public bool gameOver = false;
    private GameObject gameOverScreen;
    private Text gameOverScore;

    private PauseMenu pauseMenu;

    private List<GameObject> LastMatches;
    private bool matchAnimating = false;
    private bool animationDone = false;
    private int animationPhase = 0;
    private float animationTimer;
    public float animationLength = 0.1f;

    private Debug debug;

    private LevelMaker levelMaker;

    // Use this for initialization
    void Start () {
        levelMaker = GameObject.FindGameObjectWithTag("GameManager").GetComponent<LevelMaker>();
        grid = new GameObject[columns,rows];
        grid = levelMaker.generatePieces(7 ,grid, columns, rows);
        GenerateNextPiece();
        SpawnNextPiece();
        moveDownTimer = Time.time + normalMovDowneRate;
        debug = GetComponent<Debug>();
        pauseMenu = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PauseMenu>();
        gameOverScreen = GameObject.FindGameObjectWithTag("GameOverMenu");
        gameOverScore = GameObject.FindGameObjectWithTag("GameOverScore").GetComponent<Text>();
        gameOverScreen.SetActive(false);
        LastMatches = new List<GameObject>();
        }
	
	// Update is called once per frame
	void Update () {
        if (!gameOver && !pauseMenu.paused && !matchAnimating)
        {
            movePiece();
        }
        if (matchAnimating)
        {
            matchesFound();
        }
        if (levelMaker.isGridClear(grid, columns, rows))
        {
            levelMaker.generatePieces(7, grid, columns, rows);
        }
	}

    void matchesFound()
    {
        if (animationDone)
        {
            DestroyMatchedPieces(LastMatches);
            GridUncheckAll();
            List<GameObject> toCheck = moveRemainingPiecesDown();
            animationDone = false;
            matchAnimating = false;
            if (toCheck.Count > 0)
            {
                foreach (GameObject pieceObj in toCheck)
                {
                    animationDone = false;
                    Piece piece = pieceObj.GetComponent<Piece>();
                    checkForMatches(piece.x, piece.y);
                }
            }
            
        }
        else
        {
            if (animationPhase == 0)
            {
                animationPhase = 1;
                foreach (GameObject piece in LastMatches)
                {
                    piece.transform.localScale = new Vector3(1.3f, 1.3f);
                    piece.transform.eulerAngles = new Vector3(0, 0, 15);
                }
                animationTimer = animationLength + Time.time;
            }
            else if (animationPhase == 1 && animationTimer < Time.time)
            {
                animationPhase = 2;
                foreach (GameObject piece in LastMatches)
                {
                    piece.transform.localScale = new Vector3(1.0f, 1.0f);
                    piece.transform.eulerAngles = Vector3.zero;
                }
                animationTimer = animationLength + Time.time;
            }
            else if (animationPhase == 2 && animationTimer < Time.time)
            {
                animationPhase = 0;
                animationDone = true;
            }
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

        if (y == 0)
        {
            gameOver = true;
            pauseMenu.enabled = false;
            scoreText.enabled = false;
            gameOverScreen.SetActive(true);
            gameOverScore.text ="SCORE\n" + score.ToString();
            Cursor.lockState = CursorLockMode.None;
        }
        currentPiece.GetComponent<Piece>().assignXY(x, y);
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
        if (matches.Count >= 3)
        {
            scorePlayer(matches.Count);
            LastMatches = matches;
            matchAnimating = true;


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
        score += 50;
        matches -= 3;
        score += matches * 100;
        string scoreStr = score.ToString();
        int scoreLen = scoreStr.Length;
        for (int i = 0; i < scoreText.text.Length - scoreLen; i++)
        {
            scoreStr = "0" + scoreStr;
        }

        scoreText.text = scoreStr;
    }

    void DestroyMatchedPieces(List<GameObject> matches)
    {
        foreach (GameObject match in matches)
        {
            grid[match.GetComponent<Piece>().x, match.GetComponent<Piece>().y] = null;
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
    List<GameObject> moveRemainingPiecesDown()
    {
        List<GameObject> toCheck = new List<GameObject>();
        for (int x = 0; x < columns; x++)
        {
            for (int y = rows - 2; y >= 0; y--)
            {
                if (grid[x,y+1] == null && grid[x,y] != null)
                {
                    toCheck.Add(grid[x, y]);
                    PieceFall(x, y);
                }
            }
        }
        return toCheck;
    }

    //the piece will fall until it hits the ground or the piece below it
    void PieceFall(int x, int y)
    {
        int distance = 1;

        while (y + distance + 1 < rows){
            if (grid[x, y + distance + 1] == null)
            {
                distance++;
            }
            else
            {
                break;
            }
        }

        grid[x, y + distance] = grid[x, y];
        grid[x, y].transform.position += -distance * transform.up;
        grid[x, y].GetComponent<Piece>().assignXY(x, y + distance);
        grid[x, y] = null;

    }

    bool nextPieceDownEmpty(int x, int y)
    {
        if (y + 1 < rows)
        {
            if (grid[x, y + 1] == null)
            {
                return true;
            }
        }

        return false;
    }

    //move the active piece down one step if there are no obstructions in the way;
    void movePiece()
    {
        if ((!nextPieceDownEmpty((int)piecePos.x, (int)piecePos.y)) && moveDownTimer < Time.time)
        {
            placePiece(piecePos);
        }
        else
        {
            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                currentPiece.transform.Rotate(new Vector3(0, 0, 45));
            }
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
                if ((Input.GetKeyDown(KeyCode.DownArrow) 
                    || moveDownTimer < Time.time) 
                    && nextPieceDownEmpty((int)piecePos.x, (int)piecePos.y))
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
