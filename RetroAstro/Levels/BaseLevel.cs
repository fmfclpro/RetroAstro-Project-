using System.Collections;
using RetroAstro.Core;
using RetroAstro.Entities.PhysicsEntities;
using RetroAstro.QuickEvents;
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

namespace RetroAstro.Levels
{
    public enum DifficultyLevel
    {
        Hard,
        Medium,
        Easy
    }

    public class BaseLevel
    {
        private DifficultyLevel _difficultyLevel;
        public float LevelRadius { get; }
        public World World { get; }

        private Coroutine spawnCor;
        private Coroutine timerCor;

        public float TimePassed => timePassed;

        public BaseLevel(World world, int levelRadius, DifficultyLevel _difficultyLevel)
        {
            this.LevelRadius = levelRadius;
            this.World = world;
            SetDifficult(_difficultyLevel);
        }

        private void SetDifficult(DifficultyLevel difficultyLevel)
        {
            _difficultyLevel = difficultyLevel;

            switch (_difficultyLevel)
            {
                case DifficultyLevel.Easy:
                    timer = 0.9f;
                    break;
                case DifficultyLevel.Hard:
                    timer = 0.5f;
                    break;
                case DifficultyLevel.Medium:
                    timer = 0.6f;
                    break;
            }
        }

        public void StartLevel()
        {
            spawnCor = World.StartCoroutine(StartSpawning());
            timerCor = World.StartCoroutine(Reduce());
        }

        public void StopLevel()
        {
            World.StopCoroutine(spawnCor);
            World.StopCoroutine(timerCor);
        }

        private float timer;
        private int timePassed;

        IEnumerator Reduce()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);

                timePassed += 1;
                Debug.Log(timePassed);

                if (timePassed >= 20)
                {
                    GameEventPS.CallOnGameEnd();
                    yield break;
                }

                switch (_difficultyLevel)
                {
                    case DifficultyLevel.Easy:
                        timer -= 0.02f;
                        break;
                    case DifficultyLevel.Medium:
                        timer -= 0.025f;
                        break;
                    case DifficultyLevel.Hard:
                        timer -= 0.03f;
                        break;
                }
            }
        }

        private IEnumerator StartSpawning()
        {
            while (true)
            {
                int rn = Random.Range(0, 100);

                int chanceToDrop = 0;
                int howManyToDrop = 0;

                switch (_difficultyLevel)
                {
                    case DifficultyLevel.Easy:
                        chanceToDrop = 5;
                        howManyToDrop = 1;
                        break;
                    case DifficultyLevel.Medium:
                        chanceToDrop = 15;
                        howManyToDrop = 2;
                        break;
                    case DifficultyLevel.Hard:
                        chanceToDrop = 20;
                        howManyToDrop = 3;
                        break;
                }

                if (rn >= chanceToDrop)
                {
                    for (int i = 0; i < UnityEngine.Random.Range(1, howManyToDrop); i++)
                    {
                        World.SpawnEntity<Asteroid>();
                    }
                }
                else
                {
                    World.SpawnEntity<BigAsteroid>();
                }

                yield return new WaitForSeconds(timer);
            }
        }
    }
}