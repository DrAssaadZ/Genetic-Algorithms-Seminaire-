using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationController : MonoBehaviour
{
    List<GeneticPathfinder> population = new List<GeneticPathfinder>();
    public GameObject creaturePrefab;
    public int populationSize = 100;
    public int genomeLenght;
    public float cutoff = 0.3f;  //percentrage of population to keep for breeding
    public int survivorKeep = 5; //previlleged survivors to keep [the ones that are the best fit]
    public float mutationRate = 0.01f;
    public Transform spawnPoint;
    public Transform end;
    public int generation = 1;
    

    //printing stats on the screen
    GUIStyle guiStyle = new GUIStyle();
    void OnGUI()
    {
        guiStyle.fontSize = 25;
        guiStyle.normal.textColor = Color.red;
        GUI.BeginGroup (new Rect (10, 10, 250, 150));
        GUI.Box (new Rect (0,0,140,140), "Stats", guiStyle);
        GUI.Label(new Rect (10,25,200,30), "Gen: " + generation, guiStyle);
        GUI.EndGroup ();
    }


    void InitPopulation()
    {
        for(int i = 0; i < populationSize; i++)
        {
            GameObject go = Instantiate(creaturePrefab, spawnPoint.position, Quaternion.identity); //initializing a game object 
            go.GetComponent<GeneticPathfinder>().InitCreature(new DNA(genomeLenght), end.position); // giving the game object a dna and a destination
            population.Add(go.GetComponent<GeneticPathfinder>()); // adding the game object to the population
        }
    }
    void NextGeneration()
    {
        int survivorCut = Mathf.RoundToInt(populationSize * cutoff); // number of agents to keep
        List<GeneticPathfinder> survivors = new List<GeneticPathfinder>(); // list of agents kept
        for(int i = 0; i < survivorCut; i++) //filling the survivors list with most fit agents
        {
            survivors.Add(GetFittest());
        }
        for(int i = 0; i < population.Count; i++) // getting rid of the old population (the game objects: the visualisation of the population)
        {
            Destroy(population[i].gameObject); 
        }
        population.Clear(); // clearing the list of old population

        for(int i = 0; i < survivorKeep; i++) // keeping the best survivors of the previous generation
        {
            GameObject go = Instantiate(creaturePrefab, spawnPoint.position, Quaternion.identity);
            go.GetComponent<GeneticPathfinder>().InitCreature(survivors[i].dna, end.position);
            population.Add(go.GetComponent<GeneticPathfinder>());

        }
        while(population.Count < populationSize) // filling the rest of the new population list using crossover from the survivors
        {
            for(int i = 0; i < survivors.Count; i++)
            {
                GameObject go = Instantiate(creaturePrefab, spawnPoint.position, Quaternion.identity);
                go.GetComponent<GeneticPathfinder>().InitCreature(new DNA(survivors[i].dna, survivors[Random.Range(0, 10)].dna, mutationRate), end.position);
                population.Add(go.GetComponent<GeneticPathfinder>());
                if(population.Count >= populationSize)
                {
                    break;
                }
            }
        }
        for(int i = 0; i < survivors.Count; i++) // getting rid of the survivors (we dont need them anymore)
        {
            Destroy(survivors[i].gameObject);
        }
    }
    private void Start()
    {
        InitPopulation();
    }
    private void Update()
    {
        if (!HasActive())
        {
            NextGeneration();
            generation++;
        }
    }
    GeneticPathfinder GetFittest()
    {
        float maxFitness = float.MinValue; //initiazing the maxvalue of the fitness with the minimun possible float value
        int index = 0;
        for(int i = 0; i < population.Count; i++) // getting the fittest and his index and removing it from the population
        {
            if(population[i].fitness > maxFitness)  
            {
                maxFitness = population[i].fitness;
                index = i;
            }
        }

        GeneticPathfinder fittest = population[index];
        population.Remove(fittest);
        return fittest;
    }

    // check if there is active agents in the population
    bool HasActive()
    {
        for(int i = 0; i < population.Count; i++)
        {
            if (!population[i].hasFinished)
            {
                return true;
            }
        }
        return false;
    }
}
