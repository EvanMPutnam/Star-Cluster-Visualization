using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pleiades_Parser : ParseStars
{
    private int MOD_VAL = 0;
    private float SCALE_VAL = 0.4f;
    private bool COLOR_ACCURATE = false;
    private int X = 99;
    private int Y = 100;
    private int Z = 101;

    public Pleiades_Parser(){
    }

    public override StarContainer parseFile(TextAsset text, ParticleSystem partSystem){
        StarContainer starContainer = new StarContainer(MOD_VAL, SCALE_VAL);

        int starSize = 0;
		int count = 0;
		//Split the file and get the stars.
		string[] star_file_lines = text.text.Split('\n');
		//Process each star.
		foreach (string line in star_file_lines) {
			var values = line.Split (',');
			if(count == 0)
			{
				Debug.Log(values[X]);
				Debug.Log(values[Y]);
				Debug.Log(values[Z]);
			}
			//First line should ALWAYS be an indexer to tell where values are.
			if (count != 0) {
				//Modulo value to help limit large datasets but keep even distribution.
				if(count % MOD_VAL == 0){
					//If you want color accurate to be true
					if (COLOR_ACCURATE == true) {

					}

					//Note that y and z coordinates are flipped because unity has a different coordinate system.  Y is up/down
                    starContainer.addStar(float.Parse (values[X]), 
                                            float.Parse (values[Z]), 
                                            float.Parse (values[Y]), 0.0f);
					partSystem.Emit (1);
					//Counter for particle system to know how many stars.
					starSize += 1;
				}

			} else {
			}

			count += 1;

			
        }
        return starContainer;
    }
}
