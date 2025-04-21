using UnityEngine;
using UnityEngine.Tilemaps;

public class create_Map : MonoBehaviour
{
    public int mapsize = 10;
    public Tilemap tileMap;
    public TileBase test1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i=0; i<mapsize; i++){
            for (int l=0; l<mapsize; l++){
                int index = (i*mapsize)+l;
                TileBase tile = Resources.Load<TileBase>("map/"+index.ToString());
                //Debug.Log(test1.name);
                tileMap.SetTile(new Vector3Int(l, mapsize-i, 0), tile);
                Debug.Log("SET "+ index.ToString() +" TILE ("+l.ToString()+", "+i.ToString()+")");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
