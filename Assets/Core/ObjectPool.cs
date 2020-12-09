using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Pool
{
	public class ObjectPool //: Singleton<ObjectPool>
	{
		public static ObjectPool instance { get; private set; }

		private Dictionary<Type, ObjectSubPool> _subPools;

		public static void InitializeInstance()
		{
			instance = new ObjectPool();
		}

		public static void DestroyInstance()
		{
			instance.Clear();
			instance = null;
		}

		public T Acquire<T>() where T : IRecycleObject, new()
		{
			var subPool = GetSubPool(typeof(T));
			return subPool.Acquire<T>();
		}

		public object Acquire( Type type )
		{
			var subPool = GetSubPool( type );
			return subPool.Acquire( type );
		}

		public void Recycle<T>(T target) where T : IRecycleObject, new()
		{
			if (target == null)
			{
				Debug.LogWarning("Try to Recycle null object");
				return;
			}

			var subPool = GetSubPool(typeof(T));
			subPool.Recycle(target);
		}

		public void Recycle( object target)
		{
			if (target == null)
			{
				Debug.LogWarning("Try to Recycle null object");
				return;
			}

			var subPool = GetSubPool( target.GetType() );
			subPool.Recycle( target );
		}

		private ObjectPool()
		{
			_subPools = new Dictionary<Type, ObjectSubPool>();
		}

		private ObjectSubPool GetSubPool(Type type)
		{
			if (_subPools.TryGetValue(type, out var subPool) == false)
			{
				subPool = new ObjectSubPool(type);
				_subPools.Add(type, subPool);
			}
			return subPool;
		}

		private void Clear()
		{
			foreach (var subPoolPair in _subPools)
			{
				var subPool = subPoolPair.Value;
				subPool.Clear();
			}

			_subPools.Clear();
		}
	}

	public class ObjectSubPool
	{
		private Type _objectType;

		private int _increaseSize = 1;

		private List<IRecycleObject> _usingList;

		private Stack<IRecycleObject> _usableList;

		public int totalCount
		{
			get
			{
				return _usingList.Count + _usableList.Count;
			}
		}

		public int currentCount
		{
			get { return _usingList.Count; }
		}

		public ObjectSubPool(Type type)
		{
			_objectType = type;
			_usingList = new List<IRecycleObject>();
			_usableList = new Stack<IRecycleObject>();
		}

		public T Acquire<T>() where T : IRecycleObject, new()
		{
			if (_objectType != typeof(T))
			{
				Debug.LogError($"Try get object :{typeof(T)} in {_objectType} pool");
				return default;
			}

			EnsureCapacity(_usableList.Count);

			var result = _usableList.Pop();
			_usingList.Add(result);
			return (T)result;
		}

		public object Acquire( Type type )
		{
			if (_objectType != type)
			{
				Debug.LogError($"Try get object :{type} in {_objectType} pool");
				return null;
			}

			EnsureCapacity(_usableList.Count);

			var result = _usableList.Pop();
			_usingList.Add(result);
			return result;
		}

		public void Recycle<T>(T target) where T : IRecycleObject, new()
		{
			if (_objectType != typeof(T))
			{
				Debug.LogError($"Try recycle object : {typeof(T)} to {_objectType} pool");
				return;
			}

			var recycleObject = target as IRecycleObject;
			recycleObject.Release();

			if (_usingList.Remove(recycleObject) == false)
			{
				Debug.LogError($"Check position that this object create : {recycleObject}");
			}

			_usableList.Push(recycleObject);
		}

		public void Recycle( object target )
		{
			if (_objectType != target.GetType())
			{
				Debug.LogError($"Try recycle object : {target} to {_objectType} pool");
				return;
			}

			var recycleObject = target as IRecycleObject;
			recycleObject.Release();

			if (_usingList.Remove(recycleObject) == false)
			{
				Debug.LogError($"Check position that this object create : {recycleObject}");
			}

			_usableList.Push(recycleObject);
		}

		public void Clear()
		{
			//TODO 
			_usableList.Clear();
			_usableList = null;

			_usingList = null;

			_objectType = null;
		}

		private void EnsureCapacity(int count)
		{
			if (count == 0)
			{
				for (int i = 0; i < _increaseSize; i++)
				{
					var temp = System.Activator.CreateInstance(_objectType) as IRecycleObject;
					_usableList.Push(temp);
				}
			}
		}
	}

	public interface IRecycleObject
	{
		void Release();
	}
}
