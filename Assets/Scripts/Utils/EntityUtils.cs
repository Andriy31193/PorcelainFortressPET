using UnityEngine;
public static class EntityUtils
{
    public static GameObject HandleCreation(EntityType entityType, Vector3 position)
    {
        if (entityType == EntityType.Void)
            return null;


        Entity entity = DIContainer.Resolve<EntitiesCollection>().GetEntitity(entityType);

        if (entity != null && !string.IsNullOrEmpty(entity.GetPrefabPath()))
        {
            GameObject creation = Resources.Load<GameObject>(entity.GetPrefabPath());

            if (creation != null)
            {
                return Object.Instantiate(creation, position, creation.transform.rotation);
            }
            else Debug.LogError($"Resource with path: {entity.GetPrefabPath()} was not found.");
        }
        return null;
    }
}
