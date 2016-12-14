using UnityEngine;
using System.Collections;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game
{
    public class BirdSpawner : MonoBehaviour
    {
        public GameObject birdSpawner;
        public GameObject[] birdSpawn;

        private GameObject mainCamera; //The mainCamera is used as the reference point to spawn birds

        PlayerEnterExit playerEnterExit;

        bool isInside;
        bool wentInside;

        // Use this for initialization
        void Start()
        {
            // Disable self if mod not enabled
            if (!DaggerfallUnity.Settings.UncannyValley_BirdsInDaggerfall)
            {
                gameObject.SetActive(false);
                return;
            }

            birdSpawn = new GameObject[9];
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            playerEnterExit = GameManager.Instance.PlayerEnterExit;
            StartCoroutine("SpawnBirds");
        }

        void Update()
        {
            //Check if the player goes inside, if the he/she does, then all bird spawners are destroyed
            isInside = playerEnterExit.IsPlayerInside;

            if (isInside && wentInside == false)
            {
                wentInside = true;
                for (int i = 0; i < birdSpawn.Length; i++)
                {
                    if (birdSpawn[i] != null)
                        GameObject.Destroy(birdSpawn[i]);
                }
            }
            if (!isInside && wentInside == true)
            {
                wentInside = false;
            }

        }

        IEnumerator SpawnBirds()
        {
            RaycastHit hit;
            ParticleSystem particleSystem;
            int spawnID = 0;

            yield return new WaitForSeconds(0.5f); //The script needs to wait in the beginning to make sure that the player and the terrain etc has properly spawned

            while (true)
            {
                if (isInside) //If the player is inside a structure, then  don't spawn birds
                {
                    yield return new WaitForSeconds(0.1f);
                    continue;
                }

                if (spawnID > birdSpawn.Length - 1)
                    spawnID = 0;

                //To increase performance, the script reuses the bird spawners if it can. The bird spawner is created or moved to the location of the player
                if (birdSpawn[spawnID] == null)
                {
                    birdSpawn[spawnID] = Instantiate(birdSpawner, mainCamera.transform.position, Quaternion.LookRotation(Vector3.forward)) as GameObject;
                    birdSpawn[spawnID].transform.parent = gameObject.transform;
                    birdSpawn[spawnID].name = "BirdSpawn#" + spawnID;
                }
                else
                {
                    birdSpawn[spawnID].SetActive(false);
                    birdSpawn[spawnID].transform.position = mainCamera.transform.position;
                }

                //We move the bird spawner up in the air  above the player and set a random direction for it
                birdSpawn[spawnID].transform.position += transform.up * Random.Range(10, 250);
                birdSpawn[spawnID].transform.Rotate(0.0f, Random.Range(0.0f, 360.0f), 0.0f);

                //We do a raycast from the birdspawner location (above the player) to find it's starting point. If the raycast hit's something (like a mountain) then we don't spawn any birds there
                if (Physics.Raycast(birdSpawn[spawnID].transform.position, birdSpawn[spawnID].transform.forward, out hit, 1000.0F))
                {
                    yield return new WaitForSeconds(0.01f);
                    continue;
                }

                //Move the bird to it's new location, a bit away from the player, then we give it a new direction in which the birds will fly, 
                //We want the birds to fly into the space where the player is, otherwise the player won't see the birds and it would be kind-of pointless
                birdSpawn[spawnID].transform.position += birdSpawn[spawnID].transform.forward * Random.Range(400, 1000);
                birdSpawn[spawnID].transform.eulerAngles += (120 + Random.Range(0, 80)) * Vector3.up;

                //Can the birds fly in their new given direction?
                if (Physics.Raycast(birdSpawn[spawnID].transform.position, birdSpawn[spawnID].transform.forward, out hit, 2000.0F))
                {
                    yield return new WaitForSeconds(0.01f);
                    continue;
                }


                birdSpawn[spawnID].SetActive(true);
                particleSystem = birdSpawn[spawnID].GetComponent<ParticleSystem>();
                particleSystem.Clear();
                birdSpawn[spawnID].transform.position += transform.up * 50; //We move the birdspawner up a bit to make sure that the birds that spawn in the bottom of the emitter doesn't fly into mountain tops
                var mainParticleSystem = particleSystem.main;
                mainParticleSystem.startRotation = Mathf.PI / 180 * birdSpawn[spawnID].transform.eulerAngles.y; //Rotate the bird "sprites" so that they are facing the direction that the fly, sprite rotation uses radians so some math is required
                mainParticleSystem.startSpeed = Random.Range(20, 40); //Random speed
                particleSystem.Emit(Random.Range(1, 10)); //Random amount of birds that spawn
                spawnID++;

                yield return new WaitForSeconds(Random.Range(5.0f, 10.0f));
            }
        }


    }
}