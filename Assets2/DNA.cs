using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DNA {

	//used to store the genes 
	List<int> genes = new List<int>();
	//dna length
	int dnaLength = 0;
	//max value used for gene initializing
	int maxValues = 0;

	//constructor
	public DNA(int l, int v)
	{
		dnaLength = l;
		maxValues = v;
		SetRandom();
	}

	public void SetRandom()
	{
		genes.Clear();
		//looping through the list of genes and initializing random genes
		for(int i = 0; i < dnaLength; i++)
		{
			genes.Add(Random.Range(0, maxValues));
		}
	}

	//setting gene value at a particular position
	public void SetInt(int pos, int value)
	{
		genes[pos] = value;
	}

	//combining the dna of parents
	public void Combine(DNA d1, DNA d2)
	{
		for(int i = 0; i < dnaLength; i++)
		{	
			//the first half of the dna is used from parent 1 
			if(i < dnaLength/2.0)
			{
				int c = d1.genes[i];
				genes[i] = c;
			}
			//the second half is from parent 2 
			else
			{
				int c = d2.genes[i]; 
				genes[i] = c;
			}
		}
	}

	//mutation method sets a random value at a random gene position
	public void Mutate()
	{
		genes[Random.Range(0,dnaLength)] = Random.Range(0, maxValues);
	}

	 
	public int GetGene(int pos)
	{
		return genes[pos];
	}

}

