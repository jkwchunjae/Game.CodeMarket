<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#load "../basecode"

record EntityProductPair(Entity Entity, Product Product);

interface IStaticData
{
    /// <summary> 진열대와 상품의 짝을 구한다. </summary>
    EntityProductPair[] GetEntityProductPair();
    /// <summary> 진열대에 진열할 수 있는 상품을 구한다.</summary>
    Product? GetProductFromEntity(Entity entity);
    /// <summary> 상품을 진열할 수 있는 진열대를 구한다.</summary>
    Entity GetEntityFromProduct(Product product);
}
class StaticData : IStaticData
{
    private readonly Dictionary<Entity, Product> _entityProductPairDic;
    private readonly Dictionary<Product, Entity> _productEntityPairDic;

    public StaticData()
    {
        _entityProductPairDic = GetEntityProductPair()
            .ToDictionary(x => x.Entity, x => x.Product);
        _productEntityPairDic = GetEntityProductPair()
            .ToDictionary(x => x.Product, x => x.Entity);
    }
    public EntityProductPair[] GetEntityProductPair()
    {
        return [
            new EntityProductPair(Entity.TomatoTable, Product.Tomato),
            new EntityProductPair(Entity.PumpkinTable, Product.Pumpkin),
            new EntityProductPair(Entity.BreadShelf, Product.Bread),
            new EntityProductPair(Entity.Fridge, Product.Soda),
            new EntityProductPair(Entity.MilkStand, Product.Milk),
            new EntityProductPair(Entity.BeerFridge, Product.Beer),
            new EntityProductPair(Entity.FertilizerTable, Product.Fertilizer),
            new EntityProductPair(Entity.CigaretteStand, Product.Cigarettes),
        ];
    }
    public Product? GetProductFromEntity(Entity entity)
    {
        if (_entityProductPairDic.TryGetValue(entity, out var product))
        {
            return product;
        }
        else
        {
            return null;
        }
    }
    public Entity GetEntityFromProduct(Product product)
    {
        return _productEntityPairDic[product];
    }
}