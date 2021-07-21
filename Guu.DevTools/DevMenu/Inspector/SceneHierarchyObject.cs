using UnityEngine;

namespace Guu.DevTools.DevMenu
{
	internal class SceneHierarchyObject
	{
		public SceneHierarchyObject Parent { get; }
		public GameObject GameObject { get; }
		public int ChildIndent { get; }
		public bool IsUnfolded { get; set; }
		public bool IsHidden { get; set; }

		internal SceneHierarchyObject(GameObject gameObject, int indent)
		{
			Parent = null;
			GameObject = gameObject;
			ChildIndent = indent;
		}
		
		internal SceneHierarchyObject(SceneHierarchyObject parent, GameObject gameObject, int indent)
		{
			Parent = parent;
			GameObject = gameObject;
			ChildIndent = indent;
		}

		internal bool HasChildren() => GameObject.transform.childCount > 0;
	}
}