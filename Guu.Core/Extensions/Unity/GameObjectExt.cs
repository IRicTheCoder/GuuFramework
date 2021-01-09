using UnityEngine;

namespace Guu
{
    /// <summary>
    /// Contains extension methods for Game Objects
    /// </summary>
    public static class GameObjectExt
    {
        /// <summary>
        /// Finds the child of a game object (using it's transform)
        /// </summary>
        /// <param name="obj">The obj to search in</param>
        /// <param name="name">The name of the child</param>
        /// <returns>The child if found, null otherwise</returns>
        public static GameObject FindChild(this GameObject obj, string name)
        {
            return obj.transform.Find(name)?.gameObject;
        }
    }
}