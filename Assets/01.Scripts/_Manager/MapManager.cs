using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapManager : Singleton<MapManager>
{

    public Define.MapControll _mapControll { get; set; }

    public Grid CurrentGrid { get; private set; }

    public int MinX { get; set; }
    public int MaxX { get; set; }
    public int MinY { get; set; }
    public int MaxY { get; set; }

    public int SizeX { get { return MaxX - MinX + 1; } }
    public int SizeY { get { return MaxY - MinY + 1; } }

    bool[,] _collision;

    bool ignoreDestCollision;

    List<int> listY = new List<int>(); //맵 소환 관련y축
    List<int> listX = new List<int>(); //맵 소환 관련x축
    List<int> _sumCount = new List<int>(); //맵 소환되는 중복 포지션 확인용 숫자
    int SumCount; // 맵에 소환되는 중복 포지션 저장용 임시 변수

    List<int> StairUpListY = new List<int>(); //맵 계단 저장 y축
    List<int> StairUpListX = new List<int>(); //맵 계단 저장 x축


    List<int> StairDownListY = new List<int>(); //맵 계단 저장 y축
    List<int> StairDownListX = new List<int>(); //맵 계단 저장 x축

    public bool CanGo(Vector3Int cellPos)
    {//갈수있으면 True
        if (cellPos.x < MinX || cellPos.x > MaxX)
            return false;
        if (cellPos.y < MinY || cellPos.y > MaxY)
            return false;

        int x = cellPos.x - MinX;
        int y = MaxY - cellPos.y;
        return !_collision[y, x];

    }

    public void CanSumlistCheck()
    {
        if (_sumCount.Count == 0)
            return;

        for (int i = 0; i < _sumCount.Count; i++)
        {
            if (_sumCount[i] == SumCount)
            {
                SumCount = UnityEngine.Random.Range(0, listY.Count);
                CanSumlistCheck();
            }
        }

    }

    public SumPos CanSum()
    {
        List<int> TempListY = new List<int>();
        List<int> TempListX = new List<int>();

        //_sumCount.Clear();
        TempListY.Clear();
        TempListX.Clear();

        switch (_mapControll)
        {
            case Define.MapControll.SumMonster:
                TempListY = listY;
                TempListX = listX;
                break;
            case Define.MapControll.SumItem:
                TempListY = listY;
                TempListX = listX;
                break;
            case Define.MapControll.SumPlayerStairUp:
                TempListY = StairUpListY;
                TempListX = StairUpListX;
                break;
            case Define.MapControll.SumPlayerStairDown:

                break;

        }

        //int[] arr = new int[2];//전달할 인자를 배열로 선언


        SumCount = UnityEngine.Random.Range(0, TempListY.Count); //리스트에 갈수있는 좌표만 있음 그중 랜덤

        CanSumlistCheck();
        _sumCount.Add(SumCount); //갈수있는 좌표 중복 확인용

        Pos pos = new Pos();
        pos.Y = TempListY[SumCount];
        pos.X = TempListX[SumCount];

        Vector3Int temp = Pos2Cell(pos);

        SumPos sumPos = new SumPos(); //전달할 좌표 struct로
        sumPos.x = temp.x;
        sumPos.y = temp.y;

        //arr[0] = temp.x;
        //arr[1] = temp.y;

        return sumPos;
    }

    public int RandomMap()
    {
        int MapId = UnityEngine.Random.Range(1, 26);
        // 나중에 일정 층 이상 도달하면 100을 넘겨줘서 마지막층 구현
        return MapId;
    }


    public void LoadMap()
    {
        DestroyMap();

        //int mapId = RandomMap();
        int mapId = 22;
        Debug.Log(mapId + "맵");

        string mapName = "Map_" + mapId.ToString("000");
        GameObject go = ResourceManager.Instance.Instantiate($"Map/Animal/{mapName}");
        go.name = "Map";

        GameObject collision = go.FindChild("CollisionMap").gameObject;

        if (collision != null)
            collision.SetActive(false);

        CurrentGrid = go.GetComponent<Grid>();

        // Collision 관련 파일
        TextAsset txt = ResourceManager.Instance.Load<TextAsset>($"Map/Animal/{mapName}");//파일로드
        StringReader reader = new StringReader(txt.text); //로드파일 스트링으로 변환

        //변환한것을 읽음

        MinX = int.Parse(reader.ReadLine());
        MaxX = int.Parse(reader.ReadLine());
        MinY = int.Parse(reader.ReadLine());
        MaxY = int.Parse(reader.ReadLine());

        int xCount = MaxX - MinX + 1;
        int yCount = MaxY - MinY + 1;
        _collision = new bool[yCount, xCount];

        for (int y = 0; y < yCount; y++)
        {
            string line = reader.ReadLine();
            for (int x = 0; x < xCount; x++)
            {
                if (line[x] == '1')
                    _collision[y, x] = true;
                else if (line[x] == '0')
                {
                    _collision[y, x] = false;
                    listY.Add(y);
                    listX.Add(x);
                }
                else if (line[x] == '2')
                {
                    _collision[y, x] = false;
                    StairUpListY.Add(y);
                    StairUpListX.Add(x);
                }

                else if (line[x] == '3')
                    _collision[y, x] = false;
                else if (line[x] == '4')
                    _collision[y, x] = false;
            }
        }
    }

    public void DestroyMap()
    {
        GameObject map = GameObject.Find("Map");
        if (map != null)
        {
            GameObject.Destroy(map);
            CurrentGrid = null;
        }
    }

    public struct SumPos
    {
        public int x;
        public int y;
    }

    public struct PQNode : IComparable<PQNode>
    {
        public int F;
        public int G;
        public int Y;
        public int X;

        public int CompareTo(PQNode other)
        {
            if (F == other.F)
                return 0;
            return F < other.F ? 1 : -1;
        }
    }

    public struct Pos
    {
        public Pos(int y, int x) { Y = y; X = x; }
        public int Y;
        public int X;
    }

    #region A* PathFinding

    // U D L R
    int[] _deltaY = new int[] { 1, -1, 0, 0 };
    int[] _deltaX = new int[] { 0, 0, -1, 1 };
    int[] _cost = new int[] { 10, 10, 10, 10 };


    public List<Vector3Int> FindPath(Vector3Int startCellPos, Vector3Int destCellPos, bool ignoreDestCollision = false)
    {
        List<Pos> path = new List<Pos>();
        int count = 0;
        // 점수 매기기
        // F = G + H
        // F = 최종 점수 (작을 수록 좋음, 경로에 따라 달라짐)
        // G = 시작점에서 해당 좌표까지 이동하는데 드는 비용 (작을 수록 좋음, 경로에 따라 달라짐)
        // H = 목적지에서 얼마나 가까운지 (작을 수록 좋음, 고정)

        // (y, x) 이미 방문했는지 여부 (방문 = closed 상태)
        bool[,] closed = new bool[SizeY, SizeX]; // CloseList

        // (y, x) 가는 길을 한 번이라도 발견했는지
        // 발견X => MaxValue
        // 발견O => F = G + H
        int[,] open = new int[SizeY, SizeX]; // OpenList
        for (int y = 0; y < SizeY; y++)
            for (int x = 0; x < SizeX; x++)
                open[y, x] = Int32.MaxValue;

        Pos[,] parent = new Pos[SizeY, SizeX];

        // 오픈리스트에 있는 정보들 중에서, 가장 좋은 후보를 빠르게 뽑아오기 위한 도구
        PriorityQueue<PQNode> pq = new PriorityQueue<PQNode>();

        // CellPos -> ArrayPos
        Pos pos = Cell2Pos(startCellPos);
        Pos dest = Cell2Pos(destCellPos);

        // 시작점 발견 (예약 진행)
        open[pos.Y, pos.X] = 10 * (Math.Abs(dest.Y - pos.Y) + Math.Abs(dest.X - pos.X));
        pq.Push(new PQNode() { F = 10 * (Math.Abs(dest.Y - pos.Y) + Math.Abs(dest.X - pos.X)), G = 0, Y = pos.Y, X = pos.X });
        parent[pos.Y, pos.X] = new Pos(pos.Y, pos.X);

        while (pq.Count > 0)
        {

            // 제일 좋은 후보를 찾는다
            PQNode node = pq.Pop();
            // 동일한 좌표를 여러 경로로 찾아서, 더 빠른 경로로 인해서 이미 방문(closed)된 경우 스킵
            if (closed[node.Y, node.X])
                continue;

            // 방문한다
            closed[node.Y, node.X] = true;
            // 목적지 도착했으면 바로 종료
            if (node.Y == dest.Y && node.X == dest.X)
                break;

            // 상하좌우 등 이동할 수 있는 좌표인지 확인해서 예약(open)한다
            for (int i = 0; i < _deltaY.Length; i++)
            {
                Pos next = new Pos(node.Y + _deltaY[i], node.X + _deltaX[i]);

                // 유효 범위를 벗어났으면 스킵
                // 벽으로 막혀서 갈 수 없으면 스킵
                if (!ignoreDestCollision || next.Y != dest.Y || next.X != dest.X)
                {
                    if (CanGo(Pos2Cell(next)) == false) // CellPos
                        continue;
                    //몬스터가 현재 위치에서 있으면 스킵
                    if (ObjectManager.Instance.Find(Pos2Cell(next)) != null)
                        continue;
                    //몬스터가 다음 위치에 있으면 스킵
                    if (ObjectManager.Instance.PosCheck(ObjectManager.Instance._checkPath, Pos2Cell(next)) == false)
                        continue;
                }

                // 이미 방문한 곳이면 스킵
                if (closed[next.Y, next.X])
                    continue;

                // 비용 계산
                int g = node.G + _cost[i];
                int h = 10 * ((dest.Y - next.Y) * (dest.Y - next.Y) + (dest.X - next.X) * (dest.X - next.X));
                // 다른 경로에서 더 빠른 길 이미 찾았으면 스킵
                if (open[next.Y, next.X] < g + h)
                    continue;

                // 예약 진행
                open[dest.Y, dest.X] = g + h;
                pq.Push(new PQNode() { F = g + h, G = g, Y = next.Y, X = next.X });
                parent[next.Y, next.X] = new Pos(node.Y, node.X);
                count++;
            }
        }
        return CalcCellPathFromParent(parent, dest, count);
    }

    List<Vector3Int> CalcCellPathFromParent(Pos[,] parent, Pos dest, int count)
    {
        List<Vector3Int> cells = new List<Vector3Int>();
        int c = count;
        int y = dest.Y;
        int x = dest.X;
        while (parent[y, x].Y != y || parent[y, x].X != x)
        {
            cells.Add(Pos2Cell(new Pos(y, x)));
            Pos pos = parent[y, x];
            y = pos.Y;
            x = pos.X;
        }
        cells.Add(Pos2Cell(new Pos(y, x)));
        cells.Reverse();

        return cells;
    }

    public Pos Cell2Pos(Vector3Int cell)
    {
        // CellPos -> ArrayPos
        return new Pos(MaxY - cell.y, cell.x - MinX);
    }

    Vector3Int Pos2Cell(Pos pos)
    {
        // ArrayPos -> CellPos
        return new Vector3Int(pos.X + MinX, MaxY - pos.Y, 0);
    }

    #endregion


}
