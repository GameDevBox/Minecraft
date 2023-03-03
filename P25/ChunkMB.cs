using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkMB: MonoBehaviour
{
	Chunk owner;
	public ChunkMB(){}
	public void SetOwner(Chunk o)
	{
		owner = o;
		InvokeRepeating("SaveProgress",8,120);
	}

	void SaveProgress()
	{
		if(owner.changed)
		{
			owner.Save();
			owner.changed = false;
		}
	}
}
