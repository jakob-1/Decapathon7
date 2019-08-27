using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMaker : MonoBehaviour
    //this is intended to fill the frame with some pieces before gameplay starts
    //once the field is cleared a new set will be generated and the player will get a score boost

{

    private Controller controller;
    private Sprite[] sprites;
    public GameObject piecePrefab;
    private Vector3 startPoint;
    private float moveDistance;
    // Start is called before the first frame update
    void Start()
    {
        controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<Controller>();
        sprites = controller.sprites;
        startPoint = controller.startPoint;
        moveDistance = controller.distance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //the level will increase how many layers up the pieces can be placed starting from 4 to 7;
    public GameObject[,] generatePieces(float level, GameObject[,] grid, int xLimit, int yLimit)
    {
        float step = 1;
        print("STEP: " + step);
        float pieceChance = level;

        for (int y = yLimit - 1; y > 0 && pieceChance > 0; y--)
        {
            for (int x = 0; x < xLimit; x++)
            {
                //there needs to be a piece below the current space being checked for a place to be pieced
                bool pieceBelow = false;
                if (y == yLimit - 1)
                {
                    pieceBelow = true;
                }
                else if (grid[x, y+1] != null)
                {
                    pieceBelow = true;
                }

                if (Random.Range(0, level) < pieceChance && pieceBelow)
                {
                    int pieceValue = Random.Range(0, 3);
                    GameObject newPiece = piecePrefab;
                    newPiece.GetComponent<Piece>().assignXY(x, y);
                    newPiece.GetComponent<Piece>().value = pieceValue;
                    newPiece.GetComponent<SpriteRenderer>().sprite = sprites[pieceValue];
                    //the instantiated piece is the one that needs to be added to the grid
                    GameObject realPiece = Instantiate(newPiece, startPoint + new Vector3(x * moveDistance, y * -moveDistance), Quaternion.identity);
                    grid[x, y] = realPiece;
                }
            }
            pieceChance -= step;
        }


        return grid;
    }

    //this is run after a match has been made
    //it checks if the bottom row is empty and if so, returns true
    public bool isGridClear(GameObject[,] grid, int columns, int rows)
    {
        for (int x = 0; x < columns; x++)
        {
            if (grid[x , rows - 1] != null)
            {
                return false;
            }
        }
        return true;
    }
}
