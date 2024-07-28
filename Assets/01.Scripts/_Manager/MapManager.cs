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

    List<int> listY = new List<int>(); //�� ��ȯ ����y��
    List<int> listX = new List<int>(); //�� ��ȯ ����x��
    List<int> _sumCount = new List<int>(); //�� ��ȯ�Ǵ� �ߺ� ������ Ȯ�ο� ����
    int SumCount; // �ʿ� ��ȯ�Ǵ� �ߺ� ������ ����� �ӽ� ����

    List<int> StairUpListY = new List<int>(); //�� ��� ���� y��
    List<int> StairUpListX = new List<int>(); //�� ��� ���� x��


    List<int> StairDownListY = new List<int>(); //�� ��� ���� y��
    List<int> StairDownListX = new List<int>(); //�� ��� ���� x��

    public bool CanGo(Vector3Int cellPos)
    {//���������� True
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

        //int[] arr = new int[2];//������ ���ڸ� �迭�� ����


        SumCount = UnityEngine.Random.Range(0, TempListY.Count); //����Ʈ�� �����ִ� ��ǥ�� ���� ���� ����

        CanSumlistCheck();
        _sumCount.Add(SumCount); //�����ִ� ��ǥ �ߺ� Ȯ�ο�

        Pos pos = new Pos();
        pos.Y = TempListY[SumCount];
        pos.X = TempListX[SumCount];

        Vector3Int temp = Pos2Cell(pos);

        SumPos sumPos = new SumPos(); //������ ��ǥ struct��
        sumPos.x = temp.x;
        sumPos.y = temp.y;

        //arr[0] = temp.x;
        //arr[1] = temp.y;

        return sumPos;
    }

    public int RandomMap()
    {
        int MapId = UnityEngine.Random.Range(1, 26);
        // ���߿� ���� �� �̻� �����ϸ� 100�� �Ѱ��༭ �������� ����
        return MapId;
    }


    public void LoadMap()
    {
        DestroyMap();

        //int mapId = RandomMap();
        int mapId = 22;
        Debug.Log(mapId + "��");

        string mapName = "Map_" + mapId.ToString("000");
        GameObject go = ResourceManager.Instance.Instantiate($"Map/Animal/{mapName}");
        go.name = "Map";

        GameObject collision = go.FindChild("CollisionMap").gameObject;

        if (collision != null)
            collision.SetActive(false);

        CurrentGrid = go.GetComponent<Grid>();

        // Collision ���� ����
        TextAsset txt = ResourceManager.Instance.Load<TextAsset>($"Map/Animal/{mapName}");//���Ϸε�
        StringReader reader = new StringReader(txt.text); //�ε����� ��Ʈ������ ��ȯ

        //��ȯ�Ѱ��� ����

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
        // ���� �ű��
        // F = G + H
        // F = ���� ���� (���� ���� ����, ��ο� ���� �޶���)
        // G = ���������� �ش� ��ǥ���� �̵��ϴµ� ��� ��� (���� ���� ����, ��ο� ���� �޶���)
        // H = ���������� �󸶳� ������� (���� ���� ����, ����)

        // (y, x) �̹� �湮�ߴ��� ���� (�湮 = closed ����)
        bool[,] closed = new bool[SizeY, SizeX]; // CloseList

        // (y, x) ���� ���� �� ���̶� �߰��ߴ���
        // �߰�X => MaxValue
        // �߰�O => F = G + H
        int[,] open = new int[SizeY, SizeX]; // OpenList
        for (int y = 0; y < SizeY; y++)
            for (int x = 0; x < SizeX; x++)
                open[y, x] = Int32.MaxValue;

        Pos[,] parent = new Pos[SizeY, SizeX];

        // ���¸���Ʈ�� �ִ� ������ �߿���, ���� ���� �ĺ��� ������ �̾ƿ��� ���� ����
        PriorityQueue<PQNode> pq = new PriorityQueue<PQNode>();

        // CellPos -> ArrayPos
        Pos pos = Cell2Pos(startCellPos);
        Pos dest = Cell2Pos(destCellPos);

        // ������ �߰� (���� ����)
        open[pos.Y, pos.X] = 10 * (Math.Abs(dest.Y - pos.Y) + Math.Abs(dest.X - pos.X));
        pq.Push(new PQNode() { F = 10 * (Math.Abs(dest.Y - pos.Y) + Math.Abs(dest.X - pos.X)), G = 0, Y = pos.Y, X = pos.X });
        parent[pos.Y, pos.X] = new Pos(pos.Y, pos.X);

        while (pq.Count > 0)
        {

            // ���� ���� �ĺ��� ã�´�
            PQNode node = pq.Pop();
            // ������ ��ǥ�� ���� ��η� ã�Ƽ�, �� ���� ��η� ���ؼ� �̹� �湮(closed)�� ��� ��ŵ
            if (closed[node.Y, node.X])
                continue;

            // �湮�Ѵ�
            closed[node.Y, node.X] = true;
            // ������ ���������� �ٷ� ����
            if (node.Y == dest.Y && node.X == dest.X)
                break;

            // �����¿� �� �̵��� �� �ִ� ��ǥ���� Ȯ���ؼ� ����(open)�Ѵ�
            for (int i = 0; i < _deltaY.Length; i++)
            {
                Pos next = new Pos(node.Y + _deltaY[i], node.X + _deltaX[i]);

                // ��ȿ ������ ������� ��ŵ
                // ������ ������ �� �� ������ ��ŵ
                if (!ignoreDestCollision || next.Y != dest.Y || next.X != dest.X)
                {
                    if (CanGo(Pos2Cell(next)) == false) // CellPos
                        continue;
                    //���Ͱ� ���� ��ġ���� ������ ��ŵ
                    if (ObjectManager.Instance.Find(Pos2Cell(next)) != null)
                        continue;
                    //���Ͱ� ���� ��ġ�� ������ ��ŵ
                    if (ObjectManager.Instance.PosCheck(ObjectManager.Instance._checkPath, Pos2Cell(next)) == false)
                        continue;
                }

                // �̹� �湮�� ���̸� ��ŵ
                if (closed[next.Y, next.X])
                    continue;

                // ��� ���
                int g = node.G + _cost[i];
                int h = 10 * ((dest.Y - next.Y) * (dest.Y - next.Y) + (dest.X - next.X) * (dest.X - next.X));
                // �ٸ� ��ο��� �� ���� �� �̹� ã������ ��ŵ
                if (open[next.Y, next.X] < g + h)
                    continue;

                // ���� ����
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
