using System.Collections;
using System.Collections.Generic;
using FMFCLPRO.Audio;
using RetroAstro.Databases;
using RetroAstro.Entities;
using RetroAstro.Entities.EffectsEntity;
using RetroAstro.Entities.PhysicsEntities;
using RetroAstro.Entities.PlayerEntity;
using RetroAstro.Levels;
using RetroAstro.Pooling;
using RetroAstro.QuickEvents;
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

namespace RetroAstro.Core
{
    public class World : MonoBehaviour
    {
        private GameState _state;
        private SoundSystem _system;
        private QuickEntityPool _quickEntityPool;
        private BaseLevel _level;
        public Player Player { get; private set; }

        private readonly List<Entity> _entitiesUpdates = new List<Entity>();
        
        public DifficultyLevel WorldDifficulty { get; private set; }

        private void Awake()
        {
            _quickEntityPool = FindObjectOfType<QuickEntityPool>();
            _system = FindObjectOfType<SoundSystem>();
        }

        private void Update()
        {
            if (_state != GameState.Running) return;

            for (var i = 0; i < _entitiesUpdates.Count; i++)
            {
                _entitiesUpdates[i].OnTick();
            }
        }

        public void PlaySound(string tag)
        {
            _system.PlaySound(tag);
        }

        public IEnumerator Innit()
        {
            DifficultyLevel difficultyLoaded = (DifficultyLevel)PlayerPrefs.GetInt("difficulty");
            WorldDifficulty = difficultyLoaded;
            _level = new BaseLevel(this, 15, difficultyLoaded);
            SpawnPlayer();
            yield return InnitWorld();
        }

        private void SpawnPlayer()
        {
            GameEntitiesDatabase gameEntity = (GameEntitiesDatabase)GameEntitiesDatabase.Instance;
            Player p = gameEntity.GetNewInstance<Player>();
            Player = p;
            p.Construct(this, _quickEntityPool);
            p.transform.position = Vector2.zero;
        }

        IEnumerator InnitWorld()
        {
            yield return _quickEntityPool.CacheEntities<Projectile>(this, 150);
            yield return _quickEntityPool.CacheEntities<Asteroid>(this, 30);
            yield return _quickEntityPool.CacheEntities<BigAsteroid>(this, 10);
            yield return _quickEntityPool.CacheEntities<Explosion>(this, 30);
        }

        private void OnEnable()
        {
            GameEventPS.OnGameEnd += EndWorld;
            GameEventPS.OnGameStart += StartWorld;
        }

        private void StartWorld()
        {
            _state = GameState.Running;
            _level.StartLevel();
        }

        private void EndWorld()
        {
            _state = GameState.Ended;
            for (var i = 0; i < _entitiesUpdates.Count; i++)
            {
                RemoveEntity(_entitiesUpdates[i]);
            }

            _level.StopLevel();
        }

        private void OnDisable()
        {
            GameEventPS.OnGameEnd -= EndWorld;
            GameEventPS.OnGameStart -= StartWorld;
        }

        public T SpawnEntity<T>() where T : Entity
        {
            T t = _quickEntityPool.Get<T>(this);
            t.OnEntitySpawn(_level);
            _entitiesUpdates.Add(t);
            return t;
        }

        public void RemoveEntity<T>(T k) where T : Entity
        {
            _entitiesUpdates.Remove(k);
        }
    }
}