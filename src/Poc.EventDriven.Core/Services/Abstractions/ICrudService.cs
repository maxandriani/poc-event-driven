namespace Poc.EventDriven.Services.Abstractions;

public interface ICrudService<TEntityDto, TKey, TSearchRequest, TCreateUpdateInput> :
  IReadOnlyService<TEntityDto, TKey, TSearchRequest>,
  ICreateable<TCreateUpdateInput, TEntityDto>,
  IUpdateable<TKey, TCreateUpdateInput, TEntityDto>,
  IDeleteable<TKey>
    where TEntityDto : class
    where TKey : class
    where TSearchRequest : class
    where TCreateUpdateInput : class
{
}

public interface ICrudService<TEntityDto, TKey, TSearchRequest, TCreateInput, TUpdateInput> :
  IReadOnlyService<TEntityDto, TKey, TSearchRequest>,
  ICreateable<TCreateInput, TEntityDto>,
  IUpdateable<TKey, TUpdateInput, TEntityDto>,
  IDeleteable<TKey>
    where TEntityDto : class
    where TKey : class
    where TSearchRequest : class
    where TCreateInput : class
    where TUpdateInput : class
{
}
