using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using System.IO;
using System;


/// <summary>
/// Basic class to represent an rgb color index.
/// </summary>
public class ColorIndex {
	public double r;
	public double g;
	public double b;
	public ColorIndex(double r, double g, double b){
		this.r = r;
		this.g = g;
		this.b = b;
	}
}


public class Star{
	public float x;
	public float y;
	public float z;

	public Star(float x, float y, float z){
		this.x = x;
		this.y = y;
		this.z = z;
	}

}

/**
Each parser needs to return an instance of this class.
*/
public class StarContainer{

	private List<Star> stars;

	private List<ColorIndex> colorsRGB;

	public int modVal { get; private set; }

	public float scaleVal { get; private set; }

	public float max_r_val {get; set;}
	public float max_g_val {get; set;}
	public float max_b_val {get; set;}

	public StarContainer(int modVal, float scaleVal){
		this.stars = new List<Star>();
		this.colorsRGB = new List<ColorIndex>();
		this.modVal = modVal;
		this.scaleVal = scaleVal;
	}

	public void addStar(float x, float y, float z, ColorIndex c = null){
		if(c != null){
			this.colorsRGB.Add(c);
		}
		this.stars.Add(new Star(x, y, z));
	}

	public int starLength(){
		return this.stars.Count;
	}

	public Star GetStar(int i){
		return this.stars[i];
	}

	public ColorIndex GetColor(int i){
		return this.colorsRGB[i];
	}

	public bool isTrueColor(){
		return colorsRGB.Count != 0;
	}
}

/**
* Each parser inherets this abstract class.
*/
public abstract class ParseStars{
	public abstract StarContainer parseFile(TextAsset file, ParticleSystem particleSystem);
}


/// <summary>
/// Main execution class for ParticleCSVs
/// Takes a csv and parses out data to create a star field.
/// </summary>
public class ParticleCSV : MonoBehaviour {

	/// <summary>
	/// Modulo value to limit ammount of stars on screen
	/// </summary>
	public int MOD_VAL = 1;


	/// <summary>
	/// Scalar for pleiades.
	/// </summary>
	private float pScalar = 0.80f;


	/// <summary>
	/// Particle system to create "stars" at given locations.
	/// Defined in editor.
	/// </summary>
	public ParticleSystem partSystem;


	/// <summary>
	/// Private variable for counting stars processed
	/// </summary>
	private int starSize = 0;


	/// <summary>
	/// Public variable to represent a text asset.
	/// </summary>
	public TextAsset file;



	/// <summary>
	/// Start function reads in the csv files and then subsequently creates the stars.
	/// </summary>
	void Start () {
		StarContainer s = Generic_Parser.Create_Star_Cluster(file, MOD_VAL, pScalar);
		Plot_Stars(s);
	}


	//Scaling function.
	private float scale_rgb_value(float value, float max){
		//Min + (Max - Min) * (new_val - Min2)/(Max2-Min2)
		return 0.0f + (1.0f - 0.0f) * ((value-0.0f)/(max-0.0f));
	}

	private void Plot_Stars(StarContainer s){
		//Create particles
		for (int i = 0; i < s.starLength(); i++)
		{
			partSystem.Emit(1);
		}

		//Get particles to place.
		ParticleSystem.Particle[] arrParts;
		arrParts = new ParticleSystem.Particle[s.starLength()];
		partSystem.GetParticles (arrParts);

		//Get values
		for(int i = 0; i < s.starLength(); i++){
			//Get the particle system.
			ParticleSystem.Particle par = arrParts[i];

			//Get the star.
			Star star = s.GetStar(i);
			
			//Set the position to be the position times the scalar.
			par.position = new Vector3(
				star.x * s.scaleVal, 
				star.y * s.scaleVal, 
				star.z * s.scaleVal
			);

			//If color accurate is true then set true color.
			if (s.isTrueColor()) {
				ColorIndex c = s.GetColor(i);
				if (c != null) {
					float r = scale_rgb_value((float)c.r, s.max_r_val);
					float g = scale_rgb_value((float)c.g, s.max_g_val);
					float b = scale_rgb_value((float)c.b, s.max_b_val);
					par.startColor = new Color (r, g, b);
				} else {
					par.startColor = Color.white;
				}
			}
			
			arrParts [i] = par;
		}
		partSystem.SetParticles(arrParts, s.starLength());
	}


	// Update is called once per frame
	void Update () {

	}
}
