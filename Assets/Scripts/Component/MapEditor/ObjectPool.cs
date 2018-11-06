using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using MyPuzzle;

namespace MapEditor
{
    [RequireComponent(typeof(GameObjectRef))]
    public class ObjectPool
        : MonoBehaviour
    {
        public static ObjectPool Instance { get; private set; }

        private GameObject PoolNode;
        private GameObjectRef refs;
        private void Awake()
        {
            Instance = this;
            this.PoolNode = new GameObject("ObjectPool");
            this.PoolNode.transform.SetParent(this.transform);
            this.refs = this.GetComponent<GameObjectRef>();
            this.InitPool();
        }

        private void InitPool()
        {
            foreach (var kvp in this.refs)
            {
                this.AppendGameObject(kvp.Key, kvp.Value, MinCount);
            }
        }

        private const int MinCount = 5;
        private const int IncCount = 5;
        private void AppendGameObject(string tag, GameObject go, int count)
        {
            if (!this.pool.ContainsKey(tag))
                this.pool.Add(tag, new Queue<GameObject>());

            var gos = this.pool[tag];
            for(var i = 0; i<count;++i)
            {
                var tmp = Instantiate(go);
                tmp.transform.SetParent(this.PoolNode.transform, false);
                gos.Enqueue(tmp);
            }
        }

        private Dictionary<string, Queue<GameObject>> pool = new Dictionary<string, Queue<GameObject>>();
        public GameObject Take(string tag)
        {
            Debug.Assert(this.pool.ContainsKey(tag) && this.pool[tag].Count > 0);
            var go = this.pool[tag].Dequeue();
            if (this.pool[tag].Count == 0)
                this.AppendGameObject(tag, this.refs[tag], IncCount);

            return go;
        }

        public void Return(string tag, GameObject go)
        {
            go.SetActiveEx(false);
            go.transform.SetParent(this.PoolNode.transform, false);
            this.pool[tag].Enqueue(go);
        }
    }
}
