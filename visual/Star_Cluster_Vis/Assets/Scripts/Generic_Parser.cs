using UnityEngine;

public class Generic_Parser {


    //These values are the strings that appear as headers in the .csv file
    private const string X_COL = "x";
    private const string Y_COL = "y";
    private const string Z_COL = "z";
    private const string R_COL = "reg";
    private const string G_COL = "green";
    private const string B_COL = "blue";
    private const string B_MINUS_V = "bp_rp";
    private const string RADIUS = "radius";

    public static StarContainer Create_Star_Cluster(TextAsset file, int mod_val, float scale_val, bool average){
        //Create the container all of our data will exist in.
        StarContainer s = new StarContainer(mod_val, scale_val);

        //Get each line to parse of the .csv
        string[] lines = file.text.Split('\n');

        //Variables for centering the data at the centroid.
        int average_count = 0;
        double x_avg = 0;
        double y_avg = 0;
        double z_avg = 0;

        //X, Y, Z coordinates for plotting
        int x_index = -1;
        int y_index = -1;
        int z_index = -1;

        //RGB values for display.
        int r_index = -1;
        int g_index = -1;
        int b_index = -1;

        //Radius of the star
        int radius_index = -1;

        //B-V value
        int b_minx_v_index = -1;

        //If rgb values are found
        bool rgb_found = false;

        //Count of stars.
        int count = 0;

        //Max rgb values present in data
        float max_r_val = 255;
        float max_g_val = 255;
        float max_b_val = 255;

        //Max radius value present in data
        float max_radius = 0;

        //max b-v value present in data
        float max_b_minus_v = 0;

        //First val variable to get labels
        bool first_val = true;


        //Iterate over each line in .csv
        foreach (string line in lines){
            //Make sure relevant labels exist and get the index dynamically.
            if(first_val){
                string[] line_parts = line.Split(',');
                for(int i = 0; i < line_parts.Length; i++){
                    line_parts[i] = line_parts[i].Trim();
                    if(line_parts[i].ToLower().Equals(X_COL)){
                        x_index = i;
                    }
                    if(line_parts[i].ToLower().Equals(Y_COL)){
                        y_index = i;
                    }
                    if(line_parts[i].ToLower().Equals(Z_COL)){
                        z_index = i;
                    }
                    if(line_parts[i].ToLower().Equals(R_COL)){
                        r_index = i;
                    }
                    if(line_parts[i].ToLower().Equals(G_COL)){
                        g_index = i;
                    }
                    if(line_parts[i].ToLower().Equals(B_COL)){
                        b_index = i;
                    }
                    if(line_parts[i].ToLower().Equals(B_MINUS_V)){
                        b_minx_v_index = i;
                    }
                    if(line_parts[i].ToLower().Equals(RADIUS)){
                        radius_index = i;
                    }
                }
                //This is once we are past the first line.
                first_val = false;
            }else{
                //Get each line
                string[] line_parts = line.Split(',');

                //Get the star data if its within the modulo range.
                if(line_parts.Length != 1 && count % mod_val == 0){
                    ColorIndex ci = null;
                    float b_minus = 4.0f;
                    //If color found then populate it.
                    if(rgb_found){
                        try{
                            ci = new ColorIndex(float.Parse(line_parts[r_index]),
                                        float.Parse(line_parts[g_index]), 
                                        float.Parse(line_parts[b_index]));
                            
                            if(float.Parse(line_parts[r_index]) > max_r_val){
                                max_r_val = float.Parse(line_parts[r_index]);
                            }
                            if(float.Parse(line_parts[g_index]) > max_g_val){
                                max_g_val = float.Parse(line_parts[g_index]);
                            }
                            if(float.Parse(line_parts[b_index]) > max_b_val){
                                max_b_val = float.Parse(line_parts[b_index]);
                            }
                        }catch{
                            ci = new ColorIndex(max_r_val, max_g_val, max_b_val);
                        }
                        try{
                            b_minus = float.Parse(line_parts[b_minx_v_index]);
                            if(b_minus > max_b_minus_v){
                                max_b_minus_v = b_minus;
                            }
                        }catch{
                            b_minus = 4.0f;
                        }
                    }

                    //Populate star container with a new star. (y,z are flipped for unity.)
                    if(average){
                        x_avg += double.Parse(line_parts[x_index]);
                        y_avg += double.Parse(line_parts[z_index]);
                        z_avg += double.Parse(line_parts[y_index]);
                        average_count += 1;
                    }

                    //Add the star and its respective values to the chart.
                    float rad = 0.5f;
                    try{
                        rad = float.Parse(line_parts[radius_index]);
                    } catch {

                    }
                    rad = Mathf.Abs(rad);

                    s.addStar(float.Parse(line_parts[x_index]), 
                                float.Parse(line_parts[z_index]), 
                                float.Parse(line_parts[y_index]), 
                                rad,
                                ci, b_minus);

                    //Max radius calculation
                    if(rad > max_radius){
                        max_radius = rad;
                    }
                    
                }
                count += 1;
            }

            //RGB values present
            if(r_index != -1 && g_index != -1 && b_index != -1){
                rgb_found = true;
            }
            //Did not find
            if(x_index == -1 || y_index == -1 || z_index == -1 || radius_index == -1){
                Debug.LogError("Error: Could not find one of the following -> x, y, z or radius");
                return null;
            }

        }


        //Logic to center all the stars around the centroid of the data.
        if(average){
            //Calculate out the respective averages
            double average_x = x_avg/average_count;
            double average_y = y_avg/average_count;
            double average_z = z_avg/average_count;

            //Vectors for centering the stars.
            Vector3 final_point = new Vector3(0,0,0);
            Vector3 current_point = new Vector3((float)average_x, (float)average_y, (float)average_z);
            Vector3 vector_to_add = final_point - current_point;

            //Update all the stars with new positions.
            for (int i = 0; i < s.starLength(); i++){
                Star st = s.GetStar(i);
                st.x = st.x + vector_to_add.x;
                st.y = st.y + vector_to_add.y;
                st.z = st.z + vector_to_add.z;
            }

        }

        //Updating max possible values for range scaling.
        s.max_r_val = max_r_val;
        s.max_g_val = max_g_val;
        s.max_b_val = max_b_val;
        s.max_radius = max_radius;

        //Here be your magical data structure!
        Debug.Log(s.max_r_val);
        Debug.Log(s.max_g_val);
        Debug.Log(s.max_b_val);
        Debug.Log(s.max_radius);
        return s;
    }

}