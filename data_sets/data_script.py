import math
import pandas as pd


def distance(x):
    if x < 0:
        return 0
    else:
        return 1/x

def x(R, b, l):
    return R * math.cos(b*(math.pi/180)) * math.cos(l*(math.pi/180))

def y(R, b, l):
    return R * math.cos(b*(math.pi/180)) * math.sin(l*(math.pi/180))

def z(R, b):
    return R * math.sin(b*(math.pi/180))

def luminosity(m, d):
    return 4*math.pi*math.pow(d, 2)*m

def solar_lum(l):
    return l/(3.828*math.pow(10, 26))

def temp(l):
    sun = 5778
    frac = 1/4
    return math.pow(l, frac) * sun

#σ = 5.670374419...×10−8 W⋅m−2⋅K−4
def radius(l, t):
    const = 5.67 * math.pow(10, -8)
    ans = l/(4*math.pi*const*math.pow(t, 4))
    return math.log10(math.sqrt(ans))

def parse(csv_file):
    df = pd.read_csv(csv_file)
    df.loc[:, 'parallax_arcsec'] = df['parallax'].apply(lambda x: x * .001)
    df.loc[:, 'R'] = df['parallax_arcsec'].apply(distance)
    df.loc[:, 'x'] = df.apply(lambda r: x(r['R'], r['b'], r['l']), axis=1)
    df.loc[:, 'y'] = df.apply(lambda r: y(r['R'], r['b'], r['l']), axis=1)
    df.loc[:, 'z'] = df.apply(lambda r: z(r['R'], r['b']), axis=1)
    df.loc[:, 'luminosity'] = df.apply(lambda r: luminosity(r['phot_g_mean_mag'], r['R']), axis=1)
    df.loc[:, 'solar_luminosity'] = df.apply(lambda r: solar_lum(r['luminosity']), axis=1)
    df.loc[:, 'temperature'] = df.apply(lambda r: temp(r['solar_luminosity']), axis=1)
    df.loc[:, 'radius'] = df.apply(lambda r: radius(r['phot_g_mean_mag'], r['temperature']), axis=1)

    df.to_csv(csv_file.split('.')[0] + "_output.csv")

def parse2(csv_file, csv_file2):
    df = pd.read_csv(csv_file)
    df.loc[:, 'parallax_arcsec'] = df['parallax'].apply(lambda x: x * .001)
    df.loc[:, 'R'] = df['parallax_arcsec'].apply(distance)
    df.loc[:, 'x'] = df.apply(lambda r: x(r['R'], r['b'], r['l']), axis=1)
    df.loc[:, 'y'] = df.apply(lambda r: y(r['R'], r['b'], r['l']), axis=1)
    df.loc[:, 'z'] = df.apply(lambda r: z(r['R'], r['b']), axis=1)
    df.loc[:, 'luminosity'] = df.apply(lambda r: luminosity(r['phot_g_mean_mag'], r['R']), axis=1)
    df.loc[:, 'solar_luminosity'] = df.apply(lambda r: solar_lum(r['luminosity']), axis=1)
    df.loc[:, 'temperature'] = df.apply(lambda r: temp(r['solar_luminosity']), axis=1)
    df.loc[:, 'radius'] = df.apply(lambda r: radius(r['phot_g_mean_mag'], r['temperature']), axis=1)

    df2 = pd.read_csv(csv_file2)
    df2.loc[:, 'parallax_arcsec'] = df2['parallax'].apply(lambda x: x * .001)
    df2.loc[:, 'R'] = df2['parallax_arcsec'].apply(distance)
    df2.loc[:, 'x'] = df2.apply(lambda r: x(r['R'], r['b'], r['l']), axis=1)
    df2.loc[:, 'y'] = df2.apply(lambda r: y(r['R'], r['b'], r['l']), axis=1)
    df2.loc[:, 'z'] = df2.apply(lambda r: z(r['R'], r['b']), axis=1)
    df2.loc[:, 'luminosity'] = df2.apply(lambda r: luminosity(r['phot_g_mean_mag'], r['R']), axis=1)
    df2.loc[:, 'solar_luminosity'] = df2.apply(lambda r: solar_lum(r['luminosity']), axis=1)
    df2.loc[:, 'temperature'] = df2.apply(lambda r: temp(r['solar_luminosity']), axis=1)
    df2.loc[:, 'radius'] = df2.apply(lambda r: radius(r['phot_g_mean_mag'], r['temperature']), axis=1)

    frames = [df, df2]
    df3 = pd.concat(frames)
    df3.to_csv(csv_file.split('.')[0] + csv_file2.split('.')[0] + "_final_output.csv")


def main():
    # Data within a one degree box of Pleiades
    # RA between 56.35 and 57.35
    # DEC between 23.65 and 24.65
    #parse("pleiades_final.csv")

    # Data within a four degree box of Pleiades
    # RA between 55 and 58
    # DEC between 22 and 26
    # parallax > 6.2 AND parallax < 12.5
    #parse("pleiades4.csv")

    #parse("pleiades6-12.csv", "beehive4-7.csv")
    #parse2("pleiades-result.csv", "beehive-result.csv")


    #SELECT * from gaiadr2.gaia_source where
    #RA between 50 and 135 AND
    #DEC between 12 and 32 AND
    #parallax > 4.5 AND parallax <= 13 AND
    #phot_g_mean_mag < 16
    parse("pb2.csv")



if __name__ == '__main__':
    main()
