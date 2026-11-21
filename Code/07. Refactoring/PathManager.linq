<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#load "./StoreMap"

interface IPathManager
{
    MyCell[] GetPath(MyCell start, MyCell end);
    bool ContainsInCell(MyCell[] cells, MyCell cell);
}