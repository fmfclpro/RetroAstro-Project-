using RetroAstro.Entities.PhysicsEntities;
using RetroAstro.Levels;
using RetroAstro.Pooling;
using RetroAstro.QuickEvents;
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


namespace RetroAstro.Entities.PlayerEntity
{
    public class Player : Entity, IDamagableEntity
    {
        private Camera _camera;
        private bool _canUpdate;

        private int _points;
   
        protected override void Awake()
        {
            base.Awake();
            _camera = FindObjectOfType<Camera>();
        }

        public void GivePoints(int points)
        {
            this._points += points;
        }
        private void OnEnable()
        {
            GameEventPS.OnGameStart += Innit;
            GameEventPS.OnGameEnd += Dinnit;
        }

        private void OnDisable()
        {
            GameEventPS.OnGameStart -= Innit;
            GameEventPS.OnGameEnd -= Dinnit;
        }
        
        public int GetPoints() => _points;

        private void Innit()
        {
            _canUpdate = true;
        }

        private void Dinnit()
        {
            _canUpdate = false;

        }

        protected override void OnCustomEntityPoolAdd(QuickEntityPool quickEntityPool)
        {
            quickEntityPool.Enlist(this);
        }


        private float t = 0;
        void Update()
        {

            if(!_canUpdate) return;

            t += Time.deltaTime;
            Vector2 point = _camera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = point -  (Vector2) transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rot = Quaternion.AngleAxis(angle -90, Vector3.forward);
            transform.rotation = rot;


            float dl = 0.1f;

            if (World.WorldDifficulty == DifficultyLevel.Hard)
            {
                dl = 0.02f;
            }
            if (t >= dl)
            {
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    Projectile e = World.SpawnEntity<Projectile>();
                    Color rn = ColorUtils.RandomColor();

                    e.TargetTeam = Team.AbsolutelyNotFriendlyAtAll;
                    e.SetEntityColor(rn);
                    e.transform.position = transform.position;
                    e.SetVelocity(direction.normalized * 10);
                    t = 0;

                }
            }

          
        
        }


        public void OnDamageTaken(int amount)
        {
            ReduceHealth(amount);
        }

        protected override void OnDeath()
        {
            base.OnDeath();
            GameEventPS.CallOnGameEnd();
            
        }

        public Team Team { get; }
    }
}
