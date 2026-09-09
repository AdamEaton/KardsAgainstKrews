using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

/// <summary>
/// Exception indicating that an invalid/nonexistent path was followed in a Catalog.
/// </summary>
public class CatalogNotFoundException : Exception
{
	public CatalogNotFoundException(string message)
		: base(message)
	{ }
}

/// <summary>
/// Represents a hierarchical directory structure, containing a set of named items as children, as well as a set of named sub-Catalogs.
/// 
/// A Catalog can share its name with an Entry in the same path, but not with other sub-Catalogs in the same path, nor can an Entry share its name with other Entries in the same path.
/// </summary>
/// <typeparam name="T">The type of data to store in each Entry in this Catalog.</typeparam>
public class Catalog<T>
{
	#region Entry Class

	/// <summary>
	/// Represents a single named item in a Catalog.
	/// </summary>
	public class Entry
	{
		/// <summary>
		/// The data stored at this Entry.
		/// </summary>
		public T item;
		/// <summary>
		/// The name used to identify this entry within its containing Catalog.
		/// </summary>
		public string Name { get; private set; }
		/// <summary>
		/// The full path to the Catalog in which this Entry is directly contained.
		/// </summary>
		public string ContainingPath { get { return Parent.FullName; } }
		/// <summary>
		/// The full path to this Entry.
		/// </summary>
		public string FullName { get { return PathUtil.Combine(Parent.FullName, Name); } }
		/// <summary>
		/// The Catalog in which this Entry is directly contained.
		/// </summary>
		public Catalog<T> Parent { get; private set; }

		/// <summary>
		/// Constructs an Entry childed to the given parent Catalog, with the given name, and the given item as data.
		/// </summary>
		/// <param name="parent">The Catalog in which to store this Entry.</param>
		/// <param name="name">The identifier to use for this Entry.</param>
		/// <param name="item">The data to store in this Entry.</param>
		internal Entry(Catalog<T> parent, string name, T item)
		{
			if (parent == null)
				throw new ArgumentNullException("An Entry cannot be constructed in a null Catalog!");

			this.Parent = parent;
			this.Name = name;
			this.item = item;
		}

		/// <summary>
		/// Exception-safe implementation of GetRelativeName.
		/// 
		/// Outputs to the specified string the path that must be followed from the specified Catalog to reach this Entry.
		/// </summary>
		/// <param name="catalog">The Catalog to treat as the root.</param>
		/// <param name="path">If successful (returned true), outputs a string that can be used to index the specified Catalog to find this Entry, otherwise outputs an empty string.</param>
		/// <returns>True if this Entry exists within the given Catalog, otherwise false.</returns>
		public bool TryGetRelativeName(Catalog<T> catalog, out string path)
		{
			try
			{
				path = GetRelativeName(catalog);
				return true;
			}
			catch (CatalogNotFoundException)
			{
				path = "";
				return false;
			}
		}
		/// <summary>
		/// The path that must be followed from the specified Catalog to reach this Entry.
		/// 
		/// Will throw a CatalogNotFoundException if this Entry does not exist in the specified Catalog.
		/// </summary>
		/// <param name="catalog">The Catalog to treat as the root.</param>
		/// <returns>A string that can be used to index the specified Catalog to find this Entry.</returns>
		public string GetRelativeName(Catalog<T> catalog)
		{
			return PathUtil.GetRelativePath(catalog.FullName, FullName);
		}
	}

	#endregion

	#region Private Variables

	/// <summary>
	/// Set of entries contained directly inside this Catalog.
	/// </summary>
	Dictionary<string, Entry> entries = new Dictionary<string, Entry>();
	/// <summary>
	/// Set of sub-Catalogs contained directly inside this Catalog.
	/// </summary>
	Dictionary<string, Catalog<T>> subTree = new Dictionary<string, Catalog<T>>();

	#endregion

	#region Properties
		
	/// <summary>
	/// The name used to identify this Catalog within its containing Catalog (or the name of the root, if it has no parent).
	/// </summary>
	public string Name { get; private set; }
	/// <summary>
	/// The full path to the Catalog in which this Catalog is directly contained (empty if it is the root).
	/// </summary>
	public string ContainingPath { get { return Parent.FullName; } }
	/// <summary>
	/// The full path to this Catalog.
	/// </summary>
	public string FullName { get { return Parent == null ? Name : PathUtil.Combine(Parent.FullName, Name); } }
	
	/// <summary>
	/// The Catalog in which this Catalog is directly contained (null if it is the root).
	/// </summary>
	public Catalog<T> Parent { get; private set; }

	/// <summary>
	/// The number of Entries in this Catalog.
	/// </summary>
	public int Count { get { return GetAllEntries().Count(); } }

	#endregion

	#region Indexer

	/// <summary>
	/// The item at the specified relative path within this Catalog.
	/// 
	/// The get accessor throws a CatalogNotFoundException if the specified path doesn't reference an Entry in this Catalog.
	/// 
	/// The set accessor will create a new Entry if the specified path doesn't reference an Entry in this Catalog.
	/// </summary>
	/// <param name="path">The relative path to follow from this Catalog.</param>
	/// <returns>The item at the specified path within this Catalog.</returns>
	public T this[string path]
	{
		get { return GetItem(path); }
		set
		{
			string child = PathUtil.GetRoot(path);
			string subPath = PathUtil.PopRoot(path);

			if (subPath == "")
			{
				Entry entry;
				if (entries.TryGetValue(child, out entry))
					entry.item = value;
				else
					entries[child] = new Entry(this, child, value);
				return;
			}
			else
			{
				Catalog<T> catalog;
				if (subTree.TryGetValue(child, out catalog))
					catalog[subPath] = value;
				else
				{
					subTree.Add(child, new Catalog<T>(this, child));
					subTree[child][subPath] = value;
				}
			}
		}
	}

	#endregion

	#region Constructors

	/// <summary>
	/// Constructs a Catalog with the specified parent and name.
	/// </summary>
	/// <param name="parent">The containing Catalog, which this Catalog will become a sub-Catalog of (this Catalog will become a root Catalog if set to null).</param>
	/// <param name="name">The name used to identify this Catalog.</param>
	public Catalog(Catalog<T> parent, string name)
	{
		this.Parent = parent;
		this.Name = name;
	}

	#endregion

	#region Public methods

	/// <summary>
	/// Creates a shallow copy of this Catalog and all contained sub-Catalogs and Entries.
	/// </summary>
	/// <returns>A new Catalog with the same data as this Catalog.</returns>
	public Catalog<T> Clone()
	{
		Catalog<T> output = new Catalog<T>(null, Name);

		foreach (var entry in GetAllEntries(false))
			output[entry.GetRelativeName(this)] = entry.item;

		return output;
	}

	/// <summary>
	/// Exception-safe implementation of GetRelativeName.
	/// 
	/// Outputs to the specified string the path that must be followed from the specified Catalog to reach this sub-Catalog.
	/// </summary>
	/// <param name="catalog">The Catalog to treat as the root.</param>
	/// <param name="path">If successful (returned true), outputs a string that can be used to index the specified Catalog to find this sub-Catalog, otherwise outputs an empty string.</param>
	/// <returns>True if this sub-Catalog exists within the given Catalog, otherwise false.</returns>
	public bool TryGetRelativeName(Catalog<T> catalog, out string path)
	{
		try
		{
			path = GetRelativeName(catalog);
			return true;
		}
		catch (CatalogNotFoundException)
		{
			path = "";
			return false;
		}
	}
	/// <summary>
	/// The path that must be followed from the specified Catalog to reach this sub-Catalog.
	/// 
	/// Will throw a CatalogNotFoundException if this sub-Catalog does not exist in the specified Catalog.
	/// </summary>
	/// <param name="catalog">The Catalog to treat as the root.</param>
	/// <returns>A string that can be used to index the specified Catalog to find this sub-Catalog.</returns>
	public string GetRelativeName(Catalog<T> catalog)
	{
		return PathUtil.GetRelativePath(catalog.FullName, FullName);
	}

	/// <summary>
	/// Removes Entry-free sub-Catalogs contained directly within this Catalog.
	/// </summary>
	public void RemoveEmptySubCatalogs()
	{
		List<string> toDelete = new List<string>();

		foreach (var catalog in GetSubCatalogs())
			if (catalog.GetAllEntries().Count() <= 0)
				toDelete.Add(catalog.GetRelativeName(this));

		foreach (var path in toDelete)
			DeleteSubCatalog(path);
	}
	/// <summary>
	/// Removes Entry-free sub-Catalogs from this Catalog or any of its sub-Catalogs.
	/// </summary>
	public void RemoveAllEmptySubCatalogs()
	{
		RemoveEmptySubCatalogs();

		foreach (var catalog in GetSubCatalogs())
			catalog.RemoveAllEmptySubCatalogs();
	}

	#region Existence

	/// <summary>
	/// Does the specified item exist anywhere inside this Catalog?
	/// </summary>
	/// <param name="item">The item to test existence of.</param>
	/// <returns>True if the item exists anywhere within this Catalog, otherwise false.</returns>
	public bool HasItem(T item)
	{
		foreach (var i in GetAllItems())
			if (i.Equals(item))
				return true;
		return false;
	}
	/// <summary>
	/// Does an Entry exist at the specified path inside this Catalog?
	/// </summary>
	/// <param name="path">The relative path to follow from this Catalog.</param>
	/// <returns>True if an Entry exists at the specified path within this Catalog, otherwise false.</returns>
	public bool HasEntry(string path)
	{
		Entry entry;
		return TryGetEntry(path, out entry);
	}
	/// <summary>
	/// Does a sub-Catalog exist at the specified path inside this Catalog?
	/// </summary>
	/// <param name="path">The relative path to follow from this Catalog.</param>
	/// <returns>True if a sub-Catalog exists at the specified path within this Catalog, otherwise false.</returns>
	public bool HasSubCatalog(string path)
	{
		Catalog<T> catalog;
		return TryGetSubCatalog(path, out catalog);
	}

	#endregion

	#region Find Methods

	/// <summary>
	/// Determines the path to the first instance of the specified item found using the specified traversal of this Catalog, otherwise false.
	/// </summary>
	/// <param name="item">The item to find.</param>
	/// <param name="breadthFirst">Specifies the traversal to use. Depth-first (false) is faster. Breadth-first (true) is slower, but returns the result that is the fewest levels deep.</param>
	/// <returns>If found, the relative path of the item within this Catalog, otherwise an empty string.</returns>
	public string FindItem(T item, bool breadthFirst = false)
	{
		foreach (var entry in GetAllEntries(breadthFirst))
			if (entry.item.Equals(item))
				return entry.GetRelativeName(this);
		return "";
	}
	/// <summary>
	/// Determines the path to all instances of the specified item in order of discovery using the specified traversal of this Catalog.
	/// </summary>
	/// <param name="item">The item to find instances of.</param>
	/// <param name="breadthFirst">Specifies the traversal to use. Depth-first (false) is faster. Breadth-first (true) is slower, but returns results in order of depth.</param>
	/// <returns>All paths to instances of the the item within this Catalog.</returns>
	public IEnumerable<string> FindItemAll(T item, bool breadthFirst = false)
	{
		foreach (var entry in GetAllEntries(breadthFirst))
			if (entry.item.Equals(item))
				yield return entry.FullName;
	}

	#endregion

	#region Item Getters

	/// <summary>
	/// Exception-safe implementation of GetItem.
	/// 
	/// Outputs to the specified item the item at the specified path in this Catalog.
	/// </summary>
	/// <param name="path">The relative path to follow from this Catalog.</param>
	/// <param name="item">If successful (returned true), outputs the item that was found, otherwise outputs a default value.</param>
	/// <returns>True if the path referenced an Entry in this Catalog, otherwise false.</returns>
	public bool TryGetItem(string path, out T item)
	{
		try
		{
			item = GetItem(path);
			return true;
		}
		catch (CatalogNotFoundException)
		{
			item = default(T);
			return false;
		}
	}
	/// <summary>
	/// Gets the item stored at the specified path in this Catalog.
	/// 
	/// Throws a CatalogNotFoundException if the specified path doesn't reference an Entry in this Catalog.
	/// </summary>
	/// <param name="path">The relative path to follow from this Catalog.</param>
	/// <returns>The item at the specified path within this Catalog.</returns>
	public T GetItem(string path)
	{
		return GetEntry(path).item;
	}
	/// <summary>
	/// Gets all items stored directly within this Catalog.
	/// </summary>
	/// <returns>Every item whose parent Catalog is this Catalog.</returns>
	public IEnumerable<T> GetItems()
	{
		foreach (var entry in entries.Values)
			yield return entry.item;
	}
	/// <summary>
	/// Gets all items stored within this Catalog or any of its sub-Catalogs in order of discovery using the specified traversal of this Catalog.
	/// </summary>
	/// <param name="breadthFirst">Specifies the traversal to use. Depth-first (false) is faster. Breadth-first (true) is slower, but returns results in order of depth.</param>
	/// <returns>Every item found in this Catalog or its sub-Catalogs.</returns>
	public IEnumerable<T> GetAllItems(bool breadthFirst = false)
	{
		if (breadthFirst)
			foreach (var t in GetAllItemsBreadthFirst())
				yield return t;
		else
			foreach (var t in GetAllItemsDepthFirst())
				yield return t;
	}

	#endregion

	#region Entry Getters
	
	/// <summary>
	/// Exception-safe implementation of GetEntry.
	/// 
	/// Outputs to the specified Entry the Entry at the specified path in this Catalog.
	/// </summary>
	/// <param name="path">The relative path to follow from this Catalog.</param>
	/// <param name="item">If successful (returned true), outputs the Entry that was found, otherwise outputs null.</param>
	/// <returns>True if the path referenced an Entry in this Catalog, otherwise false.</returns>
	public bool TryGetEntry(string path, out Entry entry)
	{
		try
		{
			entry = GetEntry(path);
			return true;
		}
		catch (CatalogNotFoundException)
		{
			entry = null;
			return false;
		}
	}
	/// <summary>
	/// Gets the Entry stored at the specified path in this Catalog.
	/// 
	/// Throws a CatalogNotFoundException if the specified path doesn't reference an Entry in this Catalog.
	/// </summary>
	/// <param name="path">The relative path to follow from this Catalog.</param>
	/// <returns>The Entry at the specified path within this Catalog.</returns>
	public Entry GetEntry(string path)
	{
		if (path == "")
			throw new CatalogNotFoundException("'" + FullName + "' is a Catalog, not an Entry.");

		string child = PathUtil.GetRoot(path);
		string subPath = PathUtil.PopRoot(path);

		if (subPath == "")
		{
			Entry entry;
			if (entries.TryGetValue(child, out entry))
				return entry;
		}
		else
		{
			Catalog<T> catalog;
			if (subTree.TryGetValue(child, out catalog))
				return catalog.GetEntry(subPath);
		}

		throw new CatalogNotFoundException("The specified path '" + path + "' was not found in the catalog '" + FullName + "'.");
	}
	/// <summary>
	/// Gets all Entries stored directly within this Catalog.
	/// </summary>
	/// <returns>Every Entry whose parent Catalog is this Catalog.</returns>
	public IEnumerable<Entry> GetEntries()
	{
		foreach (var entry in entries.Values)
			yield return entry;
	}
	/// <summary>
	/// Gets all Entries stored within this Catalog or any of its sub-Catalogs in order of discovery using the specified traversal of this Catalog.
	/// </summary>
	/// <param name="breadthFirst">Specifies the traversal to use. Depth-first (false) is faster. Breadth-first (true) is slower, but returns results in order of depth.</param>
	/// <returns>Every Entry found in this Catalog or its sub-Catalogs.</returns>
	public IEnumerable<Entry> GetAllEntries(bool breadthFirst = false)
	{
		if (breadthFirst)
			foreach (var entry in GetAllEntriesBreadthFirst())
				yield return entry;
		else
			foreach (var entry in GetAllEntriesDepthFirst())
				yield return entry;
	}

	#endregion

	#region Catalog Getters
	
	/// <summary>
	/// Exception-safe implementation of GetSubCatalog.
	/// 
	/// Outputs to the specified Catalog the sub-Catalog at the specified path in this Catalog to the specified Catalog.
	/// </summary>
	/// <param name="path">The relative path to follow from this Catalog.</param>
	/// <param name="item">If successful (returned true), outputs the sub-Catalog that was found, otherwise outputs null.</param>
	/// <returns>True if the path referenced an sub-Catalog in this Catalog, otherwise false.</returns>
	public bool TryGetSubCatalog(string path, out Catalog<T> catalog)
	{
		try
		{
			catalog = GetSubCatalog(path);
			return true;
		}
		catch (CatalogNotFoundException)
		{
			catalog = null;
			return false;
		}
	}
	/// <summary>
	/// Gets the sub-Catalog stored at the specified path in this Catalog.
	/// 
	/// Throws a CatalogNotFoundException if the specified path doesn't reference a sub-Catalog in this Catalog.
	/// </summary>
	/// <param name="path">The relative path to follow from this Catalog.</param>
	/// <returns>The sub-Catalog at the specified path within this Catalog.</returns>
	public Catalog<T> GetSubCatalog(string path)
	{
		if (path == "")
			return this;

		string child = PathUtil.GetRoot(path);
		string subPath = PathUtil.PopRoot(path);

		foreach (var catalog in subTree.Values)
			if (catalog.Name == child)
				return catalog.GetSubCatalog(subPath);

		throw new CatalogNotFoundException("The specified path '" + path + "' was not found in the catalog '" + FullName + "'.");
	}
	/// <summary>
	/// Gets all sub-Catalogs stored directly within this Catalog.
	/// </summary>
	/// <returns>Every sub-Catalog whose parent Catalog is this Catalog.</returns>
	public IEnumerable<Catalog<T>> GetSubCatalogs()
	{
		foreach (var catalog in subTree.Values)
			yield return catalog;
	}
	/// <summary>
	/// Gets all sub-Catalog stored within this Catalog or any of its sub-Catalogs in order of discovery using the specified traversal of this Catalog.
	/// </summary>
	/// <param name="breadthFirst">Specifies the traversal to use. Depth-first (false) is faster. Breadth-first (true) is slower, but returns results in order of depth.</param>
	/// <returns>Every sub-Catalog found in this Catalog or its sub-Catalogs.</returns>
	public IEnumerable<Catalog<T>> GetAllSubCatalogs(bool breadthFirst = false)
	{
		if (breadthFirst)
			foreach (var catalog in GetAllSubCatalogsBreadthFirst())
				yield return catalog;
	}

	#endregion

	#region Insertion

	/// <summary>
	/// Inserts the specified Entry at the specified path in this Catalog, optionally overwriting any existing Entry at that path.
	/// </summary>
	/// <param name="path">The relative path to insert the data at. The final location of the inserted data will be this path combined with the name of the Entry being added.</param>
	/// <param name="entry">The Entry to copy to the specified path.</param>
	/// <param name="overwrite">If set to true, existing data will be overwritten. If set to false, the data will not be copied to locations already containing the Entry's name.</param>
	/// <returns>True if the insertion was carried out, otherwise false.</returns>
	public bool InsertAt(string path, Entry entry, bool overwrite = false)
	{
		if (HasEntry(PathUtil.Combine(path, entry.Name)))
			if (!overwrite)
				return false;

		this[PathUtil.Combine(path, entry.Name)] = entry.item;
		return true;
	}
	/// <summary>
	/// Inserts the specified sub-Catalog at the specified path in this Catalog, optionally merging with any existing sub-Catalog at that path.
	/// </summary>
	/// <param name="path">The relative path to insert the data at. The final location of the inserted data will be this path combined with the root name of the sub-Catalog being added.</param>
	/// <param name="catalog">The sub-Catalog to copy to the specified path.</param>
	/// <param name="merge">If set to true, the new sub-Catalog will be merged with existing data, overwriting any colliding Entries. If set to false, the data will not be copied to existing locations.</param>
	/// <returns>True if the insertion was carried out, otherwise false.</returns>
	public bool InsertAt(string path, Catalog<T> catalog, bool merge = false)
	{
		if (HasSubCatalog(PathUtil.Combine(path, catalog.Name)))
			if (!merge)
				return false;

		foreach (var entry in catalog.GetAllEntries())
			this[PathUtil.Combine(path, PathUtil.GetRelativePath(catalog.FullName, entry.FullName))] = entry.item;

		return true;
	}

	#endregion

	#region Deletion

	/// <summary>
	/// Delete the Entry at the specified path in this Catalog.
	/// </summary>
	/// <param name="path">The relative path of the Entry to delete.</param>
	/// <returns>True if the Entry existed, otherwise false.</returns>
	public bool DeleteEntry(string path)
	{
		Entry entry;
		if (TryGetEntry(path, out entry))
		{
			entry.Parent.entries.Remove(entry.Name);
			return true;
		}
		else
			return false;
	}
	/// <summary>
	/// Delete the sub-Catalog at the specified path in this Catalog.
	/// </summary>
	/// <param name="path">The relative path of the sub-Catalog to delete.</param>
	/// <returns>True if the sub-Catalog existed, otherwise false.</returns>
	public bool DeleteSubCatalog(string path)
	{
		if (path == "")
			throw new InvalidOperationException("Cannot delete the root of a catalog!");

		Catalog<T> catalog;
		if (TryGetSubCatalog(path, out catalog))
		{
			catalog.Parent.subTree.Remove(catalog.Name);
			catalog.Parent = null;
			return true;
		}
		else
			return false;
	}
	/// <summary>
	/// Removes all Entries and sub-Catalogs from this Catalog.
	/// </summary>
	public void Clear()
	{
		entries.Clear();
		subTree.Clear();
	}

	#endregion

	#region Reorganization

	/// <summary>
	/// Exception-safe implementation of RenameEntry.
	/// 
	/// Changes the name of the Entry at the specified path in this Catalog to match the specified name, optionally overwriting any existing Entry with the specified name in the same location.
	/// </summary>
	/// <param name="path">The relative path of the Entry to rename.</param>
	/// <param name="newName">The name to assign to the Entry.</param>
	/// <param name="success">If successful (returned true), outputs true if the renaming was carried out, otherwise false.</param>
	/// <param name="overwrite">If set to true, existing data will be overwritten. If set to false, the data will not be renamed to existing names.</param>
	/// <returns>True if the path referenced an Entry in this Catalog, otherwise false.</returns>
	public bool TryRenameEntry(string path, string newName, out bool success, bool overwrite = false)
	{
		try
		{
			success = RenameEntry(path, newName, overwrite);
			return true;
		}
		catch (CatalogNotFoundException)
		{
			success = false;
			return false;
		}
	}
	/// <summary>
	/// Changes the name of the Entry at the specified path in this Catalog to match the specified name, optionally overwriting any existing Entry with the specified name in the same location.
	/// 
	/// Will throw a CatalogNotFoundException if the specified path does not reference an Entry in this Catalog.
	/// </summary>
	/// <param name="path">The relative path of the Entry to rename.</param>
	/// <param name="newName">The name to assign to the Entry.</param>
	/// <param name="overwrite">If set to true, existing data will be overwritten. If set to false, the data will not be renamed to existing names.</param>
	/// <returns>True if the renaming was carried out, otherwise false.</returns>
	public bool RenameEntry(string path, string newName, bool overwrite = false)
	{
		var entry = GetEntry(path);

		if (entry.Parent.HasEntry(newName))
		{
			if (overwrite)
				entry.Parent[newName] = entry.item;
			else
				return false;
		}
		else
			entry.Parent[newName] = entry.item;

		entry.Parent.DeleteEntry(entry.Name);
		return true;
	}
	/// <summary>
	/// Exception-safe implementation of MoveEntry.
	/// 
	/// Moves the Entry at the specified path in this Catalog to the specified destination, optionally overwriting any existing Entry with the specified name in the that location.
	/// </summary>
	/// <param name="fromPath">The relative path of the Entry to move.</param>
	/// <param name="toPath">The destination location of the Entry.</param>
	/// <param name="success">If successful (returned true), outputs true if the move was carried out, otherwise false.</param>
	/// <param name="overwrite">If set to true, existing data will be overwritten. If set to false, the data will not be moved to locations already containing the specified name.</param>
	/// <returns>True if the path referenced an Entry in this Catalog, otherwise false.</returns>
	public bool TryMoveEntry(string fromPath, string toPath, out bool success, bool overwrite = false)
	{
		try
		{
			success = MoveEntry(fromPath, toPath, overwrite);
			return true;
		}
		catch (CatalogNotFoundException)
		{
			success = false;
			return false;
		}
	}
	/// <summary>
	/// Moves the Entry at the specified path in this Catalog to the specified destination, optionally overwriting any existing Entry with the specified name in the that location.
	/// 
	/// Will throw a CatalogNotFoundException if the specified path does not reference an Entry in this Catalog.
	/// </summary>
	/// <param name="fromPath">The relative path of the Entry to move.</param>
	/// <param name="toPath">The destination location of the Entry.</param>
	/// <param name="overwrite">If set to true, existing data will be overwritten. If set to false, the data will not be moved to locations already containing the specified name.</param>
	/// <returns>True if the move was carried out, otherwise false.</returns>
	public bool MoveEntry(string fromPath, string toPath, bool overwrite = false)
	{
		var entry = GetEntry(fromPath);

		if (HasEntry(toPath))
		{
			if (overwrite)
				this[toPath] = entry.item;
			else
				return false;
		}
		else
			this[toPath] = entry.item;

		DeleteEntry(fromPath);
		return true;
	}
	
	/// <summary>
	/// Exception-safe implementation of RenameSubCatalog.
	/// 
	/// Changes the name of the sub-Catalog at the specified path in this Catalog to match the specified name, optionally merging with any existing sub-Catalog with the specified name in the same location.
	/// </summary>
	/// <param name="path">The relative path of the sub-Catalog to rename.</param>
	/// <param name="newName">The name to assign to the sub-Catalog.</param>
	/// <param name="success">If successful (returned true), outputs true if the renaming was carried out, otherwise false.</param>
	/// <param name="merge">If set to true, the new sub-Catalog will be merged with existing data, overwriting any colliding Entries. If set to false, the data will not be renamed to existing names.</param>
	/// <returns>True if the path referenced a sub-Catalog in this Catalog, otherwise false.</returns>
	public bool TryRenameSubCatalog(string path, string newName, out bool success, bool merge = false)
	{
		try
		{
			success = RenameSubCatalog(path, newName, merge);
			return true;
		}
		catch (CatalogNotFoundException)
		{
			success = false;
			return false;
		}
	}
	/// <summary>
	/// Changes the name of the sub-Catalog at the specified path in this Catalog to match the specified name, optionally merging with any existing sub-Catalog with the specified name in the same location.
	/// 
	/// Will throw a CatalogNotFoundException if the specified path does not reference a sub-Catalog in this Catalog.
	/// </summary>
	/// <param name="path">The relative path of the sub-Catalog to rename.</param>
	/// <param name="newName">The name to assign to the sub-Catalog.</param>
	/// <param name="merge">If set to true, the new sub-Catalog will be merged with existing data, overwriting any colliding Entries. If set to false, the data will not be renamed to existing names.</param>
	/// <returns>True if the renaming was carried out, otherwise false.</returns>
	public bool RenameSubCatalog(string path, string newName, bool merge = false)
	{
		var catalog = GetSubCatalog(path);

		if (catalog.Parent.HasSubCatalog(newName))
		{
			if (merge)
			{
				foreach (var entry in catalog.GetAllEntries())
					catalog.Parent[PathUtil.GetRelativePath(catalog.FullName, entry.FullName)] = entry.item;
			}
			else
				return false;
		}
		else
			foreach (var entry in catalog.GetAllEntries())
				catalog.Parent[PathUtil.GetRelativePath(catalog.FullName, entry.FullName)] = entry.item;

		catalog.Parent.DeleteEntry(catalog.Name);
		return true;
	}
	/// <summary>
	/// Exception-safe implementation of MoveSubCatalog.
	/// 
	/// Moves the sub-Catalog at the specified path in this Catalog to the specified destination, optionally merging with any existing sub-Catalog with the specified name in the that location.
	/// </summary>
	/// <param name="fromPath">The relative path of the sub-Catalog to move.</param>
	/// <param name="toPath">The destination location of the sub-Catalog.</param>
	/// <param name="success">If successful (returned true), outputs true if the move was carried out, otherwise false.</param>
	/// <param name="merge">If set to true, the new sub-Catalog will be merged with existing data, overwriting any colliding Entries. If set to false, the data will not be moved to locations already containing the specified name.</param>
	/// <returns>True if the path referenced a sub-Catalog in this Catalog, otherwise false.</returns>
	public bool TryMoveSubCatalog(string fromPath, string toPath, out bool success, bool merge = false)
	{
		try
		{
			success = MoveSubCatalog(fromPath, toPath, merge);
			return true;
		}
		catch (CatalogNotFoundException)
		{
			success = false;
			return false;
		}
	}
	/// <summary>
	/// Moves the sub-Catalog at the specified path in this Catalog to the specified destination, optionally merging with any existing sub-Catalog with the specified name in the that location.
	/// 
	/// Will throw a CatalogNotFoundException if the specified path does not reference a sub-Catalog in this Catalog.
	/// </summary>
	/// <param name="fromPath">The relative path of the sub-Catalog to move.</param>
	/// <param name="toPath">The destination location of the sub-Catalog.</param>
	/// <param name="overwrite">If set to true, the new sub-Catalog will be merged with existing data, overwriting any colliding Entries. If set to false, the data will not be moved to locations already containing the specified name.</param>
	/// <returns>True if the move was carried out, otherwise false.</returns>
	public bool MoveSubCatalog(string fromPath, string toPath, bool merge = false)
	{
		var catalog = GetSubCatalog(fromPath);

		if (HasSubCatalog(toPath))
		{
			if (merge)
				foreach (var entry in catalog.GetAllEntries())
					this[PathUtil.Combine(toPath, entry.GetRelativeName(catalog))] = entry.item;
			else
				return false;
		}
		else
			foreach (var entry in catalog.GetAllEntries())
				this[PathUtil.Combine(toPath, entry.GetRelativeName(catalog))] = entry.item;

		DeleteSubCatalog(fromPath);
		return true;
	}

	#endregion

	#endregion

	#region Private methods

	/// <summary>
	/// Used by GetAllItems when breadthFirst is set to true.
	/// </summary>
	/// <returns>The result of a breadth-first traversal of this Catalog.</returns>
	IEnumerable<T> GetAllItemsBreadthFirst()
	{
		foreach (var entry in GetAllEntriesBreadthFirst())
			yield return entry.item;
	}
	/// <summary>
	/// Used by GetAllEntries when breadthFirst is set to true.
	/// </summary>
	/// <returns>The result of a breadth-first traversal of this Catalog.</returns>
	IEnumerable<Entry> GetAllEntriesBreadthFirst()
	{
		foreach (var catalog in GetAllSubCatalogsBreadthFirst())
			foreach (var entry in catalog.GetAllEntries())
				yield return entry;
	}
	/// <summary>
	/// Used by GetAllSubCatalogs when breadthFirst is set to true.
	/// </summary>
	/// <returns>The result of a breadth-first traversal of this Catalog.</returns>
	IEnumerable<Catalog<T>> GetAllSubCatalogsBreadthFirst()
	{
		List<Catalog<T>> currentPass = new List<Catalog<T>>() { this };
		List<Catalog<T>> nextPass = new List<Catalog<T>>();

		while (currentPass.Count > 0)
		{
			nextPass.Clear();
			foreach (var catalog in currentPass)
				foreach (var subCatalog in catalog.GetSubCatalogs())
				{
					yield return subCatalog;
					nextPass.Add(subCatalog);
				}

			currentPass.Clear();
			currentPass.AddRange(nextPass);
		}
	}
	/// <summary>
	/// Used by GetAllItems when breadthFirst is set to false.
	/// </summary>
	/// <returns>The result of a depth-first traversal of this Catalog.</returns>
	IEnumerable<T> GetAllItemsDepthFirst()
	{
		foreach (var entry in GetAllEntriesDepthFirst())
			yield return entry.item;
	}
	/// <summary>
	/// Used by GetAllEntries when breadthFirst is set to false.
	/// </summary>
	/// <returns>The result of a depth-first traversal of this Catalog.</returns>
	IEnumerable<Entry> GetAllEntriesDepthFirst()
	{
		foreach (var entry in entries.Values)
			yield return entry;
		foreach (var catalog in GetAllSubCatalogsDepthFirst())
			foreach (var entry in catalog.GetAllEntries())
				yield return entry;
	}
	/// <summary>
	/// Used by GetAllSubCatalogs when breadthFirst is set to false.
	/// </summary>
	/// <returns>The result of a depth-first traversal of this Catalog.</returns>
	IEnumerable<Catalog<T>> GetAllSubCatalogsDepthFirst()
	{
		foreach (var catalog in subTree.Values)
		{
			yield return catalog;
			foreach (var subCatalog in catalog.GetAllSubCatalogsDepthFirst())
				yield return subCatalog;
		}
	}

	#endregion
}