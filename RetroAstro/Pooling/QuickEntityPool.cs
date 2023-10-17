using System;
using System.Collections;
using System.Collections.Generic;
using RetroAstro.Core;
using RetroAstro.Databases;
using RetroAstro.Entities;
using UnityEngine;

/*
MIT License

Copyright (c) 2023 Filipe Lopes | FMFCLPRO

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

*/

namespace RetroAstro.Pooling
{
    public class QuickEntityPool : MonoBehaviour
    {
        private Dictionary<Type, Queue<Entity>> _quickPool = new();

        public IEnumerator CacheEntities<T>(World world, int quantity = 1) where T : Entity
        {
            if (quantity <= 0) quantity = 1;

            GameEntitiesDatabase gameEntity = (GameEntitiesDatabase)GameEntitiesDatabase.Instance;

            for (int i = 0; i < quantity; i++)
            {
                T entity = gameEntity.GetNewInstance<T>();
                entity.Construct(world, this);
                entity.OnEntityCache();
                Enlist(entity);
                yield return null;
            }
        }

        public void Enlist<T>(T obj) where T : Entity
        {
            if (_quickPool.TryGetValue(typeof(T), out Queue<Entity> q))
            {
                q.Enqueue(obj);

                return;
            }

            Queue<Entity> queue = new Queue<Entity>();

            _quickPool.Add(typeof(T), queue);
            queue.Enqueue(obj);
        }

        public T Get<T>(World world) where T : Entity
        {
            if (_quickPool.TryGetValue(typeof(T), out Queue<Entity> pool) && pool.Count != 0)
            {
                return pool.Dequeue() as T;
            }

            GameEntitiesDatabase gameEntity = (GameEntitiesDatabase)GameEntitiesDatabase.Instance;

            T entity = gameEntity.GetNewInstance<T>();
            entity.Construct(world, this);
            return entity;
        }
    }
}