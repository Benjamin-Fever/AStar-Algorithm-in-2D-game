using Godot;
using Godot.Collections;

public static class AgentPathFinding
{

    private static AStar2D aStar2D;
    private static TileMap _tileMap;
    private static int _layer;
    private static Array<Vector2I> offsets = new Array<Vector2I>(){Vector2I.Up, Vector2I.Down, Vector2I.Left, Vector2I.Right, new Vector2I(1,1), new Vector2I(-1,1),new Vector2I(1,-1), new Vector2I(-1,-1)};

    public static Array<Vector2> FindAgentPath(Vector2 startPosition, Vector2 endPosition){
        // Get ID of the positions
        long startId = _GetCoordToID(_tileMap.LocalToMap(_tileMap.ToLocal(startPosition)));
        long endId   = _GetCoordToID(_tileMap.LocalToMap(_tileMap.ToLocal(endPosition)));

        // Find the path between two nodes
        Array<Vector2> path = new Array<Vector2>(aStar2D.GetPointPath(startId, endId));

        // Turn the nodes into global coordinates
        for (int i = 0; i < path.Count; i++){
            path[i] = _tileMap.ToGlobal(_tileMap.MapToLocal(new Vector2I((int)path[i].X, (int)path[i].Y)));
        }
        return path;
    }

    public static void Setup(TileMap tileMap, int layer){
        _tileMap = tileMap;
        _layer = layer;
        aStar2D = new AStar2D();
        _AddPoints();
        _ConnectPoints();
    }

    private static void _AddPoints(){
        foreach (Vector2I cell in _tileMap.GetUsedCells(_layer)){
            long ID = _GetCoordToID(cell);
            aStar2D.AddPoint(ID, cell, _GetCellWeight(cell));
        }
    }

    private static void _ConnectPoints(){
        Array<Vector2I> usedCells = _tileMap.GetUsedCells(_layer);
        foreach (Vector2I cell in usedCells){
            foreach (Vector2I offset in offsets){
                Vector2I neigbour = cell + offset;
                if (!usedCells.Contains(neigbour)){ continue; }
                aStar2D.ConnectPoints(_GetCoordToID(cell), _GetCoordToID(neigbour));
            }
        }
    }

    private static int _GetCellWeight(Vector2I cell){
        int weight = 5;
        Array<Vector2I> usedCells = _tileMap.GetUsedCells(_layer);
        int i = 0;
        foreach (Vector2I offset in offsets){
            Vector2I neigbour = cell + offset;
            if (!usedCells.Contains(neigbour))
                weight ++;
            i++;
        }
        return weight;
    }

    private static long _GetCoordToID(Vector2 point){
        return ((uint)point.X << 16) | (uint)point.Y;
    }

    private static long _GetCoordToID(int x, int y){
        return ((uint)x << 16) | (uint)y;
    }

    private static Vector2 _GetIDToCoord(long id){
        return new Vector2(id >> 16, id & 0xFFFF);
    }

    public static bool HasPath(Vector2 startPosition, Vector2 endPosition){
        long startId = _GetCoordToID(_tileMap.LocalToMap(_tileMap.ToLocal(startPosition)));
        long endId   = _GetCoordToID(_tileMap.LocalToMap(_tileMap.ToLocal(endPosition)));
        bool validStartId = new Array<long>(aStar2D.GetPointIds()).Contains(startId);
        bool validEndId = new Array<long>(aStar2D.GetPointIds()).Contains(endId);
        if (!validEndId || !validStartId){
            return false;
        }

        Array<Vector2> path = new Array<Vector2>(aStar2D.GetPointPath(startId, endId));
        
        return path.Count != 0;
    }
}
