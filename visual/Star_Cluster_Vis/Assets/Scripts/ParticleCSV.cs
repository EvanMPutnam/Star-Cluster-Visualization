using System.Collections.Generic;
using UnityEngine;
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

/// <summary>
/// Basic class to represent a star.
/// </summary>
public class Star{
	public float x;
	public float y;
	public float z;

	public float radius;

	public float b_minus_v;

	public Star(float x, float y, float z, float radius, float b_minus_v = 4.0f){
		this.x = x;
		this.y = y;
		this.z = z;
		this.radius = radius;
		this.b_minus_v = b_minus_v;
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

	public float max_radius {get; set;}

	public StarContainer(int modVal, float scaleVal){
		this.stars = new List<Star>();
		this.colorsRGB = new List<ColorIndex>();
		this.modVal = modVal;
		this.scaleVal = scaleVal;
	}

	public void addStar(float x, float y, float z, float radius, ColorIndex c = null,  float b_minus_v = 1.0f){
		if(c != null){
			this.colorsRGB.Add(c);
		}

		this.stars.Add(new Star(x, y, z, radius, b_minus_v));		
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


	//Value to change if you want to use the b-v calcs or RGB values.
	public bool is_b_minus_v_value = true;



	/// <summary>
	/// Start function reads in the csv files and then subsequently creates the stars.
	/// </summary>
	void Start () {
		StarContainer s = Generic_Parser.Create_Star_Cluster(file, MOD_VAL, pScalar);
		Plot_Stars(s);
	}


	//Scaling function.
	private float scale_one_through_zero_value(float value, float max){
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
				if(is_b_minus_v_value){
					ColorIndex  c = B_Minus_V_Calc(star.b_minus_v);
					par.startColor = new Color((float)c.r, (float)c.g, (float)c.b);
				} else {
					ColorIndex c = s.GetColor(i);
					if (c != null) {
						float r = scale_one_through_zero_value((float)c.r, s.max_r_val);
						float g = scale_one_through_zero_value((float)c.g, s.max_g_val);
						float b = scale_one_through_zero_value((float)c.b, s.max_b_val);
						par.startColor = new Color (r, g, b);
					} else {
						par.startColor = Color.white;
					}
				}
			}
			

			float par_size = scale_one_through_zero_value((float)star.radius, s.max_radius);
			par.startSize = par_size + 0.1f;
			arrParts [i] = par;
		}
		partSystem.SetParticles(arrParts, s.starLength());
	}


	ColorIndex B_Minus_V_Calc(float bv){

		var t = 4600 * ((1 / ((0.92 * bv) + 1.7)) +(1 / ((0.92 * bv) + 0.62)) );

		// t to xyY
		var x = 0.0;
		var y = 0.0;

		if (t >= 1667 & t <= 4000) {
			x = ((-0.2661239 * Math.Pow(10,9)) / Math.Pow(t,3)) + ((-0.2343580 * Math.Pow(10,6)) / Math.Pow(t,2)) + ((0.8776956 * Math.Pow(10,3)) / t) + 0.179910;
		} else if (t > 4000 & t <= 25000) {
			x = ((-3.0258469 * Math.Pow(10,9)) / Math.Pow(t,3)) + ((2.1070379 * Math.Pow(10,6)) / Math.Pow(t,2)) + ((0.2226347 * Math.Pow(10,3)) / t) + 0.240390;
		}

		if (t >= 1667 & t <= 2222) {
			y = -1.1063814 * Math.Pow(x,3) - 1.34811020 * Math.Pow(x,2) + 2.18555832 * x - 0.20219683;
		} else if (t > 2222 & t <= 4000) {
			y = -0.9549476 * Math.Pow(x,3) - 1.37418593 * Math.Pow(x,2) + 2.09137015 * x - 0.16748867;
		} else if (t > 4000 & t <= 25000) {
			y = 3.0817580 * Math.Pow(x,3) - 5.87338670 * Math.Pow(x,2) + 3.75112997 * x - 0.37001483;
		}
		// xyY to XYZ, Y = 1
		var Y = 1.0;
		var X = (y == 0)? 0 : (x * Y) / y;
		var Z = (y == 0)? 0 : ((1 - x - y) * Y) / y;

		//XYZ to rgb
		var r = 3.2406 * X - 1.5372 * Y - 0.4986 * Z;
		var g = -0.9689 * X + 1.8758 * Y + 0.0415 * Z;
		var b = 0.0557 * X - 0.2040 * Y + 1.0570 * Z;
		
		//linear RGB to sRGB
		var R = (r <= 0.0031308)? 12.92*r : 1.055*Math.Pow(r,1/2.4)-0.055;
		var G = (g <= 0.0031308)? 12.92*g : 1.055*Math.Pow(g,1/2.4)-0.055;
		var B = (b <= 0.0031308)? 12.92*b : 1.055*Math.Pow(b,1/2.4)-0.055;

		Debug.Log(R);
		Debug.Log(G);
		Debug.Log(B);

		return new ColorIndex(R,G,B);
	}
}