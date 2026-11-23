using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ClientNoSqlDB.EntityFrameworkCore;

public abstract class ClientNoSqlDbContext : DbContext
{
    private readonly DbInstance _database;
    private bool _initialized;
    private bool _initializationStarted;
    private bool _loading;
    private bool _databaseDisposed;

    protected ClientNoSqlDbContext(string path)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);
        _database = new DbInstance(path);
    }

    public void Initialize()
    {
        if (_initializationStarted)
            throw new InvalidOperationException("The context is already initialized.");

        _initializationStarted = true;
        try
        {
            var entityTypes = Model.GetEntityTypes().ToArray();
            if (entityTypes.Length == 0)
                throw new InvalidOperationException("The context model does not contain any entity types.");

            foreach (var entityType in entityTypes)
            {
                if (entityType.IsOwned() || entityType.BaseType != null)
                    throw new NotSupportedException($"Owned and inherited entity types are not supported: '{entityType.DisplayName()}'.");

                var key = entityType.FindPrimaryKey();
                if (key == null || key.Properties.Count != 1)
                    throw new NotSupportedException($"Entity type '{entityType.DisplayName()}' must have exactly one primary key property.");

                ConfigureEntity(_database, entityType, key.Properties[0]);
            }

            _database.Initialize();
            _loading = true;
            try
            {
                foreach (var entityType in entityTypes)
                    base.AddRange(LoadEntities(_database, entityType.ClrType));

                base.SaveChanges();
            }
            finally
            {
                _loading = false;
            }

            _initialized = true;
        }
        catch
        {
            DisposeDatabase();
            throw;
        }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString("N"));
    }

    public override int SaveChanges()
    {
        return SaveChanges(acceptAllChangesOnSuccess: true);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        if (_loading)
            return base.SaveChanges(acceptAllChangesOnSuccess);

        EnsureInitialized();

        var changes = ChangeTracker.Entries()
            .Where(entry => entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToArray();

        _database.BulkWrite(() =>
        {
            foreach (var entry in changes)
            {
                if (entry.State == EntityState.Deleted)
                {
                    var keyProperty = GetKeyProperty(entry.Metadata);
                    DeleteEntity(_database, entry.Metadata.ClrType, entry.Property(keyProperty.Name).CurrentValue);
                }
                else
                {
                    SaveEntity(_database, entry.Entity);
                }
            }
        });

        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return SaveChangesAsync(acceptAllChangesOnSuccess: true, cancellationToken);
    }

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(SaveChanges(acceptAllChangesOnSuccess));
    }

    public override void Dispose()
    {
        DisposeDatabase();
        base.Dispose();
    }

    public override async ValueTask DisposeAsync()
    {
        DisposeDatabase();
        await base.DisposeAsync().ConfigureAwait(false);
    }

    private void DisposeDatabase()
    {
        if (_databaseDisposed)
            return;

        _database.Dispose();
        _databaseDisposed = true;
    }

    private void EnsureInitialized()
    {
        if (!_initialized)
            throw new InvalidOperationException("Call Initialize after constructing the context and before saving changes.");
    }

    private static void ConfigureEntity(DbInstance database, IEntityType entityType, IProperty keyProperty)
    {
        var entityTypeMethod = typeof(ClientNoSqlDbContext).GetMethod(
            nameof(ConfigureEntityGeneric),
            BindingFlags.NonPublic | BindingFlags.Static,
            [typeof(DbInstance), typeof(IEntityType), typeof(IProperty)])!;

        entityTypeMethod.MakeGenericMethod(entityType.ClrType, keyProperty.ClrType)
            .Invoke(null, [database, entityType, keyProperty]);
    }

    private static void ConfigureEntityGeneric<TEntity, TKey>(
        DbInstance database,
        IEntityType entityType,
        IProperty keyProperty)
        where TEntity : class
    {
        var constructor = typeof(TEntity).GetConstructor(Type.EmptyTypes);
        if (constructor == null)
            throw new NotSupportedException($"Entity type '{entityType.DisplayName()}' must have a public parameterless constructor.");

        var map = database.Map<TEntity>(() => (TEntity)Activator.CreateInstance(typeof(TEntity))!);
        var keyMember = GetPublicMember(keyProperty, entityType);
        var parameter = Expression.Parameter(typeof(TEntity), "entity");
        var keyBody = Expression.MakeMemberAccess(parameter, keyMember);
        var keyLambda = Expression.Lambda(
            typeof(Func<,>).MakeGenericType(typeof(TEntity), typeof(TKey)),
            keyBody,
            parameter);

        var mapKey = map.GetType().GetMethod(nameof(TypeMap<TEntity>.Key))!.MakeGenericMethod(typeof(TKey));
        mapKey.Invoke(map, [keyLambda, keyProperty.ValueGenerated == ValueGenerated.OnAdd, null]);

        foreach (var property in entityType.GetProperties().Where(property => property != keyProperty))
        {
            var member = GetPublicMember(property, entityType);
            var propertyParameter = Expression.Parameter(typeof(TEntity), "entity");
            var propertyBody = Expression.MakeMemberAccess(propertyParameter, member);
            var propertyLambda = Expression.Lambda(
                typeof(Func<,>).MakeGenericType(typeof(TEntity), property.ClrType),
                propertyBody,
                propertyParameter);
            var mapProperty = map.GetType().GetMethod(nameof(TypeMap<TEntity>.Map))!
                .MakeGenericMethod(property.ClrType);
            mapProperty.Invoke(map, [propertyLambda]);
        }
    }

    private static MemberInfo GetPublicMember(IProperty property, IEntityType entityType)
    {
        var member = (MemberInfo?)property.PropertyInfo ?? property.FieldInfo;
        if (member is PropertyInfo propertyInfo &&
            propertyInfo.GetMethod?.IsPublic == true &&
            propertyInfo.SetMethod?.IsPublic == true)
            return propertyInfo;

        if (member is FieldInfo fieldInfo && fieldInfo.IsPublic && !fieldInfo.IsInitOnly)
            return fieldInfo;

        throw new NotSupportedException(
            $"Property '{property.Name}' on entity type '{entityType.DisplayName()}' must map to a public, read-write property or field.");
    }

    private static IProperty GetKeyProperty(IEntityType entityType)
    {
        var key = entityType.FindPrimaryKey();
        if (key == null || key.Properties.Count != 1)
            throw new NotSupportedException($"Entity type '{entityType.DisplayName()}' must have exactly one primary key property.");

        return key.Properties[0];
    }

    private static object[] LoadEntities(DbInstance database, Type entityType)
    {
        var loadAll = typeof(DbInstance).GetMethod(nameof(DbInstance.LoadAll))!.MakeGenericMethod(entityType);
        return (object[])loadAll.Invoke(database, null)!;
    }

    private static void SaveEntity<TEntity>(DbInstance database, TEntity entity) where TEntity : class
    {
        database.Save(entity);
    }

    private static void SaveEntity(DbInstance database, object entity)
    {
        var save = typeof(ClientNoSqlDbContext).GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
            .Single(method => method.Name == nameof(SaveEntity)
                && method.IsGenericMethod
                && method.GetParameters().Length == 2);
        save.MakeGenericMethod(entity.GetType()).Invoke(null, [database, entity]);
    }

    private static void DeleteEntity(DbInstance database, Type entityType, object? key)
    {
        if (key == null)
            throw new InvalidOperationException($"Cannot delete entity type '{entityType.Name}' with a null key.");

        var delete = typeof(DbInstance).GetMethods()
            .Single(method => method.Name == nameof(DbInstance.DeleteByKey)
                && method.IsGenericMethod
                && method.GetParameters().Length == 1
                && method.GetParameters()[0].ParameterType == typeof(object));
        delete.MakeGenericMethod(entityType).Invoke(database, [key]);
    }
}
