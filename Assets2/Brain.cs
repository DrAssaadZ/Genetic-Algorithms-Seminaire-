//the brain script is the controller of the character
//it reads the dna and determines what to do 
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof (ThirdPersonCharacter))]
public class Brain : MonoBehaviour
{	
	//for this exp we using only a dna made of 1 gene
	public int DNALength = 1;
	public float timeAlive;
	public DNA dna;

	//accessing the ethan character script
    private ThirdPersonCharacter m_Character; 
    //ethan movement vectors
    private Vector3 m_Move;
    private bool m_Jump; 
    bool alive = true;                     

    void OnCollisionEnter(Collision obj)
    {
    	//killing the character if he hits the ground
    	if(obj.gameObject.tag == "dead")
    	{
    		alive = false;
    	}
    }
    
	public void Init()
	{
		//initialise DNA
        //0 forward
        //1 back
        //2 left
        //3 right
        //4 jump
        //5 crouch
        dna = new DNA(DNALength,6);
		m_Character = GetComponent<ThirdPersonCharacter>();
        timeAlive = 0;
        alive = true;
	}


    void Update()
    {

    }


    // Fixed update is called in sync with physics
    private void FixedUpdate()
    {
        // read DNA
        float h = 0;
        float v = 0;
        bool crouch = false;
        //reading the dna value
        if(dna.GetGene(0) == 0) v = 1;
        else if(dna.GetGene(0) == 1) v = -1;
        else if(dna.GetGene(0) == 2) h = -1;
        else if(dna.GetGene(0) == 3) h = 1;
        else if(dna.GetGene(0) == 4) m_Jump = true;
        else if(dna.GetGene(0) == 5) crouch = true;

        m_Move = v*Vector3.forward + h*Vector3.right;
        m_Character.Move(m_Move, crouch, m_Jump);
        //setting jump to false because if the gene change jump would still be jumping
        m_Jump = false;
        //if we're still alive update our time alive
        if(alive)
        	timeAlive += Time.deltaTime;
    }
}
