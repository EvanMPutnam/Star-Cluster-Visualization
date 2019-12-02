import math
import pandas as pd


def distance(x):
    if x < 0:
        return 0
    else:
        return 1/x


def x(R, b, l):
    return R * math.cos(b) * math.cos(l)


def y(R, b, l):
    return R * math.cos(b) * math.sin(l)


def z(R, b):
    return R * math.sin(b)

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


def main():
    # Data within a one degree box of Pleiades
    # RA between 56.35 and 57.35
    # DEC between 23.65 and 24.65
    parse("pleiades_final.csv")

    # Data within a four degree box of Pleiades
    # RA between 55 and 58
    # DEC between 22 and 26
    # parallax > 6.2 AND parallax < 12.5
    #parse("pleiades4.csv")


if __name__ == '__main__':
    main()
