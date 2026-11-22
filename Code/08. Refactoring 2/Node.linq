<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#load "../basecode"
#load "./StoreMap"

interface IGraph
{
    Node[][] Nodes { get; }
    Node[] Garages { get; }
    
}

/// <summary>선반 노드의 집합. 개념적인 단위.</summary>
interface ILine
{
    int Index { get; }
    INode[][] Nodes { get; }
}

interface INode
{
    MyCell Cell { get; }
    int X => Cell.X;
    int Y => Cell.Y;

    INode? Up { get; }
    INode? Down { get; }
    INode? Left { get; }
    INode? Right { get; }
    INode[] Next { get; }

    /// <summary>차고 앞인가?</summary>
    bool IsGarage { get; }
    /// <summary>진열대 앞인가? (진열대 위치가 아니다)</summary>
    bool IsShelf { get; }
    /// <summary>계산대 앞인가? (계산대 위치가 아니다)</summary>
    bool IsRegister { get; }
    /// <summary>장바구니 앞인가? (장바구니 위치가 아니다)</summary>
    bool IsBag { get; }
}

class Node : INode
{
    public MyCell Cell { get; init; }

    public INode? Up { get; private set; }
    public INode? Down { get; private set; }
    public INode? Left { get; private set; }
    public INode? Right { get; private set; }
    public INode[] Next { get; private set; }

    public bool IsGarage { get; init; }
    public bool IsRegister { get; init; }
    public bool IsBag { get; init; }
    public bool IsShelf { get; init; }
    
    public Node(MyCell cell, bool isShelf)
    {
        Cell = cell;
        IsGarage = CalcIsGarage(cell);
        IsRegister = CalcIsRegister(cell);
        IsBag = CalcIsBag(cell);
        IsShelf = isShelf;
    }
    
    bool CalcIsGarage(MyCell cell)
    {
        return store.GetGarageX() == cell.X && store.GetGarageY() == cell.Y;
    }
    
    bool CalcIsRegister(MyCell cell)
    {
        var registers = store.FindAll(Entity.Register);
        // 고객은 항상 아래에서 줄을 선다.
        // 내 아래가 계산대인지 체크한다.
        return registers.Any(c => c.GetX() == cell.X && c.GetY() == (cell.Y + 1));
    }

    bool CalcIsBag(MyCell cell)
    {
        var bags = store.FindAll(Entity.Bag);
        // 고객은 항상 아래에서 줄을 선다.
        // 내 아래가 계산대인지 체크한다.
        return bags.Any(c => c.GetX() == cell.X && c.GetY() == (cell.Y + 1));
    }
}
