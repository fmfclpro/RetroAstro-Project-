using System.Collections;
using RetroAstro.Core;
using RetroAstro.Levels;
using RetroAstro.Pooling;
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


namespace RetroAstro.Entities.PhysicsEntities
{
    public class Projectile : PhysicsEntity
    {
        public Team TargetTeam { get;  set; }
        [SerializeField] private int damage;
        
        private Coroutine _killCor;
        private Entity owner;

        public void SetOwner(Entity entity)
        {
            owner = entity;
        }

        public override void Construct(World world, QuickEntityPool quickEntityPool)
        {
            base.Construct(world, quickEntityPool);
            TargetTeam = Team.Friendly;
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            IDamagableEntity damagableEntity = col.GetComponent<IDamagableEntity>();
            if(damagableEntity != null)
            {
                if (damagableEntity.Team == TargetTeam)
                {
                    damagableEntity.OnDamageTaken(damage);
                    Kill();
                    if (_killCor != null)
                    {
                        StopCoroutine(_killCor);
                    }
                }
                
            }
        }

        public override void OnEntitySpawn(BaseLevel level)
        {
            base.OnEntitySpawn(level);

            SetVelocity(Vector2.zero);
            _killCor = StartCoroutine(KillTimer());
        }

        IEnumerator KillTimer()
        {
            yield return new WaitForSeconds(2f);
            Kill();
        }

        protected override void OnCustomEntityPoolAdd(QuickEntityPool quickEntityPool)
        {
            quickEntityPool.Enlist(this);
            
        }

    }
}

