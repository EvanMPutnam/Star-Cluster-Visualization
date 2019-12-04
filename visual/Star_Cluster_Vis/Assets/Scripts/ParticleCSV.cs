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
	/// Whether or not the stars are color accurate
	/// </summary>
	public Boolean COLOR_ACCURATE = true;


	public Boolean PLEIADIS = true;


	/// <summary>
	/// Modulo value to limit ammount of stars on screen
	/// </summary>
	public int MOD_VAL = 1;

	/// <summary>
	/// An array of x locations for the stars
	/// </summary>
	private List<float> xLocs;

	/// <summary>
	/// An array of y locations for the stars
	/// </summary>
	private List<float> yLocs;

	/// <summary>
	/// An array of z locations for the stars
	/// </summary>
	private List<float> zLocs;

	/// <summary>
	/// The rgb colors of a given star.
	/// </summary>
	private List<ColorIndex> colorsRGB;


	/// <summary>
	/// Scalar to multiply the x, y, z locations by to spread them out
	/// </summary>
	private float scaler = 1.0f;

	/// <summary>
	/// Scalar for pleiades.
	/// </summary>
	private float pScalar = 1.0f;


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
		//readCsv ();
		//createStars ();
		StarContainer s = Generic_Parser.Create_Star_Cluster(file, MOD_VAL, pScalar);
		Plot_Stars(s);
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
			ParticleSystem.Particle par = arrParts[i];
			//Get the star.
			Star star = s.GetStar(i);
			
			par.position = new Vector3(
				star.x * s.scaleVal, 
				star.y * s.scaleVal, 
				star.z * s.scaleVal
			);

			//If color accurate is true then set true color.
			if (s.isTrueColor()) {
				ColorIndex c = s.GetColor(i);
				if (c != null) {
					par.startColor = new Color ((float)c.r, (float)c.g, (float)c.b);
				} else {
					par.startColor = Color.white;
				}
			}
			
			arrParts [i] = par;
		}
		partSystem.SetParticles(arrParts, s.starLength());
	}


	/// <summary>
	/// Reads the csv file and fills the x, y, z arrays with values
	/// </summary>
	private void readCsv(){
		
		DateTime d = DateTime.Now;


		int x = 17;
		int y = 18;
		int z = 19;

		if(PLEIADIS)
		{
			x = 99;
			y = 100;
			z = 101;
			COLOR_ACCURATE = false;
			scaler = 1;
		}
		xLocs = new List<float>();
		yLocs = new List<float>();
		zLocs = new List<float>();
		colorsRGB = new List<ColorIndex> ();

		int count = 0;
		//Split the file and get the stars.
		string[] star_file_lines = file.text.Split('\n');
		//Process each star.
		foreach (string line in star_file_lines) {
			var values = line.Split (',');
			if(count == 0)
			{
				Debug.Log(values[x]);
				Debug.Log(values[y]);
				Debug.Log(values[z]);
			}
			//First line should ALWAYS be an indexer to tell where values are.
			if (count != 0) {
				//Modulo value to help limit large datasets but keep even distribution.
				if(count % MOD_VAL == 0){
					//If you want color accurate to be true
					if (COLOR_ACCURATE == true) {
						if (values [16] == "") {
							colorsRGB.Add (null);
						} else {
							colorsRGB.Add (bv2rgb (double.Parse (values [16])));
						}
					}



					//Note that y and z coordinates are flipped because unity has a different coordinate system.  Y is up/down
					xLocs.Add (float.Parse (values [x]));
					yLocs.Add (float.Parse (values [z]));
					zLocs.Add (float.Parse (values [y]));
					partSystem.Emit (1);
					//Counter for particle system to know how many stars.
					starSize += 1;
				}

			} else {
			}

			count += 1;

			

		}
		//print (DateTime.Now.Second - d.Second );
	}




	/// <summary>
	/// Creates the stars 
	/// </summary>
	private void createStars(){
		ParticleSystem.Particle[] arrParts;
		arrParts = new ParticleSystem.Particle[starSize];
		partSystem.GetParticles (arrParts);

		int count = 0;
		foreach( float x in xLocs){
			ParticleSystem.Particle par = arrParts[count];
			if(PLEIADIS)
			{
				par.position = new Vector3(
					xLocs[count] * pScalar, 
					yLocs[count] * pScalar, 
					zLocs[count] * pScalar
				);
			}
			else
			{
				par.position = new Vector3(
					xLocs[count] * scaler, 
					yLocs[count] * scaler, 
					zLocs[count] * scaler
				);
			}
			Debug.Log(par.startSize);
			Debug.Log("Hello");



			//If color accurate is true then set true color
			if (COLOR_ACCURATE == true) {
				ColorIndex c = colorsRGB [count];
				if (c != null) {
					par.startColor = new Color ((float)c.r, (float)c.g, (float)c.b);
				} else {
					par.startColor = Color.white;
				}
			}

			arrParts [count] = par;
			count += 1;
		}
		partSystem.SetParticles(arrParts, starSize);
	}





	/// <summary>
	/// Function to get the rgb value of a given star from the BV value
	/// Found here https://stackoverflow.com/questions/21977786/star-b-v-color-index-to-apparent-rgb-color
	/// </summary>
	/// <param name="bv">B-V value for star</param>
	public static ColorIndex bv2rgb(double bv)    // RGB <0,1> <- BV <-0.4,+2.0> [-]
	{

		//Init vars
		double t;
		double r;
		double g;
		double b;

		r=0.0; g=0.0; b=0.0;

		//If below values correct o make sure they are within range
		if (bv<-0.4)
			bv=-0.4; 
		if (bv> 2.0) 
			bv= 2.0;
		
		if ((bv >= -0.40) && (bv < 0.00)) { 
			t = (bv + 0.40) / (0.00 + 0.40);
			r = 0.61 + (0.11 * t) + (0.1 * t * t); 
		} else if ((bv >= 0.00) && (bv < 0.40)) {
			t = (bv - 0.00) / (0.40 - 0.00);
			r = 0.83 + (0.17 * t);
		} else if ((bv >= 0.40) && (bv < 2.10)) {
			t = (bv - 0.40) / (2.10 - 0.40);
			r = 1.00;
		}
		if ((bv >= -0.40) && (bv < 0.00)) {
			t = (bv + 0.40) / (0.00 + 0.40);
			g = 0.70 + (0.07 * t) + (0.1 * t * t);
		} else if ((bv >= 0.00) && (bv < 0.40)) {
			t = (bv - 0.00) / (0.40 - 0.00);
			g = 0.87 + (0.11 * t);
		} else if ((bv >= 0.40) && (bv < 1.60)) {
			t = (bv - 0.40) / (1.60 - 0.40);
			g = 0.98 - (0.16 * t);
		} else if ((bv >= 1.60) && (bv < 2.00)) {
			t = (bv - 1.60) / (2.00 - 1.60);
			g = 0.82 - (0.5 * t * t);
		}
		if ((bv >= -0.40) && (bv < 0.40)) {
			t = (bv + 0.40) / (0.40 + 0.40);
			b = 1.00;
		} else if ((bv >= 0.40) && (bv < 1.50)) {
			t = (bv - 0.40) / (1.50 - 0.40);
			b = 1.00 - (0.47 * t) + (0.1 * t * t);
		} else if ((bv >= 1.50) && (bv < 1.94)) {
			t = (bv - 1.50) / (1.94 - 1.50);
			b = 0.63 - (0.6 * t * t);
		}

		//print ("Red: " + r + ", Green: "+ g + ", Blue: " + b);
		return new ColorIndex (r, g, b);
	}



	// Update is called once per frame
	void Update () {

	}
}
