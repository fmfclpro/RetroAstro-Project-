using RetroAstro.Core;
using RetroAstro.Levels;
using RetroAstro.Pooling;
using RetroAstro.Utils;
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


namespace RetroAstro.Entities
{
    [RequireComponent(typeof(SpriteRenderer))]
    public abstract class Entity : MonoBehaviour
    {

        [SerializeField] private EntityProperties _properties;
        private SpriteRenderer _renderer;
        public World World { get; private set; }
        private QuickEntityPool _quickEntityPool;

        protected int CurrentHealth { get; private set; }

        private bool isDeath;

        protected virtual void Awake()
        {
          
            _renderer = GetComponent<SpriteRenderer>();
            
        }

        protected virtual void OnGameEnd()
        {
            
        }



        public virtual void OnTick()
        {
            SetEntityColor(ColorUtils.RandomColor());
        }

        public void Kill()
        {
            ReduceHealth(CurrentHealth);
        }

        public void ReduceHealth(int amount)
        {
            if(isDeath) return;
            
            CurrentHealth -= amount;

            if (CurrentHealth <= 0)
            {
                isDeath = true;
                OnDeath();
            }
           
            
        }

        public void OnEntityCache()
        {
            gameObject.SetActive(false);
        }


        public void SetEntityColor(Color color)
        {
            _renderer.color = color;
        }

        public  virtual void Construct(World world, QuickEntityPool quickEntityPool)
        {
            this.World = world;
            isDeath = false;
            this._quickEntityPool = quickEntityPool;
            CurrentHealth = _properties.BaseHealth;

        }

        protected abstract void OnCustomEntityPoolAdd(QuickEntityPool quickEntityPool);
       

        public virtual void OnEntitySpawn(BaseLevel baseLevel)
        {
            isDeath = false;
            ResetHP();
            gameObject.SetActive(true);
            
        }

        public void ResetHP()
        {
            CurrentHealth = _properties.BaseHealth;

        }

        protected virtual void OnDeath()
        {
            gameObject.SetActive(false);
            OnCustomEntityPoolAdd(_quickEntityPool);
            World.RemoveEntity(this);
            
        }

        
    }
}