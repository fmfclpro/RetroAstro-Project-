using System;
using System.Collections.Generic;
using RetroAstro.Entities;
using RetroAstro.Registry;
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


namespace RetroAstro.Databases
{

    public abstract class FastSingleThreadMonoSingleton<T>: MonoBehaviour 
    {
        public static FastSingleThreadMonoSingleton<T> Instance { get; private set; }
        
       
        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }
    }
    public class GameEntitiesDatabase : FastSingleThreadMonoSingleton<GameEntitiesDatabase>
    {
        [SerializeField] private RegisteredEntities entities;

        private Dictionary<Type, Entity> _mapTagToEntity = new();

        protected override void Awake()
        {
            base.Awake();

        }

        private void Start()
        {
            Innit();
        }

        private void Innit()
        {
            List<Entity> allRegisteredEntities = entities.AllRegisteredEntities;
            
            for (var i = 0; i < allRegisteredEntities.Count; i++)
            {
                Type entType = allRegisteredEntities[i].GetType();
                _mapTagToEntity[entType] = allRegisteredEntities[i];

            }
        }

        public T GetNewInstance<T>() where T : Entity
        {
            return  Instantiate(_mapTagToEntity[typeof(T)]) as T;
        }
    }
}