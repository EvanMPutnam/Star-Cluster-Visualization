using UnityEngine;

public class Generic_Parser {

    public static StarContainer Create_Star_Cluster(TextAsset file, int mod_val, float scale_val){
        StarContainer s = new StarContainer(mod_val, scale_val);
        string[] lines = file.text.Split('\n');

        //X, Y, Z coordinates for plotting
        int x_index = -1;
        int y_index = -1;
        int z_index = -1;

        //RGB values for display.
        int r_index = -1;
        int g_index = -1;
        int b_index = -1;

        bool rgb_found = false;

        int count = 0;

        bool first_val = true;
        foreach (string line in lines){
            if(first_val){
                string[] line_parts = line.Split(',');
                for(int i = 0; i < line_parts.Length; i++){
                    if(line_parts[i].ToLower().Equals("x")){
                        x_index = i;
                    }
                    if(line_parts[i].ToLower().Equals("y")){
                        y_index = i;
                    }
                    if(line_parts[i].ToLower().Equals("z")){
                        z_index = i;
                    }
                    if(line_parts[i].ToLower().Equals("r")){
                        r_index = i;
                    }
                    if(line_parts[i].ToLower().Equals("g")){
                        g_index = i;
                    }
                    if(line_parts[i].ToLower().Equals("b")){
                        b_index = i;
                    }
                }
                first_val = false;
            }else{
                string[] line_parts = line.Split(',');
                if(line_parts.Length != 1 && count % mod_val == 0){
                    ColorIndex ci = null;
                    //If color found then populate it.
                    if(rgb_found){
                        ci = new ColorIndex(float.Parse(line_parts[r_index]),
                                        float.Parse(line_parts[g_index]), 
                                        float.Parse(line_parts[b_index]));
                    }
                    //Populate star container with a new star. (y,z are flipped for unity.)
                    s.addStar(float.Parse(line_parts[x_index]), 
                                float.Parse(line_parts[z_index]), 
                                float.Parse(line_parts[y_index]), ci);
                    
                }
                count += 1;
            }

            //RGB values present
            if(r_index != -1 && g_index != -1 && b_index != -1){
                rgb_found = true;
            }
            //Did not find
            if(x_index == -1 || y_index == -1 || z_index == -1){
                return null;
            }
        }

        Debug.Log(count);
        return s;
    }

}
