using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothDeviceController.BluetoothDefinitionLanguage
{
    // From https://btprodspecificationrefs.blob.core.windows.net/assigned-values/16-bit%20UUID%20Numbers%20Document.pdf
    // the items marked GATT Unit
    static class BluetoothUnit
    {
        public static string Decode(UInt16 value)
        {
            switch (value)
            {
                case 0x2700: return "unitless";
                case 0x2701: return "length (metre)";
                case 0x2702: return "mass (kilogram)";
                case 0x2703: return "time (second)";
                case 0x2704: return "electric current (ampere)";
                case 0x2705: return "thermodynamic temperature (kelvin)";
                case 0x2706: return "amount of substance (mole)";
                case 0x2707: return "luminous intensity (candela)";
                case 0x2710: return "area (square metres)";
                case 0x2711: return "volume (cubic metres)";
                case 0x2712: return "velocity (metres per second)";
                case 0x2713: return "acceleration (metres per second squared)";
                case 0x2714: return "wavenumber (reciprocal metre)";
                case 0x2715: return "density (kilogram per cubic metre)";
                case 0x2716: return "surface density (kilogram per square metre)";
                case 0x2717: return "specific volume (cubic metre per kilogram)";
                case 0x2718: return "current density (ampere per square metre)";
                case 0x2719: return "magnetic field strength (ampere per metre)";
                case 0x271A: return "amount concentration (mole per cubic metre)";
                case 0x271B: return "mass concentration (kilogram per cubic metre)";
                case 0x271C: return "luminance (candela per square metre)";
                case 0x271D: return "refractive index";
                case 0x271E: return "relative permeability";
                case 0x2720: return "plane angle (radian)";
                case 0x2721: return "solid angle (steradian)";
                case 0x2722: return "frequency (hertz)";
                case 0x2723: return "force (newton)";
                case 0x2724: return "pressure (pascal)";
                case 0x2725: return "energy (joule)";
                case 0x2726: return "power (watt)";
                case 0x2727: return "electric charge (coulomb)";
                case 0x2728: return "electric potential difference (volt)";
                case 0x2729: return "capacitance (farad)";
                case 0x272A: return "electric resistance (ohm)";
                case 0x272B: return "electric conductance (siemens)";
                case 0x272C: return "magnetic flux (weber)";
                case 0x272D: return "magnetic flux density (tesla)";
                case 0x272E: return "inductance (henry)";
                case 0x272F: return "Celsius temperature (degree Celsius)";
                case 0x2730: return "luminous flux (lumen)";
                case 0x2731: return "illuminance (lux)";
                case 0x2732: return "activity referred to a radionuclide (becquerel)";
                case 0x2733: return "absorbed dose (gray)";
                case 0x2734: return "dose equivalent (sievert)";
                case 0x2735: return "catalytic activity (katal)";
                case 0x2740: return "dynamic viscosity (pascal second)";
                case 0x2741: return "moment of force (newton metre)";
                case 0x2742: return "surface tension (newton per metre)";
                case 0x2743: return "angular velocity (radian per second)";
                case 0x2744: return "angular acceleration (radian per second squared)";
                case 0x2745: return "heat flux density (watt per square metre)";
                case 0x2746: return "heat capacity (joule per kelvin)";
                case 0x2747: return "specific heat capacity (joule per kilogram kelvin)";
                case 0x2748: return "specific energy (joule per kilogram)";
                case 0x2749: return "thermal conductivity (watt per metre kelvin)";
                case 0x274A: return "energy density (joule per cubic metre)";
                case 0x274B: return "electric field strength (volt per metre)";
                case 0x274C: return "electric charge density (coulomb per cubic metre)";
                case 0x274D: return "surface charge density (coulomb per square metre)";
                case 0x274E: return "electric flux density (coulomb per square metre)";
                case 0x274F: return "permittivity (farad per metre)";
                case 0x2750: return "permeability (henry per metre)";
                case 0x2751: return "molar energy (joule per mole)";
                case 0x2752: return "molar entropy (joule per mole kelvin)";
                case 0x2753: return "exposure (coulomb per kilogram)";
                case 0x2754: return "absorbed dose rate (gray per second)";
                case 0x2755: return "radiant intensity (watt per steradian)";
                case 0x2756: return "radiance (watt per square metre steradian)";
                case 0x2757: return "catalytic activity concentration (katal per cubic metre)";
                case 0x2760: return "time (minute)";
                case 0x2761: return "time (hour)";
                case 0x2762: return "time (day)";
                case 0x2763: return "plane angle (degree)";
                case 0x2764: return "plane angle (minute)";
                case 0x2765: return "plane angle (second)";
                case 0x2766: return "area (hectare)";
                case 0x2767: return "volume (litre)";
                case 0x2768: return "mass (tonne)";
                case 0x2780: return "pressure (bar)";
                case 0x2781: return "pressure (millimetre of mercury)";
                case 0x2782: return "length (ångström)";
                case 0x2783: return "length (nautical mile)";
                case 0x2784: return "area (barn)";
                case 0x2785: return "velocity (knot)";
                case 0x2786: return "logarithmic radio quantity (neper)";
                case 0x2787: return "logarithmic radio quantity (bel)";
                case 0x27A0: return "length (yard)";
                case 0x27A1: return "length (parsec)";
                case 0x27A2: return "length (inch)";
                case 0x27A3: return "length (foot)";
                case 0x27A4: return "length (mile)";
                case 0x27A5: return "pressure (pound-force per square inch)";
                case 0x27A6: return "velocity (kilometre per hour)";
                case 0x27A7: return "velocity (mile per hour)";
                case 0x27A8: return "angular velocity (revolution per minute)";
                case 0x27A9: return "energy (gram calorie)";
                case 0x27AA: return "energy (kilogram calorie)";
                case 0x27AB: return "energy (kilowatt hour)";
                case 0x27AC: return "thermodynamic temperature (degree Fahrenheit)";
                case 0x27AD: return "percentage";
                case 0x27AE: return "per mille";
                case 0x27AF: return "period (beats per minute)";
                case 0x27B0: return "electric charge (ampere hours)";
                case 0x27B1: return "mass density (milligram per decilitre)";
                case 0x27B2: return "mass density (millimole per litre)";
                case 0x27B3: return "time (year)";
                case 0x27B4: return "time (month)";
                case 0x27B5: return "concentration (count per cubic metre)";
                case 0x27B6: return "irradiance (watt per square metre)";
                case 0x27B7: return "milliliter (per kilogram per minute)";
                case 0x27B8: return "mass (pound)";
                case 0x27B9: return "metabolic equivalent";
                case 0x27BA: return "step (per minute)";
                case 0x27BC: return "stroke (per minute)";
                case 0x27BD: return "pace (kilometre per minute)";
                case 0x27BE: return "luminous efficacy (lumen per watt)";
                case 0x27BF: return "luminous energy (lumen hour)";
                case 0x27C0: return "luminous exposure (lux hour)";
                case 0x27C1: return "mass flow (gram per second)";
                case 0x27C2: return "volume flow (litre per second)";
                case 0x27C3: return "sound pressure (decible)";
                case 0x27C4: return "parts per million";
                case 0x27C5: return "parts per billion";
            }
            return $"{value:X2}";
        }
    }
}
