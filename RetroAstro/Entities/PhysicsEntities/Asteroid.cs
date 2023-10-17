using System;
using FMFCLPRO.Audio;
using RetroAstro.Entities.EffectsEntity;
using RetroAstro.Entities.PlayerEntity;
using RetroAstro.Levels;
using RetroAstro.Pooling;
using RetroAstro.QuickEvents;
using RetroAstro.Utils.Math;
using UnityEngine;
using Random = UnityEngine.Random;

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
    public class Asteroid : PhysicsEntity, IDamagableEntity
    {
        protected override void OnCustomEntityPoolAdd(QuickEntityPool quickEntityPool)
        {
            quickEntityPool.Enlist(this);

        }
        
        private void OnTriggerEnter2D(Collider2D col)
        {
            Player p = col.GetComponent<Player>();
            if (p != null)
            {
                GameEventPS.CallOnGameEnd();
            }
        }

        protected override void OnDeath()
        {
            base.OnDeath();
            World.Player.GivePoints(GetPointsGiven());
            World.SpawnEntity<Explosion>().transform.position = transform.position;
            World.PlaySound("Explosion");
            SetVelocity(Vector2.zero);
        }

        public virtual int GetPointsGiven()
        {
            return 1;
        }
        public override void OnEntitySpawn(BaseLevel baseLevel)
        {
            base.OnEntitySpawn(baseLevel);

            Player p = World.Player;
            Vector2 point = GeometryMath2D.RandomPointOnCircleEdge(baseLevel.LevelRadius);
            transform.position = point;
            float velocity = GetVelocityStrength();
            Vector3 dir = p.transform.position - transform.position ;
            SetVelocity(dir.normalized * velocity   );

        }

        protected virtual float GetVelocityStrength()
        {
            return Random.Range(2f, 5f);
        }

        public void OnDamageTaken(int amount)
        {
            ReduceHealth(amount);
        }

        public Team Team => Team.AbsolutelyNotFriendlyAtAll;
    }
}
